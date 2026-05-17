using KilojouleTracker.Features.Chart;
using KilojouleTracker.Features.Entries;
using KilojouleTracker.Features.Terminal;
using NSubstitute;

namespace KilojouleTracker.Tests;

public class TerminalServiceTests
{
    private readonly ICommandParser _parser = Substitute.For<ICommandParser>();
    private readonly IEntryRepository _repo = Substitute.For<IEntryRepository>();
    private readonly IChartRenderer _chartRenderer = Substitute.For<IChartRenderer>();
    private readonly TerminalFormatter _formatter = new();
    private readonly TerminalService _sut;

    public TerminalServiceTests()
    {
        _sut = new TerminalService(_parser, _repo, _chartRenderer, _formatter);
    }

    [Fact]
    public async Task Execute_LogCommand_InsertsEntryAndReturnsLines()
    {
        _parser.Parse("3400").Returns(new ParsedCommand(CommandType.Log, Value: 3400));

        var result = await _sut.ExecuteAsync("3400");

        await _repo.Received(1).InsertAsync(Arg.Is<Entry>(e =>
            e.Kilojoules == 3400 && e.Date == DateTime.Today));
        Assert.Contains(result.Lines, l => l.Text.Contains("Logged"));
        Assert.Contains(result.Lines, l => l.Text == "> 3400");
    }

    [Fact]
    public async Task Execute_LogCommandWithDate_UsesProvidedDate()
    {
        var date = new DateTime(2026, 5, 17);
        _parser.Parse("3400 @2026-05-17").Returns(new ParsedCommand(CommandType.Log, Value: 3400, Date: date));

        await _sut.ExecuteAsync("3400 @2026-05-17");

        await _repo.Received(1).InsertAsync(Arg.Is<Entry>(e =>
            e.Kilojoules == 3400 && e.Date == date));
    }

    [Fact]
    public async Task Execute_SummaryCommand_FetchesFromRepo()
    {
        _parser.Parse("/summary").Returns(new ParsedCommand(CommandType.Summary));
        _repo.GetEntriesAsync(14).Returns([]);

        var result = await _sut.ExecuteAsync("/summary");

        await _repo.Received(1).GetEntriesAsync(14);
        Assert.NotEmpty(result.Lines);
    }

    [Fact]
    public async Task Execute_SummaryCommand_WithEntries_ShowsAverages()
    {
        _parser.Parse("/summary").Returns(new ParsedCommand(CommandType.Summary));
        _repo.GetEntriesAsync(14).Returns([
            new Entry { Id = 1, Date = DateTime.Today.AddDays(-1), Kilojoules = 1500 },
            new Entry { Id = 2, Date = DateTime.Today.AddDays(-1), Kilojoules = 1500 },
            new Entry { Id = 3, Date = DateTime.Today, Kilojoules = 1400 },
            new Entry { Id = 4, Date = DateTime.Today, Kilojoules = 1300 },
            new Entry { Id = 5, Date = DateTime.Today, Kilojoules = 1300 },
        ]);

        var result = await _sut.ExecuteAsync("/summary");

        Assert.Contains(result.Lines, l => l.Text.Contains("3000"));
        Assert.Contains(result.Lines, l => l.Text.Contains("4000"));
        Assert.Contains(result.Lines, l => l.Text.Contains("daily avg"));
    }

    [Fact]
    public async Task Execute_GraphCommand_RendersChart()
    {
        _parser.Parse("/graph").Returns(new ParsedCommand(CommandType.Graph, GraphDays: 14));
        var summaries = new List<DailySummary>
        {
            new(DateTime.Today, 4000, 2, 2000),
        };
        _repo.GetDailySummariesAsync(14).Returns(summaries);
        var chartData = new ChartData(["test row"], 4000, 2000, 1);
        _chartRenderer.Render(summaries).Returns(chartData);

        var result = await _sut.ExecuteAsync("/graph");

        await _repo.Received(1).GetDailySummariesAsync(14);
        _chartRenderer.Received(1).Render(summaries);
        Assert.NotNull(result.ChartData);
        Assert.Equal(4000, result.ChartData.MaxKj);
    }

    [Fact]
    public async Task Execute_GraphCommand_NoEntries_ShowsError()
    {
        _parser.Parse("/graph").Returns(new ParsedCommand(CommandType.Graph, GraphDays: 14));
        _repo.GetDailySummariesAsync(14).Returns([]);

        var result = await _sut.ExecuteAsync("/graph");

        Assert.Null(result.ChartData);
        Assert.Contains(result.Lines, l => l.Text.Contains("No entries"));
    }

    [Fact]
    public async Task Execute_DeleteCommand_DeletesEntry()
    {
        var entry = new Entry { Id = 5, Date = DateTime.Today, Kilojoules = 3400 };
        _parser.Parse("/delete 5").Returns(new ParsedCommand(CommandType.Delete, EntryId: 5));
        _repo.GetByIdAsync(5).Returns(entry);

        var result = await _sut.ExecuteAsync("/delete 5");

        await _repo.Received(1).DeleteAsync(entry);
        Assert.Contains(result.Lines, l => l.Text.Contains("Deleted entry #5"));
    }

    [Fact]
    public async Task Execute_DeleteCommand_NotFound_ReturnsError()
    {
        _parser.Parse("/delete 99").Returns(new ParsedCommand(CommandType.Delete, EntryId: 99));
        _repo.GetByIdAsync(99).Returns((Entry?)null);

        var result = await _sut.ExecuteAsync("/delete 99");

        await _repo.DidNotReceive().DeleteAsync(Arg.Any<Entry>());
        Assert.Contains(result.Lines, l => l.Text.Contains("not found"));
    }

    [Fact]
    public async Task Execute_HelpCommand_ReturnsHelpLines()
    {
        _parser.Parse("/help").Returns(new ParsedCommand(CommandType.Help));

        var result = await _sut.ExecuteAsync("/help");

        Assert.Contains(result.Lines, l => l.Text.Contains("summary"));
        Assert.Contains(result.Lines, l => l.Text.Contains("help"));
        Assert.Contains(result.Lines, l => l.Text.Contains("clear"));
    }

    [Fact]
    public async Task Execute_ClearCommand_ReturnsShouldClear()
    {
        _parser.Parse("/clear").Returns(new ParsedCommand(CommandType.Clear));

        var result = await _sut.ExecuteAsync("/clear");

        Assert.True(result.ShouldClear);
        Assert.Empty(result.Lines);
    }

    [Fact]
    public async Task Execute_UnknownCommand_ReturnsError()
    {
        _parser.Parse("/foobar").Returns(new ParsedCommand(CommandType.Unknown));

        var result = await _sut.ExecuteAsync("/foobar");

        Assert.Contains(result.Lines, l => l.Style == TerminalLineStyle.Error);
        Assert.Contains(result.Lines, l => l.Text.Contains("help"));
    }

    [Fact]
    public async Task Execute_LogCommand_CallsInsertWithCorrectDate()
    {
        _parser.Parse("2500").Returns(new ParsedCommand(CommandType.Log, Value: 2500));

        await _sut.ExecuteAsync("2500");

        await _repo.Received(1).InsertAsync(Arg.Is<Entry>(e =>
            e.Kilojoules == 2500 && e.Date == DateTime.Today));
    }

    [Fact]
    public async Task Execute_SummaryCommand_WithNoEntries_ShowsNoEntriesMessage()
    {
        _parser.Parse("/summary").Returns(new ParsedCommand(CommandType.Summary));
        _repo.GetEntriesAsync(14).Returns([]);

        var result = await _sut.ExecuteAsync("/summary");

        Assert.Contains(result.Lines, l => l.Text.Contains("No entries"));
    }
}
