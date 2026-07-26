using AndreGoepel.Design.Blazor.Components;
using Bunit;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class PageBodyTests : BunitContext
{
    [Fact]
    public void Render_RendersChildContentInsideStack()
    {
        // Act
        var cut = Render<PageBody>(parameters =>
            parameters.AddChildContent("<span>content</span>")
        );

        // Assert
        var stack = cut.Find(".rz-p-4.rz-p-md-6 .rz-stack");
        Assert.Equal("content", stack.QuerySelector("span")?.TextContent);
    }

    [Fact]
    public void Render_DefaultGap_UsesStandardValue()
    {
        // Act
        var cut = Render<PageBody>(parameters => parameters.AddChildContent("content"));

        // Assert
        var style = cut.Find(".rz-stack").GetAttribute("style");
        Assert.Contains("--rz-gap:1.5rem", style);
    }

    [Fact]
    public void Render_WithCustomGap_UsesIt()
    {
        // Act
        var cut = Render<PageBody>(parameters =>
            parameters.Add(p => p.Gap, "2rem").AddChildContent("content")
        );

        // Assert
        var style = cut.Find(".rz-stack").GetAttribute("style");
        Assert.Contains("--rz-gap:2rem", style);
    }
}
