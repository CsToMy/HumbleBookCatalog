using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HumbleBookCatalog.Export;

internal sealed class JSONExporter : ICatalogExporter
{
    public async Task Export(string fileName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
