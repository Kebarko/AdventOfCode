using KE.AoC.Core.Solution;
using System.Globalization;

namespace KE.AoC.Solutions.Y2016;

/// <summary>
/// --- Day 3: Squares With Three Sides ---
/// </summary>
[Solution(2016, 3)]
public sealed class Day03 : SolutionBase
{
    /// <summary>
    /// Calculates the number of valid triangles based on the triangle inequality theorem
    /// from the given input considering the triangles defined horizontally.
    /// </summary>
    /// <param name="input">The input string containing the triangle side lengths.</param>
    /// <returns>The number of valid triangles.</returns>
    public override object PartOne(string input)
    {
        List<(int A, int B, int C)> triangles = ParseTrianglesHorizontally(input.AsSpan());

        return triangles.Count(t => IsValidTriangle(t.A, t.B, t.C));
    }

    /// <summary>
    /// Calculates the number of valid triangles based on the triangle inequality theorem
    /// from the given input, considering the triangles defined vertically.
    /// </summary>
    /// <param name="input">The input string containing the triangle side lengths.</param>
    /// <returns>The number of valid triangles.</returns>
    public override object PartTwo(string input)
    {
        List<(int A, int B, int C)> triangles = ParseTrianglesVertically(input.AsSpan());

        return triangles.Count(t => IsValidTriangle(t.A, t.B, t.C));
    }

    /// <summary>
    /// Determines whether the given side lengths can form a valid triangle
    /// based on the triangle inequality theorem.
    /// </summary>
    /// <param name="a">The length of the first side.</param>
    /// <param name="b">The length of the second side.</param>
    /// <param name="c">The length of the third side.</param>
    /// <returns>true if the side lengths can form a valid triangle; otherwise, false.</returns>
    private static bool IsValidTriangle(int a, int b, int c)
        => a + b > c && a + c > b && b + c > a;

    /// <summary>
    /// Parses the input span into a list of triangles, where each triangle
    /// is represented by a tuple of three integers defined horizontally.
    /// </summary>
    /// <param name="span">The input span containing the triangle side lengths.</param>
    /// <returns>A list of triangles represented by tuples of three integers.</returns>
    private static List<(int, int, int)> ParseTrianglesHorizontally(ReadOnlySpan<char> span)
    {
        List<int> values = ParseValues(span);

        List<(int, int, int)> result = [];
        for (int i = 0; i < values.Count - 2; i += 3)
        {
            result.Add((values[i], values[i + 1], values[i + 2]));
        }

        return result;
    }

    /// <summary>
    /// Parses the input span into a list of triangles, where each triangle
    /// is represented by a tuple of three integers defined vertically.
    /// </summary>
    /// <param name="span">The input span containing the triangle side lengths.</param>
    /// <returns>A list of triangles represented by tuples of three integers.</returns>
    private static List<(int, int, int)> ParseTrianglesVertically(ReadOnlySpan<char> span)
    {
        List<int> values = ParseValues(span);

        List<(int, int, int)> result = [];
        for (int i = 0; i < values.Count - 8; i += 9)
        {
            result.Add((values[i], values[i + 3], values[i + 6]));
            result.Add((values[i + 1], values[i + 4], values[i + 7]));
            result.Add((values[i + 2], values[i + 5], values[i + 8]));
        }

        return result;
    }

    /// <summary>
    /// Parses the input span into a list of integer values.
    /// </summary>
    /// <param name="span">The input span containing the triangle side lengths.</param>
    /// <returns>A list of integer values representing the triangle side lengths.</returns>
    private static List<int> ParseValues(ReadOnlySpan<char> span)
    {
        List<int> result = [];
        foreach (ReadOnlySpan<char> line in span.EnumerateLines())
        {
            foreach (Range range in line.SplitAny(ReadOnlySpan<char>.Empty))
            {
                if (line[range].IsEmpty)
                    continue;

                result.Add(int.Parse(line[range], NumberStyles.None, NumberFormatInfo.InvariantInfo));
            }
        }

        return result;
    }
}
