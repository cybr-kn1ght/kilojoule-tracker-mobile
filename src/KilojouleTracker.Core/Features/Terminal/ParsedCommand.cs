namespace KilojouleTracker.Features.Terminal;

public record ParsedCommand(
    CommandType Type,
    double? Value = null,
    DateTime? Date = null,
    int? EntryId = null,
    int GraphDays = 14,
    DateTime? TargetDate = null
);
