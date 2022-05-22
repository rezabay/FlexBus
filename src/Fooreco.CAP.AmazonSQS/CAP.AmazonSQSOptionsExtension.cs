// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Fooreco.CAP.AmazonSQS;
using Fooreco.CAP.Transport;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Fooreco.CAP
{
    internal sealed class AmazonSQSOptionsExtension : ICapOptionsExtension
    {
        private readonly Action<AmazonSQSOptions> _configure;

        public AmazonSQSOptionsExtension(Action<AmazonSQSOptions> configure)
        {
            _configure = configure;
        }

        public void AddServices(IServiceCollection services)
        {
            services.AddSingleton<CapMessageQueueMakerService>();
             
            services.Configure(_configure);
            services.AddSingleton<ITransport, AmazonSQSTransport>();
            services.AddSingleton<IConsumerClientFactory, AmazonSQSConsumerClientFactory>();
        }
    }
}