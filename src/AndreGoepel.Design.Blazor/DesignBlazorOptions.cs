namespace AndreGoepel.Design.Blazor;

/// <summary>Host-level configuration for the design system, set via <c>AddDesignBlazor</c>.</summary>
public sealed class DesignBlazorOptions
{
    /// <summary>
    /// Route of the culture-switch endpoint mapped by <c>UseDesignBlazorLocalization</c>
    /// and linked to by <c>LanguageSwitcher</c>.
    /// </summary>
    public const string CultureEndpointPath = "/ag-culture";

    /// <summary>
    /// The application/brand name. When set, <c>AppPageTitle</c> appends it to the
    /// document title as "{Title} – {BrandName}" unless a page passes an explicit
    /// <c>Suffix</c>. Leave null to render plain page titles.
    /// </summary>
    public string? BrandName { get; set; }

    /// <summary>
    /// Culture used until the visitor picks one, as a two-letter code. Should also
    /// appear in <see cref="SupportedCultures"/>.
    /// </summary>
    public string DefaultCulture { get; set; } = "en";

    /// <summary>
    /// Cultures the app offers, in the order <c>LanguageSwitcher</c> renders them.
    /// Anything outside this list is rejected by the culture endpoint.
    /// </summary>
    public IList<string> SupportedCultures { get; set; } = ["en", "de"];
}
