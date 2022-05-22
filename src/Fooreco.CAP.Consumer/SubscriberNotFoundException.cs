// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Fooreco.CAP.Consumer
{
    public class SubscriberNotFoundException : Exception
    {
        public SubscriberNotFoundException(string message) : base(message)
        {
        }
    }
}
