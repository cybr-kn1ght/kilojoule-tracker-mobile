using KilojouleTracker.Features.Chart;
using KilojouleTracker.Features.Entries;

namespace KilojouleTracker.Tests;

public class ChartRendererTests
{
    private readonly ChartRenderer _sut = new();

    [Fact]
    public void Render_EmptyList_ReturnsEmptyChartData()
    {
        var result = _sut.Render([]);
        Assert.Empty(result.Rows);
        Assert.Equal(0, result.MaxKj);
        Assert.Equal(0, result.AverageKj);
        Assert.Equal(0, result.DayCount);
    }

    [Fact]
    public void Render_SingleDay_ReturnsOneBar()
    {
        var summaries = new List<DailySummary>
        {
            new(new DateTime(2026, 5, 17), 5000, 3, 1667),
        };

        var result = _sut.Render(summaries);
        Assert.Equal(1, result.DayCount);
        Assert.Equal(5000, result.MaxKj);
        Assert.Equal(5000, result.AverageKj);
        Assert.NotEmpty(result.Rows);
    }

    [Fact]
    public void Render_MultipleDays_ReturnsCorrectDayCount()
    {
        var summaries = new List<DailySummary>
        {
            new(new DateTime(2026, 5, 17), 4000, 2, 2000),
            new(new DateTime(2026, 5, 18), 3000, 1, 3000),
            new(new DateTime(2026, 5, 19), 5000, 4, 1250),
        };

        var result = _sut.Render(summaries);
        Assert.Equal(3, result.DayCount);
        Assert.Equal(5000, result.MaxKj);
        Assert.Equal(4000, result.AverageKj, 0);
    }

    [Fact]
    public void Render_AllRowsStartWithYAxisLabel()
    {
        var summaries = new List<DailySummary>
        {
            new(new DateTime(2026, 5, 17), 1000, 1, 1000),
        };

        var result = _sut.Render(summaries);
        foreach (var row in result.Rows)
        {
            if (row.StartsWith("     └") || row.StartsWith("      "))
                continue;
            Assert.Contains("┤", row);
        }
    }

    [Fact]
    public void Render_LowerValues_ProducesShorterBars()
    {
        var summaries = new List<DailySummary>
        {
            new(new DateTime(2026, 5, 17), 5000, 1, 5000),
            new(new DateTime(2026, 5, 18), 1000, 1, 1000),
        };

        var result = _sut.Render(summaries);
        Assert.Equal(2, result.DayCount);
        Assert.Equal(5000, result.MaxKj);
    }

    [Fact]
    public void Render_SameValues_ProducesEqualBars()
    {
        var summaries = new List<DailySummary>
        {
            new(new DateTime(2026, 5, 17), 3000, 1, 3000),
            new(new DateTime(2026, 5, 18), 3000, 1, 3000),
        };

        var result = _sut.Render(summaries);
        Assert.Equal(3000, result.MaxKj);
        Assert.Equal(3000, result.AverageKj, 0);
    }

    [Fact]
    public void Render_DayLabelsPresent()
    {
        var summaries = new List<DailySummary>
        {
            new(new DateTime(2026, 5, 17), 2000, 1, 2000),
            new(new DateTime(2026, 5, 18), 2000, 1, 2000),
        };

        var result = _sut.Render(summaries);
        var lastRow = result.Rows.Last();
        Assert.Contains("17", lastRow);
        Assert.Contains("18", lastRow);
    }

    [Fact]
    public void Render_XAxisPresent()
    {
        var summaries = new List<DailySummary>
        {
            new(new DateTime(2026, 5, 17), 2000, 1, 2000),
        };

        var result = _sut.Render(summaries);
        Assert.Contains(result.Rows, r => r.Contains("└") || r.Contains("┬"));
    }
}
