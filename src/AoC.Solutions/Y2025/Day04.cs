using KE.AoC.Core.Solution;
using KE.AoC.Solutions.Common;

namespace KE.AoC.Solutions.Y2025;

/// <summary>
/// --- Day 4: Printing Department ---
/// </summary>
[Solution(2025, 4)]
public sealed class Day04 : SolutionBase
{
    /// <summary>
    /// Counts the number of rolls that can be removed from the grid based on the removal criteria.
    /// </summary>
    /// <param name="input">The input string representing the grid of rolls.</param>
    /// <returns>The number of rolls that can be removed.</returns>
    public override object PartOne(string input)
    {
        Grid<bool> grid = Grid.OfBools(input, '@');

        return grid.Cells()
            .Count(cell => cell.Value && grid.Neighbours8(cell.X, cell.Y).Count(cell => cell.Value) < 4);
    }

    /// <summary>
    /// Counts the total number of rolls that can be removed from the grid by repeatedly
    /// removing removable rolls until no more can be removed.
    /// </summary>
    /// <param name="input">The input string representing the grid of rolls.</param>
    /// <returns>The total number of rolls that can be removed.</returns>
    public override object PartTwo(string input)
    {
        Grid<bool> grid = Grid.OfBools(input, '@');
        Grid<int> counts = new(grid.Width, grid.Height);
        Queue<(int X, int Y)> queue = new();
        int removed = 0;

        // Count the number of neighboring rolls for each roll in the grid
        foreach ((int X, int Y, bool IsRoll) in grid.Cells())
        {
            if (IsRoll)
            {
                counts[X, Y] = grid.Neighbours8(X, Y).Count(cell => cell.Value);
            }
        }

        // Enqueue all rolls that can be removed (i.e., those with fewer than 4 neighboring rolls)
        // and mark them as removed
        foreach ((int X, int Y, bool IsRoll) in grid.Cells())
        {
            if (IsRoll && counts[X, Y] < 4)
            {
                queue.Enqueue((X, Y));
                grid[X, Y] = false;
                removed++;
            }
        }

        // Process the queue of removable rolls, updating the counts of neighboring rolls
        // and enqueuing any new removable rolls as they are found
        while (queue.Count > 0)
        {
            (int X, int Y) cell = queue.Dequeue();

            foreach ((int X, int Y, bool IsRoll) nb in grid.Neighbours8(cell.X, cell.Y))
            {
                if (nb.IsRoll)
                {
                    counts[nb.X, nb.Y]--;

                    if (counts[nb.X, nb.Y] < 4)
                    {
                        queue.Enqueue((nb.X, nb.Y));
                        grid[nb.X, nb.Y] = false;
                        removed++;
                    }
                }
            }
        }

        return removed;
    }
}
