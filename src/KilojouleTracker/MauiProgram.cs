using CommunityToolkit.Mvvm.ComponentModel;
using KilojouleTracker.Features.Terminal;
using KilojouleTracker.Features.Chart;
using KilojouleTracker.Features.Entries;
using KilojouleTracker.Features.Navigation;

namespace KilojouleTracker;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                // Uses Android's built-in monospace font
            });

        // Domain services (stateless, can be singleton)
        builder.Services.AddSingleton<ICommandParser, CommandParser>();
        builder.Services.AddSingleton<ITerminalFormatter, TerminalFormatter>();
        builder.Services.AddSingleton<IChartRenderer, ChartRenderer>();

        // Data
        builder.Services.AddSingleton<IEntryRepository, SqliteEntryRepository>();

        // Application services
        builder.Services.AddSingleton<ITerminalService, TerminalService>();
        builder.Services.AddSingleton<INavigationService, MauiNavigationService>();

        // ViewModels
        builder.Services.AddTransient<TerminalViewModel>();

        // Pages
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}
