using System.Collections;
using System.Collections.Generic;

namespace HumbleBookCatalog.Tests.TestUtils;

internal sealed class InvalidHTMLGenerator : IEnumerable<string>
{
    public IEnumerator<string> GetEnumerator()
    {
        yield return "<div>";
        yield return"</div>";
        yield return "<h2";
        yield return "<";
        yield return ">";
        yield return "<<<<<";
        yield return ">>>>>>";
        yield return "<div><span>";
        yield return "<div><span></div>";
        yield return "<div><div><div>";
        yield return "<html><body><div>";
        yield return "<!DOCTYPE html><html><body>";
        yield return "<div><p>Book title</div></p>";
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
