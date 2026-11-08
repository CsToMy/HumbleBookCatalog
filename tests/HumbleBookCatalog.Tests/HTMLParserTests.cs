using HumbleBookCatalog.Import;
using HumbleBookCatalog.Tests.TestUtils;
using System.IO;
using System.Linq;

namespace HumbleBookCatalog.Tests;

[TestFixture]
internal sealed class HTMLParserTests
{
    [Test]
    [TestCaseSource(typeof(WhitespaceGenerator))]
    public void EmptyOrWhiteSpaceHTML(string input)
    {
        var result = HTMLParser.Parse(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void NullParsing()
    {
        var result = HTMLParser.Parse(null!);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ParseMixedWhitespaceInput()
    {
        var input = " \t\n\r\u00A0\u2003\u3000 ";
        var result = HTMLParser.Parse(input);
        Assert.That(result, Is.Empty);
    }

    [TestCase("\u200B")]
    [TestCase("\uFEFF")]
    public void ParseNonWhitespaceUnicodeCharacters(string input)
    {
        var result = HTMLParser.Parse(input);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("\t\n\r")]
    [TestCase("\u00A0")]
    public void ParseBookWithWhitespaceTitle(string title)
    {
        var html = $"""
        <div class="book-item">
            <div class="book-title">{title}</div>
            <div class="publisher">O'Reilly</div>
        </div>
        """;

        var result = HTMLParser.Parse(html);

        Assert.That(result, Is.Empty);
    }

    [TestCaseSource(typeof(InvalidHTMLGenerator))]
    public void ParseInvalidHTML(string invalidHtml)
    {
        Assert.Throws<InvalidDataException>(() => HTMLParser.Parse(invalidHtml));
    }

    [TestCaseSource(typeof(ValidEmptyHTMLGenerator))]
    public void ParseValidHTMLWithoutAnyData(string validHtml)
    {
        var result = HTMLParser.Parse(validHtml);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ParseBookWithoutPublisher()
    {
        var html = """
        <div class="book-item">
            <div class="book-title">
                Fundamentals of Software Architecture
            </div>
        </div>
        """;

        var result = HTMLParser.Parse(html);

        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].Title,
            Is.EqualTo("Fundamentals of Software Architecture"));
        Assert.That(result[0].Publisher, Is.Null.Or.Empty);
    }

    [Test]
    public void ParseBookWithoutTitle()
    {
        var html = """
        <div class="book-item">
            <div class="publisher">O'Reilly</div>
        </div>
        """;

        var result = HTMLParser.Parse(html);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ParseHtmlEntities()
    {
        var html = """
        <div class="book-item">
            <div class="book-title">
                C# &amp; .NET Development
            </div>
            <div class="publisher">O'Reilly</div>
        </div>
        """;

        var result = HTMLParser.Parse(html);

        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].Title,
            Is.EqualTo("C# & .NET Development"));
        Assert.That(result[0].Publisher, Is.EqualTo("O'Reilly"));
    }

    [Test]
    public void ParseNestedElementsInTitle()
    {
        var html = """
        <div class="book-item">
            <div class="book-title">
                <span>
                    <strong>Fundamentals</strong>
                    of Software Architecture
                </span>
            </div>
            <div class="publisher">O'Reilly</div>
        </div>
        """;

        var result = HTMLParser.Parse(html);

        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].Title,
            Is.EqualTo("Fundamentals of Software Architecture"));
    }

    [Test]
    public void ParseMultipleBooks()
    {
        var html = """
        <div class="book-item">
            <div class="book-title">Book One</div>
            <div class="publisher">Publisher One</div>
        </div>

        <div class="book-item">
            <div class="book-title">Book Two</div>
            <div class="publisher">Publisher Two</div>
        </div>

        <div class="book-item">
            <div class="book-title">Book Three</div>
        </div>
        """;

        var result = HTMLParser.Parse(html);

        Assert.That(result, Has.Length.EqualTo(3));
        Assert.That(result.Select(x => x.Title),
            Is.EqualTo(new[] { "Book One", "Book Two", "Book Three" }));
    }

    [Test]
    public void ParseMixedValidAndInvalidBooks()
    {
        var html = """
        <div class="book-item">
            <div class="book-title">Valid Book</div>
            <div class="publisher">O'Reilly</div>
        </div>

        <div class="book-item">
            <div class="publisher">Missing title</div>
        </div>

        <div class="book-item">
            <div class="book-title">Another Valid Book</div>
        </div>
        """;

        var result = HTMLParser.Parse(html);

        Assert.That(result, Has.Length.EqualTo(2));
        Assert.That(result[0].Title, Is.EqualTo("Valid Book"));
        Assert.That(result[1].Title, Is.EqualTo("Another Valid Book"));
    }

    [Test]
    public void ParseBookMarkupInsideComment()
    {
        var html = """
        <!--
        <div class="book-item">
            <div class="book-title">Fake Book</div>
        </div>
        -->
        """;

        var result = HTMLParser.Parse(html);

        Assert.That(result, Is.Empty);
    }
}
