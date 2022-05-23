using System.Threading;
using System.Threading.Tasks;

namespace FlexBus;

public abstract class CapTransactionBase : ICapTransaction
{
    protected CapTransactionBase()
    {
    }

    public bool AutoCommit { get; set; }

    public object DbTransaction { get; set; }

    public abstract void Commit();

    public abstract Task CommitAsync(CancellationToken cancellationToken = default);

    public abstract void Rollback();

    public abstract Task RollbackAsync(CancellationToken cancellationToken = default);

    public abstract void Dispose();
}