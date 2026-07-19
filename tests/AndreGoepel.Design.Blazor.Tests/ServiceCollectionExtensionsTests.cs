using AndreGoepel.Design.Blazor;
using Microsoft.Extensions.DependencyInjection;

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
}
