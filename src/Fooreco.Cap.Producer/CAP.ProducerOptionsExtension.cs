// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Fooreco.CAP.Internal;
using Fooreco.CAP.Processor;
using Fooreco.CAP.Producer.Internal;
using Fooreco.CAP.Producer.Processor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fooreco.CAP.Producer
{
    internal sealed class ProducerOptionsExtension : ICapOptionsExtension
    {
        internal static IServiceCollection ServiceCollection;
        private readonly Action<ProducerOptions> _configure;

        public ProducerOptionsExtension(Action<ProducerOptions> configure)
        {
            _configure = configure ?? throw new ArgumentNullException(nameof(configure));
        }

        public void AddServices(IServiceCollection services)
        {
            ServiceCollection = services;

            services.Configure(_configure);

            services.TryAddSingleton<IMessageSender, MessageSender>();

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IProcessingServer, MessagePublisherProcessor>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IProcessor, CollectorProcessor>());
        }
    }
}
