namespace KilojouleTracker.Features.Entries;

public interface IEntryRepository
{
    Task InsertAsync(Entry entry);
    Task<List<DailySummary>> GetDailySummariesAsync(int days);
    Task<List<Entry>> GetEntriesAsync(int days);
    Task<List<Entry>> GetEntriesForDateAsync(DateTime date);
    Task<Entry?> GetByIdAsync(int id);
    Task DeleteAsync(Entry entry);
}
