using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;
using FlexBus.Transport;

namespace FlexBus;

public class FlexBusHealthCheck : IHealthCheck
{
    private readonly ITransport _transport;

    public FlexBusHealthCheck(ITransport transport)
    {
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (await _transport.IsConnected())
            {
                return HealthCheckResult.Healthy($"{_transport.BrokerAddress.Name} connection established.");
            }
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(status: context.Registration.FailureStatus,
                description: $"Unable to connect to {_transport.BrokerAddress.Name}.",
                exception: ex);
        }

        return HealthCheckResult.Unhealthy($"{_transport.BrokerAddress.Name} connection failure.");
    }
}