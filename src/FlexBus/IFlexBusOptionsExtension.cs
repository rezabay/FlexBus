using Microsoft.Extensions.DependencyInjection;

namespace FlexBus;

/// <summary>
/// Cap options extension
/// </summary>
public interface IFlexBusOptionsExtension
{
    /// <summary>
    /// Registered child service.
    /// </summary>
    /// <param name="services">add service to the <see cref="IServiceCollection" /></param>
    void AddServices(IServiceCollection services);
}