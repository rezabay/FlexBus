// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Fooreco.CAP.Persistence;
using System.Threading.Tasks;

namespace Fooreco.CAP.Transport
{
    public interface IDispatcher
    {
        Task EnqueueToPublish(MediumMessage message);
    }
}
