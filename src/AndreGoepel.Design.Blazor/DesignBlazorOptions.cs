namespace AndreGoepel.Design.Blazor;

/// <summary>Host-level configuration for the design system, set via <c>AddDesignBlazor</c>.</summary>
public sealed class DesignBlazorOptions
{
    /// <summary>
    /// The application/brand name. When set, <c>AppPageTitle</c> appends it to the
    /// document title as "{Title} – {BrandName}" unless a page passes an explicit
    /// <c>Suffix</c>. Leave null to render plain page titles.
    /// </summary>
    public string? BrandName { get; set; }
}
