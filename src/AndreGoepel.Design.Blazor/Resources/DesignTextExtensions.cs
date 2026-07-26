using System.Resources;

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
/// <para>
/// A thin wrapper over the generic <see cref="LocalizedTextExtensions.LocalizedText{TMarker}(IServiceProvider, string, ResourceManager)"/>,
/// closed over <see cref="DesignStrings"/>, so this library's own call sites
/// (<c>Services.DesignText("Key")</c>, and every component's <c>T(key)</c> via
/// <see cref="AndreGoepel.Design.Blazor.Components.LocalizedComponentBase"/>) don't need to change.
/// </para>
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
    internal static string DesignText(this IServiceProvider services, string key) =>
        services.LocalizedText<DesignStrings>(key, Fallback);

    /// <inheritdoc cref="DesignText(IServiceProvider, string)"/>
    internal static string DesignText(
        this IServiceProvider services,
        string key,
        params object[] arguments
    ) => services.LocalizedText<DesignStrings>(key, Fallback, arguments);
}
