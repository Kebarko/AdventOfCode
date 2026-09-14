using KE.AoC.Core.Solution;
using KE.AoC.Solutions.Common;
using System.Diagnostics;
using System.Globalization;

namespace KE.AoC.Solutions.Y2016;

/// <summary>
/// --- Day 1: No Time for a Taxicab ---
/// </summary>
[Solution(2016, 1)]
public sealed class Day01 : SolutionBase
{
    /// <summary>
    /// The steps corresponding to the four directions (North, East, South, West).
    /// </summary>
    private static readonly Point2D<int>[] Steps = [new(0, 1), new(1, 0), new(0, -1), new(-1, 0)];

    /// <summary>
    /// Calculates the Manhattan distance from the origin to the final position
    /// after following the instructions.
    /// </summary>
    /// <param name="input">The input instructions.</param>
    /// <returns>The Manhattan distance.</returns>
    public override object PartOne(string input)
    {
        Point2D<int> origin = new(0, 0);
        int direction = 0; // North
        Point2D<int> position = origin;

        var instructions = ParseInstructions(input.AsSpan());
        foreach ((int rotation, int distance) in instructions)
        {
            direction = Turn(direction, rotation);
            position = new Point2D<int>(
                position.X + Steps[direction].X * distance,
                position.Y + Steps[direction].Y * distance);
        }

        return origin.GetManhattanDistance(position);
    }

    /// <summary>
    /// Calculates the Manhattan distance to the first location visited twice
    /// after following the instructions.
    /// </summary>
    /// <param name="input">The input instructions.</param>
    /// <returns>The Manhattan distance.</returns>
    /// <exception cref="UnreachableException">
    /// Thrown when no location is visited twice.
    /// </exception>
    public override object PartTwo(string input)
    {
        Point2D<int> origin = new(0, 0);
        int direction = 0; // North
        Point2D<int> position = origin;
        HashSet<Point2D<int>> visited = [origin];

        var instructions = ParseInstructions(input.AsSpan());
        foreach ((int rotation, int distance) in instructions)
        {
            direction = Turn(direction, rotation);
            Point2D<int> step = Steps[direction];

            for (int i = 0; i < distance; i++)
            {
                position = new Point2D<int>(position.X + step.X, position.Y + step.Y);

                if (!visited.Add(position))
                    return origin.GetManhattanDistance(position);
            }
        }

        throw new UnreachableException("Failed to find a repeated position.");
    }

    /// <summary>
    /// Turns the direction based on the current direction and the rotation value.
    /// </summary>
    /// <param name="direction">The current direction.</param>
    /// <param name="rotation">The rotation value.</param>
    /// <returns>The new direction.</returns>
    private static int Turn(int direction, int rotation) => (direction + rotation + 4) % 4;

    /// <summary>
    /// Parses the input instructions into a list of tuples containing rotation and distance.
    /// </summary>
    /// <param name="span">The input span containing the instructions.</param>
    /// <returns>A list of tuples representing the rotation and distance for each instruction.</returns>
    /// <exception cref="FormatException">Thrown when the input format is invalid.</exception>
    private static List<(int, int)> ParseInstructions(ReadOnlySpan<char> span)
    {
        List<(int, int)> result = [];
        foreach (Range range in span.Split(", "))
        {
            var rotation = span[range][0] switch
            {
                'L' => -1,
                'R' => 1,
                _ => throw new FormatException("Invalid rotation."),
            };

            int distance = int.Parse(span[range][1..], NumberStyles.None, NumberFormatInfo.InvariantInfo);
            result.Add((rotation, distance));
        }

        return result;
    }
}
