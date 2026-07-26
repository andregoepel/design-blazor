using System.Globalization;
using System.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AndreGoepel.Design.Blazor.Resources;

/// <summary>
/// Generic version of the "resolve this library's own strings, tolerating a host that hasn't
/// registered localization" pattern that used to be copy-pasted, once per marker type, as
/// <c>DesignTextExtensions</c> (this library), <c>AppFoundationTextExtensions</c>
/// (<c>AndreGoepel.AppFoundation</c>) and <c>IdentityTextExtensions</c>
/// (<c>AndreGoepel.Marten.Identity.Blazor</c>) — all three read byte-for-byte identical except
/// for the marker type baked into their name.
/// </summary>
/// <remarks>
/// <typeparamref name="TMarker"/> is the resx-generated marker type — the same type used as
/// <c>IStringLocalizer&lt;TMarker&gt;</c>'s generic argument (e.g. <c>DesignStrings</c>,
/// <c>IdentityStrings</c>, <c>AppFoundationStrings</c>). <paramref name="fallback"/> must be the
/// <see cref="ResourceManager"/> backing that same resx pair — the one thing this method cannot
/// derive on its own, since each consumer's marker type sits in a different assembly with its
/// own resx-generated manager. Callers are expected to cache it once (a <c>private static
/// readonly ResourceManager</c> field, exactly as every existing *TextExtensions class already
/// does) rather than constructing it on every lookup.
/// <para>
/// Components must not <c>@inject IStringLocalizer&lt;TMarker&gt;</c> directly: that is a
/// required injection, so rendering a component throws on any host — or bUnit test — that never
/// registered localization for that marker type. Resolving through <see cref="IServiceProvider"/>
/// instead avoids that.
/// </para>
/// </remarks>
public static class LocalizedTextExtensions
{
    /// <summary>
    /// Looks <paramref name="key"/> up for the current UI culture. Prefers a registered
    /// <see cref="IStringLocalizer{TMarker}"/> so a host can substitute one; falls back to
    /// reading <paramref name="fallback"/>'s embedded resources directly.
    /// </summary>
    public static string LocalizedText<TMarker>(
        this IServiceProvider services,
        string key,
        ResourceManager fallback
    )
    {
        if (services.GetService<IStringLocalizer<TMarker>>() is { } localizer)
        {
            var localized = localizer[key];
            if (!localized.ResourceNotFound)
            {
                return localized.Value;
            }
        }

        // CurrentUICulture is what request localization sets per request, so the fallback
        // stays culture-aware without any DI involvement.
        return fallback.GetString(key, CultureInfo.CurrentUICulture) ?? key;
    }

    /// <inheritdoc cref="LocalizedText{TMarker}(IServiceProvider, string, ResourceManager)"/>
    /// <remarks>Formats the resolved string with <paramref name="arguments"/>.</remarks>
    public static string LocalizedText<TMarker>(
        this IServiceProvider services,
        string key,
        ResourceManager fallback,
        params object[] arguments
    ) =>
        string.Format(
            CultureInfo.CurrentCulture,
            services.LocalizedText<TMarker>(key, fallback),
            arguments
        );
}
