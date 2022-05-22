using System;
using FlexBus;
using FlexBus.Producer;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CapOptionsExtensions
{
    public static CapOptions UseProducer(this CapOptions options, Action<ProducerOptions> configure)
    {
        if (configure is null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        options.RegisterExtension(new ProducerOptionsExtension(configure));

        return options;
    }
}