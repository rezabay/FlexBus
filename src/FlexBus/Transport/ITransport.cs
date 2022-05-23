using System.Threading.Tasks;
using FlexBus.Messages;

namespace FlexBus.Transport;

public interface ITransport
{
    BrokerAddress BrokerAddress { get; }

    Task<bool> IsConnected();

    Task Connect();

    Task<OperateResult> SendAsync(TransportMessage message);
}