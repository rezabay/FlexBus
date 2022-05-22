using Fooreco.CAP.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Fooreco.CAP.Dashboard
{
    public static class CapEndpointRouteBuilderExtensions
    {
        public static IEndpointConventionBuilder MapCapDashboard(
            this IEndpointRouteBuilder endpoints,
            DashboardOptions options = null,
            IDataStorage storage = null)
        {
            if (endpoints == null) throw new ArgumentNullException(nameof(endpoints));

            var app = endpoints.CreateApplicationBuilder();

            var services = app.ApplicationServices;

            storage ??= services.GetRequiredService<IDataStorage>();
            options ??= services.GetService<DashboardOptions>() ?? new DashboardOptions();
            var pattern = options.PathMatch;

            var routes = app.ApplicationServices.GetRequiredService<RouteCollection>();

            var pipeline = app
                .UsePathBase(pattern)
                .UseMiddleware<DashboardMiddleware>(storage, options, routes)
                .Build();

            return endpoints.Map(pattern + "/{**path}", pipeline);
        }
    }
}
