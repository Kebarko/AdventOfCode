using KE.AoC.Core.Solution;

namespace KE.AoC.Solutions.Y2015;

[Solution(2015, 8)]
public sealed class Day08 : SolutionBase
{
    /// <summary>
    /// Calculates the difference between the number of characters of code for string literals and the number of characters in memory for the values of the strings.
    /// </summary>
    /// <param name="input">The input string containing the string literals.</param>
    /// <returns>The difference between the number of characters of code and the number of characters in memory.</returns>
    public override object PartOne(string input)
    {
        // number of characters of code for string literals
        int totalCode = 0;

        // number of characters in memory for the values of the strings
        int totalMemory = 0;

        foreach (ReadOnlySpan<char> line in input.EnumerateLines())
        {
            totalCode += line.Length;

            for (int i = 1; i < line.Length - 1; i++) // skip surrounding double quotes
            {
                if (line[i] == '\\')
                    i += line[i + 1] == 'x' ? 3 : 1; // \xHH - 3 extra, \\ or \" - 1 extra

                totalMemory++;
            }
        }

        return totalCode - totalMemory;
    }

    /// <summary>
    /// Calculates the difference between the total number of characters to represent the newly encoded strings and the number of characters of code in each original string literal.
    /// </summary>
    /// <param name="input">The input string containing the string literals.</param>
    /// <returns>The difference between the total number of characters to represent the newly encoded strings and the number of characters of code in each original string literal.</returns>
    public override object PartTwo(string input)
    {
        // number of characters of code in each original string literal
        int totalCode = 0;

        // total number of characters to represent the newly encoded strings
        int totalEncoded = 0;

        foreach (ReadOnlySpan<char> line in input.EnumerateLines())
        {
            totalCode += line.Length;

            totalEncoded += 2; // new surrounding double quotes
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] is '"' or '\\') // escape these characters
                    totalEncoded++;

                totalEncoded++;
            }
        }

        return totalEncoded - totalCode;
    }
}
