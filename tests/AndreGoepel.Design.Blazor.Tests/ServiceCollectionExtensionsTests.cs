using AndreGoepel.Design.Blazor;
using Microsoft.Extensions.DependencyInjection;
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
}
