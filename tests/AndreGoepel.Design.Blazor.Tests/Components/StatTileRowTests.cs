using AndreGoepel.Design.Blazor.Components;
using Bunit;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class StatTileRowTests : BunitContext
{
    [Fact]
    public void Render_WrapsChildrenInTheTileGrid()
    {
        var cut = Render<StatTileRow>(parameters =>
            parameters.AddChildContent<StatTile>(tile =>
                tile.Add(p => p.Label, "Income").Add(p => p.Value, "€4,250.00")
            )
        );

        var grid = cut.Find(".ag-stat-tiles");
        Assert.Contains("Income", grid.TextContent);
        Assert.Contains("€4,250.00", grid.TextContent);
    }

    [Fact]
    public void Render_Default_OmitsInlineStyleSoTheCssDefaultApplies()
    {
        var cut = Render<StatTileRow>(parameters => parameters.AddChildContent("<span>x</span>"));

        Assert.Null(cut.Find(".ag-stat-tiles").GetAttribute("style"));
    }

    [Fact]
    public void Render_WithMinTileWidth_SetsTheCssVariable()
    {
        var cut = Render<StatTileRow>(parameters =>
            parameters.Add(p => p.MinTileWidth, "10rem").AddChildContent("<span>x</span>")
        );

        Assert.Equal(
            "--ag-stat-tile-min: 10rem;",
            cut.Find(".ag-stat-tiles").GetAttribute("style")
        );
    }

    [Fact]
    public void Render_KeepsMultipleTilesAsDirectGridChildren()
    {
        // Direct children matter: each grid track is formed from an immediate
        // child, so a wrapper element would break the equal-width layout.
        var cut = Render<StatTileRow>(parameters =>
            parameters.AddChildContent("<span>a</span><span>b</span><span>c</span>")
        );

        Assert.Equal(3, cut.FindAll(".ag-stat-tiles > span").Count);
    }
}
