using System;
using FlexBus;
using FlexBus.Consumer;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class FlexBusOptionsExtensions
{
    public static FlexBusOptions UseConsumer(this FlexBusOptions options, Action<ConsumerOptions> configure)
    {
        if (configure is null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        options.RegisterExtension(new ConsumerOptionsExtension(configure));

        return options;
    }
}