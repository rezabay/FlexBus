using FlexBus.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace FlexBus.Test.FakeInMemoryQueue
{
    internal sealed class FakeQueueOptionsExtension : ICapOptionsExtension
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