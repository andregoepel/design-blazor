using AndreGoepel.Design.Blazor.Components;
using Bunit;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class PageHeaderTests : BunitContext
{
    [Fact]
    public void Render_TitleOnly_ShowsHeadingAndNoSubtitleOrActions()
    {
        // Act
        var cut = Render<PageHeader>(parameters => parameters.Add(p => p.Title, "Roles"));

        // Assert
        Assert.Equal("Roles", cut.Find("h1").TextContent);
        Assert.Empty(cut.FindAll("p.rz-text-body2"));
        Assert.Empty(cut.FindAll(".rz-stack"));
    }

    [Fact]
    public void Render_WithSubtitle_ShowsSubtitleText()
    {
        // Act
        var cut = Render<PageHeader>(parameters =>
            parameters.Add(p => p.Title, "Roles").Add(p => p.Subtitle, "Manage role assignments.")
        );

        // Assert
        Assert.Contains("Manage role assignments.", cut.Markup);
    }

    [Fact]
    public void Render_WithoutSubtitle_OmitsSubtitleParagraph()
    {
        // Act
        var cut = Render<PageHeader>(parameters => parameters.Add(p => p.Title, "Roles"));

        // Assert
        Assert.DoesNotContain("rz-text-body2", cut.Markup);
    }

    [Fact]
    public void Render_WithActions_RendersActionContent()
    {
        // Act
        var cut = Render<PageHeader>(parameters =>
            parameters.Add(p => p.Title, "Roles").Add(p => p.Actions, "<button>New role</button>")
        );

        // Assert
        Assert.Equal("New role", cut.Find("button").TextContent);
    }

    [Fact]
    public void Render_UsesAgPageHeadClass()
    {
        // Act
        var cut = Render<PageHeader>(parameters => parameters.Add(p => p.Title, "Roles"));

        // Assert
        Assert.NotNull(cut.Find(".ag-page-head"));
    }
}
