namespace KE.AoC.Core.Input;

/// <summary>
/// Represents a provider for retrieving input data for Advent of Code puzzles.
/// </summary>
public interface IInputProvider
{
    /// <summary>
    /// Retrieves the input data for a specific year and day of the Advent of Code puzzle.
    /// </summary>
    /// <param name="year">The year of the puzzle.</param>
    /// <param name="day">The day of the puzzle.</param>
    /// <returns>The input data for the puzzle.</returns>
    string GetInput(int year, int day);
}
