namespace KilojouleTracker.Features.Terminal;

public interface ITerminalService
{
    Task<InterpreterResult> ExecuteAsync(string input);
}
