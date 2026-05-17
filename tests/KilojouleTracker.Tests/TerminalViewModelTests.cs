using KilojouleTracker.Features.Chart;
using KilojouleTracker.Features.Navigation;
using KilojouleTracker.Features.Terminal;
using NSubstitute;

namespace KilojouleTracker.Tests;

public class TerminalViewModelTests
{
    private readonly ITerminalService _terminalService = Substitute.For<ITerminalService>();
    private readonly INavigationService _navigationService = Substitute.For<INavigationService>();
    private readonly TerminalViewModel _sut;

    public TerminalViewModelTests()
    {
        _sut = new TerminalViewModel(_terminalService, _navigationService);
    }

    [Fact]
    public async Task Submit_EmptyInput_DoesNothing()
    {
        _sut.InputText = "";
        await _sut.SubmitCommand.ExecuteAsync(null);

        await _terminalService.DidNotReceive().ExecuteAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task Submit_WhitespaceInput_DoesNothing()
    {
        _sut.InputText = "   ";
        await _sut.SubmitCommand.ExecuteAsync(null);

        await _terminalService.DidNotReceive().ExecuteAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task Submit_ValidInput_CallsServiceAndClearsInput()
    {
        var result = new InterpreterResult([
            new TerminalLine("> 3400", TerminalLineStyle.Input),
            new TerminalLine("✓ Logged 3400 kJ for today", TerminalLineStyle.Success),
        ]);
        _terminalService.ExecuteAsync("3400").Returns(result);
        _sut.InputText = "3400";

        await _sut.SubmitCommand.ExecuteAsync(null);

        await _terminalService.Received(1).ExecuteAsync("3400");
        Assert.Equal(2, _sut.Output.Count);
        Assert.Equal("> 3400", _sut.Output[0].Text);
        Assert.Equal("", _sut.InputText);
    }

    [Fact]
    public async Task Submit_TrimsInput()
    {
        var result = new InterpreterResult([
            new TerminalLine("> 3400", TerminalLineStyle.Input),
        ]);
        _terminalService.ExecuteAsync("3400").Returns(result);
        _sut.InputText = "  3400  ";

        await _sut.SubmitCommand.ExecuteAsync(null);

        await _terminalService.Received(1).ExecuteAsync("3400");
    }

    [Fact]
    public async Task Submit_GraphResult_NavigatesToGraph()
    {
        var chartData = new ChartData(["row1"], 5000, 2500, 2);
        var result = new InterpreterResult(
            [new TerminalLine("Loading...", TerminalLineStyle.System)],
            ChartData: chartData
        );
        _terminalService.ExecuteAsync("/graph").Returns(result);
        _sut.InputText = "/graph";

        await _sut.SubmitCommand.ExecuteAsync(null);

        await _navigationService.Received(1).ShowGraphAsync(chartData);
    }

    [Fact]
    public async Task Submit_NoGraphResult_DoesNotNavigate()
    {
        var result = new InterpreterResult([
            new TerminalLine("help text", TerminalLineStyle.System),
        ]);
        _terminalService.ExecuteAsync("/help").Returns(result);
        _sut.InputText = "/help";

        await _sut.SubmitCommand.ExecuteAsync(null);

        await _navigationService.DidNotReceive().ShowGraphAsync(Arg.Any<ChartData>());
    }

    [Fact]
    public async Task Submit_ClearCommand_ClearsOutput()
    {
        _sut.Output.Add(new TerminalLine("old line", TerminalLineStyle.System));
        var result = new InterpreterResult([], ShouldClear: true);
        _terminalService.ExecuteAsync("/clear").Returns(result);
        _sut.InputText = "/clear";

        await _sut.SubmitCommand.ExecuteAsync(null);

        Assert.Empty(_sut.Output);
    }
}
