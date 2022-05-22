using System.Threading.Tasks;
using Fooreco.CAP.Persistence;

namespace Fooreco.CAP.Producer.Internal;

public interface IMessageSender
{
    Task Connect();
    Task<OperateResult> SendAsync(MediumMessage message);
}