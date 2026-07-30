using AndreGoepel.Design.Blazor.Components;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Radzen;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class ToggleFieldTests : BunitContext
{
    public ToggleFieldTests() => Services.AddRadzenComponents();

    [Fact]
    public void Render_Default_RendersACheckbox()
    {
        var cut = Render<ToggleField>(parameters => parameters.Add(p => p.Label, "Show archived"));

        Assert.NotNull(cut.Find(".ag-toggle-field .rz-chkbox"));
        Assert.Empty(cut.FindAll(".rz-switch"));
    }

    [Fact]
    public void Render_WithSwitch_RendersASwitch()
    {
        var cut = Render<ToggleField>(parameters =>
            parameters.Add(p => p.Label, "Compact mode").Add(p => p.Switch, true)
        );

        Assert.NotNull(cut.Find(".ag-toggle-field .rz-switch"));
        Assert.Empty(cut.FindAll(".rz-chkbox"));
    }

    [Fact]
    public void Render_ShowsLabelText()
    {
        var cut = Render<ToggleField>(parameters => parameters.Add(p => p.Label, "Show archived"));

        Assert.Equal("Show archived", cut.Find("label").TextContent.Trim());
    }

    [Fact]
    public void Render_LinksLabelToControlViaGeneratedName()
    {
        var cut = Render<ToggleField>(parameters => parameters.Add(p => p.Label, "Show archived"));

        var forAttribute = cut.Find("label").GetAttribute("for");
        Assert.NotNull(forAttribute);
        Assert.StartsWith("ag-toggle-", forAttribute);
    }

    [Fact]
    public void Render_WithExplicitName_LinksLabelToIt()
    {
        var cut = Render<ToggleField>(parameters =>
            parameters.Add(p => p.Label, "Show archived").Add(p => p.Name, "ShowArchived")
        );

        Assert.Equal("ShowArchived", cut.Find("label").GetAttribute("for"));
    }

    [Fact]
    public void Toggling_RaisesValueChangedWithNewValue()
    {
        bool? raised = null;
        var cut = Render<ToggleField>(parameters =>
            parameters
                .Add(p => p.Label, "Show archived")
                .Add(p => p.Value, false)
                .Add(p => p.ValueChanged, EventCallback.Factory.Create<bool>(this, v => raised = v))
        );

        cut.Find(".rz-chkbox").Click();

        Assert.True(raised);
    }

    [Fact]
    public void Render_WhenDisabled_ControlIsDisabled()
    {
        var cut = Render<ToggleField>(parameters =>
            parameters.Add(p => p.Label, "Show archived").Add(p => p.Disabled, true)
        );

        Assert.Contains("rz-state-disabled", cut.Find(".rz-chkbox").ClassList);
    }
}
