using KilojouleTracker.Features.Chart;

namespace KilojouleTracker.Features.Navigation;

public class MauiNavigationService : INavigationService
{
    public async Task ShowGraphAsync(ChartData data)
    {
        var vm = new GraphViewModel(data, this);
        var page = new GraphPage { BindingContext = vm };
        await Shell.Current.Navigation.PushModalAsync(page);
    }

    public async Task GoBackAsync()
    {
        await Shell.Current.Navigation.PopModalAsync();
    }
}
