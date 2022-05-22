using Fooreco.CAP.Dashboard;
using Fooreco.CAP.Dashboard.GatewayProxy;
using Fooreco.CAP.Dashboard.GatewayProxy.Requester;
using Fooreco.CAP.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DashboardServiceCollectionExtension
    {
        public static IServiceCollection AddCapDashboard(this IServiceCollection services)
        {
            services.AddSingleton(x => DashboardRoutes.GetDashboardRoutes(x.GetRequiredService<ISerializer>()));
            services.AddSingleton<IHttpRequester, HttpClientHttpRequester>();
            services.AddSingleton<IHttpClientCache, MemoryHttpClientCache>();
            services.AddSingleton<IRequestMapper, RequestMapper>();

            return services;
        }
    }
}
