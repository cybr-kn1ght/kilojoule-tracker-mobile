namespace KilojouleTracker.Features.Terminal;

public partial class MainPage : ContentPage
{
    public MainPage(TerminalViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        // ItemsUpdatingScrollMode="KeepLastItemInView" on CollectionView handles auto-scroll
    }
}
