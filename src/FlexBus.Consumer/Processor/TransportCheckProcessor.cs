using System;
using System.Threading.Tasks;
using FlexBus.Consumer.Internal;
using FlexBus.Processor;
using Microsoft.Extensions.Logging;

namespace FlexBus.Consumer.Processor;

public class TransportCheckProcessor : IProcessor
{
    private readonly ILogger<TransportCheckProcessor> _logger;
    private readonly IConsumerRegister _register;
    private readonly TimeSpan _waitingInterval;

    public TransportCheckProcessor(ILogger<TransportCheckProcessor> logger, IConsumerRegister register)
    {
        _logger = logger;
        _register = register;
        _waitingInterval = TimeSpan.FromSeconds(30);
    }

    public async Task ProcessAsync(ProcessingContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        _logger.LogDebug("Transport connection checking...");

        if (!_register.IsHealthy())
        {
            _logger.LogWarning("Transport connection is unhealthy, reconnection...");

            _register.Restart();
        }
        else
        {
            _logger.LogDebug("Transport connection healthy!");
        }

        await context.WaitAsync(_waitingInterval);
    }
}