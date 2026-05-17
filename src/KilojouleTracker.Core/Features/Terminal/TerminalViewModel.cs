using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KilojouleTracker.Features.Navigation;

namespace KilojouleTracker.Features.Terminal;

public partial class TerminalViewModel : ObservableObject
{
    private readonly ITerminalService _terminalService;
    private readonly INavigationService _navigationService;

    private static readonly string[] CommandKeywords = [
        "summary", "graph", "help", "delete", "clear"
    ];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowSuggestions))]
    private string _inputText = "";

    [ObservableProperty]
    private ObservableCollection<string> _suggestions = [];

    public bool ShowSuggestions => Suggestions.Count > 0;

    public ObservableCollection<TerminalLine> Output { get; } = [];

    public TerminalViewModel(ITerminalService terminalService, INavigationService navigationService)
    {
        _terminalService = terminalService;
        _navigationService = navigationService;
    }

    partial void OnInputTextChanged(string value)
    {
        if (string.IsNullOrEmpty(value) || char.IsDigit(value[0]) || value[0] == '/')
        {
            Suggestions.Clear();
            return;
        }

        var matches = CommandKeywords
            .Where(c => c.StartsWith(value.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToList();

        Suggestions = new ObservableCollection<string>(matches);
    }

    [RelayCommand]
    private void SelectSuggestion(string suggestion)
    {
        InputText = suggestion;
        Suggestions.Clear();
        OnPropertyChanged(nameof(InputText));
    }

    [RelayCommand]
    private async Task Submit()
    {
        var raw = InputText?.Trim() ?? "";
        if (string.IsNullOrEmpty(raw))
            return;

        Suggestions.Clear();

        var result = await _terminalService.ExecuteAsync(raw);

        if (result.ShouldClear)
        {
            Output.Clear();
        }
        else
        {
            foreach (var line in result.Lines)
                Output.Add(line);
        }

        if (result.ChartData is not null)
            await _navigationService.ShowGraphAsync(result.ChartData);

        InputText = "";
    }
}
