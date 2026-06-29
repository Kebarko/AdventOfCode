using KE.AoC.Core.Solution;

namespace KE.AoC.Solutions.Y2025;

[Solution(2025, 3)]
public sealed class Day03 : SolutionBase
{
    /// <summary>
    /// Calculates the sum of the maximum bank values that can be formed by selecting 2 digits from each bank in the input.
    /// </summary>
    public override object PartOne(string input)
    {
        return Part(input, 2);
    }

    /// <summary>
    /// Calculates the sum of the maximum bank values that can be formed by selecting 12 digits from each bank in the input.
    /// </summary>
    public override object PartTwo(string input)
    {
        return Part(input, 12);
    }

    /// <summary>
    /// Calculates the sum of the maximum bank values that can be formed by selecting a specified number of digits from each bank in the input.
    /// </summary>
    private static ulong Part(string input, int digits)
    {
        string[] banks = Lines(input);

        ulong result = 0;
        foreach (string bank in banks)
        {
            result += CalculateMaxBankValue(ParseBank(bank), digits);
        }

        return result;
    }

    /// <summary>
    /// Parses a string representation of a bank into an array of integers.
    /// </summary>
    private static int[] ParseBank(string bank)
    {
        return bank.Select(c => c - '0').ToArray();
    }

    /// <summary>
    /// Calculates the maximum bank value that can be formed by selecting a specified number of digits from the given array of battery values.
    /// </summary>
    private static ulong CalculateMaxBankValue(int[] batteries, int digits)
    {
        if (batteries.Length < digits)
            return 0;

        int[] values = new int[digits];

        int startIdx = 0;
        for (int i = 0; i < digits; i++)
        {
            int? max = null;
            int? maxIdx = null;
            for (int j = startIdx; j < batteries.Length - (digits - i) + 1; j++)
            {
                int current = batteries[j];
                if (max == null || current > max)
                {
                    max = current;
                    maxIdx = j;
                }
            }
            values[i] = max!.Value;
            startIdx = maxIdx!.Value + 1;
        }

        ulong result = 0;
        for (int i = 0; i < values.Length; i++)
        {
            result += (ulong)values[i] * (ulong)Math.Pow(10, values.Length - i - 1);
        }

        return result;
    }
}
