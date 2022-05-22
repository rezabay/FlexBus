// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Fooreco.CAP.Persistence;

namespace Fooreco.CAP.Producer.Internal
{
    public interface IMessageSender
    {
        Task Connect();
        Task<OperateResult> SendAsync(MediumMessage message);
    }
}
