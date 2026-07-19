using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AndreGoepel.Design.Blazor;

/// <summary>DI registration for the design-system services.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the design-system services (currently <see cref="ConfirmService"/>).
    /// Call <b>after</b> <c>AddRadzenComponents()</c> — the services here build on
    /// Radzen's scoped services (e.g. <c>DialogService</c>).
    /// </summary>
    public static IServiceCollection AddDesignBlazor(this IServiceCollection services)
    {
        // Scoped to match DialogService's lifetime (per Blazor Server circuit).
        services.TryAddScoped<ConfirmService>();
        return services;
    }
}
