using AndreGoepel.Design.Blazor.Components;
using Bunit;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class StatusBadgeTests : BunitContext
{
    [Fact]
    public void Render_ShowsTextInBadge()
    {
        // Act
        var cut = Render<StatusBadge>(parameters => parameters.Add(p => p.Text, "Active"));

        // Assert
        var span = cut.Find("span.ag-badge");
        Assert.Equal("Active", span.TextContent);
    }

    [Fact]
    public void Render_DefaultVariant_IsNeutral()
    {
        // Act
        var cut = Render<StatusBadge>(parameters => parameters.Add(p => p.Text, "Draft"));

        // Assert
        Assert.NotNull(cut.Find("span.ag-badge.ag-badge-neutral"));
    }

    [Theory]
    [InlineData(BadgeVariant.Success, "ag-badge-success")]
    [InlineData(BadgeVariant.Danger, "ag-badge-danger")]
    [InlineData(BadgeVariant.Warning, "ag-badge-warn")]
    [InlineData(BadgeVariant.Info, "ag-badge-info")]
    [InlineData(BadgeVariant.Neutral, "ag-badge-neutral")]
    public void Render_MapsVariantToExpectedClass(BadgeVariant variant, string expectedClass)
    {
        // Act
        var cut = Render<StatusBadge>(parameters =>
            parameters.Add(p => p.Text, "Status").Add(p => p.Variant, variant)
        );

        // Assert
        var span = cut.Find("span.ag-badge");
        Assert.Contains(expectedClass, span.ClassList);
    }

    [Fact]
    public void Render_AlwaysCarriesBaseBadgeClass()
    {
        // Act
        var cut = Render<StatusBadge>(parameters =>
            parameters.Add(p => p.Text, "X").Add(p => p.Variant, BadgeVariant.Warning)
        );

        // Assert
        Assert.Contains("ag-badge", cut.Find("span").ClassList);
    }
}
