using SQLite;

namespace KilojouleTracker.Features.Entries;

public class Entry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public double Kilojoules { get; set; }
}
