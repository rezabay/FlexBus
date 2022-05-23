using System;

namespace FlexBus.Consumer;

public class SubscriberNotFoundException : Exception
{
    public SubscriberNotFoundException(string message) : base(message)
    {
    }
}