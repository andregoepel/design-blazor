using System.Resources;
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
/// <para>
/// Closed over <see cref="DesignStrings"/> specifically — a different library with its own
/// marker type (e.g. <c>IdentityStrings</c>, <c>AppFoundationStrings</c>) should inherit
/// <see cref="LocalizedComponentBase{TMarker}"/> instead, not this type.
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

/// <summary>
/// Generic counterpart to <see cref="LocalizedComponentBase"/> for a <em>different</em>
/// library's own localized components — e.g. <c>AndreGoepel.Marten.Identity.Blazor</c>'s
/// <c>IdentityStrings</c> or <c>AndreGoepel.AppFoundation</c>'s <c>AppFoundationStrings</c> —
/// so those libraries can inherit this instead of re-implementing the same
/// <c>[Inject] IServiceProvider</c> + <c>T(string)</c> pair against their own resx marker type.
/// </summary>
/// <typeparam name="TMarker">
/// The resx-generated marker type — the same type used as
/// <c>IStringLocalizer&lt;TMarker&gt;</c>'s generic argument (e.g. <c>IdentityStrings</c>).
/// </typeparam>
/// <remarks>
/// The fallback <see cref="ResourceManager"/> that
/// <see cref="LocalizedTextExtensions.LocalizedText{TMarker}(IServiceProvider, string, ResourceManager)"/>
/// requires is derived from <typeparamref name="TMarker"/> once per closed generic type (a
/// <c>static readonly</c> field on a generic type is per-instantiation, not shared across
/// different <typeparamref name="TMarker"/>s) using the exact convention every existing
/// marker/fallback pair already follows: <c>new(typeof(TMarker).FullName!,
/// typeof(TMarker).Assembly)</c>. That convention only works because it is universal —
/// <c>DesignStrings</c>, <c>IdentityStrings</c> and <c>AppFoundationStrings</c> all follow it —
/// which is why it can be derived here automatically instead of asking every consumer to supply
/// it, the way the lower-level <see cref="LocalizedTextExtensions"/> methods do.
/// <para>
/// A consuming library typically wraps this in its own thin, non-generic subclass, so
/// <c>@inherits</c> doesn't need the closed generic name spelled out on every page, and so the
/// name doesn't collide with this library's own non-generic <see cref="LocalizedComponentBase"/>
/// when both namespaces are imported (a shared name would be ambiguous — CS0104):
/// <code>
/// public abstract class IdentityLocalizedComponentBase : LocalizedComponentBase&lt;IdentityStrings&gt;;
/// </code>
/// </para>
/// </remarks>
public abstract class LocalizedComponentBase<TMarker> : ComponentBase
{
    private static readonly ResourceManager Fallback = new(
        typeof(TMarker).FullName!,
        typeof(TMarker).Assembly
    );

    [Inject]
    private IServiceProvider Services { get; set; } = default!;

    /// <summary>Looks up <paramref name="key"/> for the current UI culture.</summary>
    protected string T(string key) => Services.LocalizedText<TMarker>(key, Fallback);

    /// <inheritdoc cref="T(string)"/>
    /// <remarks>Formats the resolved string with <paramref name="arguments"/>.</remarks>
    protected string T(string key, params object[] arguments) =>
        Services.LocalizedText<TMarker>(key, Fallback, arguments);
}
