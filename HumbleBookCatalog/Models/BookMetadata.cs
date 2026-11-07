using System.Text.Json.Serialization;

namespace HumbleBookCatalog.Models;

internal sealed record class BookMetadata
{
    [JsonPropertyName("authors")]
    [JsonRequired]
    public required string[] Authors { get; init; }

    [JsonPropertyName("isbn")]
    [JsonRequired]
    public required string ISBN { get; init; }

    [JsonPropertyName("publishedYear")]
    [JsonRequired]
    public int? PublishedYear { get; init; }
}
