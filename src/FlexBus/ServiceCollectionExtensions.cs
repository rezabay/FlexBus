using System;
using FlexBus;
using FlexBus.Internal;
using FlexBus.Processor;
using FlexBus.Serialization;
using FlexBus.Transport;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Contains extension methods to <see cref="IServiceCollection" /> for configuring consistence services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures the consistence services for the consistency.
    /// </summary>
    /// <param name="services">The services available in the application.</param>
    /// <param name="setupAction">An action to configure the <see cref="FlexBusOptions" />.</param>
    /// <returns>An <see cref="FlexBusBuilder" /> for application services.</returns>
    public static FlexBusBuilder AddFlexBus(this IServiceCollection services, Action<FlexBusOptions> setupAction)
    {
        if (setupAction == null)
        {
            throw new ArgumentNullException(nameof(setupAction));
        }

        services.TryAddSingleton<CapMarkerService>();
        services.TryAddSingleton<IFlexBusPublisher, FlexBusPublisher>();

        // Processors
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IProcessingServer, FlexBusProcessingServer>());

        // Sender and Executors
        services.TryAddSingleton<IDispatcher, Dispatcher>();

        services.TryAddSingleton<ISerializer, JsonUtf8Serializer>();

        // Options and extension service
        var options = new FlexBusOptions();
        setupAction(options);
        foreach (var serviceExtension in options.Extensions)
        {
            serviceExtension.AddServices(services);
        }
        services.Configure(setupAction);

        // Startup and Hosted 
        services.AddSingleton<IBootstrapper, Bootstrapper>();
        services.AddHostedService<Bootstrapper>();

        return new FlexBusBuilder(services);
    }
}