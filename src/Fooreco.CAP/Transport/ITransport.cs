// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Fooreco.CAP.Messages;

namespace Fooreco.CAP.Transport
{
    public interface ITransport
    {
        BrokerAddress BrokerAddress { get; }

        Task<bool> IsConnected();

        Task Connect();

        Task<OperateResult> SendAsync(TransportMessage message);
    }
}
