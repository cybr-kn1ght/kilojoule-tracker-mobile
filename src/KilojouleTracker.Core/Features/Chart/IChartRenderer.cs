using KilojouleTracker.Features.Entries;

namespace KilojouleTracker.Features.Chart;

public interface IChartRenderer
{
    ChartData Render(IReadOnlyList<DailySummary> summaries);
}
