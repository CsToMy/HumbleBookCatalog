using AngleSharp.Html.Parser;
using HumbleBookCatalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HumbleBookCatalog.Import;

internal static class HTMLParser
{
    public static IReadOnlyList<Book> Parse(string html)
    {
        if (IsNullOrWhiteSpaceWithExtraCheck(html))
        {
            return [];
        }

        var parser = new HtmlParser();
        
        using var document = parser.ParseDocument(html);
        var catalog = document.QuerySelector(".subproducts-holder");
            
        if (catalog is null)
        {
            return [];
        }
        
        var bookEntries = catalog.QuerySelectorAll(".subproduct-selector");
        
        List<Book> books = [];

        foreach (var bookEntry in bookEntries)
        {
            var title = NormalizeText(
                bookEntry.QuerySelector(".text-holder h2")?.TextContent);

            if (IsNullOrWhiteSpaceWithExtraCheck(title))
            {
                continue;
            }

            var publisher = NormalizeText(
                bookEntry.QuerySelector(".text-holder p")?.TextContent);

            books.Add(new Book
            {
                Title = title,
                Publisher = publisher
            });
        }

        return books.AsReadOnly();
    }

    private static string NormalizeText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return string.Join(
            ' ',
            value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }

    private static bool IsNullOrWhiteSpaceWithExtraCheck(string text)
    {
        return string.IsNullOrWhiteSpace(text)
            || text.All(char.IsWhiteSpace)
            || text.All(c => char.IsWhiteSpace(c)
            || c == '\u200B'
            || c == '\uFEFF');
    }
}
