using System;
using System.Threading;
using System.Threading.Tasks;
using FlexBus.Consumer.Internal;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FlexBus.Test
{
    public class ConsumerServiceSelectorTest
    {
        private readonly IServiceProvider _provider;

        public ConsumerServiceSelectorTest()
        {
            var services = new ServiceCollection();
            
            services.AddOptions();
            services.PostConfigure<FlexBusOptions>(_ => { });
            services.AddSingleton<IConsumerServiceSelector, ConsumerServiceSelector>();
            services.AddScoped<IFlexBusSubscriber, CandidatesFooTest>();			
            services.AddScoped<IFlexBusSubscriber, CandidatesBarTest>();
            services.AddLogging();
            _provider = services.BuildServiceProvider();
        }

        [Fact]
        public void CanFindAllConsumerService()
        {
            var selector = _provider.GetRequiredService<IConsumerServiceSelector>();
            var candidates = selector.SelectCandidates();

            Assert.Equal(2, candidates.Count);
        }

        [Theory]
        [InlineData("Candidates.Foo")]
        public void CanFindSpecifiedTopic(string topic)
        {
            var selector = _provider.GetRequiredService<IConsumerServiceSelector>();
            var bestCandidates = selector.SelectBestCandidate(topic);

            Assert.NotNull(bestCandidates);
        }
    }
    
    public class CandidatesFooTest : IFlexBusSubscriber
    {
        public string Topic { get; } = "Candidates.Foo";
        public Type MessageType { get; } = typeof(object);
        
        public Task ProcessAsync(object obj, CancellationToken cancellationToken)
        {
	        Console.WriteLine("GetFoo() method has bee executed.");
	        return Task.CompletedTask;
        }
    }

    public class CandidatesBarTest : IFlexBusSubscriber
    {
        public string Topic { get; } = "Candidates.Bar";
        public Type MessageType { get; } = typeof(object);
        
        public Task ProcessAsync(object obj, CancellationToken cancellationToken)
        {
            Console.WriteLine("GetBar() method has bee executed.");
            return Task.CompletedTask;
        }
    }
}