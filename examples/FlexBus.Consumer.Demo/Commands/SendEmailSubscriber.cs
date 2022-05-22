using System.Threading.Tasks;
using FlexBus;
using FlexBus.Common.Demo;

namespace FlexBus.Consumer.Demo.Commands;

public class SendEmailSubscriber : ICapSubscribe
{
    [CapSubscribe("SendEmailCommand")]
    public Task ProcessAsync(SendEmailCommand command)
    {
        return Task.Delay(2000);
    }
}