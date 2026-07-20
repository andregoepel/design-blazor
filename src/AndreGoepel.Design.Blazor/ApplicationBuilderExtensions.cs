using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AndreGoepel.Design.Blazor;

/// <summary>Request-pipeline wiring for the design system.</summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Activates request localization from the cultures configured via
    /// <c>AddDesignBlazor</c> and maps the culture-switch endpoint that
    /// <c>LanguageSwitcher</c> links to.
    /// </summary>
    /// <remarks>
    /// Call this <b>before</b> <c>MapRazorComponents</c>: the middleware has to set
    /// <c>CurrentUICulture</c> on the request that establishes a Blazor Server
    /// circuit, because that is where the circuit takes its culture from.
    /// </remarks>
    public static WebApplication UseDesignBlazorLocalization(this WebApplication app)
    {
        app.UseRequestLocalization();
        app.MapGet(DesignBlazorOptions.CultureEndpointPath, SetCulture);
        return app;
    }

    /// <summary>
    /// Persists the chosen culture in the standard localization cookie and returns
    /// the visitor to where they were. Deliberately a GET so <c>LanguageSwitcher</c>
    /// can be plain anchors and keep working without interactivity — the only state
    /// it writes is a UI preference.
    /// </summary>
    /// <remarks>
    /// Internal rather than private so the test project can drive it with a
    /// <c>DefaultHttpContext</c> — same reason as <c>AppPageTitle.Compose</c>.
    /// </remarks>
    internal static IResult SetCulture(
        HttpContext http,
        IOptions<DesignBlazorOptions> options,
        string? c,
        string? redirect
    )
    {
        var design = options.Value;

        // Never echo an arbitrary culture into the cookie: an unsupported value
        // would make RequestLocalizationMiddleware fall back on every later
        // request, leaving a cookie that quietly does nothing.
        var culture =
            design.SupportedCultures.FirstOrDefault(supported =>
                string.Equals(supported, c, StringComparison.OrdinalIgnoreCase)
            ) ?? design.DefaultCulture;

        http.Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                SameSite = SameSiteMode.Lax,
                Secure = http.Request.IsHttps,
                HttpOnly = true,
                // Exempt from cookie-consent policy: without it the preference
                // cannot be stored, so the switcher would appear to do nothing.
                IsEssential = true,
            }
        );

        // Open-redirect guard. Results.LocalRedirect would throw on a non-local
        // target, turning a hand-edited URL into a 500 — screen it ourselves and
        // fall back to the app root instead.
        return Results.LocalRedirect(IsLocalUrl(redirect) ? redirect! : "~/");
    }

    /// <summary>
    /// Mirrors <c>IUrlHelper.IsLocalUrl</c>: accepts "/path" and "~/path", rejects
    /// absolute URLs as well as "//host" and "/\host", which browsers read as
    /// protocol-relative links to another origin.
    /// </summary>
    private static bool IsLocalUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return false;
        }

        if (url[0] == '/')
        {
            return url.Length == 1 || (url[1] != '/' && url[1] != '\\');
        }

        return url.Length > 1 && url[0] == '~' && url[1] == '/';
    }
}
