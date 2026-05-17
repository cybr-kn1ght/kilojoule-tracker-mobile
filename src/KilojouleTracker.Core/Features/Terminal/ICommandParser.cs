namespace KilojouleTracker.Features.Terminal;

public interface ICommandParser
{
    ParsedCommand Parse(string input);
}
