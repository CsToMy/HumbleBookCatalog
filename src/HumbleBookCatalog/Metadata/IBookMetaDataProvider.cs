using HumbleBookCatalog.Models;
using System.Threading;
using System.Threading.Tasks;

namespace HumbleBookCatalog.Metadata;

internal interface IBookMetaDataProvider
{
    Task<BookMetadata> GetBookMetadataAsync(string title, string publisher, CancellationToken cancellationToken);
}
