using AndreGoepel.Design.Blazor.Components;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Radzen;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class SettingToggleRowTests : BunitContext
{
    public SettingToggleRowTests() => Services.AddRadzenComponents();

    [Fact]
    public void Render_ShowsLabel()
    {
        var cut = Render<SettingToggleRow>(parameters => parameters.Add(p => p.Label, "Passkeys"));

        Assert.Equal("Passkeys", cut.Find(".ag-toggle-row-label").TextContent);
    }

    [Fact]
    public void Render_WithDescription_ShowsIt()
    {
        var cut = Render<SettingToggleRow>(parameters =>
            parameters.Add(p => p.Label, "Passkeys").Add(p => p.Description, "WebAuthn sign-in.")
        );

        Assert.Equal("WebAuthn sign-in.", cut.Find(".ag-toggle-row-description").TextContent);
    }

    [Fact]
    public void Render_WithoutDescription_OmitsDescriptionSpan()
    {
        var cut = Render<SettingToggleRow>(parameters => parameters.Add(p => p.Label, "Passkeys"));

        Assert.Empty(cut.FindAll(".ag-toggle-row-description"));
    }

    [Fact]
    public void Render_ReflectsValueOnSwitch()
    {
        var cut = Render<SettingToggleRow>(parameters =>
            parameters.Add(p => p.Label, "Passkeys").Add(p => p.Value, true)
        );

        Assert.Equal("true", cut.Find(".rz-switch input").GetAttribute("aria-checked"));
    }

    [Fact]
    public void Toggling_RaisesValueChangedWithNewValue()
    {
        bool? raised = null;
        var cut = Render<SettingToggleRow>(parameters =>
            parameters
                .Add(p => p.Label, "Passkeys")
                .Add(p => p.Value, false)
                .Add(p => p.ValueChanged, EventCallback.Factory.Create<bool>(this, v => raised = v))
        );

        cut.Find(".rz-switch").Click();

        Assert.True(raised);
    }

    [Fact]
    public void Render_WhenDisabled_SwitchIsDisabled()
    {
        var cut = Render<SettingToggleRow>(parameters =>
            parameters.Add(p => p.Label, "Passkeys").Add(p => p.Disabled, true)
        );

        Assert.Contains("rz-state-disabled", cut.Find(".rz-switch").ClassList);
    }
}
