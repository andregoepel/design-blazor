namespace AndreGoepel.Design.Blazor.Resources;

/// <summary>
/// Marker type for the design system's own UI strings — the generic argument of
/// <c>IStringLocalizer&lt;DesignStrings&gt;</c>. It carries no members; the strings
/// live in <c>DesignStrings.resx</c> (English, neutral) and <c>DesignStrings.de.resx</c>
/// next to this file.
/// </summary>
/// <remarks>
/// The type sits in the <c>Resources</c> namespace on purpose: that makes its full
/// name match the embedded resource path exactly, so hosts do <b>not</b> need to set
/// <c>LocalizationOptions.ResourcesPath</c> — which is global per app and would
/// otherwise clash with the host's own convention.
/// </remarks>
public sealed class DesignStrings;
