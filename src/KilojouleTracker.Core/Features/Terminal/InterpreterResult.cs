using KilojouleTracker.Features.Chart;

namespace KilojouleTracker.Features.Terminal;

public record InterpreterResult(
    IReadOnlyList<TerminalLine> Lines,
    ChartData? ChartData = null,
    bool ShouldClear = false
);
