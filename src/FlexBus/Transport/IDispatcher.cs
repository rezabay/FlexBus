// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading.Tasks;
using FlexBus.Persistence;

namespace FlexBus.Transport
{
    public interface IDispatcher
    {
        Task EnqueueToPublish(MediumMessage message);
    }
}
