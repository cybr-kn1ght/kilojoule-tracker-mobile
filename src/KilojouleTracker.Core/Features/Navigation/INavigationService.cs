using KilojouleTracker.Features.Chart;

namespace KilojouleTracker.Features.Navigation;

public interface INavigationService
{
    Task ShowGraphAsync(ChartData data);
    Task GoBackAsync();
}
