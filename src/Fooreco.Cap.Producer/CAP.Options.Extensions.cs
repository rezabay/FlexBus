// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Fooreco.CAP;
using Fooreco.CAP.Producer;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CapOptionsExtensions
    {
        public static CapOptions UseProducer(this CapOptions options, Action<ProducerOptions> configure)
        {
            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            options.RegisterExtension(new ProducerOptionsExtension(configure));

            return options;
        }
    }
}
