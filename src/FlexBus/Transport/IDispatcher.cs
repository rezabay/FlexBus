using System.Threading.Tasks;
using FlexBus.Persistence;

namespace FlexBus.Transport;

public interface IDispatcher
{
    Task EnqueueToPublish(MediumMessage message);
}