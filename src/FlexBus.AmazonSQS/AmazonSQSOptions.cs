using Amazon;
using Amazon.Runtime;

// ReSharper disable once CheckNamespace
namespace FlexBus;

// ReSharper disable once InconsistentNaming
public class AmazonSQSOptions
{
    public RegionEndpoint Region { get; set; }

    public AWSCredentials Credentials { get; set; }
}