using KE.AoC.Core.Solution;
using KE.AoC.Solutions.Common;
using System.Globalization;

namespace KE.AoC.Solutions.Y2025;

[Solution(2025, 2)]
public sealed class Day02 : SolutionBase
{
    /// <summary>
    /// Calculates the sum of all numbers within the specified intervals that have equal halves.
    /// </summary>
    public override object PartOne(string input)
    {
        return Part(input.AsSpan(), FindNumbersWithEqualHalves);
    }

    /// <summary>
    /// Calculates the sum of all numbers within the specified intervals that can be divided into equal parts of digits.
    /// </summary>
    public override object PartTwo(string input)
    {
        return Part(input.AsSpan(), FindNumbersWithEqualParts);
    }

    /// <summary>
    /// Calculates the sum of all numbers that meet the criteria defined by the provided function within the specified intervals.
    /// </summary>
    private static ulong Part(ReadOnlySpan<char> span, Func<ulong, ulong, IEnumerable<ulong>> findInvalidIds)
    {
        ulong result = 0;
        foreach (Range range in span.Split(','))
        {
            ReadOnlySpan<char> interval = span[range];
            int dash = interval.IndexOf('-');

            ulong lower = ulong.Parse(interval[..dash], NumberStyles.None, NumberFormatInfo.InvariantInfo);
            ulong upper = ulong.Parse(interval[(dash + 1)..], NumberStyles.None, NumberFormatInfo.InvariantInfo);

            result += (ulong)findInvalidIds(lower, upper).Sum(x => (decimal)x);
        }

        return result;
    }

    /// <summary>
    /// Finds all numbers in the given range that have equal halves.
    /// </summary>
    private static IEnumerable<ulong> FindNumbersWithEqualHalves(ulong lower, ulong upper)
    {
        if (lower > upper)
            yield break;

        int lowerDigits = MathUtils.Digits(lower);
        int upperDigits = MathUtils.Digits(upper);

        for (int digits = lowerDigits; digits <= upperDigits; digits++)
        {
            if (digits % 2 != 0)
                continue;

            int halfDigits = digits / 2;

            ulong min = (ulong)Math.Pow(10, halfDigits - 1);
            ulong max = (ulong)Math.Pow(10, halfDigits) - 1;
            ulong mul = (ulong)Math.Pow(10, halfDigits) + 1;

            for (ulong val = min; val <= max; val++)
            {
                ulong number = val * mul;
                if (number >= lower && number <= upper)
                    yield return number;
            }
        }
    }

    /// <summary>
    /// Finds all numbers within the specified range that can be divided into equal parts of digits.
    /// </summary>
    private static IEnumerable<ulong> FindNumbersWithEqualParts(ulong lower, ulong upper)
    {
        HashSet<ulong> result = [];

        if (lower > upper)
            return result;

        int lowerDigits = MathUtils.Digits(lower);
        int upperDigits = MathUtils.Digits(upper);

        for (int digits = lowerDigits; digits <= upperDigits; digits++)
        {
            foreach (int partDigits in MathUtils.Divisors(digits))
            {
                if (partDigits == digits)
                    continue;

                ulong min = (ulong)Math.Pow(10, partDigits - 1);
                ulong max = (ulong)Math.Pow(10, partDigits) - 1;
                ulong mul = (ulong)Math.Pow(10, partDigits);

                for (ulong val = min; val <= max; val++)
                {
                    ulong number = 0;
                    for (int rep = 1; rep <= digits / partDigits; rep++)
                    {
                        number = number * mul + val;
                    }

                    if (number > upper)
                        break;

                    if (number >= lower && number <= upper)
                        result.Add(number);
                }
            }
        }

        return result;
    }
}
