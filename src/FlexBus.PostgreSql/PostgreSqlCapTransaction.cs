using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace FlexBus.PostgreSql;

public class PostgreSqlCapTransaction : CapTransactionBase
{
    public PostgreSqlCapTransaction()
    {
    }

    public override void Commit()
    {
        Debug.Assert(DbTransaction != null);

        switch (DbTransaction)
        {
            case IDbTransaction dbTransaction:
                dbTransaction.Commit();
                break;
            case IDbContextTransaction dbContextTransaction:
                dbContextTransaction.Commit();
                break;
        }
    }

    public override async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        Debug.Assert(DbTransaction != null);

        switch (DbTransaction)
        {
            case IDbTransaction dbTransaction:
                dbTransaction.Commit();
                break;
            case IDbContextTransaction dbContextTransaction:
                await dbContextTransaction.CommitAsync(cancellationToken);
                break;
        }
    }

    public override void Rollback()
    {
        Debug.Assert(DbTransaction != null);

        switch (DbTransaction)
        {
            case IDbTransaction dbTransaction:
                dbTransaction.Rollback();
                break;
            case IDbContextTransaction dbContextTransaction:
                dbContextTransaction.Rollback();
                break;
        }
    }

    public override async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        Debug.Assert(DbTransaction != null);

        switch (DbTransaction)
        {
            case IDbTransaction dbTransaction:
                dbTransaction.Rollback();
                break;
            case IDbContextTransaction dbContextTransaction:
                await dbContextTransaction.RollbackAsync(cancellationToken);
                break;
        }
    }

    public override void Dispose()
    {
        (DbTransaction as IDbTransaction)?.Dispose();
        DbTransaction = null;
    }
}

public static class CapTransactionExtensions
{
    public static ICapTransaction Begin(this ICapTransaction transaction,
        IDbTransaction dbTransaction, bool autoCommit = false)
    {
        transaction.DbTransaction = dbTransaction;
        transaction.AutoCommit = autoCommit;

        return transaction;
    }

    public static ICapTransaction Begin(this ICapTransaction transaction,
        IDbContextTransaction dbTransaction, bool autoCommit = false)
    {
        transaction.DbTransaction = dbTransaction;
        transaction.AutoCommit = autoCommit;

        return transaction;
    }

    /// <summary>
    /// Start the CAP transaction
    /// </summary>
    /// <param name="dbConnection">The <see cref="IDbConnection" />.</param>
    /// <param name="publisher">The <see cref="IFlexBusPublisher" />.</param>
    /// <param name="autoCommit">Whether the transaction is automatically committed when the message is published</param>
    /// <returns>The <see cref="ICapTransaction" /> object.</returns>
    public static ICapTransaction BeginTransaction(this IDbConnection dbConnection,
        IFlexBusPublisher publisher, bool autoCommit = false)
    {
        if (dbConnection.State == ConnectionState.Closed) dbConnection.Open();

        var dbTransaction = dbConnection.BeginTransaction();
        publisher.Transaction.Value = publisher.ServiceProvider.GetService<ICapTransaction>();
        return publisher.Transaction.Value.Begin(dbTransaction, autoCommit);
    }

    /// <summary>
    /// Start the CAP transaction
    /// </summary>
    /// <param name="database">The <see cref="DatabaseFacade" />.</param>
    /// <param name="publisher">The <see cref="IFlexBusPublisher" />.</param>
    /// <param name="autoCommit">Whether the transaction is automatically committed when the message is published</param>
    /// <returns>The <see cref="IDbContextTransaction" /> of EF DbContext transaction object.</returns>
    public static IDbContextTransaction BeginTransaction(this DatabaseFacade database,
        IFlexBusPublisher publisher, bool autoCommit = false)
    {
        var trans = database.BeginTransaction();
        publisher.Transaction.Value = publisher.ServiceProvider.GetService<ICapTransaction>();
        var capTrans = publisher.Transaction.Value.Begin(trans, autoCommit);
        return new FlexBusEFDbTransaction(capTrans);
    }
}