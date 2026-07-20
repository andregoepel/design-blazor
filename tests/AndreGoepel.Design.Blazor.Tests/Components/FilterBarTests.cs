using AndreGoepel.Design.Blazor.Components;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Radzen;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class FilterBarTests : DesignBlazorTestContext
{
    public FilterBarTests() => Services.AddRadzenComponents();

    [Fact]
    public void Render_RendersChildContentFields()
    {
        var cut = Render<FilterBar>(parameters =>
            parameters.AddChildContent("<input class=\"field\" />")
        );

        Assert.NotNull(cut.Find("input.field"));
    }

    [Fact]
    public void Render_ShowsApplyButtonWithDefaultText()
    {
        var cut = Render<FilterBar>();

        Assert.Contains(cut.FindAll("button"), b => b.TextContent.Contains("Apply"));
    }

    [Fact]
    public void Render_WithCustomApplyText_UsesIt()
    {
        var cut = Render<FilterBar>(parameters => parameters.Add(p => p.ApplyText, "Search"));

        Assert.Contains(cut.FindAll("button"), b => b.TextContent.Contains("Search"));
    }

    [Fact]
    public void ClickingApply_RaisesOnApply()
    {
        var applied = false;
        var cut = Render<FilterBar>(parameters =>
            parameters.Add(p => p.OnApply, EventCallback.Factory.Create(this, () => applied = true))
        );

        cut.FindAll("button").First(b => b.TextContent.Contains("Apply")).Click();

        Assert.True(applied);
    }

    [Fact]
    public void Render_WithActions_RendersThemAfterApply()
    {
        var cut = Render<FilterBar>(parameters =>
            parameters.Add(p => p.Actions, "<button class=\"reset\">Reset</button>")
        );

        Assert.NotNull(cut.Find("button.reset"));
    }
}
