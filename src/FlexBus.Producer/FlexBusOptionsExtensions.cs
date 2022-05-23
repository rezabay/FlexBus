using System;
using FlexBus;
using FlexBus.Producer;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class FlexBusOptionsExtensions
{
    public static FlexBusOptions UseProducer(this FlexBusOptions options, Action<ProducerOptions> configure)
    {
        if (configure is null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        options.RegisterExtension(new ProducerOptionsExtension(configure));

        return options;
    }
}