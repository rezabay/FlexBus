using System;
using Fooreco.CAP.Messages;
using Fooreco.CAP.Transport;

namespace Fooreco.CAP.Diagnostics
{
    public class CapEventDataPubStore
    {
        public long? OperationTimestamp { get; set; }

        public string Operation { get; set; }

        public Message Message { get; set; }

        public long? ElapsedTimeMs { get; set; }

        public Exception Exception { get; set; }
    }

    public class CapEventDataPubSend
    {
        public long? OperationTimestamp { get; set; }

        public string Operation { get; set; }

        public TransportMessage TransportMessage { get; set; }

        public BrokerAddress BrokerAddress { get; set; }

        public long? ElapsedTimeMs { get; set; }

        public Exception Exception { get; set; }
    }
}
