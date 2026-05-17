using KilojouleTracker.Features.Terminal;

namespace KilojouleTracker.Tests;

public class TerminalFormatterTests
{
    private readonly TerminalFormatter _sut = new();

    [Fact]
    public void Input_PrependsChevron()
    {
        var line = _sut.Input("3400");
        Assert.Equal("> 3400", line.Text);
        Assert.Equal(TerminalLineStyle.Input, line.Style);
    }

    [Fact]
    public void Success_PrependsCheckmark()
    {
        var line = _sut.Success("Logged 3400 kJ for today");
        Assert.StartsWith("✓ ", line.Text);
        Assert.Equal(TerminalLineStyle.Success, line.Style);
    }

    [Fact]
    public void Error_PrependsX()
    {
        var line = _sut.Error("Entry not found");
        Assert.StartsWith("✗ ", line.Text);
        Assert.Equal(TerminalLineStyle.Error, line.Style);
    }

    [Fact]
    public void System_PrependsDash()
    {
        var line = _sut.System("Daily Summary");
        Assert.StartsWith("── ", line.Text);
        Assert.Equal(TerminalLineStyle.System, line.Style);
    }

    [Fact]
    public void Chart_ReturnsAsIs()
    {
        var line = _sut.Chart("  5000┤ ████");
        Assert.Equal("  5000┤ ████", line.Text);
        Assert.Equal(TerminalLineStyle.Chart, line.Style);
    }

    [Fact]
    public void Input_WithEmptyString_StillPrependsChevron()
    {
        var line = _sut.Input("");
        Assert.Equal("> ", line.Text);
    }

    [Fact]
    public void Success_MessagePreserved()
    {
        var line = _sut.Success("test message");
        Assert.Contains("test message", line.Text);
    }

    [Fact]
    public void Error_MessagePreserved()
    {
        var line = _sut.Error("error detail");
        Assert.Contains("error detail", line.Text);
    }
}
