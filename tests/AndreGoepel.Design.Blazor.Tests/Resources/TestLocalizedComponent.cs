using AndreGoepel.Design.Blazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace AndreGoepel.Design.Blazor.Tests.Resources;

/// <summary>
/// Minimal component double for testing <see cref="LocalizedComponentBase{TMarker}"/> closed over
/// <see cref="TestMarkerStrings"/> — the marker type a "different library" would use. Hand-written
/// (rather than a <c>.razor</c> file) since the test project targets the plain SDK, not the Razor
/// SDK.
/// </summary>
internal sealed class TestLocalizedComponent : LocalizedComponentBase<TestMarkerStrings>
{
    [Parameter]
    public string Key { get; set; } = "";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "span");
        builder.AddContent(1, T(Key));
        builder.CloseElement();
    }
}
