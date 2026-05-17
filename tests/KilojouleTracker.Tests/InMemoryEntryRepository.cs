using KilojouleTracker.Features.Entries;

namespace KilojouleTracker.Tests;

public class InMemoryEntryRepository : IEntryRepository
{
    private readonly List<Entry> _entries = [];
    private int _nextId = 1;

    public Task InsertAsync(Entry entry)
    {
        entry.Id = _nextId++;
        _entries.Add(new Entry
        {
            Id = entry.Id,
            Date = entry.Date,
            Kilojoules = entry.Kilojoules
        });
        return Task.CompletedTask;
    }

    public Task<List<DailySummary>> GetDailySummariesAsync(int days)
    {
        var cutoff = DateTime.Today.AddDays(-(days - 1));
        var summaries = _entries
            .Where(e => e.Date >= cutoff)
            .GroupBy(e => e.Date.Date)
            .OrderBy(g => g.Key)
            .Select(g => new DailySummary(
                g.Key,
                g.Sum(e => e.Kilojoules),
                g.Count(),
                Math.Round(g.Average(e => e.Kilojoules), 0)
            ))
            .ToList();
        return Task.FromResult(summaries);
    }

    public Task<List<Entry>> GetEntriesAsync(int days)
    {
        var cutoff = DateTime.Today.AddDays(-(days - 1));
        var result = _entries
            .Where(e => e.Date >= cutoff)
            .OrderBy(e => e.Date)
            .ThenBy(e => e.Id)
            .Select(e => new Entry { Id = e.Id, Date = e.Date, Kilojoules = e.Kilojoules })
            .ToList();
        return Task.FromResult(result);
    }

    public Task<List<Entry>> GetEntriesForDateAsync(DateTime date)
    {
        var result = _entries
            .Where(e => e.Date == date.Date)
            .OrderBy(e => e.Id)
            .Select(e => new Entry { Id = e.Id, Date = e.Date, Kilojoules = e.Kilojoules })
            .ToList();
        return Task.FromResult(result);
    }

    public Task<Entry?> GetByIdAsync(int id)
    {
        var entry = _entries.FirstOrDefault(e => e.Id == id);
        if (entry is null)
            return Task.FromResult<Entry?>(null);

        return Task.FromResult<Entry?>(new Entry
        {
            Id = entry.Id,
            Date = entry.Date,
            Kilojoules = entry.Kilojoules
        });
    }

    public Task DeleteAsync(Entry entry)
    {
        _entries.RemoveAll(e => e.Id == entry.Id);
        return Task.CompletedTask;
    }
}
