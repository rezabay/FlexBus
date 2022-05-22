using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Fooreco.CAP.Consumer.Internal;
using Fooreco.CAP.Messages;
using Fooreco.CAP.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Fooreco.CAP.Test
{
    public class SubscribeInvokerTest
    {
        private readonly IServiceProvider _serviceProvider;

        public SubscribeInvokerTest()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.AddSingleton<ISerializer, JsonUtf8Serializer>();
            serviceCollection.AddSingleton<ISubscribeInvoker, SubscribeInvoker>();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private ISubscribeInvoker SubscribeInvoker => _serviceProvider.GetService<ISubscribeInvoker>();

        [Fact]
        public async Task InvokeTest()
        {
            var descriptor = new ConsumerExecutorDescriptor()
            {
                Attribute = new CandidatesTopic("fake.output.integer"),
                ServiceTypeInfo = typeof(FakeSubscriber).GetTypeInfo(),
                ImplTypeInfo = typeof(FakeSubscriber).GetTypeInfo(),
                MethodInfo = typeof(FakeSubscriber).GetMethod(nameof(FakeSubscriber.OutputIntegerSub)),
                Parameters = new List<ParameterDescriptor>()
            };

            var header = new Dictionary<string, string>();
            var message = new Message(header, null);
            var context = new ConsumerContext(descriptor, message);

            var ret = await SubscribeInvoker.InvokeAsync(context);
            Assert.Equal(int.MaxValue, ret.Result);
        }
    }

    public class FakeSubscriber : ICapSubscribe
    {
        [CapSubscribe("fake.output.integer")]
        public int OutputIntegerSub()
        {
            return int.MaxValue;
        }
    }
}
