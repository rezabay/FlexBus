using System.Threading.Tasks;
using FlexBus.Persistence;

namespace FlexBus.Producer.Internal;

public interface IMessageSender
{
    Task Connect();
    Task<OperateResult> SendAsync(MediumMessage message);
}