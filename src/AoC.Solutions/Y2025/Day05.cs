using KE.AoC.Core.Solution;
using KE.AoC.Solutions.Common;
using System.Globalization;

namespace KE.AoC.Solutions.Y2025;

/// <summary>
/// --- Day 5: Cafeteria ---
/// </summary>
[Solution(2025, 5)]
public sealed class Day05 : SolutionBase
{
    /// <summary>
    /// Counts the number of ingredients that fall within any of the fresh intervals.
    /// </summary>
    /// <param name="input">The input string containing the fresh intervals and ingredients.</param>
    /// <returns>The number of ingredients that fall within any of the fresh intervals.</returns>
    public override object PartOne(string input)
    {
        ParseInput(input.AsSpan(), out ICollection<LongInterval> freshIntervals, out ISet<long> ingredients);

        int freshCount = 0;

        foreach (long ingredient in ingredients)
        {
            foreach (LongInterval interval in freshIntervals)
            {
                if (ingredient >= interval.Start && ingredient <= interval.End)
                {
                    freshCount++;
                    break;
                }
            }
        }

        return freshCount;
    }

    /// <summary>
    /// Calculates the total length of merged fresh intervals.
    /// </summary>
    /// <param name="input">The input string containing the fresh intervals.</param>
    /// <returns>The total length of merged fresh intervals.</returns>
    public override object PartTwo(string input)
    {
        ParseInput(input.AsSpan(), out ICollection<LongInterval>? freshIntervals, out _);

        return LongInterval.Merge(freshIntervals)
            .Sum(interval => (double)interval.Length);
    }

    /// <summary>
    /// Parses the input string into a collection of intervals and a set of ingredients.
    /// </summary>
    /// <param name="span">The span containing the input string.</param>
    /// <param name="freshIntervals">The collection of fresh intervals.</param>
    /// <param name="ingredients">The set of ingredients.</param>
    private static void ParseInput(ReadOnlySpan<char> span, out ICollection<LongInterval> freshIntervals, out ISet<long> ingredients)
    {
        freshIntervals = [];
        ingredients = new HashSet<long>();

        foreach (ReadOnlySpan<char> line in span.EnumerateLines())
        {
            if (LongInterval.TryParse(line, out LongInterval longInterval))
            {
                freshIntervals.Add(longInterval);
            }
            else if (long.TryParse(line, NumberStyles.None, NumberFormatInfo.InvariantInfo, out long value))
            {
                ingredients.Add(value);
            }
        }
    }
}
