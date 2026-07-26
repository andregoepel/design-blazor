using AndreGoepel.Design.Blazor.Tests.Resources;
using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace AndreGoepel.Design.Blazor.Tests.Components;

/// <summary>
/// Exercises <c>LocalizedComponentBase&lt;TMarker&gt;</c> via <see cref="TestLocalizedComponent"/>
/// — closed over <see cref="TestMarkerStrings"/>, a marker type other than this library's own
/// <c>DesignStrings</c> — to prove the generic base isn't secretly tied to one marker type.
/// </summary>
public class LocalizedComponentBaseOfTTests : BunitContext
{
    [Fact]
    public void T_WithoutRegisteredLocalizer_ResolvesFromEmbeddedResource()
    {
        // Act
        var cut = Render<TestLocalizedComponent>(parameters =>
            parameters.Add(p => p.Key, "Greeting")
        );

        // Assert
        Assert.Equal("Hello from the fallback resx.", cut.Find("span").TextContent);
    }

    [Fact]
    public void T_UnknownKey_ReturnsTheKeyItself()
    {
        // Act
        var cut = Render<TestLocalizedComponent>(parameters =>
            parameters.Add(p => p.Key, "NoSuchKey")
        );

        // Assert
        Assert.Equal("NoSuchKey", cut.Find("span").TextContent);
    }

    [Fact]
    public void T_WithLocalizationRegistered_StillResolvesTheSameMarkerType()
    {
        Services.AddLocalization();

        // Act
        var cut = Render<TestLocalizedComponent>(parameters =>
            parameters.Add(p => p.Key, "Greeting")
        );

        // Assert
        Assert.Equal("Hello from the fallback resx.", cut.Find("span").TextContent);
    }
}
