using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fooreco.CAP.Messages;
using Fooreco.CAP.Transport;
using Microsoft.Extensions.Logging;

namespace Fooreco.CAP.Test.FakeInMemoryQueue
{
    internal sealed class InMemoryConsumerClient : IConsumerClient
    {
        private readonly ILogger _logger;
        private readonly InMemoryQueue _queue;
        private readonly string _subscriptionName;

        public event EventHandler<TransportMessage> OnMessageReceived;
        public event EventHandler<LogMessageEventArgs> OnLog;

        public InMemoryConsumerClient(
            ILogger logger,
            InMemoryQueue queue,
            string subscriptionName)
        {
            _logger = logger;
            _queue = queue;
            _subscriptionName = subscriptionName;
        }

        public BrokerAddress BrokerAddress => new("InMemory", string.Empty);

        public void Subscribe(IEnumerable<string> topics)
        {
            if (topics == null) throw new ArgumentNullException(nameof(topics));

            foreach (var topic in topics)
            {
                _queue.Subscribe(_subscriptionName, OnConsumerReceived, topic);

                _logger.LogInformation($"InMemory message queue initialize the topic: {topic}");
            }
        }

        public Task Listening(TimeSpan timeout, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                cancellationToken.WaitHandle.WaitOne(timeout);
            }

            return Task.CompletedTask;
        }

        public void Commit(object sender)
        {
            // ignore
        }

        public void Reject(object sender)
        {
            // ignore
        }

        public void Dispose()
        {
            _queue.ClearSubscriber();
        }

        private void OnConsumerReceived(TransportMessage e)
        {
            OnMessageReceived?.Invoke(null, e);
            OnLog?.Invoke(null, null);
        }

        public Task<bool> IsConnected()
        {
            return Task.FromResult(true);
        }
    }
}