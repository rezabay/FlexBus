// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Amazon.SQS.Model;
using FlexBus.Internal;
using FlexBus.Messages;
using FlexBus.Transport;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlexBus;

namespace FlexBus.AmazonSQS
{
    internal sealed class AmazonSQSTransport : AmazonSQSClientWrapper, ITransport
    {
        private readonly ILogger _logger;

        public AmazonSQSTransport(ILogger<AmazonSQSTransport> logger,
                                  IOptions<AmazonSQSOptions> sqsOptions,
                                  IOptions<CapOptions> capOptions) : base(sqsOptions, capOptions)
        {
            _logger = logger;
        }

        public BrokerAddress BrokerAddress => new("AmazonSQS", string.Empty);

        public Task Connect() => ConnectToSQS();

        public async Task<bool> IsConnected()
        {
            try
            {
                await Connect();
                await SQSClient.GetQueueUrlAsync(QueueName.NormalizeForAws());
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cannot connect to SQS");
                return false;
            }
        }

        public async Task<OperateResult> SendAsync(TransportMessage message)
        {
            try
            {
                string bodyJson = null;
                if (message.Body != null)
                {
                    bodyJson = Encoding.UTF8.GetString(message.Body);
                }

                var attributes = message.Headers.Where(x => x.Value != null).ToDictionary(x => x.Key,
                    x => new MessageAttributeValue
                    {
                        StringValue = x.Value,
                        DataType = "String"
                    });

                var request = new SendMessageRequest(QueueUrl, bodyJson)
                {
                    MessageAttributes = attributes
                };

                var response = await SQSClient.SendMessageAsync(request);

                _logger.LogDebug($"Message({response.MessageId}) [{message.GetName().NormalizeForAws()}] has been published to {QueueName}.");

                return OperateResult.Success;
            }
            catch (Exception ex)
            {
                var wrapperEx = new PublisherSentFailedException(ex.Message, ex);
                var errors = new OperateError
                {
                    Code = ex.HResult.ToString(),
                    Description = ex.Message
                };

                return OperateResult.Failed(wrapperEx, errors);
            }
        }
    }
}
