using AndreGoepel.Design.Blazor.Components;
using Bunit;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class ThemeToggleTests : DesignBlazorTestContext
{
    [Fact]
    public void Render_ShowsLightAutoDarkButtons()
    {
        var cut = Render<ThemeToggle>();

        var buttons = cut.FindAll(".ag-theme-btn");
        Assert.Equal(3, buttons.Count);
        Assert.Equal("light", buttons[0].GetAttribute("data-mode"));
        Assert.Equal("auto", buttons[1].GetAttribute("data-mode"));
        Assert.Equal("dark", buttons[2].GetAttribute("data-mode"));
    }

    [Fact]
    public void Render_ButtonsCallAgThemeWithoutACircuit()
    {
        // The toggle must work on statically rendered pages (e.g. LoginShell-hosted
        // auth pages), so each button carries a plain onclick instead of a Blazor
        // event handler.
        var cut = Render<ThemeToggle>();

        var buttons = cut.FindAll(".ag-theme-btn");
        Assert.Equal("agTheme.set('light')", buttons[0].GetAttribute("onclick"));
        Assert.Equal("agTheme.set('auto')", buttons[1].GetAttribute("onclick"));
        Assert.Equal("agTheme.set('dark')", buttons[2].GetAttribute("onclick"));
    }

    [Fact]
    public void Render_ButtonsHaveLocalizedLabels()
    {
        var cut = Render<ThemeToggle>();

        Assert.Equal("Theme", cut.Find(".ag-theme-toggle").GetAttribute("aria-label"));
        var buttons = cut.FindAll(".ag-theme-btn");
        Assert.Equal("Light", buttons[0].GetAttribute("aria-label"));
        Assert.Equal("Match system", buttons[1].GetAttribute("aria-label"));
        Assert.Equal("Dark", buttons[2].GetAttribute("aria-label"));
    }

    [Fact]
    public void Render_InGerman_LabelsAreGerman()
    {
        UseCulture("de");

        var cut = Render<ThemeToggle>();

        Assert.Equal("Design", cut.Find(".ag-theme-toggle").GetAttribute("aria-label"));
        Assert.Equal("Hell", cut.FindAll(".ag-theme-btn")[0].GetAttribute("aria-label"));
    }
}
