using System;
using FlexBus.Messages;

namespace FlexBus.Persistence;

public class MediumMessage
{
    public string DbId { get; set; }

    public Message Origin { get; set; }

    public string Content { get; set; }

    public DateTime Added { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public int Retries { get; set; }

    public DateTime? ScheduleDate { get; set; }
}