using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FlexBus.Internal;
using FlexBus.Messages;
using FlexBus.Monitoring;

namespace FlexBus.Persistence;

public interface IDataStorage
{
    Task<OperateResult> ChangePublishStateAsync(MediumMessage message, StatusName state, Func<Task<OperateResult>> action = null);

    Task<OperateResult> ChangeReceiveStateAsync(MediumMessage message, StatusName state, Func<Task<OperateResult>> action = null);

    Task<MediumMessage> StoreMessage(string name, Message content, object dbTransaction = null);

    Task StoreReceivedExceptionMessage(string name, string group, string content);

    Task<MediumMessage> StoreReceivedMessage(string name, string group, Message content);

    Task<int> DeleteExpiresAsync(string table, DateTime timeout, int batchCount = 1000,
        CancellationToken token = default);

    Task<IEnumerable<MediumMessage>> GetNextPublishedMessage();

    //dashboard api
    IMonitoringApi GetMonitoringApi();
}