using System.Text.Json.Serialization;

namespace HumbleBookCatalog.Models;

internal sealed record class Book
{
    [JsonRequired]
    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("publisher")]
    public string? Publisher { get; init; }
}
