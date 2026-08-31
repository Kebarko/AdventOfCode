using KE.AoC.Core.Solution;
using System.Text;

namespace KE.AoC.Solutions.Y2015;

[Solution(2015, 10)]
public sealed class Day10 : SolutionBase
{
    /// <summary>
    /// Calculates the length of the look-and-say sequence after 40 iterations for the given input.
    /// </summary>
    /// <param name="input">The input string for the sequence.</param>
    /// <returns>The length of the resulting string after 40 iterations.</returns>
    public override object PartOne(string input)
    {
        return LookAndSay(input, 40).Length;
    }

    /// <summary>
    /// Calculates the length of the look-and-say sequence after 50 iterations for the given input.
    /// </summary>
    /// <param name="input">The input string for the sequence.</param>
    /// <returns>The length of the resulting string after 50 iterations.</returns>
    public override object PartTwo(string input)
    {
        return LookAndSay(input, 50).Length;
    }

    /// <summary>
    /// Generates the look-and-say sequence for a given seed and number of iterations.
    /// </summary>
    /// <param name="seed">The seed value for the sequence.</param>
    /// <param name="iterations">The number of iterations to perform.</param>
    /// <returns>The resulting string after the specified number of iterations.</returns>
    private static string LookAndSay(string seed, int iterations)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(iterations);

        if (string.IsNullOrEmpty(seed))
            return string.Empty;

        if (iterations == 0)
            return new string(seed);

        string result = seed;

        for (int i = 0; i < iterations; i++)
        {
            StringBuilder builder = new(2 * result.Length);
            int index = 0;

            while (index < result.Length)
            {
                char current = result[index];
                int count = 0;

                while (index < result.Length && result[index] == current)
                {
                    index++;
                    count++;
                }

                builder.Append(count).Append(current);
            }

            result = builder.ToString();
        }

        return result;
    }
}
