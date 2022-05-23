using System.Threading.Tasks;
using FlexBus.Persistence;
using FlexBus.Messages;

namespace FlexBus.Transport;

public class Dispatcher : IDispatcher
{
    private readonly IFlexBusPublisher _publisher;

    public Dispatcher(IFlexBusPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task EnqueueToPublish(MediumMessage message)
    {
        var originMessage = message.Origin;
        await _publisher.PublishAsync(originMessage.GetName(), originMessage.Value);
    }
}