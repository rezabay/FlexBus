using FlexBus.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace FlexBus.Test.FakeInMemoryQueue
{
    internal sealed class FakeQueueOptionsExtension : IFlexBusOptionsExtension
    {

        public void AddServices(IServiceCollection services)
        {
            services.AddSingleton<CapMessageQueueMakerService>();

            services.AddSingleton<InMemoryQueue>();
            services.AddSingleton<IConsumerClientFactory, InMemoryConsumerClientFactory>();
            services.AddSingleton<ITransport, FakeInMemoryQueueTransport>();
        }
    }
}