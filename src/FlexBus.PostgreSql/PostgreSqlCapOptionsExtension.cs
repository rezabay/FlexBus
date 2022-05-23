using System;
using FlexBus.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FlexBus.PostgreSql;

internal class PostgreSqlFlexBusOptionsExtension : IFlexBusOptionsExtension
{
    private readonly Action<PostgreSqlOptions> _configure;

    public PostgreSqlFlexBusOptionsExtension(Action<PostgreSqlOptions> configure)
    {
        _configure = configure;
    }

    public void AddServices(IServiceCollection services)
    {
        services.AddSingleton<CapStorageMarkerService>();
        services.Configure(_configure);
        services.AddSingleton<IConfigureOptions<PostgreSqlOptions>, ConfigurePostgreSqlOptions>();

        services.AddSingleton<IDataStorage, PostgreSqlDataStorage>();
        services.AddSingleton<IStorageInitializer, PostgreSqlStorageInitializer>();
        services.AddTransient<ICapTransaction, PostgreSqlFlexBusTransaction>();
    }
}