using Microsoft.AspNetCore.Builder;
using Amazon;
using FlexBus;
using FlexBus.Consumer.Demo.Commands;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers();
services.AddHealthChecks()
    .AddCheck<FlexBusHealthCheck>("sqs-check", HealthStatus.Unhealthy, new[] { "ready" });

// Subscribers
services.AddTransient<SendEmailSubscriber>();

services.AddFlexBus(x =>
{
    x.DefaultGroup = configuration["Cap:DefaultGroupName"];
    x.Version = configuration["Cap:DefaultVersion"];
    x.FailedRetryCount = 20;

    x.UsePostgreSql(configuration.GetConnectionString("Sql"));

    x.UseAmazonSQS(RegionEndpoint.USWest2);

    x.UseConsumer(config =>
    {
        config.ThreadCount = 1;
    });
});

var app = builder.Build();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    endpoints.MapHealthChecks("/health", new HealthCheckOptions()
    {
        Predicate = _ => false
    });
    endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions()
    {
        Predicate = check => check.Tags.Contains("ready")
    });
});

app.Run();
