// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace FlexBus.Dashboard.Pages
{
    internal partial class InlineMetric
    {
        public InlineMetric(DashboardMetric dashboardMetric)
        {
            DashboardMetric = dashboardMetric;
        }

        public DashboardMetric DashboardMetric { get; }
    }
}