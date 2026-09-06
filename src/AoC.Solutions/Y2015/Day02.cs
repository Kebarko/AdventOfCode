using KE.AoC.Core.Solution;
using System.Globalization;

namespace KE.AoC.Solutions.Y2015;

[Solution(2015, 2)]
public sealed class Day02 : SolutionBase
{
    /// <summary>
    /// Calculates the total amount of wrapping paper needed for a list of boxes, given their dimensions in the format "AxBxC".
    /// </summary>
    /// <param name="input">The input string containing box dimensions.</param>
    /// <returns>The total amount of wrapping paper needed.</returns>
    public override object PartOne(string input)
    {
        int result = 0;
        foreach (ReadOnlySpan<char> line in input.EnumerateLines())
        {
            (int a, int b, int c) = ParseBox(line);

            int s1 = a * b;
            int s2 = a * c;
            int s3 = b * c;

            int area = 2 * (s1 + s2 + s3);
            int add = Math.Min(s1, Math.Min(s2, s3));

            result += area + add;
        }

        return result;
    }

    /// <summary>
    /// Calculates the total amount of ribbon needed for a list of boxes, given their dimensions in the format "AxBxC".
    /// </summary>
    /// <param name="input">The input string containing box dimensions.</param>
    /// <returns>The total amount of ribbon needed.</returns>
    public override object PartTwo(string input)
    {
        int result = 0;
        foreach (ReadOnlySpan<char> line in input.EnumerateLines())
        {
            (int a, int b, int c) = ParseBox(line);

            int perimeter = 2 * (a + b + c - Math.Max(a, Math.Max(b, c)));
            int volume = a * b * c;

            result += perimeter + volume;
        }

        return result;
    }

    /// <summary>
    /// Parses a box dimension string in the format "AxBxC" into a tuple of integers (A, B, C).
    /// </summary>
    /// <param name="span">The span containing the box dimensions.</param>
    /// <returns>A tuple of integers representing the box dimensions (A, B, C).</returns>
    /// <exception cref="ArgumentException">Thrown when the input string is not in the correct format.</exception>
    private static (int A, int B, int C) ParseBox(ReadOnlySpan<char> span)
    {
        List<int> edges = [];

        foreach (Range range in span.Split('x'))
            edges.Add(int.Parse(span[range], NumberStyles.None, NumberFormatInfo.InvariantInfo));

        if (edges.Count != 3)
            throw new ArgumentException("Invalid box dimensions");

        return (edges[0], edges[1], edges[2]);
    }
}
