using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AndreGoepel.Design.Blazor;

/// <summary>DI registration for the design-system services.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the design-system services (currently <see cref="ConfirmService"/>)
    /// and, optionally, configures <see cref="DesignBlazorOptions"/> (e.g. the brand
    /// name used by <c>AppPageTitle</c>). Call <b>after</b> <c>AddRadzenComponents()</c>
    /// — the services here build on Radzen's scoped services (e.g. <c>DialogService</c>).
    /// </summary>
    public static IServiceCollection AddDesignBlazor(
        this IServiceCollection services,
        Action<DesignBlazorOptions>? configure = null
    )
    {
        // Scoped to match DialogService's lifetime (per Blazor Server circuit).
        services.TryAddScoped<ConfirmService>();

        var options = services.AddOptions<DesignBlazorOptions>();
        if (configure is not null)
        {
            options.Configure(configure);
        }

        return services;
    }
}
