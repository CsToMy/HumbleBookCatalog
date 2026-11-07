using HumbleBookCatalog.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HumbleBookCatalog.Metadata;

internal sealed class OpenLibraryProvider : IBookMetaDataProvider
{
    public async Task<BookMetadata> GetBookMetadataAsync(string title, string publisher, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
