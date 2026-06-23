using System.Text.RegularExpressions;

namespace KE.AoC.Core.Solution;

/// <summary>
/// Represents the base class for Advent of Code solutions, providing common utility methods for processing input data.
/// </summary>
/// <remarks>
/// Inherit this for every puzzle. Override <see cref="PartOne"/> always; override <see cref="PartTwo"/> only once you start solving part two. If you do NOT override <see cref="PartTwo"/>, the app shows a single column for that day instead of two.
/// </remarks>
public abstract class SolutionBase : ISolution
{
    /// <summary>
    /// Executes the solution for part one of the Advent of Code puzzle.
    /// </summary>
    /// <param name="input">The input for the puzzle.</param>
    /// <returns>The result of the solution.</returns>
    public abstract object PartOne(string input);

    /// <summary>
    /// Executes the solution for part two of the Advent of Code puzzle.
    /// </summary>
    /// <param name="input">The input for the puzzle.</param>
    /// <returns>The result of the solution.</returns>
    public virtual object PartTwo(string input) =>
        throw new NotImplementedException("Part two is not implemented yet.");

    // ---- Common parsing helpers used by most puzzles ----

    /// <summary>
    /// Splits the input string into an array of lines.Empty entries are removed and whitespace is trimmed.
    /// </summary>
    protected static string[] Lines(string input) =>
        input.ReplaceLineEndings("\n").Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    /// <summary>
    /// Splits the input string into an array of paragraphs, where paragraphs are separated by two consecutive newlines. Empty entries are removed and whitespace is trimmed.
    /// </summary>
    protected static string[] Paragraphs(string input) =>
        input.ReplaceLineEndings("\n").Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    /// <summary>
    /// Extracts all integers from the input string and returns them as an array of integers.
    /// </summary>
    protected static int[] Ints(string input) =>
        Regex.Matches(input, @"-?\d+").Select(m => int.Parse(m.Value)).ToArray();

    /// <summary>
    /// Extracts all long integers from the input string and returns them as an array of long integers.
    /// </summary>
    protected static long[] Longs(string input) =>
        Regex.Matches(input, @"-?\d+").Select(m => long.Parse(m.Value)).ToArray();
}
