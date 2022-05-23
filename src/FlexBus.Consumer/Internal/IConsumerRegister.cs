using FlexBus.Internal;

namespace FlexBus.Consumer.Internal;

/// <summary>
/// Handler received message of subscribed.
/// </summary>
public interface IConsumerRegister : IProcessingServer
{
    bool IsHealthy();

    void Restart(bool force = false);
}