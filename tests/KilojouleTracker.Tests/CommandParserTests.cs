using KilojouleTracker.Features.Terminal;

namespace KilojouleTracker.Tests;

public class CommandParserTests
{
    private readonly CommandParser _sut = new();

    [Fact]
    public void Parse_Number_ReturnsLog()
    {
        var result = _sut.Parse("3400");
        Assert.Equal(CommandType.Log, result.Type);
        Assert.Equal(3400, result.Value);
        Assert.Null(result.Date);
    }

    [Fact]
    public void Parse_Decimal_ReturnsLog()
    {
        var result = _sut.Parse("1234.56");
        Assert.Equal(CommandType.Log, result.Type);
        Assert.Equal(1234.56, result.Value);
    }

    [Fact]
    public void Parse_NumberWithDate_ReturnsLogWithDate()
    {
        var result = _sut.Parse("3400 @2026-05-17");
        Assert.Equal(CommandType.Log, result.Type);
        Assert.Equal(3400, result.Value);
        Assert.Equal(new DateTime(2026, 5, 17), result.Date);
    }

    [Fact]
    public void Parse_Zero_ReturnsLog()
    {
        var result = _sut.Parse("0");
        Assert.Equal(CommandType.Log, result.Type);
        Assert.Equal(0, result.Value);
    }

    [Fact]
    public void Parse_NegativeNumber_ReturnsUnknown()
    {
        var result = _sut.Parse("-100");
        Assert.Equal(CommandType.Unknown, result.Type);
    }

    [Fact]
    public void Parse_Summary_ReturnsSummary()
    {
        var result = _sut.Parse("summary");
        Assert.Equal(CommandType.Summary, result.Type);
    }

    [Fact]
    public void Parse_SummaryWithSlash_ReturnsSummary()
    {
        var result = _sut.Parse("/summary");
        Assert.Equal(CommandType.Summary, result.Type);
    }

    [Fact]
    public void Parse_SummaryAliasList_ReturnsSummary()
    {
        var result = _sut.Parse("list");
        Assert.Equal(CommandType.Summary, result.Type);
    }

    [Fact]
    public void Parse_SummaryAliasLs_ReturnsSummary()
    {
        var result = _sut.Parse("ls");
        Assert.Equal(CommandType.Summary, result.Type);
    }

    [Fact]
    public void Parse_Graph_ReturnsGraphWithDefaultDays()
    {
        var result = _sut.Parse("graph");
        Assert.Equal(CommandType.Graph, result.Type);
        Assert.Equal(14, result.GraphDays);
    }

    [Fact]
    public void Parse_GraphWithSlash_ReturnsGraphWithDefaultDays()
    {
        var result = _sut.Parse("/graph");
        Assert.Equal(CommandType.Graph, result.Type);
        Assert.Equal(14, result.GraphDays);
    }

    [Fact]
    public void Parse_GraphWithNumber_ReturnsGraphWithDays()
    {
        var result = _sut.Parse("graph 30");
        Assert.Equal(CommandType.Graph, result.Type);
        Assert.Equal(30, result.GraphDays);
    }

    [Fact]
    public void Parse_GraphWithInvalidArg_ReturnsGraphWithDefaultDays()
    {
        var result = _sut.Parse("graph abc");
        Assert.Equal(CommandType.Graph, result.Type);
        Assert.Equal(14, result.GraphDays);
    }

    [Fact]
    public void Parse_DeleteById_ReturnsDelete()
    {
        var result = _sut.Parse("delete 5");
        Assert.Equal(CommandType.Delete, result.Type);
        Assert.Equal(5, result.EntryId);
        Assert.Null(result.TargetDate);
    }

    [Fact]
    public void Parse_DeleteByDate_ReturnsDeleteWithTargetDate()
    {
        var result = _sut.Parse("delete 2026-05-17");
        Assert.Equal(CommandType.Delete, result.Type);
        Assert.Null(result.EntryId);
        Assert.Equal(new DateTime(2026, 5, 17), result.TargetDate);
    }

    [Fact]
    public void Parse_DeleteByDateWithSlash_ReturnsDeleteWithTargetDate()
    {
        var result = _sut.Parse("/delete 2026-05-17");
        Assert.Equal(CommandType.Delete, result.Type);
        Assert.Equal(new DateTime(2026, 5, 17), result.TargetDate);
    }

    [Fact]
    public void Parse_DeleteAliasDel_ReturnsDelete()
    {
        var result = _sut.Parse("del 5");
        Assert.Equal(CommandType.Delete, result.Type);
        Assert.Equal(5, result.EntryId);
    }

    [Fact]
    public void Parse_DeleteAliasRm_ReturnsDelete()
    {
        var result = _sut.Parse("rm 5");
        Assert.Equal(CommandType.Delete, result.Type);
        Assert.Equal(5, result.EntryId);
    }

    [Fact]
    public void Parse_DeleteNoArg_ReturnsUnknown()
    {
        var result = _sut.Parse("delete");
        Assert.Equal(CommandType.Unknown, result.Type);
    }

    [Fact]
    public void Parse_DeleteInvalidArg_ReturnsUnknown()
    {
        var result = _sut.Parse("delete abc");
        Assert.Equal(CommandType.Unknown, result.Type);
    }

    [Fact]
    public void Parse_Help_ReturnsHelp()
    {
        var result = _sut.Parse("help");
        Assert.Equal(CommandType.Help, result.Type);
    }

    [Fact]
    public void Parse_HelpWithSlash_ReturnsHelp()
    {
        var result = _sut.Parse("/help");
        Assert.Equal(CommandType.Help, result.Type);
    }

    [Fact]
    public void Parse_HelpAliasH_ReturnsHelp()
    {
        var result = _sut.Parse("h");
        Assert.Equal(CommandType.Help, result.Type);
    }

    [Fact]
    public void Parse_HelpAliasQuestion_ReturnsHelp()
    {
        var result = _sut.Parse("?");
        Assert.Equal(CommandType.Help, result.Type);
    }

    [Fact]
    public void Parse_Clear_ReturnsClear()
    {
        var result = _sut.Parse("clear");
        Assert.Equal(CommandType.Clear, result.Type);
    }

    [Fact]
    public void Parse_ClearWithSlash_ReturnsClear()
    {
        var result = _sut.Parse("/clear");
        Assert.Equal(CommandType.Clear, result.Type);
    }

    [Fact]
    public void Parse_ClearAliasCls_ReturnsClear()
    {
        var result = _sut.Parse("cls");
        Assert.Equal(CommandType.Clear, result.Type);
    }

    [Fact]
    public void Parse_EmptyString_ReturnsUnknown()
    {
        var result = _sut.Parse("");
        Assert.Equal(CommandType.Unknown, result.Type);
    }

    [Fact]
    public void Parse_Whitespace_ReturnsUnknown()
    {
        var result = _sut.Parse("   ");
        Assert.Equal(CommandType.Unknown, result.Type);
    }

    [Fact]
    public void Parse_UnknownCommand_ReturnsUnknown()
    {
        var result = _sut.Parse("foobar");
        Assert.Equal(CommandType.Unknown, result.Type);
    }

    [Fact]
    public void Parse_JunkText_ReturnsUnknown()
    {
        var result = _sut.Parse("hello world");
        Assert.Equal(CommandType.Unknown, result.Type);
    }

    [Fact]
    public void Parse_NumberWithTrailingSpace_ReturnsLog()
    {
        var result = _sut.Parse("3400  ");
        Assert.Equal(CommandType.Log, result.Type);
        Assert.Equal(3400, result.Value);
    }

    [Fact]
    public void Parse_NumberWithBadDate_ReturnsUnknown()
    {
        var result = _sut.Parse("3400 @not-a-date");
        Assert.Equal(CommandType.Unknown, result.Type);
    }

    [Fact]
    public void Parse_Summary_IsCaseInsensitive()
    {
        var result = _sut.Parse("SUMMARY");
        Assert.Equal(CommandType.Summary, result.Type);
    }

    [Fact]
    public void Parse_GraphZeroDays_ReturnsDefault()
    {
        var result = _sut.Parse("graph 0");
        Assert.Equal(CommandType.Graph, result.Type);
        Assert.Equal(14, result.GraphDays);
    }

    [Fact]
    public void Parse_LargeNumber_ReturnsLog()
    {
        var result = _sut.Parse("50000");
        Assert.Equal(CommandType.Log, result.Type);
        Assert.Equal(50000, result.Value);
    }

    [Fact]
    public void Parse_NumberDoesNotMatchDeleteKeyword()
    {
        var result = _sut.Parse("5");
        Assert.Equal(CommandType.Log, result.Type);
        Assert.Equal(5, result.Value);
    }

    [Fact]
    public void Parse_DeleteByDate_AliasesWork()
    {
        var result = _sut.Parse("del 2026-05-17");
        Assert.Equal(CommandType.Delete, result.Type);
        Assert.Equal(new DateTime(2026, 5, 17), result.TargetDate);

        result = _sut.Parse("rm 2026-05-17");
        Assert.Equal(CommandType.Delete, result.Type);
        Assert.Equal(new DateTime(2026, 5, 17), result.TargetDate);
    }
}
