using System.Collections.Generic;

namespace Fooreco.CAP.AmazonSQS
{
    class SQSReceivedMessage
    {
        public string Message { get; set; }

        public Dictionary<string, SQSReceivedMessageAttributes> MessageAttributes { get; set; }
    }

    class SQSReceivedMessageAttributes
    {
        public string Type { get; set; }

        public string Value { get; set; }
    }
}
