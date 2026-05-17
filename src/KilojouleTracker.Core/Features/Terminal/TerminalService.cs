using KilojouleTracker.Features.Chart;
using KilojouleTracker.Features.Entries;

namespace KilojouleTracker.Features.Terminal;

public class TerminalService : ITerminalService
{
    private readonly ICommandParser _parser;
    private readonly IEntryRepository _repository;
    private readonly IChartRenderer _chartRenderer;
    private readonly ITerminalFormatter _formatter;

    public TerminalService(
        ICommandParser parser,
        IEntryRepository repository,
        IChartRenderer chartRenderer,
        ITerminalFormatter formatter)
    {
        _parser = parser;
        _repository = repository;
        _chartRenderer = chartRenderer;
        _formatter = formatter;
    }

    public async Task<InterpreterResult> ExecuteAsync(string input)
    {
        var cmd = _parser.Parse(input);
        return cmd.Type switch
        {
            CommandType.Log     => await HandleLog(cmd),
            CommandType.Summary => await HandleSummary(),
            CommandType.Graph   => await HandleGraph(cmd),
            CommandType.Delete  => await HandleDelete(cmd),
            CommandType.Help    => HandleHelp(),
            CommandType.Clear   => HandleClear(),
            _                   => HandleUnknown(cmd),
        };
    }

    private async Task<InterpreterResult> HandleLog(ParsedCommand cmd)
    {
        var entry = new Entry
        {
            Date = cmd.Date ?? DateTime.Today,
            Kilojoules = cmd.Value!.Value
        };
        await _repository.InsertAsync(entry);

        var dateStr = cmd.Date?.ToString("yyyy-MM-dd") ?? "today";
        return new InterpreterResult([
            _formatter.Input($"{cmd.Value:F0}"),
            _formatter.Success($"Logged {cmd.Value:F0} kJ for {dateStr}"),
        ]);
    }

    private async Task<InterpreterResult> HandleSummary()
    {
        var entries = await _repository.GetEntriesAsync(14);

        if (entries.Count == 0)
            return new InterpreterResult([
                _formatter.System("No entries found in the last 14 days.")
            ]);

        var lines = new List<TerminalLine>();

        var grouped = entries
            .GroupBy(e => e.Date.Date)
            .OrderByDescending(g => g.Key);

        lines.Add(_formatter.System($"─ Summary ({grouped.Count()} days) ─"));

        foreach (var day in grouped)
        {
            lines.Add(_formatter.System($"{day.Key:yyyy-MM-dd}"));
            foreach (var entry in day)
            {
                lines.Add(_formatter.System(
                    $"  {entry.Id,3}  {entry.Kilojoules,6:F0} kJ"));
            }
            var total = day.Sum(e => e.Kilojoules);
            var avg = Math.Round(day.Average(e => e.Kilojoules), 0);
            lines.Add(_formatter.System($"  Total {total,7:F0} kJ  Avg {avg,5:F0}"));
            lines.Add(_formatter.System("  ─────"));
        }

        var overallAvg = Math.Round(entries.GroupBy(e => e.Date.Date).Average(g => g.Sum(e => e.Kilojoules)), 0);
        lines.Add(_formatter.System($"Overall daily avg: {overallAvg,6:F0} kJ"));

        return new InterpreterResult(lines);
    }

    private async Task<InterpreterResult> HandleGraph(ParsedCommand cmd)
    {
        var summaries = await _repository.GetDailySummariesAsync(cmd.GraphDays);

        if (summaries.Count == 0)
            return new InterpreterResult([
                _formatter.System("No entries found to graph.")
            ]);

        var chartData = _chartRenderer.Render(summaries);
        return new InterpreterResult([
            _formatter.System($"Loading graph ({summaries.Count} days)...")
        ], ChartData: chartData);
    }

    private async Task<InterpreterResult> HandleDelete(ParsedCommand cmd)
    {
        if (cmd.TargetDate is not null)
        {
            var date = cmd.TargetDate.Value;
            var entries = await _repository.GetEntriesForDateAsync(date);
            if (entries.Count == 0)
                return new InterpreterResult([
                    _formatter.Error($"No entries found for {date:yyyy-MM-dd}.")
                ]);

            foreach (var e in entries)
                await _repository.DeleteAsync(e);

            return new InterpreterResult([
                _formatter.Input($"delete {date:yyyy-MM-dd}"),
                _formatter.Success($"Cleared {entries.Count} entries for {date:yyyy-MM-dd}"),
            ]);
        }

        var entry = await _repository.GetByIdAsync(cmd.EntryId!.Value);
        if (entry is null)
            return new InterpreterResult([
                _formatter.Error($"Entry #{cmd.EntryId} not found.")
            ]);

        await _repository.DeleteAsync(entry);
        return new InterpreterResult([
            _formatter.Input($"delete {cmd.EntryId}"),
            _formatter.Success($"Deleted entry #{cmd.EntryId}: {entry.Kilojoules:F0} kJ on {entry.Date:yyyy-MM-dd}"),
        ]);
    }

    private InterpreterResult HandleHelp()
    {
        return new InterpreterResult([
            _formatter.System("╭─ Commands ──────────────╮"),
            _formatter.System("│ <number>   Log kJ today │"),
            _formatter.System("│ <n> @<dt>  Log on date  │"),
            _formatter.System("│ summary    Daily log    │"),
            _formatter.System("│ graph [n]  Bar chart    │"),
            _formatter.System("│ delete <i> Remove entry │"),
            _formatter.System("│ delete <d> Clear day    │"),
            _formatter.System("│ help       Show this    │"),
            _formatter.System("│ clear      Clear screen │"),
            _formatter.System("╰─────────────────────────╯"),
        ]);
    }

    private InterpreterResult HandleClear()
    {
        return new InterpreterResult([], ShouldClear: true);
    }

    private InterpreterResult HandleUnknown(ParsedCommand cmd)
    {
        return new InterpreterResult([
            _formatter.Error($"Unknown input"),
            _formatter.System("Type help for available commands."),
        ]);
    }
}
