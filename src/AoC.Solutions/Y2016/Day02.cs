using KE.AoC.Core.Solution;
using KE.AoC.Solutions.Common;

namespace KE.AoC.Solutions.Y2016;

/// <summary>
/// --- Day 2: Bathroom Security ---
/// </summary>
[Solution(2016, 2)]
public sealed class Day02 : SolutionBase
{
    /// <summary>
    /// The steps corresponding to the four directions (up, right, down, left).
    /// </summary>
    private static readonly Point2D<int>[] Steps = [new(0, 1), new(1, 0), new(0, -1), new(-1, 0)];

    /// <summary>
    /// Calculates the code for the bathroom keypad based on the input instructions.
    /// </summary>
    /// <param name="input">The input instructions.</param>
    /// <returns>The calculated code.</returns>
    public override object PartOne(string input)
    {
        const string keypad = "123" +
                              "456" +
                              "789";

        Point2D<int> button = new(1, 1);

        string result = string.Empty;

        var instructions = ParseInstructions(input.AsSpan());
        foreach (List<int> group in instructions)
        {
            foreach (int direction in group)
            {
                Point2D<int> step = Steps[direction];
                int x = button.X + step.X;
                int y = button.Y + step.Y;

                if ((x is >= 0 and <= 2) && (y is >= 0 and <= 2))
                {
                    button = new Point2D<int>(x, y);
                }
            }

            result += keypad[3 * (2 - button.Y) + button.X];
        }

        return result;
    }

    /// <summary>
    /// Calculates the code for the more complex bathroom keypad based on the input instructions.
    /// </summary>
    /// <param name="input">The input instructions.</param>
    /// <returns>The calculated code.</returns>
    public override object PartTwo(string input)
    {
        const string keypad = "  1  " +
                              " 234 " +
                              "56789" +
                              " ABC " +
                              "  D  ";

        Point2D<int> button = new(0, 2);

        string result = string.Empty;

        var instructions = ParseInstructions(input.AsSpan());
        foreach (List<int> group in instructions)
        {
            foreach (int direction in group)
            {
                Point2D<int> step = Steps[direction];
                int x = button.X + step.X;
                int y = button.Y + step.Y;

                if (int.Abs(x - 2) + int.Abs(y - 2) <= 2)
                {
                    button = new Point2D<int>(x, y);
                }
            }

            result += keypad[5 * (4 - button.Y) + button.X];
        }

        return result;
    }

    /// <summary>
    /// Parses the input instructions into a list of lists of integers,
    /// where each integer represents a direction (0 for up, 1 for right, 2 for down, 3 for left).
    /// </summary>
    /// <param name="span">The input span containing the instructions.</param>
    /// <returns>A list of lists of integers representing the parsed instructions.</returns>
    /// <exception cref="FormatException">Thrown when an invalid direction is encountered.</exception>
    private static List<List<int>> ParseInstructions(ReadOnlySpan<char> span)
    {
        List<List<int>> result = [];
        foreach (ReadOnlySpan<char> line in span.EnumerateLines())
        {
            List<int> subResult = [];
            foreach (char c in line)
            {
                int direction = c switch
                {
                    'U' => 0,
                    'R' => 1,
                    'D' => 2,
                    'L' => 3,
                    _ => throw new FormatException("Invalid direction."),
                };

                subResult.Add(direction);
            }

            result.Add(subResult);
        }

        return result;
    }
}
