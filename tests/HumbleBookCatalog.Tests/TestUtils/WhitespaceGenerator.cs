using System.Collections;
using System.Collections.Generic;

namespace HumbleBookCatalog.Tests.TestUtils;

/// <summary>
/// Generate whitespace inputs for tests.
/// </summary>
internal sealed class WhitespaceGenerator : IEnumerable<string>
{
    /// <summary>
    /// Iterates over every whitespace characters.
    /// </summary>
    /// <returns>Whitespace characters.</returns>
    public IEnumerator<string> GetEnumerator()
    {
        for (var i = 0; i <= char.MaxValue; i++)
        {
            var character = (char)i;
            if (char.IsWhiteSpace(character))
                yield return character.ToString();
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
