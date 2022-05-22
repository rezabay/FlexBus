using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using FlexBus.Dashboard.Resources;
using FlexBus.Persistence;
using FlexBus;
using Microsoft.AspNetCore.Http;

namespace FlexBus.Dashboard
{
    public class DashboardMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly DashboardOptions _options;
        private readonly RouteCollection _routes;
        private readonly IDataStorage _storage;

        public DashboardMiddleware(RequestDelegate next, DashboardOptions options, IDataStorage storage,
            RouteCollection routes)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _routes = routes ?? throw new ArgumentNullException(nameof(routes));
        }

        public async Task Invoke(HttpContext context)
        {
            var userLanguages = context.Request.Headers["Accept-Language"].ToString();
            Strings.Culture = userLanguages.Contains("zh-") ? new CultureInfo("zh-CN") : new CultureInfo("en-US");

            var dashboardContext = new CapDashboardContext(_storage, _options, context);
            var findResult = _routes.FindDispatcher(context.Request.Path.Value);

            if (findResult == null)
            {
                await _next.Invoke(context);
                return;
            }

            foreach (var authorizationFilter in _options.Authorization)
            {
                var authenticateResult = await authorizationFilter.AuthorizeAsync(dashboardContext);
                if (authenticateResult) continue;

                var isAuthenticated = context.User?.Identity?.IsAuthenticated;

                context.Response.StatusCode = isAuthenticated == true
                    ? (int)HttpStatusCode.Forbidden
                    : (int)HttpStatusCode.Unauthorized;

                return;
            }

            dashboardContext.UriMatch = findResult.Item2;

            await findResult.Item1.Dispatch(dashboardContext);
        }
    }
}
