using KE.AoC.Core.Solution;
using KE.AoC.Solutions.Common;

namespace KE.AoC.Solutions.Y2015;

[Solution(2015, 18)]
public sealed class Day18 : SolutionBase
{
    private const int Steps = 100;

    /// <summary>
    /// Simulates the grid of lights for a given number of steps and returns the number of lights that are on after the simulation.
    /// </summary>
    /// <param name="input">The input string representing the initial state of the grid.</param>
    /// <returns>The number of lights that are on after the simulation.</returns>
    public override object PartOne(string input)
    {
        return Simulate(input);
    }

    /// <summary>
    /// Simulates the grid of lights for a given number of steps, keeping the corners fixed (always on), and returns the number of lights that are on after the simulation.
    /// </summary>
    /// <param name="input">The input string representing the initial state of the grid.</param>
    /// <returns>The number of lights that are on after the simulation.</returns>
    public override object PartTwo(string input)
    {
        return Simulate(input, true);
    }

    /// <summary>
    /// Simulates the grid of lights for a given number of steps, optionally keeping the corners fixed (always on).
    /// </summary>
    /// <param name="input">The input string representing the initial state of the grid.</param>
    /// <param name="fixedCorners">Indicates whether the corners should be kept fixed (always on).</param>
    /// <returns>The number of lights that are on after the simulation.</returns>
    private static int Simulate(string input, bool fixedCorners = false)
    {
        Grid<bool> current = Grid.OfBools(input, '#');

        if (fixedCorners)
            LightCorners(current);

        Grid<bool> next = new(current);

        for (int i = 0; i < Steps; i++)
        {
            foreach ((int X, int Y, bool IsOn) in current.Cells())
            {
                int onNeighbours = current.Neighbours8Count(X, Y, x => x);
                next[X, Y] = IsOn ? onNeighbours is 2 or 3 : onNeighbours is 3;
            }

            if (fixedCorners)
                LightCorners(next);

            (current, next) = (next, current);
        }

        return current.Count(c => c);
    }

    /// <summary>
    /// Lights the corners of the grid by setting them to true (on).
    /// </summary>
    /// <param name="grid">The grid whose corners will be lit.</param>
    private static void LightCorners(Grid<bool> grid)
    {
        grid[0, 0] = true;
        grid[0, grid.Height - 1] = true;
        grid[grid.Width - 1, 0] = true;
        grid[grid.Width - 1, grid.Height - 1] = true;
    }
}
