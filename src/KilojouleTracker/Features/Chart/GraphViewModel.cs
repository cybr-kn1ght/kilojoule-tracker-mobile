using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KilojouleTracker.Features.Navigation;

namespace KilojouleTracker.Features.Chart;

public partial class GraphViewModel : ObservableObject
{
    public ChartData ChartData { get; }
    public string ChartTitle => $"Last {ChartData.DayCount} days — Avg {ChartData.AverageKj:F0} kJ";
    public string AverageLabel => $"Max: {ChartData.MaxKj:F0} kJ    Daily Avg: {ChartData.AverageKj:F0} kJ";

    private readonly INavigationService _navigation;

    public GraphViewModel(ChartData chartData, INavigationService navigation)
    {
        ChartData = chartData;
        _navigation = navigation;
    }

    [RelayCommand]
    private async Task Dismiss()
    {
        await _navigation.GoBackAsync();
    }
}
