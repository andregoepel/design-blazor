using AndreGoepel.Design.Blazor.Components;
using Bunit;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class DataCardTests : BunitContext
{
    [Fact]
    public void Render_RendersChildContentInsideRadzenCard()
    {
        // Act
        var cut = Render<DataCard>(parameters => parameters.AddChildContent("<span>grid</span>"));

        // Assert
        var card = cut.Find(".rz-card");
        Assert.Equal("grid", card.QuerySelector("span")?.TextContent);
    }

    [Fact]
    public void Render_UsesFlushPadding()
    {
        // Act
        var cut = Render<DataCard>(parameters => parameters.AddChildContent("content"));

        // Assert
        var style = cut.Find(".rz-card").GetAttribute("style");
        Assert.Contains("padding: 0", style);
        Assert.Contains("overflow: hidden", style);
    }
}
