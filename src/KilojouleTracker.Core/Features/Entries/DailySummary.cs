namespace KilojouleTracker.Features.Entries;

public record DailySummary(DateTime Date, double TotalKj, int EntryCount, double AverageKj);
