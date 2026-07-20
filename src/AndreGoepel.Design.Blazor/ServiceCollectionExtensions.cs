using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace AndreGoepel.Design.Blazor;

/// <summary>DI registration for the design-system services.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the design-system services (currently <see cref="ConfirmService"/>),
    /// localization for the components' own strings, and, optionally, configures
    /// <see cref="DesignBlazorOptions"/> (e.g. the brand name used by <c>AppPageTitle</c>
    /// or the supported cultures). Call <b>after</b> <c>AddRadzenComponents()</c> — the
    /// services here build on Radzen's scoped services (e.g. <c>DialogService</c>).
    /// </summary>
    /// <remarks>
    /// Pair this with <c>app.UseDesignBlazorLocalization()</c>, which activates the
    /// request-localization middleware and maps the culture-switch endpoint. Without
    /// it the strings still resolve, but every request stays on the default culture.
    /// </remarks>
    public static IServiceCollection AddDesignBlazor(
        this IServiceCollection services,
        Action<DesignBlazorOptions>? configure = null
    )
    {
        // Scoped to match DialogService's lifetime (per Blazor Server circuit).
        services.TryAddScoped<ConfirmService>();

        var options = services.AddOptions<DesignBlazorOptions>();
        if (configure is not null)
        {
            options.Configure(configure);
        }

        // Backs IStringLocalizer<DesignStrings> for the components' built-in text.
        // No ResourcesPath: DesignStrings' full name already matches the embedded
        // resource path, and the setting is global — claiming it here would break
        // hosts that use a different layout for their own resources.
        services.AddLocalization();

        services
            .AddOptions<RequestLocalizationOptions>()
            .Configure<IOptions<DesignBlazorOptions>>(
                (localization, design) => Configure(localization, design.Value)
            );

        return services;
    }

    private static void Configure(
        RequestLocalizationOptions localization,
        DesignBlazorOptions design
    )
    {
        var supported = design.SupportedCultures.Select(CultureInfo.GetCultureInfo).ToList();

        // An empty list would make RequestLocalizationMiddleware reject every
        // culture; fall back to the default so the app still starts.
        if (supported.Count == 0)
        {
            supported.Add(CultureInfo.GetCultureInfo(design.DefaultCulture));
        }

        localization.SetDefaultCulture(design.DefaultCulture);
        localization.SupportedCultures = supported;
        localization.SupportedUICultures = supported;
        localization.ApplyCurrentCultureToResponseHeaders = true;

        // Cookie only — deliberately dropping the framework's Accept-Language and
        // query-string providers. Issue #25 specifies English until the visitor
        // chooses otherwise, whereas Accept-Language would silently open a German
        // browser in German and make the default culture unobservable.
        localization.RequestCultureProviders = [new CookieRequestCultureProvider()];
    }
}
