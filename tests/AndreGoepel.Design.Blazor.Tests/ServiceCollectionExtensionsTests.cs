using AndreGoepel.Design.Blazor;
using AndreGoepel.Design.Blazor.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace AndreGoepel.Design.Blazor.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddDesignBlazor_RegistersConfirmServiceAsScoped()
    {
        var services = new ServiceCollection();

        services.AddDesignBlazor();

        var descriptor = Assert.Single(services, s => s.ServiceType == typeof(ConfirmService));
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void AddDesignBlazor_IsIdempotent()
    {
        var services = new ServiceCollection();

        services.AddDesignBlazor();
        services.AddDesignBlazor();

        Assert.Single(services, s => s.ServiceType == typeof(ConfirmService));
    }

    [Fact]
    public void AddDesignBlazor_WithConfigure_SetsBrandName()
    {
        var services = new ServiceCollection();

        services.AddDesignBlazor(o => o.BrandName = "Acme");

        var options = services
            .BuildServiceProvider()
            .GetRequiredService<IOptions<DesignBlazorOptions>>();
        Assert.Equal("Acme", options.Value.BrandName);
    }

    [Fact]
    public void AddDesignBlazor_WithoutConfigure_ResolvesOptionsWithNullBrandName()
    {
        var services = new ServiceCollection();

        services.AddDesignBlazor();

        var options = services
            .BuildServiceProvider()
            .GetRequiredService<IOptions<DesignBlazorOptions>>();
        Assert.Null(options.Value.BrandName);
    }

    [Fact]
    public void AddDesignBlazor_RegistersTheDesignSystemLocalizer()
    {
        var services = new ServiceCollection();

        services.AddDesignBlazor();

        // ResourceManagerStringLocalizerFactory needs an ILoggerFactory, which a
        // real host always has but a bare ServiceCollection does not.
        var localizer = services
            .AddLogging()
            .BuildServiceProvider()
            .GetRequiredService<IStringLocalizer<DesignStrings>>();
        Assert.Equal("Search…", localizer["GridToolbar.SearchPlaceholder"]);
    }

    [Fact]
    public void AddDesignBlazor_ConfiguresRequestLocalizationFromTheOptions()
    {
        var services = new ServiceCollection();

        services.AddDesignBlazor(o =>
        {
            o.DefaultCulture = "de";
            o.SupportedCultures = ["de", "en", "fr"];
        });

        var localization = Resolve(services);
        Assert.Equal("de", localization.DefaultRequestCulture.UICulture.Name);
        Assert.Equal(["de", "en", "fr"], localization.SupportedUICultures!.Select(c => c.Name));
    }

    [Fact]
    public void AddDesignBlazor_PrefersTheCookieOverTheBrowsersPreference()
    {
        // Order decides precedence: a visitor who explicitly picked a language must
        // not be overruled by Accept-Language on the next request.
        var services = new ServiceCollection();

        services.AddDesignBlazor();

        var providers = Resolve(services).RequestCultureProviders;
        Assert.Collection(
            providers,
            p => Assert.IsType<CookieRequestCultureProvider>(p),
            p => Assert.IsType<AcceptLanguageHeaderRequestCultureProvider>(p)
        );
    }

    [Fact]
    public void AddDesignBlazor_WithNoSupportedCultures_StillStarts()
    {
        // An empty list would make the middleware reject every culture.
        var services = new ServiceCollection();

        services.AddDesignBlazor(o => o.SupportedCultures = []);

        var localization = Resolve(services);
        Assert.Equal(["en"], localization.SupportedUICultures!.Select(c => c.Name));
    }

    private static RequestLocalizationOptions Resolve(IServiceCollection services) =>
        services
            .BuildServiceProvider()
            .GetRequiredService<IOptions<RequestLocalizationOptions>>()
            .Value;
}
