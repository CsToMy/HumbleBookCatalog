using HumbleBookCatalog.Import;
using HumbleBookCatalog.Tests.TestUtils;
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
        <div class="subproducts-holder">
            <div class="subproduct-selector">
                <div class="text-holder">
                    <h2>{title}</h2>
                </div>
            </div>
        </div>
        """;

        var result = HTMLParser.Parse(html);
        Assert.That(result, Is.Empty);
    }

    [TestCaseSource(typeof(InvalidHTMLGenerator))]
    public void ParseInvalidHTML(string invalidHtml)
    {
        var result = HTMLParser.Parse(invalidHtml);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
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
        <div class="subproducts-holder">
            <div class="subproduct-selector">
                <div class="text-holder">
                    <h2>Fundamentals of Software Architecture</h2>
                </div>
            </div>
        </div>
        """;

        var result = HTMLParser.Parse(html);

        Assert.That(result, Has.Count.EqualTo(1));
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
        <div class="subproducts-holder">
            <div class="subproduct-selector">
                <div class="text-holder">
                    <p>O'Reilly</p>
                </div>
            </div>
        </div>
        """;

        var result = HTMLParser.Parse(html);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ParseHtmlEntities()
    {
        var html = """
        <div class="subproducts-holder">
            <div class="subproduct-selector">
                <div class="text-holder">
                    <h2>C# &amp; .NET Development</h2>
                    <p>O'Reilly</p>
                </div>
            </div>
        </div>
        """;

        var result = HTMLParser.Parse(html);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Title,
            Is.EqualTo("C# & .NET Development"));
        Assert.That(result[0].Publisher, Is.EqualTo("O'Reilly"));
    }

    [Test]
    public void ParseNestedElementsInTitle()
    {
        var html = """
        <div class="subproducts-holder">
            <div class="subproduct-selector">
                <div class="text-holder">
                    <h2>
                        <strong>Fundamentals</strong>
                        of Software Architecture
                    </h2>
                    <p>O'Reilly</p>
                </div>
            </div>
        </div>
        """;

        var result = HTMLParser.Parse(html);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Title,
            Is.EqualTo("Fundamentals of Software Architecture"));
        Assert.That(result[0].Publisher, Is.EqualTo("O'Reilly"));
    }

    [Test]
    public void ParseMultipleBooks()
    {
        var html = """
        <div class="subproducts-holder">
            <div class="subproduct-selector">
                <div class="text-holder">
                    <h2>Clean Architecture</h2>
                    <p>O'Reilly</p>
                </div>
            </div>
        
            <div class="subproduct-selector">
                <div class="text-holder">
                    <h2>Python Cookbook</h2>
                </div>
            </div>
        
            <div class="subproduct-selector">
                <div class="text-holder">
                    <h2>Domain-Driven Design</h2>
                    <p>Addison-Wesley</p>
                </div>
            </div>
        </div>
        """;

        var result = HTMLParser.Parse(html);

        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result.Select(x => x.Title),
            Is.EqualTo(new[] { "Clean Architecture", "Python Cookbook", "Domain-Driven Design" }));
    }

    [Test]
    public void ParseMixedValidAndInvalidBooks()
    {
        var html = """
        <div class="subproducts-holder">
            <div class="subproduct-selector">
                <div class="text-holder">
                    <h2>Clean Architecture</h2>
                    <p>O'Reilly</p>
                </div>
            </div>

            <div class="subproduct-selector">
                <div class="text-holder">
                    <p>Unknown Publisher</p>
                </div>
            </div>

            <div class="subproduct-selector">
                <div class="text-holder">
                    <h2>Domain-Driven Design</h2>
                    <p>Addison-Wesley</p>
                </div>
            </div>
        </div>
        """;

        var result = HTMLParser.Parse(html);

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Title, Is.EqualTo("Clean Architecture"));
        Assert.That(result[1].Title, Is.EqualTo("Domain-Driven Design"));
    }

    [Test]
    public void ParseBookMarkupInsideComment()
    {
        var html = """
        <!--
        <div class="subproducts-holder">
            <div class="subproduct-selector">
                <div class="text-holder">
                    <h2>Clean Architecture</h2>
                </div>
            </div>
        </div>
        -->
        """;

        var result = HTMLParser.Parse(html);
        Assert.That(result, Is.Empty);
    }

    [TestCase("\u200B\u200B")]
    [TestCase("\uFEFF\uFEFF")]
    [TestCase(" \u200B ")]
    [TestCase("\uFEFF\u200B")]
    public void ParseInvisibleCharactersOnly(string input)
    {
        var result = HTMLParser.Parse(input);
        Assert.That(result, Is.Empty);
    }
}
