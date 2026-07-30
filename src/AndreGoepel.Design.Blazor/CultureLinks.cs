using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace AndreGoepel.Design.Blazor;

/// <summary>
/// One entry of a culture switch: the upper-cased culture code, the language's
/// own name (its endonym), the culture-endpoint href, and whether it is the
/// active culture.
/// </summary>
public sealed record CultureLink(string Code, string DisplayName, string Href, bool IsActive);

/// <summary>
/// Builds the culture links behind <c>LanguageSwitcher</c>, exposed so a host
/// can render its own differently-styled switch (e.g. a statically rendered
/// public page that must not load <c>design.css</c>) without duplicating the
/// endonym / href / active-culture logic.
/// </summary>
public static class CultureLinks
{
    /// <summary>
    /// One link per supported culture, in configuration order, hrefs targeting
    /// the culture endpoint mapped by <c>UseDesignBlazorLocalization</c> with the
    /// current page as the return URL.
    /// </summary>
    public static IReadOnlyList<CultureLink> Build(
        DesignBlazorOptions options,
        NavigationManager navigation
    ) =>
        [
            .. options.SupportedCultures.Select(code => new CultureLink(
                code.ToUpperInvariant(),
                DisplayName(code),
                BuildHref(code, navigation),
                IsActive(code)
            )),
        ];

    // Endonym — the language named in itself, so someone who cannot read the
    // current language still recognises their own. Also means a host adding a
    // culture gets a correct label without touching any resource file.
    private static string DisplayName(string code)
    {
        var name = CultureInfo.GetCultureInfo(code).NativeName;
        return name.Length > 0
            ? char.ToUpper(name[0], CultureInfo.InvariantCulture) + name[1..]
            : code;
    }

    private static string BuildHref(string code, NavigationManager navigation)
    {
        // Base-relative on both counts: the href resolves against <base href>, and
        // "~/…" is expanded against the path base by Results.LocalRedirect. Rooted
        // paths would break an app hosted under a sub-path.
        var current = "~/" + navigation.ToBaseRelativePath(navigation.Uri);
        var endpoint = DesignBlazorOptions.CultureEndpointPath.TrimStart('/');
        return $"{endpoint}?c={Uri.EscapeDataString(code)}&redirect={Uri.EscapeDataString(current)}";
    }

    // Walks up the parent chain so a request culture of "de-DE" still marks the
    // supported "de" as active.
    private static bool IsActive(string code)
    {
        for (
            var culture = CultureInfo.CurrentUICulture;
            !string.IsNullOrEmpty(culture.Name);
            culture = culture.Parent
        )
        {
            if (string.Equals(culture.Name, code, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
