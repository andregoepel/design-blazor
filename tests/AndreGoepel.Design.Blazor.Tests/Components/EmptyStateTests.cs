using AndreGoepel.Design.Blazor.Components;
using Bunit;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class EmptyStateTests : BunitContext
{
    [Fact]
    public void Render_TitleOnly_ShowsTitleAndNoIconTextOrActions()
    {
        // Act
        var cut = Render<EmptyState>(parameters => parameters.Add(p => p.Title, "No results"));

        // Assert
        Assert.Equal("No results", cut.Find(".ag-empty-title").TextContent);
        Assert.Empty(cut.FindAll(".ag-empty-icon"));
        Assert.Empty(cut.FindAll(".ag-empty-text"));
    }

    [Fact]
    public void Render_WithIcon_ShowsIconGlyph()
    {
        // Act
        var cut = Render<EmptyState>(parameters =>
            parameters.Add(p => p.Title, "No passkeys").Add(p => p.Icon, "🔑")
        );

        // Assert
        Assert.Equal("🔑", cut.Find(".ag-empty-icon").TextContent);
    }

    [Fact]
    public void Render_WithText_ShowsSupportingCopy()
    {
        // Act
        var cut = Render<EmptyState>(parameters =>
            parameters
                .Add(p => p.Title, "No passkeys")
                .Add(p => p.Text, "Register one to sign in faster.")
        );

        // Assert
        Assert.Equal("Register one to sign in faster.", cut.Find(".ag-empty-text").TextContent);
    }

    [Fact]
    public void Render_WithActions_RendersActionContent()
    {
        // Act
        var cut = Render<EmptyState>(parameters =>
            parameters
                .Add(p => p.Title, "No roles")
                .Add(p => p.Actions, "<button>New role</button>")
        );

        // Assert
        Assert.Equal("New role", cut.Find("button").TextContent);
    }

    [Fact]
    public void Render_UsesAgEmptyClass()
    {
        // Act
        var cut = Render<EmptyState>(parameters => parameters.Add(p => p.Title, "No results"));

        // Assert
        Assert.NotNull(cut.Find(".ag-empty"));
    }
}
