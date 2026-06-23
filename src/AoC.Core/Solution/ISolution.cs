namespace KE.AoC.Core.Solution;

/// <summary>
/// Represents a solution for a specific day of Advent of Code.
/// </summary>
/// <remarks>
/// You should inherit <see cref="SolutionBase"/> rather than implementing this directly.
/// </remarks>
public interface ISolution
{
    /// <summary>
    /// Executes the solution for part one of the Advent of Code puzzle.
    /// </summary>
    /// <param name="input">The input for the puzzle.</param>
    /// <returns>The result of the solution.</returns>
    object PartOne(string input);

    /// <summary>
    /// Executes the solution for part two of the Advent of Code puzzle.
    /// </summary>
    /// <param name="input">The input for the puzzle.</param>
    /// <returns>The result of the solution.</returns>
    object PartTwo(string input);
}

/// <summary>
/// Attribute to specify the year and day of the Advent of Code solution.
/// </summary>
/// <param name="year">The year of the Advent of Code solution.</param>
/// <param name="day">The day of the Advent of Code solution.</param>
/// <remarks>
/// The app uses this to place the solution under the right year tab and day row automatically.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class SolutionAttribute(int year, int day) : Attribute
{
    /// <summary>
    /// Gets the year of the Advent of Code solution.
    /// </summary>
    public int Year { get; } = year;

    /// <summary>
    /// Gets the day of the Advent of Code solution.
    /// </summary>
    public int Day { get; } = day;
}
