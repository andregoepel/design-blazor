using System.Resources;
using AndreGoepel.Design.Blazor.Resources;
using AndreGoepel.Design.Blazor.Tests.Resources;
using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace AndreGoepel.Design.Blazor.Tests;

/// <summary>
/// Exercises <see cref="LocalizedTextExtensions.LocalizedText{TMarker}(IServiceProvider, string, ResourceManager)"/>
/// against <see cref="TestMarkerStrings"/> — a marker type other than this library's own
/// <c>DesignStrings</c> — to prove the generic path isn't secretly tied to one marker type.
/// </summary>
public class LocalizedTextExtensionsTests : BunitContext
{
    private static readonly ResourceManager Fallback = new(
        typeof(TestMarkerStrings).FullName!,
        typeof(TestMarkerStrings).Assembly
    );

    [Fact]
    public void LocalizedText_WithoutRegisteredLocalizer_FallsBackToEmbeddedResource()
    {
        // Deliberately no Services.AddLocalization() — matches a host that never registered
        // localization at all, the scenario the fallback exists for.
        var provider = Services.BuildServiceProvider();

        // Act
        var result = provider.LocalizedText<TestMarkerStrings>("Greeting", Fallback);

        // Assert
        Assert.Equal("Hello from the fallback resx.", result);
    }

    [Fact]
    public void LocalizedText_WithoutRegisteredLocalizer_UnknownKey_ReturnsTheKeyItself()
    {
        var provider = Services.BuildServiceProvider();

        // Act
        var result = provider.LocalizedText<TestMarkerStrings>("NoSuchKey", Fallback);

        // Assert
        Assert.Equal("NoSuchKey", result);
    }

    [Fact]
    public void LocalizedText_WithArguments_FormatsTheResolvedString()
    {
        var provider = Services.BuildServiceProvider();

        // Act
        var result = provider.LocalizedText<TestMarkerStrings>("NoSuchKey {0}", Fallback, "X");

        // Assert
        Assert.Equal("NoSuchKey X", result);
    }

    [Fact]
    public void LocalizedText_WithLocalizationRegistered_StillResolvesTheSameMarkerType()
    {
        // AddLocalization() makes IStringLocalizer<T> resolvable for any T via the open-generic
        // registration — the value now comes from the IStringLocalizer path instead of Fallback,
        // but for a marker type whose only resx is the neutral one, the text is identical either
        // way. This proves the generic method works end-to-end through DI too, not just the
        // fallback branch exercised above.
        Services.AddLocalization();
        var provider = Services.BuildServiceProvider();

        // Act
        var result = provider.LocalizedText<TestMarkerStrings>("Greeting", Fallback);

        // Assert
        Assert.Equal("Hello from the fallback resx.", result);
    }
}
