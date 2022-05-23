using System;
using Amazon;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddFlexBus(x =>
{
    x.DefaultGroup = configuration["Cap:DefaultGroupName"];
    x.Version = configuration["Cap:DefaultVersion"];
    x.FailedRetryCount = 20;

    x.UsePostgreSql(configuration.GetConnectionString("Sql"));

    x.UseAmazonSQS(RegionEndpoint.USWest2);

    x.UseProducer(config =>
    {
        config.ThreadCount = 1;
        config.ProcessorInterval = TimeSpan.FromSeconds(1);
    });
});

var app = builder.Build();
app.UseRouting();

app.Run();
