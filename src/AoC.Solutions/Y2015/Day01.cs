using KE.AoC.Core.Solution;

namespace KE.AoC.Solutions.Y2015;

[Solution(2015, 1)]
public sealed class Day01 : SolutionBase
{
    /// <summary>
    /// Calculates the final floor as a difference between the number of '(' and ')' characters in the input string.
    /// </summary>
    /// <param name="input">The input string containing '(' and ')' characters.</param>
    /// <returns>The final floor number.</returns>
    public override object PartOne(string input)
    {
        ReadOnlySpan<char> span = input.AsSpan();

        return span.Count('(') - span.Count(')');
    }

    /// <summary>
    /// Calculates the position of the first character that causes the floor to become negative.
    /// </summary>
    /// <param name="input">The input string containing '(' and ')' characters.</param>
    /// <returns>The 1-based position of the first character that causes the floor to become negative, or -1 if it never becomes negative.</returns>
    public override object PartTwo(string input)
    {
        ReadOnlySpan<char> span = input.AsSpan();

        int floor = 0;
        for (int i = 0; i < span.Length; i++)
        {
            floor += span[i] switch
            {
                '(' => 1,
                ')' => -1,
                _ => 0
            };

            if (floor < 0)
                return i + 1;
        }

        return -1;
    }
}
