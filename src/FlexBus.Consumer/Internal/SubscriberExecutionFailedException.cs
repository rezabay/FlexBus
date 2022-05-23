﻿using System;

namespace FlexBus.Consumer.Internal;

internal class SubscriberExecutionFailedException : Exception
{
    public SubscriberExecutionFailedException(string message, Exception ex) : base(message, ex)
    {
    }
}