using System;
using FlexBus.Messages;

namespace FlexBus.Producer;

public class ProducerOptions
{
    public ProducerOptions()
    {
        SucceedMessageExpiredAfter = 24 * 3600;
        ThreadCount = 2;
        ProcessorInterval = TimeSpan.FromSeconds(5);
    }

    /// <summary>
    /// Sent or received succeed message after time span of due, then the message will be deleted at due time.
    /// Default is 24*3600 seconds.
    /// </summary>
    public int SucceedMessageExpiredAfter { get; set; }

    /// <summary>
    /// We’ll invoke this call-back with message type,name,content when retry failed (send or executed).
    /// </summary>
    public Action<FailedInfo> FailedThresholdCallback { get; set; }

    public int ThreadCount { get; set; }

    public TimeSpan ProcessorInterval { get; set; }
}