// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading.Tasks;
using FlexBus.Persistence;
using FlexBus.Messages;

namespace FlexBus.Transport
{
    public class Dispatcher : IDispatcher
    {
        private readonly ICapPublisher _publisher;

        public Dispatcher(ICapPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task EnqueueToPublish(MediumMessage message)
        {
            var originMessage = message.Origin;
            await _publisher.PublishAsync(originMessage.GetName(), originMessage.Value);
        }
    }
}
