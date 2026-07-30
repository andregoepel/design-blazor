using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace AndreGoepel.Design.Blazor.Tests;

public class CultureLinksTests : DesignBlazorTestContext
{
    private NavigationManager Navigation => Services.GetRequiredService<NavigationManager>();

    [Fact]
    public void Build_ReturnsOneLinkPerSupportedCultureInConfiguredOrder()
    {
        // Arrange
        var options = new DesignBlazorOptions { SupportedCultures = ["en", "de", "fr"] };

        // Act
        var links = CultureLinks.Build(options, Navigation);

        // Assert
        Assert.Equal(["EN", "DE", "FR"], links.Select(l => l.Code));
    }

    [Fact]
    public void Build_UsesTheCapitalizedEndonymAsDisplayName()
    {
        // Arrange
        var options = new DesignBlazorOptions();

        // Act
        var links = CultureLinks.Build(options, Navigation);

        // Assert
        Assert.Equal("English", links[0].DisplayName);
        Assert.Equal("Deutsch", links[1].DisplayName);
    }

    [Fact]
    public void Build_LinksToTheCultureEndpointWithTheCurrentPageAsReturnUrl()
    {
        // Arrange
        var options = new DesignBlazorOptions();
        Navigation.NavigateTo("settings?tab=general");

        // Act
        var links = CultureLinks.Build(options, Navigation);

        // Assert — the return URL stays percent-encoded, an unescaped "?" would
        // make the page's own query string part of the endpoint's.
        Assert.Equal("ag-culture?c=de&redirect=~%2Fsettings%3Ftab%3Dgeneral", links[1].Href);
    }

    [Fact]
    public void Build_HrefIsRelativeSoAPathBaseIsPreserved()
    {
        // Arrange
        var options = new DesignBlazorOptions();

        // Act
        var links = CultureLinks.Build(options, Navigation);

        // Assert
        Assert.All(links, l => Assert.StartsWith("ag-culture?", l.Href));
    }

    [Fact]
    public void Build_MarksTheCurrentUiCultureActive()
    {
        // Arrange
        UseCulture("de");
        var options = new DesignBlazorOptions();

        // Act
        var links = CultureLinks.Build(options, Navigation);

        // Assert
        Assert.False(links[0].IsActive);
        Assert.True(links[1].IsActive);
    }

    [Fact]
    public void Build_WithRegionalCulture_StillMatchesTheSupportedParent()
    {
        // Arrange — a request culture of de-DE has to light up the supported "de".
        UseCulture("de-DE");
        var options = new DesignBlazorOptions();

        // Act
        var links = CultureLinks.Build(options, Navigation);

        // Assert
        Assert.True(links[1].IsActive);
    }
}
