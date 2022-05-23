using System;
using FlexBus.Consumer.Internal;
using FlexBus.Consumer.Processor;
using FlexBus.Internal;
using FlexBus.Processor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FlexBus.Consumer;

internal sealed class ConsumerOptionsExtension : ICapOptionsExtension
{
    private readonly Action<ConsumerOptions> _configure;
    
    public ConsumerOptionsExtension(Action<ConsumerOptions> configure)
    {
        _configure = configure ?? throw new ArgumentNullException(nameof(configure));
    }

    public void AddServices(IServiceCollection services)
    {
        services.Configure(_configure);

        services.TryAddSingleton<IConsumerServiceSelector, ConsumerServiceSelector>();
        services.TryAddSingleton<IConsumerRegister, ConsumerRegister>();
        services.TryAddSingleton<ISubscribeDispatcher, SubscribeDispatcher>();

        services.TryAddEnumerable(ServiceDescriptor.Singleton<IProcessor, TransportCheckProcessor>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IProcessingServer, ConsumerRegister>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IProcessor, CollectorProcessor>());
    }
}