using System.Globalization;
using System.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AndreGoepel.Design.Blazor.Resources;

/// <summary>
/// Resolves the design system's own strings for the components, tolerating a host that
/// has not registered localization.
/// </summary>
/// <remarks>
/// Components must not <c>@inject IStringLocalizer&lt;DesignStrings&gt;</c> directly: that is a
/// required injection, so merely rendering a component throws on any host — or bUnit test —
/// that never called <c>AddDesignBlazor</c>. Consumers hit this on upgrade even in tests they
/// had not touched. Resolving optionally instead matches what the library already does for
/// <c>DesignBlazorOptions</c> in <c>AppPageTitle</c> and for the localizer in
/// <see cref="ConfirmService"/>.
/// </remarks>
internal static class DesignTextExtensions
{
    // Same base name the IStringLocalizer path uses, so both routes read one resx pair and
    // the English text is never duplicated in code.
    private static readonly ResourceManager Fallback = new(
        typeof(DesignStrings).FullName!,
        typeof(DesignStrings).Assembly
    );

    /// <summary>
    /// Looks <paramref name="key"/> up in the design system's resources for the current UI
    /// culture. Prefers the registered <see cref="IStringLocalizer{T}"/> so a host can
    /// substitute one; falls back to reading the embedded resources directly.
    /// </summary>
    internal static string DesignText(this IServiceProvider services, string key)
    {
        if (services.GetService<IStringLocalizer<DesignStrings>>() is { } localizer)
        {
            var localized = localizer[key];
            if (!localized.ResourceNotFound)
            {
                return localized.Value;
            }
        }

        // CurrentUICulture is what request localization sets per request, so the fallback
        // stays culture-aware without any DI involvement.
        return Fallback.GetString(key, CultureInfo.CurrentUICulture) ?? key;
    }

    /// <inheritdoc cref="DesignText(IServiceProvider, string)"/>
    internal static string DesignText(
        this IServiceProvider services,
        string key,
        params object[] arguments
    ) => string.Format(CultureInfo.CurrentCulture, services.DesignText(key), arguments);
}
