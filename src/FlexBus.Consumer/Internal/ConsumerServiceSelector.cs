using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace FlexBus.Consumer.Internal;

/// <inheritdoc />
/// <summary>
/// A default <see cref="T:FlexBus.Abstractions.IConsumerServiceSelector" /> implementation.
/// </summary>
public class ConsumerServiceSelector : IConsumerServiceSelector
{
    private readonly IServiceProvider _serviceProvider;
    
    /// <summary>
    /// Creates a new <see cref="ConsumerServiceSelector" />.
    /// </summary>
    public ConsumerServiceSelector(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IReadOnlyList<string> SelectCandidates()
    {
        var executorDescriptorList = new List<string>();

        executorDescriptorList.AddRange(FindConsumersFromInterfaceTypes());

        return executorDescriptorList;
    }

    public IFlexBusSubscriber SelectBestCandidate(string key)
    {
        return _serviceProvider.GetServices<IFlexBusSubscriber>()
            .SingleOrDefault(x => x.Topic == key);
    }

    private IEnumerable<string> FindConsumersFromInterfaceTypes()
    {
        return _serviceProvider.GetServices<IFlexBusSubscriber>()
            .Select(service => service.Topic);
    }
}