using Amazon.Runtime;
using Amazon.SQS;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fooreco.CAP.AmazonSQS
{
    internal class AmazonSQSClientWrapper
    {
        private static readonly SemaphoreSlim ConnectionLock = new(initialCount: 1, maxCount: 1);

        protected readonly AmazonSQSOptions SQSOptions;
        protected readonly CapOptions CapOptions;

        protected IAmazonSQS SQSClient;
        protected string QueueUrl = string.Empty;
        protected string QueueName;

        protected AmazonSQSClientWrapper(IOptions<AmazonSQSOptions> sqsOptions,
                                         IOptions<CapOptions> capOptions)
        {
            SQSOptions = sqsOptions.Value;
            CapOptions = capOptions.Value;
        }

        protected async Task ConnectToSQS(string queueName = null)
        {
            if (SQSClient != null)
            {
                return;
            }

            ConnectionLock.Wait();

            try
            {
                var config = new AmazonSQSConfig()
                {
                    RegionEndpoint = SQSOptions.Region,
                    Timeout = TimeSpan.FromSeconds(10),
                    RetryMode = RequestRetryMode.Standard,
                    MaxErrorRetry = 3
                };
                SQSClient = SQSOptions.Credentials != null
                    ? new AmazonSQSClient(SQSOptions.Credentials, config)
                    : new AmazonSQSClient(config);

                QueueName = string.IsNullOrEmpty(queueName) 
                    ? CapOptions.DefaultGroup + "." + CapOptions.Version 
                    : queueName;
                var queue = await SQSClient.CreateQueueAsync(QueueName.NormalizeForAws());
                QueueUrl = queue.QueueUrl;
            }
            finally
            {
                ConnectionLock.Release();
            }
        }
    }
}
