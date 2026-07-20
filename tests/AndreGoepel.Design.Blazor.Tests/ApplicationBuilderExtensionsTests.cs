using AndreGoepel.Design.Blazor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AndreGoepel.Design.Blazor.Tests;

/// <summary>
/// Drives the culture endpoint's handler directly. It is reached through a real
/// pipeline in the app, but the behaviour worth pinning down — which culture is
/// persisted and where the visitor is sent — is all observable from the response.
/// </summary>
public class ApplicationBuilderExtensionsTests
{
    private static async Task<HttpContext> SetCultureAsync(
        string? culture,
        string? redirect = null,
        Action<DesignBlazorOptions>? configure = null
    )
    {
        var design = new DesignBlazorOptions();
        configure?.Invoke(design);

        var http = new DefaultHttpContext();
        // LocalRedirect resolves "~/" against the request's path base, and writing
        // the response needs the endpoint's services.
        http.RequestServices = new ServiceCollection().AddLogging().BuildServiceProvider();
        http.Response.Body = new MemoryStream();

        var result = ApplicationBuilderExtensions.SetCulture(
            http,
            Options.Create(design),
            culture,
            redirect
        );
        await result.ExecuteAsync(http);

        return http;
    }

    private static string? CookieCulture(HttpContext http)
    {
        var cookie = http.Response.Headers.SetCookie.FirstOrDefault(value =>
            value?.StartsWith(CookieRequestCultureProvider.DefaultCookieName) == true
        );
        if (cookie is null)
        {
            return null;
        }

        var value = Uri.UnescapeDataString(cookie.Split(';')[0].Split('=', 2)[1]);
        return CookieRequestCultureProvider.ParseCookieValue(value)?.UICultures[0].Value;
    }

    [Fact]
    public async Task SetCulture_WithSupportedCulture_PersistsIt()
    {
        var http = await SetCultureAsync("de");

        Assert.Equal("de", CookieCulture(http));
    }

    [Fact]
    public async Task SetCulture_WithSupportedCulture_RedirectsToTheReturnUrl()
    {
        var http = await SetCultureAsync("de", "~/settings");

        Assert.Equal(StatusCodes.Status302Found, http.Response.StatusCode);
        Assert.Equal("/settings", http.Response.Headers.Location);
    }

    [Fact]
    public async Task SetCulture_WithUnsupportedCulture_FallsBackToTheDefault()
    {
        // Persisting an unsupported culture would leave a cookie that the
        // middleware rejects on every later request — silently doing nothing.
        var http = await SetCultureAsync("fr");

        Assert.Equal("en", CookieCulture(http));
    }

    [Fact]
    public async Task SetCulture_MatchesTheCultureCaseInsensitively()
    {
        var http = await SetCultureAsync("DE");

        Assert.Equal("de", CookieCulture(http));
    }

    [Fact]
    public async Task SetCulture_WithConfiguredCultures_AcceptsThem()
    {
        var http = await SetCultureAsync(
            "fr",
            configure: o => o.SupportedCultures = ["en", "de", "fr"]
        );

        Assert.Equal("fr", CookieCulture(http));
    }

    [Theory]
    [InlineData("https://evil.example")]
    [InlineData("//evil.example")]
    [InlineData("/\\evil.example")]
    [InlineData("javascript:alert(1)")]
    public async Task SetCulture_WithNonLocalReturnUrl_RedirectsToTheAppRoot(string redirect)
    {
        var http = await SetCultureAsync("de", redirect);

        Assert.Equal("/", http.Response.Headers.Location);
    }

    [Fact]
    public async Task SetCulture_WithoutReturnUrl_RedirectsToTheAppRoot()
    {
        var http = await SetCultureAsync("de");

        Assert.Equal("/", http.Response.Headers.Location);
    }

    [Fact]
    public async Task SetCulture_CookieSurvivesTheSession()
    {
        var http = await SetCultureAsync("de");

        var cookie = Assert.Single(http.Response.Headers.SetCookie!);
        Assert.Contains("expires=", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("path=/", cookie, StringComparison.OrdinalIgnoreCase);
    }
}
