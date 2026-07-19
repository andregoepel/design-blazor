using AndreGoepel.Design.Blazor.Components;
using Bunit;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class AppIconTests : BunitContext
{
    [Theory]
    [InlineData("sun")]
    [InlineData("monitor")]
    [InlineData("moon")]
    [InlineData("edit")]
    [InlineData("delete")]
    [InlineData("search")]
    [InlineData("more-horizontal")]
    [InlineData("plus")]
    [InlineData("chevron-left")]
    [InlineData("chevron-right")]
    [InlineData("key")]
    [InlineData("check")]
    [InlineData("x")]
    public void Render_KnownGlyph_RendersExactlyOneSvgWithGeometry(string name)
    {
        // Act
        var cut = Render<AppIcon>(parameters => parameters.Add(p => p.Name, name));

        // Assert
        var svg = Assert.Single(cut.FindAll("svg"));
        Assert.False(string.IsNullOrWhiteSpace(svg.InnerHtml));
    }

    [Fact]
    public void Render_UnknownGlyph_RendersNothing()
    {
        // Act
        var cut = Render<AppIcon>(parameters => parameters.Add(p => p.Name, "does-not-exist"));

        // Assert
        Assert.Empty(cut.FindAll("svg"));
    }

    [Fact]
    public void Render_UsesDefaultSizeOf16()
    {
        // Act
        var cut = Render<AppIcon>(parameters => parameters.Add(p => p.Name, "sun"));

        // Assert
        var svg = cut.Find("svg");
        Assert.Equal("16", svg.GetAttribute("width"));
        Assert.Equal("16", svg.GetAttribute("height"));
    }

    [Fact]
    public void Render_WithCustomSize_ReflectsSizeOnSvg()
    {
        // Act
        var cut = Render<AppIcon>(parameters =>
            parameters.Add(p => p.Name, "sun").Add(p => p.Size, 24)
        );

        // Assert
        var svg = cut.Find("svg");
        Assert.Equal("24", svg.GetAttribute("width"));
        Assert.Equal("24", svg.GetAttribute("height"));
    }
}
