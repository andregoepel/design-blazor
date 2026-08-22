using AndreGoepel.Design.Blazor.Components;
using Bunit;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class ReconnectModalTests : DesignBlazorTestContext
{
    [Fact]
    public void Render_ShowsDialogWithExpectedId()
    {
        // Act
        var cut = Render<ReconnectModal>();

        // Assert
        Assert.NotNull(cut.Find("dialog#components-reconnect-modal"));
    }

    [Fact]
    public void Render_ShowsRejoiningText()
    {
        // Act
        var cut = Render<ReconnectModal>();

        // Assert
        Assert.Contains("Rejoining the server...", cut.Markup);
    }

    [Fact]
    public void Render_ShowsRetryButtonWithFailedText()
    {
        // Act
        var cut = Render<ReconnectModal>();

        // Assert
        var button = cut.Find("button#components-reconnect-button");
        Assert.Contains("Retry", button.TextContent);
        Assert.Contains("components-reconnect-failed-visible", button.ClassList);
    }

    [Fact]
    public void Render_ShowsResumeButtonVisibleInPausedAndResumeFailedStates()
    {
        // Act
        var cut = Render<ReconnectModal>();

        // Assert
        var button = cut.Find("button#components-resume-button");
        Assert.Contains("Resume", button.TextContent);
        Assert.Contains("components-pause-visible", button.ClassList);
        Assert.Contains("components-resume-failed-visible", button.ClassList);
    }

    [Fact]
    public void Render_ShowsPausedAndResumeFailedMessages()
    {
        // Act
        var cut = Render<ReconnectModal>();

        // Assert
        Assert.Contains("The session has been paused by the server.", cut.Markup);
        Assert.Contains("Failed to resume the session.", cut.Markup);
    }

    [Fact]
    public void Render_InGerman_ShowsGermanText()
    {
        using var _ = new CultureScope("de");

        // Act
        var cut = Render<ReconnectModal>();

        // Assert
        Assert.Contains("Verbindung zum Server wird wiederhergestellt", cut.Markup);
        Assert.Contains("Erneut versuchen", cut.Markup);
        Assert.Contains("Fortsetzen", cut.Markup);
    }
}
