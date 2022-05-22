// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading.Tasks;
using FlexBus.Messages;

namespace FlexBus.Transport
{
    public interface ITransport
    {
        BrokerAddress BrokerAddress { get; }

        Task<bool> IsConnected();

        Task Connect();

        Task<OperateResult> SendAsync(TransportMessage message);
    }
}
