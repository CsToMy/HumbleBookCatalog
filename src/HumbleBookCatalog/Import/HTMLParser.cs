using HumbleBookCatalog.Models;

namespace HumbleBookCatalog.Import;

internal static class HTMLParser
{
    public static Book[] ParseHTML(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return [];
        }

        return [];   
    }
}
