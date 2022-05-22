// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace FlexBus.Dashboard.GatewayProxy
{
    public class DownstreamUrl
    {
        public DownstreamUrl(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}