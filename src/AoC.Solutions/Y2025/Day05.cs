using KE.AoC.Core.Solution;
using KE.AoC.Solutions.Common;

namespace KE.AoC.Solutions.Y2025;

[Solution(2025, 5)]
public sealed class Day05 : SolutionBase
{
    /// <summary>
    /// Counts the number of ingredients that fall within any of the fresh intervals.
    /// </summary>
    public override object PartOne(string input)
    {
        ParseInput(input, out ICollection<LongInterval> freshIntervals, out ISet<long> ingredients);

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
    public override object PartTwo(string input)
    {
        ParseInput(input, out ICollection<LongInterval>? freshIntervals, out _);

        return LongInterval.Merge(freshIntervals)
            .Sum(interval => (double)interval.Length);
    }

    /// <summary>
    /// Parses the input string into a collection of intervals and a set of ingredients.
    /// </summary>
    private static void ParseInput(string input, out ICollection<LongInterval> freshIntervals, out ISet<long> ingredients)
    {
        string[] lines = Lines(input);

        freshIntervals = [];
        ingredients = new HashSet<long>();

        foreach (string line in lines)
        {
            if (LongInterval.TryParse(line, out LongInterval longInterval))
            {
                freshIntervals.Add(longInterval);
            }
            else if (long.TryParse(line, out long value))
            {
                ingredients.Add(value);
            }
        }
    }
}
