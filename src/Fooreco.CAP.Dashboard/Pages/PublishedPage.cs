// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Fooreco.CAP.Monitoring;

namespace Fooreco.CAP.Dashboard.Pages
{
    internal partial class PublishedPage
    {
        public PublishedPage(string statusName)
        {
            Name = statusName;
        }

        public string Name { get; set; }

        public int GetTotal(IMonitoringApi api)
        {
            if (string.Equals(Name, nameof(Internal.StatusName.Succeeded),
                StringComparison.CurrentCultureIgnoreCase))
            {
                return api.PublishedSucceededCount();
            }

            return api.PublishedFailedCount();
        }
    }
}