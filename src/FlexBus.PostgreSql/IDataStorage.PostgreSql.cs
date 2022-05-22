// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FlexBus.Internal;
using FlexBus.Messages;
using FlexBus.Monitoring;
using FlexBus.Persistence;
using FlexBus.Serialization;
using FlexBus;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Npgsql;

namespace FlexBus.PostgreSql;

public class PostgreSqlDataStorage : IDataStorage
{
    private readonly IOptions<CapOptions> _capOptions;
    private readonly IStorageInitializer _initializer;
    private readonly IOptions<PostgreSqlOptions> _options;
    private readonly ISerializer _serializer;
    private readonly string _pubName;
    private readonly string _recName;

    public PostgreSqlDataStorage(
        IOptions<PostgreSqlOptions> options,
        IOptions<CapOptions> capOptions,
        IStorageInitializer initializer,
        ISerializer serializer)
    {
        _capOptions = capOptions;
        _initializer = initializer;
        _options = options;
        _serializer = serializer;
        _pubName = initializer.GetPublishedTableName();
        _recName = initializer.GetReceivedTableName();
    }

    public Task<OperateResult> ChangePublishStateAsync(MediumMessage message, 
        StatusName state, 
        Func<Task<OperateResult>> action) =>
        ChangeMessageStateAsync(_pubName, message, state, action);

    public Task<OperateResult> ChangeReceiveStateAsync(MediumMessage message, 
        StatusName state,
        Func<Task<OperateResult>> action) =>
        ChangeMessageStateAsync(_recName, message, state, action);

    public async Task<MediumMessage> StoreMessage(string name, Message content, object dbTransaction = null)
    {
        var sql = @$"
INSERT INTO {_pubName} (""Id"", ""Version"", ""Name"", ""Content"", ""Retries"", ""Added"", ""ExpiresAt"", ""StatusName"", ""ScheduleDate"")
VALUES(@Id, '{_options.Value.Version}', @Name, @Content, @Retries, @Added, @ExpiresAt, @StatusName, @ScheduleDate);";

        var message = new MediumMessage
        {
            DbId = content.GetId(),
            Origin = content,
            Content = _serializer.Serialize(content),
            Added = DateTime.UtcNow,
            ExpiresAt = null,
            Retries = 0,
            ScheduleDate = content.GetScheduleDate()
        };

        object sqlParams = new
        {
            Id = long.Parse(message.DbId),
            Name = name,
            Content = message.Content,
            Retries = message.Retries,
            Added = message.Added,
            ExpiresAt = message.ExpiresAt.HasValue ? (object)message.ExpiresAt.Value : DBNull.Value,
            StatusName = nameof(StatusName.Scheduled),
            ScheduleDate = message.ScheduleDate
        };

        if (dbTransaction == null)
        {
            using var connection = CreateAndOpenConnection();
            await connection.ExecuteScalarAsync(sql, sqlParams);
        }
        else
        {
            var dbTrans = dbTransaction as IDbTransaction;
            if (dbTrans == null && dbTransaction is IDbContextTransaction dbContextTrans)
                dbTrans = dbContextTrans.GetDbTransaction();

            var conn = dbTrans?.Connection;
            await conn.ExecuteScalarAsync(sql, sqlParams, dbTrans);
        }

        return message;
    }

    public Task StoreReceivedExceptionMessage(string name, string group, string content)
    {
        object sqlParams = new
        {
            Id = SnowflakeId.Default().NextId(),
            Name = name,
            Group = group,
            Content = content,
            Retries = _capOptions.Value.FailedRetryCount,
            Added = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(15),
            StatusName = nameof(StatusName.Failed)
        };

        return StoreReceivedMessage(sqlParams);
    }

    public async Task<MediumMessage> StoreReceivedMessage(string name, string group, Message message)
    {
        var mdMessage = new MediumMessage
        {
            DbId = SnowflakeId.Default().NextId().ToString(),
            Origin = message,
            Added = DateTime.UtcNow,
            ExpiresAt = null,
            Retries = 0
        };

        object sqlParams = new
        {
            Id = long.Parse(mdMessage.DbId),
            Name = name,
            Group = group,
            Content = _serializer.Serialize(mdMessage.Origin),
            Retries = mdMessage.Retries,
            Added = mdMessage.Added,
            ExpiresAt = mdMessage.ExpiresAt.HasValue ? (object)mdMessage.ExpiresAt.Value : DBNull.Value,
            StatusName = nameof(StatusName.Scheduled)
        };

        await StoreReceivedMessage(sqlParams);
        return mdMessage;
    }

    public async Task<int> DeleteExpiresAsync(string table, 
        DateTime timeout, 
        int batchCount = 1000,
        CancellationToken token = default)
    {
        var query = @$"
DELETE FROM {table} 
WHERE ""Id"" IN (SELECT ""Id"" FROM {table} WHERE ""ExpiresAt"" < @timeout LIMIT @batchCount);
";

        using var connection = CreateAndOpenConnection();

        var count = await connection.ExecuteScalarAsync<int>(query, new { timeout = timeout, batchCount = batchCount });
        return count;
    }

    public async Task<IEnumerable<MediumMessage>> GetNextPublishedMessage()
    {
        var sql = @$"
UPDATE {_pubName} 
SET ""FetcheDate"" = NOW() AT TIME ZONE 'UTC'
WHERE ""Id"" IN (
    SELECT ""Id"" 
    FROM {_pubName} 
    WHERE 
        ""Retries"" < {_capOptions.Value.FailedRetryCount}
        AND ""Version"" = '{_capOptions.Value.Version}' 
        AND (""FetcheDate"" IS NULL OR ""FetcheDate"" < NOW() AT TIME ZONE 'UTC' + INTERVAL '{_capOptions.Value.SendMessageTimeout} SECONDS')
        AND ""StatusName"" <> '{StatusName.Succeeded}'
        AND (""ScheduleDate"" IS NULL OR ""ScheduleDate"" < NOW() AT TIME ZONE 'UTC')
    ORDER BY
        ""Added""
    FOR UPDATE SKIP LOCKED
    LIMIT 100
)
RETURNING ""Id"" AS ""DbId"", ""Content"", ""Retries"", ""Added"", ""ScheduleDate"";
";

        using var connection = CreateAndOpenConnection();
        using var trx = connection.BeginTransaction(IsolationLevel.ReadCommitted);

        var messages = await connection.QueryAsync<MediumMessage>(sql, transaction: trx);
        trx.Commit();

        foreach (var message in messages)
        {
            message.Origin = _serializer.Deserialize(message.Content);
        }

        return messages;
    }

    public IMonitoringApi GetMonitoringApi()
    {
        return new PostgreSqlMonitoringApi(_options, _initializer);
    }

    private async Task<OperateResult> ChangeMessageStateAsync(string tableName,
        MediumMessage message,
        StatusName state,
        Func<Task<OperateResult>> action)
    {
        var sql = $@"
UPDATE {tableName} 
SET 
    ""Content""= @Content, 
    ""Retries""=@Retries,
    ""ExpiresAt"" = @ExpiresAt, 
    ""StatusName"" = @StatusName 
WHERE ""Id"" = @Id";

        using var connection = CreateAndOpenConnection();
        using var trx = connection.BeginTransaction(IsolationLevel.ReadCommitted);

        await connection.ExecuteScalarAsync(sql, new
        {
            Id = long.Parse(message.DbId),
            Content = _serializer.Serialize(message.Origin),
            Retries = message.Retries,
            ExpiresAt = message.ExpiresAt,
            StatusName = state.ToString("G")
        });

        OperateResult result = OperateResult.Success;
        if (action != null)
        {
            result = await action?.Invoke();
        }

        if (result == OperateResult.Success)
        {
            trx.Commit();
        }

        return result;
    }

    private async Task StoreReceivedMessage(object sqlParams)
    {
        var sql = @$"
INSERT INTO {_recName}(""Id"",""Version"", ""Name"", ""Group"", ""Content"", ""Retries"", ""Added"", ""ExpiresAt"", ""StatusName"")
VALUES(@Id,'{_capOptions.Value.Version}', @Name, @Group, @Content, @Retries, @Added, @ExpiresAt, @StatusName) 
RETURNING ""Id"";
";

        using var connection = CreateAndOpenConnection();
        await connection.ExecuteScalarAsync(sql, sqlParams);
    }

    internal NpgsqlConnection CreateAndOpenConnection()
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(_options.Value.ConnectionString);

        var connection = new NpgsqlConnection(connectionStringBuilder.ToString());
        connection.Open();

        return connection;
    }
}