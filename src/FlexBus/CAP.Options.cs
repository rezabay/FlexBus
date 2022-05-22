// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;

// ReSharper disable InconsistentNaming

namespace FlexBus
{
    /// <summary>
    /// Represents all the options you can use to configure the system.
    /// </summary>
    public class CapOptions
    {
        public CapOptions()
        {
            ProcessorInterval = 5;
            Extensions = new List<ICapOptionsExtension>();
            Version = "v1";
            DefaultGroup = "cap.queue." + Assembly.GetEntryAssembly()?.GetName().Name.ToLower();
            FailedRetryCount = 50;
            SendMessageTimeout = 30;
        }

        internal IList<ICapOptionsExtension> Extensions { get; }

        /// <summary>
        /// Subscriber default group name. kafka-->group name. rabbitmq --> queue name.
        /// </summary>
        public string DefaultGroup { get; set; }

        /// <summary>
        /// The default version of the message, configured to isolate data in the same instance. The length must not exceed 20
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Failed messages polling delay time.
        /// Default is 60 seconds.
        /// </summary>
        public int ProcessorInterval { get; set; }

        public int SendMessageTimeout { get; set; }

        /// <summary>
        /// The number of message retries, the retry will stop when the threshold is reached.
        /// Default is 50 times.
        /// </summary>
        public int FailedRetryCount { get; set; }

        /// <summary>
        /// Registers an extension that will be executed when building services.
        /// </summary>
        /// <param name="extension"></param>
        public void RegisterExtension(ICapOptionsExtension extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException(nameof(extension));
            }

            Extensions.Add(extension);
        }
    }
}
