using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlexBus.Messages;
using FlexBus.Persistence;

namespace FlexBus.Monitoring;

public interface IMonitoringApi
{
    Task<MediumMessage> GetPublishedMessageAsync(long id);

    Task<MediumMessage> GetReceivedMessageAsync(long id);

    StatisticsDto GetStatistics();

    IList<MessageDto> Messages(MessageQueryDto queryDto);

    int PublishedFailedCount();

    int PublishedSucceededCount();

    int ReceivedFailedCount();

    int ReceivedSucceededCount();

    IDictionary<DateTime, int> HourlySucceededJobs(MessageType type);

    IDictionary<DateTime, int> HourlyFailedJobs(MessageType type);
}