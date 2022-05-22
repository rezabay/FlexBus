// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using FlexBus.Persistence;
using FlexBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace FlexBus.PostgreSql
{
    public class PostgreSqlStorageInitializer : IStorageInitializer
    {
        private readonly ILogger _logger;
        private readonly IOptions<PostgreSqlOptions> _options;

        public PostgreSqlStorageInitializer(
            ILogger<PostgreSqlStorageInitializer> logger,
            IOptions<PostgreSqlOptions> options)
        {
            _options = options;
            _logger = logger;
        }

        public virtual string GetPublishedTableName()
        {
            return $"\"{_options.Value.Schema}\".\"published\"";
        }

        public virtual string GetReceivedTableName()
        {
            return $"\"{_options.Value.Schema}\".\"received\"";
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            var sql = CreateDbTablesScript(_options.Value.Schema);
            using (var connection = new NpgsqlConnection(_options.Value.ConnectionString))
                connection.ExecuteNonQuery(sql);

            await Task.CompletedTask;

            _logger.LogDebug("Ensuring all create database tables script are applied.");
        }


        protected virtual string CreateDbTablesScript(string schema)
        {
            var batchSql = $@"
CREATE SCHEMA IF NOT EXISTS ""{schema}"";

CREATE TABLE IF NOT EXISTS {GetReceivedTableName()}(
	""Id"" BIGINT PRIMARY KEY NOT NULL,
    ""Version"" VARCHAR(20) NOT NULL,
	""Name"" VARCHAR(200) NOT NULL,
	""Group"" VARCHAR(200) NULL,
	""Content"" TEXT NULL,
	""Retries"" INT NOT NULL,
	""Added"" TIMESTAMP NOT NULL,
    ""ExpiresAt"" TIMESTAMP NULL,
	""StatusName"" VARCHAR(50) NOT NULL,
    ""FetcheDate"" TIMESTAMP NULL
);

CREATE TABLE IF NOT EXISTS {GetPublishedTableName()}(
	""Id"" BIGINT PRIMARY KEY NOT NULL,
    ""Version"" VARCHAR(20) NOT NULL,
	""Name"" VARCHAR(200) NOT NULL,
	""Content"" TEXT NULL,
	""Retries"" INT NOT NULL,
	""Added"" TIMESTAMP NOT NULL,
    ""ExpiresAt"" TIMESTAMP NULL,
	""StatusName"" VARCHAR(50) NOT NULL,
    ""FetcheDate"" TIMESTAMP NULL
);

ALTER TABLE {GetPublishedTableName()} ADD COLUMN IF NOT EXISTS ""ScheduleDate"" TIMESTAMP NULL;

alter table {GetPublishedTableName()} alter column ""Added"" type timestamptz using ""Added""::timestamptz;
alter table {GetPublishedTableName()} alter column ""ExpiresAt"" type timestamptz using ""ExpiresAt""::timestamptz;
alter table {GetPublishedTableName()} alter column ""FetcheDate"" type timestamptz using ""FetcheDate""::timestamptz;
alter table {GetPublishedTableName()} alter column ""ScheduleDate"" type timestamptz using ""ScheduleDate""::timestamptz;

alter table {GetReceivedTableName()} alter column ""Added"" type timestamptz using ""Added""::timestamptz;
alter table {GetReceivedTableName()} alter column ""ExpiresAt"" type timestamptz using ""ExpiresAt""::timestamptz;
alter table {GetReceivedTableName()} alter column ""FetcheDate"" type timestamptz using ""FetcheDate""::timestamptz;
";
            return batchSql;
        }
    }
}
