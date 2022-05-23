using System;
using System.Threading;
using System.Threading.Tasks;
using FlexBus.Common.Demo;

namespace FlexBus.Consumer.Demo.Commands;

public class SendEmailSubscriber : IFlexBusSubscriber
{
    public string Topic => "SendEmailCommand";
    public Type MessageType { get; } = typeof(SendEmailCommand);

    public Task ProcessAsync(object obj, CancellationToken cancellationToken)
    {
        return Task.Delay(2000, cancellationToken);
    }
}