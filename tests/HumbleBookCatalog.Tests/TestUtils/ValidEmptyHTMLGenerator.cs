using System.Collections;
using System.Collections.Generic;

namespace HumbleBookCatalog.Tests.TestUtils;

internal sealed class ValidEmptyHTMLGenerator : IEnumerable<string>
{
    public IEnumerator<string> GetEnumerator()
    {
        yield return "<h1>Some title</h1>";
        yield return "<html><body><h1>Welcome</h1></body></html>";
        yield return "<div>Hello World</div>";
        yield return "<p>This is a paragraph.</p>";
        yield return "<button>Click me</button>";
        yield return "<nav><a href='/'>Home</a></nav>";
        yield return "<table><tr><td>Product</td></tr></table>";
        yield return "<article><h1>News</h1><p>Latest news</p></article>";
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
