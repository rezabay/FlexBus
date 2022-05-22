// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FlexBus.Internal;
using FlexBus.Messages;
using FlexBus.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FlexBus.Consumer.Internal
{
    internal class SubscribeDispatcher : ISubscribeDispatcher
    {
        private readonly IDataStorage _dataStorage;
        private readonly ILogger _logger;
        private readonly IServiceProvider _provider;
        private readonly CapOptions _options;
        private readonly ConsumerOptions _consumerOptions;

        public SubscribeDispatcher(
            ILogger<SubscribeDispatcher> logger,
            IOptions<CapOptions> options,
            IOptions<ConsumerOptions> consumerOptions,
            IServiceProvider provider)
        {
            _provider = provider;
            _logger = logger;
            _options = options.Value;
            _consumerOptions = consumerOptions.Value;

            _dataStorage = _provider.GetService<IDataStorage>();
            Invoker = _provider.GetService<ISubscribeInvoker>();
        }

        private ISubscribeInvoker Invoker { get; }

        public Task<OperateResult> DispatchAsync(MediumMessage message, CancellationToken cancellationToken)
        {
            var selector = _provider.GetService<MethodMatcherCache>();
            if (!selector.TryGetTopicExecutor(message.Origin.GetName(), message.Origin.GetGroup(), out var executor))
            {
                var error = $"Message (Name:{message.Origin.GetName()},Group:{message.Origin.GetGroup()}) can not be found subscriber." +
                            $"{Environment.NewLine} see: https://github.com/dotnetcore/CAP/issues/63";
                _logger.LogError(error);

                TracingError(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), message.Origin, new Exception(error));

                return Task.FromResult(OperateResult.Failed(new SubscriberNotFoundException(error)));
            }

            return DispatchAsync(message, executor, cancellationToken);
        }

        public async Task<OperateResult> DispatchAsync(MediumMessage message, ConsumerExecutorDescriptor descriptor, CancellationToken cancellationToken)
        {
            bool retry;
            OperateResult result;
            do
            {
                var executedResult = await ExecuteWithoutRetryAsync(message, descriptor, cancellationToken);
                result = executedResult.Item2;
                if (result == OperateResult.Success)
                {
                    return result;
                }
                retry = executedResult.Item1;
            } while (retry);

            return result;
        }

        private async Task<(bool, OperateResult)> ExecuteWithoutRetryAsync(MediumMessage message, ConsumerExecutorDescriptor descriptor, CancellationToken cancellationToken)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var sp = Stopwatch.StartNew();

                await InvokeConsumerMethodAsync(message, descriptor, cancellationToken);

                sp.Stop();

                await SetSuccessfulState(message);

                _logger.ConsumerExecuted(sp.Elapsed.TotalMilliseconds);

                return (false, OperateResult.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An exception occurred while executing the subscription method. Topic:{message.Origin.GetName()}, Id:{message.DbId}");

                return (await SetFailedState(message, ex), OperateResult.Failed(ex));
            }
        }

        private Task SetSuccessfulState(MediumMessage message)
        {
            message.ExpiresAt = DateTime.Now.AddSeconds(_consumerOptions.SucceedMessageExpiredAfter);
            return _dataStorage.ChangeReceiveStateAsync(message, StatusName.Succeeded);
        }

        private async Task<bool> SetFailedState(MediumMessage message, Exception ex)
        {
            if (ex is SubscriberNotFoundException)
            {
                message.Retries = _options.FailedRetryCount; // not retry if SubscriberNotFoundException
            }

            var needRetry = UpdateMessageForRetry(message);

            message.Origin.AddOrUpdateException(ex);
            message.ExpiresAt = message.Added.AddDays(15);

            await _dataStorage.ChangeReceiveStateAsync(message, StatusName.Failed);

            return needRetry;
        }

        private bool UpdateMessageForRetry(MediumMessage message)
        {
            var retries = ++message.Retries;

            var retryCount = Math.Min(_options.FailedRetryCount, 3);
            if (retries >= retryCount)
            {
                if (retries == _options.FailedRetryCount)
                {
                    try
                    {
                        _consumerOptions.FailedThresholdCallback?.Invoke(new FailedInfo
                        {
                            ServiceProvider = _provider,
                            MessageType = MessageType.Subscribe,
                            Message = message.Origin
                        });

                        _logger.ConsumerExecutedAfterThreshold(message.DbId, _options.FailedRetryCount);
                    }
                    catch (Exception ex)
                    {
                        _logger.ExecutedThresholdCallbackFailed(ex);
                    }
                }
                return false;
            }

            _logger.ConsumerExecutionRetrying(message.DbId, retries);

            return true;
        }

        private async Task InvokeConsumerMethodAsync(MediumMessage message, ConsumerExecutorDescriptor descriptor, CancellationToken cancellationToken)
        {
            var consumerContext = new ConsumerContext(descriptor, message.Origin);
            var tracingTimestamp = TracingBefore(message.Origin);
            try
            {
                var ret = await Invoker.InvokeAsync(consumerContext, cancellationToken);

                TracingAfter(tracingTimestamp, message.Origin);

                if (!string.IsNullOrEmpty(ret.CallbackName))
                {
                    var header = new Dictionary<string, string>()
                    {
                        [Headers.CorrelationId] = message.Origin.GetId(),
                        [Headers.CorrelationSequence] = (message.Origin.GetCorrelationSequence() + 1).ToString()
                    };

                    await _provider.GetService<ICapPublisher>().PublishAsync(ret.CallbackName, ret.Result, header, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                //ignore
            }
            catch (Exception ex)
            {
                var e = new SubscriberExecutionFailedException(ex.Message, ex);

                TracingError(tracingTimestamp, message.Origin, e);

                throw e;
            }
        }

        #region tracing

        private long? TracingBefore(Message message)
        {
            var operationTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            _logger.LogInformation($"Executing: {message.GetName()}");

            return operationTimestamp;
        }

        private void TracingAfter(long? tracingTimestamp, Message message)
        {
            if (tracingTimestamp != null)
            {
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                _logger.LogInformation($"Executed: {message.GetName()}, ElapsedTimeMs = {now - tracingTimestamp.Value}");
            }
            else
            {
                _logger.LogInformation($"Executed: {message.GetName()}");
            }
        }

        private void TracingError(long? tracingTimestamp, Message message, Exception ex)
        {
            if (tracingTimestamp != null)
            {
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                _logger.LogInformation(ex, $"Executed with error: {message.GetName()}, ElapsedTimeMs = {now - tracingTimestamp.Value}");
            }
            else
            {
                _logger.LogInformation(ex, $"Executed with error: {message.GetName()}");
            }
        }

        #endregion
    }
}
