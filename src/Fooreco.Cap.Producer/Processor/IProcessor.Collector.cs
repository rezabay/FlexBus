// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Fooreco.CAP.Persistence;
using Fooreco.CAP.Processor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fooreco.CAP.Producer.Processor
{
    public class CollectorProcessor : IProcessor
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        private const int ItemBatch = 1000;
        private readonly TimeSpan _waitingInterval = TimeSpan.FromHours(1);
        private readonly TimeSpan _delay = TimeSpan.FromSeconds(1);
        private readonly string _publishedTableName;

        public CollectorProcessor(
            ILogger<CollectorProcessor> logger,
            IStorageInitializer initializer,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _publishedTableName = initializer.GetPublishedTableName();
            _serviceProvider = serviceProvider;
        }

        public async Task ProcessAsync(ProcessingContext context)
        {
            var dataStorage = _serviceProvider.GetRequiredService<IDataStorage>();

            _logger.LogDebug($"Collecting expired data from table: {_publishedTableName}");

            int deletedCount;
            var time = DateTime.Now;
            do
            {
                deletedCount = await dataStorage.DeleteExpiresAsync(_publishedTableName, time, ItemBatch, context.CancellationToken);

                if (deletedCount != 0)
                {
                    await context.WaitAsync(_delay);
                    context.ThrowIfStopping();
                }
            } while (deletedCount != 0);

            await context.WaitAsync(_waitingInterval);
        }
    }
}