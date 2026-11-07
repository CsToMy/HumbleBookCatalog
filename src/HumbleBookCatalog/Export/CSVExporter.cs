using System;
using System.Threading;
using System.Threading.Tasks;

namespace HumbleBookCatalog.Export;

internal sealed class CSVExporter : ICatalogExporter
{
    public async Task Export(string fileName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
