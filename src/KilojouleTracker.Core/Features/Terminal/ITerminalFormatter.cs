namespace KilojouleTracker.Features.Terminal;

public interface ITerminalFormatter
{
    TerminalLine Input(string raw);
    TerminalLine Success(string message);
    TerminalLine Error(string message);
    TerminalLine System(string message);
    TerminalLine Chart(string line);
}
