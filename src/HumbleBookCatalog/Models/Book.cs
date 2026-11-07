using System.Text.Json.Serialization;

namespace HumbleBookCatalog.Models;

internal sealed record class Book
{
    [JsonRequired]
    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonRequired]
    [JsonPropertyName("publisher")]
    public required string Publisher { get; init; }
}
