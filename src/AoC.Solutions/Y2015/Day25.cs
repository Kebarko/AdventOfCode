using KE.AoC.Core.Solution;
using System.Numerics;
using System.Text.RegularExpressions;

namespace KE.AoC.Solutions.Y2015;

/// <summary>
/// --- Day 25: Let It Snow ---
/// </summary>
[Solution(2015, 25)]
public sealed partial class Day25 : SolutionBase
{
    /// <summary>
    /// Calculates the code at the specified row and column in the code grid based on a specific formula.
    /// </summary>
    /// <param name="input">The input string containing the row and column information.</param>
    /// <returns>The calculated code at the specified position.</returns>
    /// <exception cref="FormatException">
    /// Thrown when the input does not match the expected format.
    /// </exception>
    public override object PartOne(string input)
    {
        Regex regex = InputRegex();
        Match match = regex.Match(input);

        if (match.Groups.Count != 3)
            throw new FormatException("Input does not match expected format.");

        int row = int.Parse(match.Groups[1].Value);
        int col = int.Parse(match.Groups[2].Value);

        const int init = 20151125;
        const int mul = 252533;
        const int mod = 33554393;

        int diag = row + col - 1;
        int index = diag * (diag - 1) / 2 + col;

        return init * BigInteger.ModPow(mul, index - 1, mod) % mod;
    }

    [GeneratedRegex(@"row\s+(\d+).+column\s+(\d+)")]
    private static partial Regex InputRegex();
}
