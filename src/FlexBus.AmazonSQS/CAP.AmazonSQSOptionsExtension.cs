using System;
using FlexBus.Transport;
using FlexBus;
using Microsoft.Extensions.DependencyInjection;

namespace FlexBus.AmazonSQS;

internal sealed class AmazonSQSOptionsExtension : ICapOptionsExtension
{
    private readonly Action<AmazonSQSOptions> _configure;

    public AmazonSQSOptionsExtension(Action<AmazonSQSOptions> configure)
    {
        _configure = configure;
    }

    public void AddServices(IServiceCollection services)
    {
        services.AddSingleton<CapMessageQueueMakerService>();
             
        services.Configure(_configure);
        services.AddSingleton<ITransport, AmazonSQSTransport>();
        services.AddSingleton<IConsumerClientFactory, AmazonSQSConsumerClientFactory>();
    }
}