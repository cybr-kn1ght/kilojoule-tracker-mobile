using System.Globalization;
using System.Text.RegularExpressions;

namespace KilojouleTracker.Features.Terminal;

public partial class CommandParser : ICommandParser
{
    public ParsedCommand Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new ParsedCommand(CommandType.Unknown);

        var trimmed = input.Trim();

        var keywordResult = TryParseKeyword(trimmed);
        if (keywordResult is not null)
            return keywordResult;

        return ParseNumberInput(trimmed);
    }

    private static ParsedCommand? TryParseKeyword(string input)
    {
        var lower = input.ToLowerInvariant();
        var stripped = lower.StartsWith('/') ? lower[1..] : lower;

        if (stripped is "summary" or "list" or "ls")
            return new ParsedCommand(CommandType.Summary);

        if (stripped is "help" or "h" or "?")
            return new ParsedCommand(CommandType.Help);

        if (stripped is "clear" or "cls")
            return new ParsedCommand(CommandType.Clear);

        if (stripped.StartsWith("graph"))
        {
            var rest = stripped["graph".Length..].Trim();
            var days = 14;
            if (rest.Length > 0 && int.TryParse(rest, out var parsedDays) && parsedDays > 0)
                days = parsedDays;
            return new ParsedCommand(CommandType.Graph, GraphDays: days);
        }

        if (stripped.StartsWith("delete") || stripped.StartsWith("del") || stripped.StartsWith("rm"))
        {
            var keyword = stripped.StartsWith("delete") ? "delete"
                       : stripped.StartsWith("del") ? "del"
                       : "rm";
            var rest = stripped[keyword.Length..].Trim();

            if (rest.Length == 0)
                return new ParsedCommand(CommandType.Unknown);

            if (int.TryParse(rest, out var id) && id > 0)
                return new ParsedCommand(CommandType.Delete, EntryId: id);

            if (DateTime.TryParseExact(rest, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return new ParsedCommand(CommandType.Delete, TargetDate: date);

            return new ParsedCommand(CommandType.Unknown);
        }

        return null;
    }

    private static ParsedCommand ParseNumberInput(string input)
    {
        var match = NumberWithDateRegex().Match(input);
        if (!match.Success)
            return new ParsedCommand(CommandType.Unknown);

        if (!double.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            return new ParsedCommand(CommandType.Unknown);

        if (value < 0)
            return new ParsedCommand(CommandType.Unknown);

        DateTime? date = null;
        if (match.Groups[2].Success)
        {
            if (!DateTime.TryParseExact(match.Groups[2].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                return new ParsedCommand(CommandType.Unknown);
            date = parsedDate;
        }

        return new ParsedCommand(CommandType.Log, Value: value, Date: date);
    }

    [GeneratedRegex(@"^(\d+(?:\.\d+)?)(?:\s+@(\d{4}-\d{2}-\d{2}))?$")]
    private static partial Regex NumberWithDateRegex();
}
