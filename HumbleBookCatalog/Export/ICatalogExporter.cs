using System.Threading;
using System.Threading.Tasks;

namespace HumbleBookCatalog.Export;

internal interface ICatalogExporter
{
    Task Export(string fileName, CancellationToken cancellationToken);
}
