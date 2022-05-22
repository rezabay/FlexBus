using FlexBus.Dashboard;
using FlexBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddRazorPages();
services.AddControllers();
services.AddCap(x =>
{
    x.UsePostgreSql(configuration.GetConnectionString("Sql"));
});
services.AddCapDashboard();

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();

    endpoints.MapCapDashboard(new DashboardOptions
    {
        AppPath = null,
        StatsPollingInterval = 5000,
        PathMatch = ""
    });
});

app.Run();
