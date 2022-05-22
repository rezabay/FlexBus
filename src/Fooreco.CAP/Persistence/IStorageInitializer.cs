using System.Threading;
using System.Threading.Tasks;

namespace Fooreco.CAP.Persistence
{
    public interface IStorageInitializer
    {
        Task InitializeAsync(CancellationToken cancellationToken);

        string GetPublishedTableName();

        string GetReceivedTableName();
    }
}
