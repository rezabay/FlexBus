using System.Threading.Tasks;
using Fooreco.CAP;
using Fooreco.Cap.Common.Demo;

namespace Fooreco.Cap.Consumer.Demo.Commands;

public class SendEmailSubscriber : ICapSubscribe
{
    [CapSubscribe("SendEmailCommand")]
    public Task ProcessAsync(SendEmailCommand command)
    {
        return Task.Delay(2000);
    }
}