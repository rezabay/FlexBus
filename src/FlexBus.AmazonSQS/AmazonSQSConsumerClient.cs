// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Amazon.SQS.Model;
using FlexBus.Messages;
using FlexBus.Transport;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlexBus;
using Headers = FlexBus.Messages.Headers;

namespace FlexBus.AmazonSQS
{
    internal sealed class AmazonSQSConsumerClient : AmazonSQSClientWrapper, IConsumerClient
    {
        private readonly string _groupId;

        public event EventHandler<TransportMessage> OnMessageReceived;
        public event EventHandler<LogMessageEventArgs> OnLog;

        public BrokerAddress BrokerAddress => new("AmazonSQS", QueueUrl);

        public AmazonSQSConsumerClient(string groupId, 
                                       IOptions<AmazonSQSOptions> options,
                                       IOptions<CapOptions> capOptions) : base(options, capOptions)
        {
            _groupId = groupId;
        }

        public async Task Listening(TimeSpan timeout, CancellationToken cancellationToken)
        {
            await Connect();

            var request = new ReceiveMessageRequest(QueueUrl)
            {
                WaitTimeSeconds = 5,
                MaxNumberOfMessages = 10,
                MessageAttributeNames =
                {
                    "cap-*"
                }
            };

            while (!cancellationToken.IsCancellationRequested)
            {
                var response = await SQSClient.ReceiveMessageAsync(request, cancellationToken);

                if (response.Messages.Any())
                {
                    foreach (var message in response.Messages)
                    {
                        var header = message.MessageAttributes?.ToDictionary(x => x.Key, x => x.Value.StringValue);
                        var body = message.Body;

                        var transportMessage = new TransportMessage(header, body != null ? Encoding.UTF8.GetBytes(body) : null);
                        transportMessage.Headers.Add(Headers.Group, _groupId);

                        OnMessageReceived?.Invoke(message.ReceiptHandle, transportMessage);
                    }
                }
                else
                {
                    await Task.Delay(timeout, cancellationToken);
                }
            }
        }

        public void Commit(object sender)
        {
            try
            {
                SQSClient.DeleteMessageAsync(QueueUrl, (string)sender);
            }
            catch (InvalidIdFormatException ex)
            {
                InvalidIdFormatLog(ex.Message);
            }
        }

        public void Reject(object sender)
        {
            try
            {
                // Visible again in 3 seconds
                SQSClient.ChangeMessageVisibilityAsync(QueueUrl, (string)sender, 3);
            }
            catch (MessageNotInflightException ex)
            {
                MessageNotInflightLog(ex.Message);
            }
        }

        public void Dispose()
        {
            SQSClient?.Dispose();
        }

        public Task Connect() =>  ConnectToSQS(queueName: _groupId);

        private Task InvalidIdFormatLog(string exceptionMessage)
        {
            var logArgs = new LogMessageEventArgs
            {
                LogType = MqLogType.InvalidIdFormat,
                Reason = exceptionMessage
            };

            OnLog?.Invoke(null, logArgs);

            return Task.CompletedTask;
        }

        private Task MessageNotInflightLog(string exceptionMessage)
        {
            var logArgs = new LogMessageEventArgs
            {
                LogType = MqLogType.MessageNotInflight,
                Reason = exceptionMessage
            };

            OnLog?.Invoke(null, logArgs);

            return Task.CompletedTask;
        }
    }
}