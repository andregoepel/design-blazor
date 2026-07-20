using System.Globalization;
using AndreGoepel.Design.Blazor.Components;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Radzen;

namespace AndreGoepel.Design.Blazor.Tests.Components;

/// <summary>
/// Renders the localized components on a context that has <b>no</b> localization registered —
/// deliberately plain <see cref="BunitContext"/>, not <c>DesignBlazorTestContext</c>.
/// </summary>
/// <remarks>
/// Regression guard for the 1.2.0 upgrade: those components injected
/// <c>IStringLocalizer&lt;DesignStrings&gt;</c> directly, which is a required injection, so
/// consuming apps saw bUnit tests they had never touched start throwing
/// "no registered service of type IStringLocalizer&lt;DesignStrings&gt;" merely from rendering a
/// CardForm. Components must degrade to the embedded resources instead.
/// </remarks>
public class NoLocalizationRegisteredTests : BunitContext
{
    public NoLocalizationRegisteredTests() => Services.AddRadzenComponents();

    private sealed class Model
    {
        public string? Name { get; set; }
    }

    [Fact]
    public void CardForm_RendersLocalizedDefaults_WithoutLocalizationRegistered()
    {
        var cut = Render<CardForm<Model>>(parameters => parameters.Add(p => p.Data, new Model()));

        Assert.Contains(
            cut.FindAll(".ag-card-actions button"),
            b => b.TextContent.Contains("Save changes")
        );
    }

    [Fact]
    public void GridToolbar_RendersLocalizedPlaceholder_WithoutLocalizationRegistered()
    {
        var cut = Render<GridToolbar>(parameters => parameters.Add(p => p.Search, ""));

        Assert.Equal("Search…", cut.Find("input.ag-search-input").GetAttribute("placeholder"));
    }

    [Fact]
    public void FilterBar_RendersLocalizedApplyText_WithoutLocalizationRegistered()
    {
        var cut = Render<FilterBar>();

        Assert.Contains(cut.FindAll("button"), b => b.TextContent.Contains("Apply"));
    }

    [Fact]
    public void AppShell_RendersLocalizedAriaLabel_WithoutLocalizationRegistered()
    {
        var cut = Render<AppShell>(parameters => parameters.Add(p => p.BrandName, "Acme"));

        Assert.Equal("Toggle navigation", cut.Find(".ag-hamburger").GetAttribute("aria-label"));
    }

    [Fact]
    public void GridToolbar_StillFollowsTheCulture_WithoutLocalizationRegistered()
    {
        // The fallback reads the satellite resources off CurrentUICulture, so dropping the
        // IStringLocalizer must not quietly pin the UI to English.
        var original = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("de");

            var cut = Render<GridToolbar>(parameters => parameters.Add(p => p.Search, ""));

            Assert.Equal("Suchen…", cut.Find("input.ag-search-input").GetAttribute("placeholder"));
        }
        finally
        {
            CultureInfo.CurrentUICulture = original;
        }
    }
}
