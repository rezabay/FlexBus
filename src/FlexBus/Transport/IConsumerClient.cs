using JetBrains.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;
using FlexBus.Messages;

namespace FlexBus.Transport;

/// <inheritdoc />
/// <summary>
/// Message queue consumer client
/// </summary>
public interface IConsumerClient : IDisposable
{
    event EventHandler<TransportMessage> OnMessageReceived;

    event EventHandler<LogMessageEventArgs> OnLog;

    BrokerAddress BrokerAddress { get; }

    /// <summary>
    /// Start listening
    /// </summary>
    Task Listening(TimeSpan timeout, CancellationToken cancellationToken);

    /// <summary>
    /// Manual submit message offset when the message consumption is complete
    /// </summary>
    void Commit([NotNull] object sender);

    /// <summary>
    /// Reject message and resumption
    /// </summary>
    void Reject([CanBeNull] object sender);
}