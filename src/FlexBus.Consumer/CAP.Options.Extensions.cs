// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using FlexBus;
using FlexBus.Consumer;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CapOptionsExtensions
    {
        public static CapOptions UseConsumer(this CapOptions options, Action<ConsumerOptions> configure)
        {
            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            options.RegisterExtension(new ConsumerOptionsExtension(configure));

            return options;
        }
    }
}
