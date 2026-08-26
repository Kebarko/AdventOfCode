using KE.AoC.Core.Solution;

namespace KE.AoC.Solutions.Y2015;

[Solution(2015, 5)]
public sealed class Day05 : SolutionBase
{
    /// <summary>
    /// Calculates the number of "nice" strings based on specific criteria:
    /// 1. The string must contain at least three vowels (a, e, i, o, u).
    /// 2. The string must contain at least one letter that appears twice in a row.
    /// 3. The string must not contain the substrings "ab", "cd", "pq", or "xy".
    /// </summary>
    /// <param name="input">The input string containing lines to evaluate.</param>
    /// <returns>The number of "nice" strings.</returns>
    public override object PartOne(string input)
    {
        int result = 0;
        foreach (ReadOnlySpan<char> line in input.EnumerateLines())
        {
            if (line.IsEmpty)
                continue;

            int vowels = 0;
            bool hasDouble = false;
            bool hasDisallowed = false;
            char prev = '\0';
            foreach (char c in line)
            {
                if (c is 'a' or 'e' or 'i' or 'o' or 'u')
                    vowels++;

                if (c == prev)
                    hasDouble = true;

                if ((prev, c) is ('a', 'b') or ('c', 'd') or ('p', 'q') or ('x', 'y'))
                    hasDisallowed = true;

                prev = c;
            }

            if (vowels >= 3 && hasDouble && !hasDisallowed)
                result++;
        }

        return result;
    }

    /// <summary>
    /// Calculates the number of "nice" strings based on updated criteria:
    /// 1. The string must contain a pair of any two letters that appears at least twice in the string without overlapping.
    /// 2. The string must contain at least one letter which repeats with exactly one letter between them (e.g., "xyx", "aba").
    /// </summary>
    /// <param name="input">The input string containing lines to evaluate.</param>
    /// <returns>The number of "nice" strings.</returns>
    public override object PartTwo(string input)
    {
        int result = 0;
        foreach (ReadOnlySpan<char> line in input.EnumerateLines())
        {
            if (line.IsEmpty)
                continue;

            bool hasSandwich = false;
            char prev = '\0';
            char prevPrev = '\0';
            foreach (char c in line)
            {
                if (c == prevPrev)
                {
                    hasSandwich = true;
                    break;
                }

                prevPrev = prev;
                prev = c;
            }

            if (!hasSandwich)
                continue;

            bool hasRepeatedPair = false;
            for (int i = 0; i < line.Length - 1; i++)
            {
                if (line[(i + 2)..].Contains(line.Slice(i, 2), StringComparison.Ordinal))
                {
                    hasRepeatedPair = true;
                    break;
                }
            }

            if (hasRepeatedPair)
                result++;
        }

        return result;
    }
}
