// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using FlexBus;
using FlexBus.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace FlexBus.Dashboard.NodeDiscovery
{
    internal sealed class DiscoveryOptionsExtension : ICapOptionsExtension
    {
        private readonly Action<DiscoveryOptions> _options;

        public DiscoveryOptionsExtension(Action<DiscoveryOptions> option)
        {
            _options = option;
        }

        public void AddServices(IServiceCollection services)
        {
            var discoveryOptions = new DiscoveryOptions();

            _options?.Invoke(discoveryOptions);
            services.AddSingleton(discoveryOptions);

            services.AddSingleton<IDiscoveryProviderFactory, DiscoveryProviderFactory>();
            services.AddSingleton<IProcessingServer, ConsulProcessingNodeServer>();
            services.AddSingleton(x =>
            {
                var configOptions = x.GetService<DiscoveryOptions>();
                var factory = x.GetService<IDiscoveryProviderFactory>();
                return factory.Create(configOptions);
            });
        }
    }

    public static class CapDiscoveryOptionsExtensions
    {
        public static FlexBusOptions UseDiscovery(this FlexBusOptions flexBusOptions)
        {
            return flexBusOptions.UseDiscovery(opt => { });
        }

        public static FlexBusOptions UseDiscovery(this FlexBusOptions flexBusOptions, Action<DiscoveryOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            flexBusOptions.RegisterExtension(new DiscoveryOptionsExtension(options));

            return flexBusOptions;
        }
    }
}