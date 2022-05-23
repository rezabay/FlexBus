using System;
using FlexBus;
using FlexBus.PostgreSql;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class FlexBusOptionsExtensions
{
    public static FlexBusOptions UsePostgreSql(this FlexBusOptions options, string connectionString)
    {
        return options.UsePostgreSql(opt => { opt.ConnectionString = connectionString; });
    }

    public static FlexBusOptions UsePostgreSql(this FlexBusOptions options, Action<PostgreSqlOptions> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        configure += x => x.Version = options.Version;

        options.RegisterExtension(new PostgreSqlCapOptionsExtension(configure));

        return options;
    }

    public static FlexBusOptions UseEntityFramework<TContext>(this FlexBusOptions options)
        where TContext : DbContext
    {
        return options.UseEntityFramework<TContext>(opt => { });
    }

    public static FlexBusOptions UseEntityFramework<TContext>(this FlexBusOptions options, Action<EFOptions> configure)
        where TContext : DbContext
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        options.RegisterExtension(new PostgreSqlCapOptionsExtension(x =>
        {
            configure(x);
            x.Version = options.Version;
            x.DbContextType = typeof(TContext);
        }));

        return options;
    }
}