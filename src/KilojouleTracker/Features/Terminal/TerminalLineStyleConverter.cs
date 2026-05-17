using System.Globalization;
using System.Windows.Input;

namespace KilojouleTracker.Features.Terminal;

public class TerminalLineStyleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            TerminalLineStyle.Input   => Color.FromArgb("#00FF41"),
            TerminalLineStyle.Success => Color.FromArgb("#33FF33"),
            TerminalLineStyle.Error   => Color.FromArgb("#FF3333"),
            TerminalLineStyle.System  => Color.FromArgb("#888888"),
            TerminalLineStyle.Chart   => Color.FromArgb("#00FF41"),
            _                         => Colors.White,
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
