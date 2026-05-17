namespace KilojouleTracker.Features.Chart;

public record ChartData(
    IReadOnlyList<string> Rows,
    double MaxKj,
    double AverageKj,
    int DayCount
);
