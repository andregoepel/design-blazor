using AndreGoepel.Design.Blazor.Components;
using Bunit;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class AppShellTests : DesignBlazorTestContext
{
    [Fact]
    public void Render_ShowsBrandNameAndDerivedInitial()
    {
        var cut = Render<AppShell>(parameters => parameters.Add(p => p.BrandName, "Acme"));

        Assert.Equal("Acme", cut.Find(".ag-brand-name").TextContent);
        Assert.Equal("A", cut.Find(".ag-brand-mark").TextContent);
    }

    [Fact]
    public void Render_HamburgerHasLocalizedAccessibleLabel()
    {
        var cut = Render<AppShell>(parameters => parameters.Add(p => p.BrandName, "Acme"));

        Assert.Equal("Toggle navigation", cut.Find(".ag-hamburger").GetAttribute("aria-label"));
    }

    [Fact]
    public void Render_InGerman_HamburgerLabelIsGerman()
    {
        UseCulture("de");

        var cut = Render<AppShell>(parameters => parameters.Add(p => p.BrandName, "Acme"));

        Assert.Equal("Navigation umschalten", cut.Find(".ag-hamburger").GetAttribute("aria-label"));
    }
}
