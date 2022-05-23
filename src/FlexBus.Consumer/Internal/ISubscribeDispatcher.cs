using System.Threading;
using System.Threading.Tasks;
using FlexBus.Persistence;

namespace FlexBus.Consumer.Internal;

/// <summary>
/// Consumer executor
/// </summary>
public interface ISubscribeDispatcher
{
    Task<OperateResult> DispatchAsync(MediumMessage message, CancellationToken cancellationToken = default);

    Task<OperateResult> DispatchAsync(
        MediumMessage message, 
        IFlexBusSubscriber subscribe, 
        CancellationToken cancellationToken = default);
}