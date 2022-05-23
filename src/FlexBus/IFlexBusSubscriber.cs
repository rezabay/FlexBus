using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlexBus;

/// <summary>
/// An empty interface, which is used to mark the current class have a CAP subscriber methods.
/// </summary>
public interface IFlexBusSubscriber
{
    string Topic { get; }
    Type MessageType { get; }

    Task ProcessAsync(object obj, CancellationToken cancellationToken);
}