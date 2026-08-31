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
}
