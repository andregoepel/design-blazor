using AndreGoepel.Design.Blazor.Components;
using Bunit;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class SectionCardTests : BunitContext
{
    [Fact]
    public void Render_ShowsTitleAsHeading()
    {
        // Act
        var cut = Render<SectionCard>(parameters => parameters.Add(p => p.Title, "Notes"));

        // Assert
        Assert.Equal("Notes", cut.Find(".ag-card-head h2").TextContent);
    }

    [Fact]
    public void Render_UsesAgCardHeadClass()
    {
        // Act
        var cut = Render<SectionCard>(parameters => parameters.Add(p => p.Title, "Notes"));

        // Assert
        Assert.NotNull(cut.Find(".ag-card-head"));
    }

    [Fact]
    public void Render_WithoutActions_RendersNoStack()
    {
        // Act
        var cut = Render<SectionCard>(parameters => parameters.Add(p => p.Title, "Notes"));

        // Assert
        Assert.Empty(cut.FindAll(".rz-stack"));
    }

    [Fact]
    public void Render_WithActions_RendersActionContent()
    {
        // Act
        var cut = Render<SectionCard>(parameters =>
            parameters.Add(p => p.Title, "Notes").Add(p => p.Actions, "<button>New note</button>")
        );

        // Assert
        Assert.Equal("New note", cut.Find("button").TextContent);
    }

    [Fact]
    public void Render_RendersChildContentInsideCard()
    {
        // Act
        var cut = Render<SectionCard>(parameters =>
            parameters.Add(p => p.Title, "Notes").AddChildContent("<span>content</span>")
        );

        // Assert
        var card = cut.Find(".rz-card");
        Assert.Equal("content", card.QuerySelector("span")?.TextContent);
    }

    [Fact]
    public void Render_FullHeightDefaultFalse_OmitsHeightClass()
    {
        // Act
        var cut = Render<SectionCard>(parameters => parameters.Add(p => p.Title, "Notes"));

        // Assert
        Assert.DoesNotContain("rz-height-100", cut.Find(".rz-card").ClassList);
    }

    [Fact]
    public void Render_FullHeightTrue_AddsHeightClass()
    {
        // Act
        var cut = Render<SectionCard>(parameters =>
            parameters.Add(p => p.Title, "Notes").Add(p => p.FullHeight, true)
        );

        // Assert
        Assert.Contains("rz-height-100", cut.Find(".rz-card").ClassList);
    }
}
