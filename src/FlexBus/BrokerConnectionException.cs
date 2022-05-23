using System;

namespace FlexBus;

public class BrokerConnectionException : Exception
{
    public BrokerConnectionException(Exception innerException)
        : base("Broker Unreachable", innerException)
    {

    }
}