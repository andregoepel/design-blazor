using AndreGoepel.Design.Blazor.Resources;
using Microsoft.AspNetCore.Components;

namespace AndreGoepel.Design.Blazor.Components;

/// <summary>
/// Base for design-system components that render their own text via <see cref="T(string)"/>
/// instead of a component-local copy of the same helper.
/// </summary>
/// <remarks>
/// A component must not <c>@inject IStringLocalizer&lt;DesignStrings&gt;</c> directly: that is
/// a required injection, so rendering it throws on any host — or bUnit test — that never called
/// <c>AddDesignBlazor</c>. Resolving through <see cref="IServiceProvider"/> instead avoids that,
/// which is why every localized component needs the same pair of methods; this base class is
/// the one place that pair is defined. Inherit it with <c>@inherits LocalizedComponentBase</c>
/// rather than repeating <c>@inject IServiceProvider Services</c> + the two <c>T</c> overloads
/// in each component.
/// <para>
/// Public rather than internal: the Razor compiler generates every component in this packable
/// class library as a public partial class, and a public class cannot derive from an internal
/// base (CS0060). Not intended for a host to inherit from — it exists purely so this library's
/// own components share one implementation.
/// </para>
/// </remarks>
public abstract class LocalizedComponentBase : ComponentBase
{
    [Inject]
    private IServiceProvider Services { get; set; } = default!;

    /// <summary>Looks up <paramref name="key"/> for the current UI culture.</summary>
    protected string T(string key) => Services.DesignText(key);

    /// <inheritdoc cref="T(string)"/>
    /// <remarks>Formats the resolved string with <paramref name="arguments"/>.</remarks>
    protected string T(string key, params object[] arguments) =>
        Services.DesignText(key, arguments);
}
