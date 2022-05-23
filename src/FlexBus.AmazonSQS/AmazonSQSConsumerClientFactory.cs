using FlexBus.Transport;
using FlexBus;
using Microsoft.Extensions.Options;

namespace FlexBus.AmazonSQS;

internal sealed class AmazonSQSConsumerClientFactory : IConsumerClientFactory
{
    private readonly IOptions<AmazonSQSOptions> _amazonSQSOptions;
    private readonly IOptions<FlexBusOptions> _capOptions;

    public AmazonSQSConsumerClientFactory(
        IOptions<AmazonSQSOptions> amazonSQSOptions, 
        IOptions<FlexBusOptions> capOptions)
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