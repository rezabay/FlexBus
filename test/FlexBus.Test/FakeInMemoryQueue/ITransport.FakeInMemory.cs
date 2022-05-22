using System;
using System.Threading.Tasks;
using FlexBus.Internal;
using FlexBus.Messages;
using FlexBus.Transport;
using Microsoft.Extensions.Logging;

namespace FlexBus.Test.FakeInMemoryQueue
{
    internal class FakeInMemoryQueueTransport : ITransport
    {
        private readonly InMemoryQueue _queue;
        private readonly ILogger _logger;

        public FakeInMemoryQueueTransport(InMemoryQueue queue, ILogger<FakeInMemoryQueueTransport> logger)
        {
            _queue = queue;
            _logger = logger;
        }

        public BrokerAddress BrokerAddress { get; } = new BrokerAddress("InMemory", string.Empty);

        public Task Connect()
        {
            return Task.CompletedTask;
        }

        public Task<bool> IsConnected()
        {
            return Task.FromResult(true);
        }

        public Task<OperateResult> SendAsync(TransportMessage message)
        {
            try
            {
                _queue.Send(message.GetName(), message);

                _logger.LogDebug($"Event message [{message.GetName()}] has been published.");

                return Task.FromResult(OperateResult.Success);
            }
            catch (Exception ex)
            {
                var wrapperEx = new PublisherSentFailedException(ex.Message, ex);

                return Task.FromResult(OperateResult.Failed(wrapperEx));
            }
        }
    }
}