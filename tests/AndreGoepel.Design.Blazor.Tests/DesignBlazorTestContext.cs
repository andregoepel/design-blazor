using System.Globalization;
using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace AndreGoepel.Design.Blazor.Tests;

/// <summary>
/// bUnit context for components that resolve their text through
/// <c>IStringLocalizer&lt;DesignStrings&gt;</c>.
/// </summary>
/// <remarks>
/// Also pins the culture to English for the duration of the test. Without that the
/// expected strings would depend on the machine's regional settings — the suite
/// would pass in CI and fail on a German workstation.
/// </remarks>
public abstract class DesignBlazorTestContext : BunitContext
{
    private readonly CultureInfo _originalCulture = CultureInfo.CurrentCulture;
    private readonly CultureInfo _originalUiCulture = CultureInfo.CurrentUICulture;

    protected DesignBlazorTestContext()
    {
        Services.AddLocalization();
        UseCulture("en");
    }

    /// <summary>Runs the rest of the test in <paramref name="culture"/>.</summary>
    protected static void UseCulture(string culture)
    {
        var info = CultureInfo.GetCultureInfo(culture);
        CultureInfo.CurrentCulture = info;
        CultureInfo.CurrentUICulture = info;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Thread-pool threads are reused across tests — leaving the culture
            // changed would make an unrelated test's result depend on run order.
            CultureInfo.CurrentCulture = _originalCulture;
            CultureInfo.CurrentUICulture = _originalUiCulture;
        }

        base.Dispose(disposing);
    }
}
