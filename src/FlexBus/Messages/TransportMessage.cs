using JetBrains.Annotations;
using System.Collections.Generic;

namespace FlexBus.Messages;

/// <summary>
/// Message content field
/// </summary>
public class TransportMessage
{
    public TransportMessage(IDictionary<string, string> headers, [CanBeNull] byte[] body)
    {
        Headers = headers ?? new Dictionary<string, string>();
        Body = body;
    }

    /// <summary>
    /// Gets the headers of this message
    /// </summary>
    public IDictionary<string, string> Headers { get; }

    /// <summary>
    /// Gets the body object of this message
    /// </summary>
    [CanBeNull]
    public byte[] Body { get; }

    public string GetId()
    {
        return Headers.TryGetValue(Messages.Headers.MessageId, out var value) ? value : null;
    }

    public string GetName()
    {
        return Headers.TryGetValue(Messages.Headers.MessageName, out var value) ? value : null;
    }

    public string GetGroup()
    {
        return Headers.TryGetValue(Messages.Headers.Group, out var value) ? value : null;
    }
        
    public string GetCorrelationId()
    {
        return Headers.TryGetValue(Messages.Headers.CorrelationId, out var value) ? value : null;
    }
}