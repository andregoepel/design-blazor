using Bunit;

namespace AndreGoepel.Design.Blazor.Tests;

/// <summary>
/// bUnit context for components that resolve their text through
/// <c>IStringLocalizer&lt;DesignStrings&gt;</c>.
/// </summary>
/// <remarks>
/// Also pins the culture to English for the duration of the test. Without that the
/// expected strings would depend on the machine's regional settings — the suite
/// would pass in CI and fail on a German workstation. A test that needs a different
/// culture nests its own <c>using var _ = new CultureScope("de");</c> — it unwinds
/// back to English (this scope), not the machine's original culture, when it disposes.
/// </remarks>
public abstract class DesignBlazorTestContext : BunitContext
{
    private readonly CultureScope _culture = new("en");

    protected DesignBlazorTestContext() => this.UseLocalization();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _culture.Dispose();
        }

        base.Dispose(disposing);
    }
}
