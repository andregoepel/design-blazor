namespace AndreGoepel.Design.Blazor.Tests.Resources;

/// <summary>
/// Marker type + backing resx (<c>TestMarkerStrings.resx</c>, next to this file) used only to
/// exercise <c>LocalizedTextExtensions.LocalizedText&lt;TMarker&gt;</c> and
/// <c>LocalizedComponentBase&lt;TMarker&gt;</c> against a marker type other than this library's
/// own <c>DesignStrings</c> — proving the generic path works for an arbitrary consumer, not just
/// the one marker type every other test in this suite happens to use.
/// </summary>
public sealed class TestMarkerStrings;
