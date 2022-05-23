using System.Collections.Generic;

namespace FlexBus.Consumer.Internal;

/// <summary>
/// Defines an interface for selecting an consumer service method to invoke for the current message.
/// </summary>
public interface IConsumerServiceSelector
{
    /// <summary>
    /// Selects a set of candidates for the current message associated with
    /// </summary>
    /// <returns>List of registered topics.</returns>
    IReadOnlyList<string> SelectCandidates();

    /// <summary>
    /// Selects the best <see cref="IFlexBusSubscriber" /> candidate
    /// </summary>
    /// <param name="key">topic or exchange router key.</param>
    IFlexBusSubscriber SelectBestCandidate(string key);
}