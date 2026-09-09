using KE.AoC.Core.Solution;
using System.Diagnostics;
using System.Globalization;

namespace KE.AoC.Solutions.Y2015;

[Solution(2015, 20)]
public sealed class Day20 : SolutionBase
{
    /// <summary>
    /// Calculates the lowest house number that receives at least a specified number of presents for part one of the puzzle.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The lowest house number that receives at least the specified number of presents.</returns>
    public override object PartOne(string input)
    {
        return GetLowestHouse(ParseInput(input), 10, int.MaxValue);
    }

    /// <summary>
    /// Calculates the lowest house number that receives at least a specified number of presents for part two of the puzzle.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The lowest house number that receives at least the specified number of presents.</returns>
    public override object PartTwo(string input)
    {
        return GetLowestHouse(ParseInput(input), 11, 50);
    }

    /// <summary>
    /// Calculates the lowest house number that receives at least a specified number of presents.
    /// </summary>
    /// <param name="presents">The minimum number of presents required.</param>
    /// <param name="presentsPerElf">The number of presents each elf delivers.</param>
    /// <param name="maxHouses">The maximum number of houses an elf can visit.</param>
    /// <returns>The lowest house number that receives at least the specified number of presents.</returns>
    /// <exception cref="UnreachableException">Thrown when no house receives the specified number of presents.</exception>
    private static int GetLowestHouse(int presents, int presentsPerElf, int maxHouses)
    {
        int limit = presents / presentsPerElf + 1;
        int[] houses = new int[limit + 1];

        for (int elf = 1; elf <= limit; elf++)
        {
            int last = (int)Math.Min((long)elf * maxHouses, limit);
            int delta = elf * presentsPerElf;

            for (int house = elf; house <= last; house += elf)
            {
                houses[house] += delta;
            }
        }

        for (int house = 1; house <= limit; house++)
        {
            if (houses[house] >= presents)
            {
                return house;
            }
        }

        throw new UnreachableException();
    }

    /// <summary>
    /// Parses the input string into an integer.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The parsed integer.</returns>
    private static int ParseInput(string input)
    {
        return int.Parse(input, NumberStyles.None, NumberFormatInfo.InvariantInfo);
    }
}
