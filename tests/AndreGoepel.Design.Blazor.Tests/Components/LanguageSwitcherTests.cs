using AndreGoepel.Design.Blazor.Components;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace AndreGoepel.Design.Blazor.Tests.Components;

public class LanguageSwitcherTests : DesignBlazorTestContext
{
    private void Configure(Action<DesignBlazorOptions>? configure = null) =>
        Services.AddDesignBlazor(configure);

    [Fact]
    public void Render_ShowsOneItemPerConfiguredCulture()
    {
        Configure();

        var cut = Render<LanguageSwitcher>();

        var links = cut.FindAll(".ag-lang-toggle a");
        Assert.Equal(2, links.Count);
        Assert.Equal("EN", links[0].TextContent);
        Assert.Equal("DE", links[1].TextContent);
    }

    [Fact]
    public void Render_LabelsLanguagesWithTheirOwnName()
    {
        Configure();

        var cut = Render<LanguageSwitcher>();

        var links = cut.FindAll(".ag-lang-toggle a");
        Assert.Equal("English", links[0].GetAttribute("title"));
        Assert.Equal("Deutsch", links[1].GetAttribute("title"));
    }

    [Fact]
    public void Render_MarksTheCurrentCultureActive()
    {
        Configure();

        var cut = Render<LanguageSwitcher>();

        var links = cut.FindAll(".ag-lang-toggle a");
        Assert.Contains("ag-active", links[0].ClassName);
        Assert.Equal("true", links[0].GetAttribute("aria-current"));
        Assert.DoesNotContain("ag-active", links[1].ClassName);
        Assert.Null(links[1].GetAttribute("aria-current"));
    }

    [Fact]
    public void Render_InGerman_MarksGermanActiveAndLocalizesTheGroupLabel()
    {
        UseCulture("de");
        Configure();

        var cut = Render<LanguageSwitcher>();

        Assert.Equal("Sprache", cut.Find(".ag-lang-toggle").GetAttribute("aria-label"));
        Assert.Contains("ag-active", cut.FindAll(".ag-lang-toggle a")[1].ClassName);
    }

    [Fact]
    public void Render_WithRegionalCulture_StillMatchesTheSupportedParent()
    {
        // A request culture of de-DE has to light up the supported "de" — otherwise
        // no item would appear active for most German browsers.
        UseCulture("de-DE");
        Configure();

        var cut = Render<LanguageSwitcher>();

        Assert.Contains("ag-active", cut.FindAll(".ag-lang-toggle a")[1].ClassName);
    }

    [Fact]
    public void Render_LinksToTheCultureEndpointWithTheCurrentPageAsReturnUrl()
    {
        Configure();
        Services.GetRequiredService<NavigationManager>().NavigateTo("settings?tab=general");

        var cut = Render<LanguageSwitcher>();

        // The return URL has to stay percent-encoded — an unescaped "?" would make
        // the page's own query string part of the endpoint's.
        var href = cut.FindAll(".ag-lang-toggle a")[1].GetAttribute("href");
        Assert.Equal("ag-culture?c=de&redirect=~%2Fsettings%3Ftab%3Dgeneral", href);
    }

    [Fact]
    public void Render_HrefIsRelativeSoAPathBaseIsPreserved()
    {
        Configure();

        var cut = Render<LanguageSwitcher>();

        // A rooted "/ag-culture" would bypass <base href> and 404 for an app
        // hosted under a sub-path.
        Assert.All(
            cut.FindAll(".ag-lang-toggle a"),
            a => Assert.StartsWith("ag-culture?", a.GetAttribute("href")!)
        );
    }

    [Fact]
    public void Render_OptsOutOfEnhancedNavigation()
    {
        // Enhanced navigation would morph in the response without rebuilding the
        // circuit, leaving the UI rendering in the previous culture.
        Configure();

        var cut = Render<LanguageSwitcher>();

        Assert.All(
            cut.FindAll(".ag-lang-toggle a"),
            a => Assert.Equal("false", a.GetAttribute("data-enhance-nav"))
        );
    }

    [Fact]
    public void Render_WithExtraCulture_ShowsIt()
    {
        Configure(o => o.SupportedCultures = ["en", "de", "fr"]);

        var cut = Render<LanguageSwitcher>();

        var links = cut.FindAll(".ag-lang-toggle a");
        Assert.Equal(3, links.Count);
        Assert.Equal("FR", links[2].TextContent);
        // Endonyms come from the culture itself, so a new language needs no resource.
        Assert.Equal("Français", links[2].GetAttribute("title"));
    }
}
