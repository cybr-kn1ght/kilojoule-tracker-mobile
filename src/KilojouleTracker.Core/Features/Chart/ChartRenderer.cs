using KilojouleTracker.Features.Entries;

namespace KilojouleTracker.Features.Chart;

public class ChartRenderer : IChartRenderer
{
    private const int VerticalRows = 6;
    private const int BarWidth = 4;

    public ChartData Render(IReadOnlyList<DailySummary> summaries)
    {
        if (summaries.Count == 0)
            return new ChartData([], 0, 0, 0);

        var maxKj = summaries.Max(s => s.TotalKj);
        var avgKj = Math.Round(summaries.Average(s => s.TotalKj), 0);
        var rowHeight = maxKj > 0 ? maxKj / (double)VerticalRows : 1;

        var rows = new List<string>();

        for (int row = VerticalRows; row >= 1; row--)
        {
            var yLabel = row == VerticalRows
                ? $"{maxKj,5} "
                : "      ";

            var line = yLabel + "┤";
            foreach (var summary in summaries)
            {
                var barHeight = (int)Math.Ceiling(summary.TotalKj / rowHeight);
                var isAvgRow = avgKj > 0
                    && row * rowHeight >= avgKj
                    && (row - 1) * rowHeight < avgKj;

                if (barHeight >= row)
                    line += new string('█', BarWidth);
                else if (isAvgRow)
                    line += new string('═', BarWidth);
                else
                    line += new string(' ', BarWidth);
            }
            rows.Add(line);
        }

        var xAxis = "     └" + string.Concat(Enumerable.Repeat("┬", summaries.Count)) +
                    new string('─', Math.Max(0, BarWidth * summaries.Count - summaries.Count));

        rows.Add(xAxis);

        var labels = "      ";
        foreach (var summary in summaries)
        {
            var dayLabel = summary.Date.ToString("d/M");
            labels += dayLabel.PadRight(BarWidth + 1).Substring(0, BarWidth);
        }
        rows.Add(labels);

        return new ChartData(rows.AsReadOnly(), maxKj, avgKj, summaries.Count);
    }
}
