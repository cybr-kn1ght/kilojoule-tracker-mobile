namespace KilojouleTracker.Features.Terminal;

public class TerminalFormatter : ITerminalFormatter
{
    public TerminalLine Input(string raw)
        => new($"> {raw}", TerminalLineStyle.Input);

    public TerminalLine Success(string message)
        => new($"✓ {message}", TerminalLineStyle.Success);

    public TerminalLine Error(string message)
        => new($"✗ {message}", TerminalLineStyle.Error);

    public TerminalLine System(string message)
        => new($"── {message}", TerminalLineStyle.System);

    public TerminalLine Chart(string line)
        => new(line, TerminalLineStyle.Chart);
}
