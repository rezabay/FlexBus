// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace Fooreco.CAP
{
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
}
