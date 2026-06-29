using KE.AoC.Core.Solution;

namespace KE.AoC.Solutions.Y2025;

[Solution(2025, 4)]
public sealed class Day04 : SolutionBase
{
    /// <summary>
    /// Counts the number of rolls that can be removed from the grid based on the removal criteria.
    /// </summary>
    public override object PartOne(string input)
    {
        string[] lines = Lines(input);

        bool[][] grid = InitializeGrid(lines);

        List<(int i, int j)> rollsToRemove = GetRollsToRemove(grid);

        return rollsToRemove.Count;
    }

    /// <summary>
    /// Counts the total number of rolls that can be removed from the grid by repeatedly removing removable rolls until no more can be removed.
    /// </summary>
    public override object PartTwo(string input)
    {
        string[] lines = Lines(input);

        bool[][] grid = InitializeGrid(lines);

        int totalRemoved = 0;

        List<(int i, int j)> rollsToRemove;
        while ((rollsToRemove = GetRollsToRemove(grid)).Count > 0)
        {
            totalRemoved += rollsToRemove.Count;

            RemoveRolls(grid, rollsToRemove);
        }

        return totalRemoved;
    }

    /// <summary>
    /// Initializes the grid based on the input lines, where '@' represents an accessible cell and any other character represents an inaccessible cell.
    /// </summary>
    private static bool[][] InitializeGrid(string[] lines)
    {
        bool[][] grid = new bool[lines.Length][];

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            grid[i] = new bool[line.Length];

            for (int j = 0; j < line.Length; j++)
            {
                grid[i][j] = line[j] == '@';
            }
        }

        return grid;
    }

    /// <summary>
    /// Gets the collection of rolls that can be removed from the grid based on the removal criteria.
    /// </summary>
    private static List<(int i, int j)> GetRollsToRemove(bool[][] grid)
    {
        List<(int i, int j)> rollsToRemove = [];

        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[i].Length; j++)
            {
                if (grid[i][j] && IsRemovable(grid, i, j))
                {
                    rollsToRemove.Add((i, j));
                }
            }
        }

        return rollsToRemove;
    }

    /// <summary>
    /// Checks if the roll at (i, j) is removable based on the number of adjacent rolls. A roll is considered removable if it has fewer than 4 adjacent rolls.
    /// </summary>
    private static bool IsRemovable(bool[][] grid, int i, int j)
    {
        int adjacent = 0;

        for (int a = i - 1; a <= i + 1; a++)
        {
            for (int b = j - 1; b <= j + 1; b++)
            {
                if (!(a == i && b == j) && IsValid(grid, a, b) && grid[a][b])
                {
                    adjacent++;
                }
            }
        }

        return adjacent < 4;
    }

    /// <summary>
    /// Removes the specified rolls from the grid by setting the corresponding cells to false.
    /// </summary>
    private static void RemoveRolls(bool[][] grid, IEnumerable<(int i, int j)> rolls)
    {
        foreach ((int i, int j) in rolls)
        {
            grid[i][j] = false;
        }
    }

    /// <summary>
    /// Checks if the given indices are valid within the grid boundaries.
    /// </summary>
    private static bool IsValid(bool[][] grid, int i, int j)
    {
        return i >= 0 && j >= 0 && i < grid.Length && j < grid[i].Length;
    }
}
