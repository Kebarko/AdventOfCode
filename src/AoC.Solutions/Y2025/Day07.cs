using KE.AoC.Core.Solution;
using KE.AoC.Solutions.Common;

namespace KE.AoC.Solutions.Y2025;

[Solution(2025, 7)]
public sealed class Day07 : SolutionBase
{
    public override object PartOne(string input)
    {
        Grid<char> grid = Grid.OfChars(input);

        int splits = 0;

        for (int y = 1; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                if (grid[x, y] == '.')
                {
                    if (grid.TryGet(x, y - 1, out char above) && (above == 'S' || above == '|'))
                    {
                        grid[x, y] = '|';
                    }
                }
                else if (grid[x, y] == '^')
                {
                    if (grid.TryGet(x, y - 1, out char above) && above == '|')
                    {
                        if (grid.TryGet(x - 1, y, out char left) && left == '.')
                        {
                            grid[x - 1, y] = '|';
                        }

                        if (grid.TryGet(x + 1, y, out char right) && right == '.')
                        {
                            grid[x + 1, y] = '|';
                        }

                        splits++;
                    }
                }
            }
        }

        return splits;
    }

    public override object PartTwo(string input)
    {
        Grid<char> grid = Grid.OfChars(input);
        Grid<long> timelines = new(grid.Width, grid.Height);

        timelines[grid.Find(c => c == 'S')!.Value] = 1;

        for (int y = 1; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                if (grid[x, y] == '^')
                {
                    continue;
                }

                long count = 0;

                if (grid.TryGet(x, y - 1, out char above) && (above == 'S' || above == '|'))
                {
                    count += timelines[x, y - 1];
                }

                if (grid.TryGet(x - 1, y, out char left) && left == '^' &&
                    grid.TryGet(x - 1, y - 1, out char leftAbove) && leftAbove == '|')
                {
                    count += timelines[x - 1, y - 1];
                }

                if (grid.TryGet(x + 1, y, out char right) && right == '^' &&
                    grid.TryGet(x + 1, y - 1, out char rightAbove) && rightAbove == '|')
                {
                    count += timelines[x + 1, y - 1];
                }

                if (count > 0)
                {
                    grid[x, y] = '|';
                    timelines[x, y] = count;
                }
            }
        }

        return timelines.Row(grid.Height - 1).Sum();
    }
}
