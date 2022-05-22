using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlexBus.Internal;
using FlexBus.Persistence;
using FlexBus.Producer.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FlexBus.Producer.Processor;

public class MessagePublisherProcessor : IProcessingServer
{
    private readonly ILogger<MessagePublisherProcessor> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _waitingInterval;
    private readonly ProducerOptions _options;
    private readonly CancellationTokenSource _cts  = new CancellationTokenSource();

    private Task _compositeTask;
    private bool _disposed;

    public MessagePublisherProcessor(IOptions<ProducerOptions> options,
        ILogger<MessagePublisherProcessor> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _waitingInterval = _options.ProcessorInterval;
    }

    public void Start()
    {
        for (var i = 0; i < _options.ThreadCount; i++)
        {
            Task.Factory.StartNew(ProcessAsync, 
                _cts.Token, 
                TaskCreationOptions.LongRunning, 
                TaskScheduler.Default);
        }

        _compositeTask = Task.CompletedTask;
    }

    public void Pulse()
    {
        _cts?.Cancel();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        try
        {
            Pulse();

            _compositeTask?.Wait(TimeSpan.FromSeconds(2));
        }
        catch (AggregateException ex)
        {
            var innerEx = ex.InnerExceptions[0];
            if (!(innerEx is OperationCanceledException))
            {
                _logger.ExpectedOperationCanceledException(innerEx);
            }
        }
    }

    private async Task ProcessAsync()
    {
        var dataStorage = _serviceProvider.GetRequiredService<IDataStorage>();
        var messageSender = _serviceProvider.GetRequiredService<IMessageSender>();
        await messageSender.Connect();

        while (!_cts.IsCancellationRequested)
        {
            try
            {
                await ProcessPublishedAsync(dataStorage, messageSender);

                await Task.Delay(_waitingInterval, _cts.Token);
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Producer service failed. Retrying...");
                await Task.Delay(TimeSpan.FromSeconds(2), _cts.Token);
            }
        }
    }

    private async Task ProcessPublishedAsync(IDataStorage dataStorage, IMessageSender messageSender)
    {
        var messages = await dataStorage.GetNextPublishedMessage();
        if (messages.Any())
        {
            foreach (var message in messages)
            {
                await messageSender.SendAsync(message);
            }
        }
    }
}