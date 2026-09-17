using KE.AoC.Core.Solution;

namespace KE.AoC.Solutions.Y2016;

/// <summary>
/// --- Day 6: Signals and Noise ---
/// </summary>
[Solution(2016, 6)]
public sealed class Day06 : SolutionBase
{
    /// <summary>
    /// Calculates the message by analyzing the character frequencies in each column of the input,
    /// finding the character with the maximum count.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The decoded message.</returns>
    public override object PartOne(string input)
    {
        return GetMessage(input.AsSpan(), true);
    }

    /// <summary>
    /// Calculates the message by analyzing the character frequencies in each column of the input,
    /// finding the character with the minimum count.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The decoded message.</returns>
    public override object PartTwo(string input)
    {
        return GetMessage(input.AsSpan(), false);
    }

    /// <summary>
    /// Gets the message by analyzing the character frequencies in each column of the input.
    /// </summary>
    /// <param name="span">The input span.</param>
    /// <param name="findMax">
    /// If true, finds the character with the maximum count;
    /// otherwise, finds the character with the minimum count.
    /// </param>
    /// <returns>The decoded message.</returns>
    private static string GetMessage(ReadOnlySpan<char> span, bool findMax)
    {
        int width = span.IndexOfAny('\r', '\n');
        Span<int> counts = stackalloc int[26 * width];
        foreach (ReadOnlySpan<char> line in span.EnumerateLines())
        {
            for (int col = 0; col < width; col++)
            {
                int letter = line[col] - 'a';
                counts[26 * col + letter]++;
            }
        }

        Span<char> result = stackalloc char[width];

        for (int col = 0; col < width; col++)
        {
            result[col] = GetChar(counts.Slice(26 * col, 26), findMax);
        }

        return new string(result);
    }

    /// <summary>
    /// Gets the character corresponding to the maximum or minimum count in a column of letter counts.
    /// </summary>
    /// <param name="column">The column of letter counts.</param>
    /// <param name="findMax">
    /// If true, finds the character with the maximum count;
    /// otherwise, finds the character with the minimum count.
    /// </param>
    /// <returns>The character with the maximum or minimum count in the column.</returns>
    private static char GetChar(Span<int> column, bool findMax)
    {
        int bestIndex = 0;
        int bestValue = column[0];

        for (int i = 1; i < column.Length; i++)
        {
            if (findMax ? column[i] > bestValue : column[i] < bestValue)
            {
                bestIndex = i;
                bestValue = column[i];
            }
        }

        return (char)('a' + bestIndex);
    }
}
