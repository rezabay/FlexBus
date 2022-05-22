// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using FlexBus.Transport;
using FlexBus;
using Microsoft.Extensions.Options;

namespace FlexBus.AmazonSQS
{
    internal sealed class AmazonSQSConsumerClientFactory : IConsumerClientFactory
    {
        private readonly IOptions<AmazonSQSOptions> _amazonSQSOptions;
        private readonly IOptions<CapOptions> _capOptions;

        public AmazonSQSConsumerClientFactory(IOptions<AmazonSQSOptions> amazonSQSOptions, 
                                              IOptions<CapOptions> capOptions)
        {
            _amazonSQSOptions = amazonSQSOptions;
            _capOptions = capOptions;
        }

        public IConsumerClient Create(string groupId)
        {
            try
            {
               var client = new AmazonSQSConsumerClient(groupId, _amazonSQSOptions, _capOptions);
               return client;
            }
            catch (System.Exception e)
            {
                throw new BrokerConnectionException(e);
            }
        }
    }
}