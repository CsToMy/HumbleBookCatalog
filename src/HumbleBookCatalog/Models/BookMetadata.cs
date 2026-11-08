using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HumbleBookCatalog.Models;

internal sealed record class BookMetadata
{
    [JsonPropertyName("authors")]
    [JsonRequired]
    public required IReadOnlyList<string> Authors { get; init; }

    [JsonPropertyName("isbn10")]
    [JsonRequired]
    public string? ISBN10 { get; init; }

    [JsonPropertyName("isbn13")]
    [JsonRequired]
    public string? ISBN13 { get; init; }

    [JsonPropertyName("publishedYear")]
    [JsonRequired]
    public int? PublishedYear { get; init; }
}
