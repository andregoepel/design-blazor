using AndreGoepel.Design.Blazor.Components;
using Bunit;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class StatTileTests : BunitContext
{
    [Fact]
    public void Render_ShowsLabelAndValue()
    {
        var cut = Render<StatTile>(parameters =>
            parameters.Add(p => p.Label, "Income").Add(p => p.Value, "€1,234.56")
        );

        Assert.Contains("Income", cut.Markup);
        Assert.Contains("€1,234.56", cut.Markup);
    }

    [Fact]
    public void Render_DefaultVariant_LeavesValueUncoloured()
    {
        var cut = Render<StatTile>(parameters =>
            parameters.Add(p => p.Label, "Income").Add(p => p.Value, "€1,234.56")
        );

        // The value <h5> has no explicit style attribute (inherits the default colour).
        Assert.Null(cut.Find("h5").GetAttribute("style"));
    }

    [Theory]
    [InlineData(BadgeVariant.Success, "var(--ag-accent-text)")]
    [InlineData(BadgeVariant.Danger, "var(--ag-danger)")]
    [InlineData(BadgeVariant.Warning, "var(--ag-warn)")]
    [InlineData(BadgeVariant.Info, "var(--ag-info)")]
    public void Render_ColouredVariant_AppliesExpectedColour(
        BadgeVariant variant,
        string expectedVar
    )
    {
        var cut = Render<StatTile>(parameters =>
            parameters
                .Add(p => p.Label, "Income")
                .Add(p => p.Value, "€1,234.56")
                .Add(p => p.Variant, variant)
        );

        Assert.Contains(expectedVar, cut.Find("h5").GetAttribute("style"));
    }

    [Fact]
    public void Render_WithIcon_ShowsIconNextToLabel()
    {
        var cut = Render<StatTile>(parameters =>
            parameters
                .Add(p => p.Label, "Income")
                .Add(p => p.Value, "€1,234.56")
                .Add(p => p.Icon, "search")
        );

        Assert.NotNull(cut.Find("svg"));
    }

    [Fact]
    public void Render_WithoutIcon_OmitsSvg()
    {
        var cut = Render<StatTile>(parameters =>
            parameters.Add(p => p.Label, "Income").Add(p => p.Value, "€1,234.56")
        );

        Assert.Empty(cut.FindAll("svg"));
    }
}
