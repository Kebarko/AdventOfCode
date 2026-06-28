using KE.AoC.Core.Solution;
using KE.AoC.Solutions.Common;

namespace KE.AoC.Solutions.Y2025;

[Solution(2025, 2)]
public sealed class Day02 : SolutionBase
{
    /// <summary>
    /// Calculates the sum of all numbers within the specified intervals that have equal halves.
    /// </summary>
    public override object PartOne(string input)
    {
        return Part(input, FindNumbersWithEqualHalves);
    }

    /// <summary>
    /// Calculates the sum of all numbers within the specified intervals that can be divided into equal blocks of digits.
    /// </summary>
    public override object PartTwo(string input)
    {
        return Part(input, FindNumbersWithEqualBlocks);
    }

    /// <summary>
    /// Calculates the sum of all numbers that meet the criteria defined by the provided function within the specified intervals.
    /// </summary>
    private object Part(string input, Func<ulong, ulong, IEnumerable<ulong>> findInvalidIds)
    {
        string[] intervals = input.Trim().Split(',');

        ulong result = 0;
        foreach (string interval in intervals)
        {
            string[] bounds = interval.Split('-');

            ulong lower = ulong.Parse(bounds[0]);
            ulong upper = ulong.Parse(bounds[1]);

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

            ulong halfMin = (ulong)Math.Pow(10, halfDigits - 1);
            ulong halfMax = (ulong)Math.Pow(10, halfDigits) - 1;
            ulong multiplier = (ulong)Math.Pow(10, halfDigits) + 1;

            for (ulong half = halfMin; half <= halfMax; half++)
            {
                ulong number = half * multiplier;
                if (number >= lower && number <= upper)
                {
                    yield return number;
                }
            }
        }
    }

    /// <summary>
    /// Finds all numbers within the specified range that can be divided into equal blocks of digits.
    /// </summary>
    private static IEnumerable<ulong> FindNumbersWithEqualBlocks(ulong lower, ulong upper)
    {
        if (lower > upper)
            yield break;

        for (ulong number = lower; number <= upper; number++)
        {
            int totalDigits = MathUtils.Digits(number);
            int lowerHalfDigits = totalDigits / 2;
            bool found = false;

            for (int digits = 1; digits <= lowerHalfDigits; digits++)
            {
                if (totalDigits % digits != 0)
                    continue;

                ulong divisor = (ulong)Math.Pow(10, digits);
                ulong firstPart = number % divisor;
                bool allPartsEqual = true;
                ulong tmp = number;
                while (tmp > 0)
                {
                    ulong currentPart = tmp % divisor;
                    if (currentPart != firstPart)
                    {
                        allPartsEqual = false;
                        break;
                    }
                    tmp /= divisor;
                }

                if (allPartsEqual)
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                yield return number;
            }
        }
    }
}
