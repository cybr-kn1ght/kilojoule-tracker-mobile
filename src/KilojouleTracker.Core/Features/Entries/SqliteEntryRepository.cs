using SQLite;

namespace KilojouleTracker.Features.Entries;

public class SqliteEntryRepository : IEntryRepository
{
    private SQLiteAsyncConnection? _db;

    private async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_db is not null)
            return _db;

        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "kilojoules.db3");
        _db = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        await _db.CreateTableAsync<Entry>();
        return _db;
    }

    public async Task InsertAsync(Entry entry)
    {
        var db = await GetConnectionAsync();
        await db.InsertAsync(entry);
    }

    public async Task<List<DailySummary>> GetDailySummariesAsync(int days)
    {
        var db = await GetConnectionAsync();
        var cutoff = DateTime.Today.AddDays(-(days - 1));
        var entries = await db.Table<Entry>()
            .Where(e => e.Date >= cutoff)
            .OrderBy(e => e.Date)
            .ToListAsync();

        return entries
            .GroupBy(e => e.Date.Date)
            .Select(g => new DailySummary(
                g.Key,
                g.Sum(e => e.Kilojoules),
                g.Count(),
                Math.Round(g.Average(e => e.Kilojoules), 0)
            ))
            .ToList();
    }

    public async Task<List<Entry>> GetEntriesAsync(int days)
    {
        var db = await GetConnectionAsync();
        var cutoff = DateTime.Today.AddDays(-(days - 1));
        return await db.Table<Entry>()
            .Where(e => e.Date >= cutoff)
            .OrderBy(e => e.Date)
            .ThenBy(e => e.Id)
            .ToListAsync();
    }

    public async Task<List<Entry>> GetEntriesForDateAsync(DateTime date)
    {
        var db = await GetConnectionAsync();
        return await db.Table<Entry>()
            .Where(e => e.Date == date.Date)
            .OrderBy(e => e.Id)
            .ToListAsync();
    }

    public async Task<Entry?> GetByIdAsync(int id)
    {
        var db = await GetConnectionAsync();
        return await db.FindAsync<Entry>(id);
    }

    public async Task DeleteAsync(Entry entry)
    {
        var db = await GetConnectionAsync();
        await db.DeleteAsync(entry);
    }
}
