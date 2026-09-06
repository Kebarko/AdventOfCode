using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace KE.AoC.Solutions.Common;

/// <summary>
/// A generic grid class that represents a 2D grid of elements of type T.
/// </summary>
public sealed class Grid<T>(int width, int height)
{
    private readonly T[,] cells = new T[height, width];

    /// <summary>
    /// Gets the height of the grid (number of rows).
    /// </summary>
    public int Height => height;

    /// <summary>
    /// Gets the width of the grid (number of columns).
    /// </summary>
    public int Width => width;

    /// <summary>
    /// Gets or sets the value of the cell at the specified coordinates (x, y) in the grid.
    /// </summary>
    public T this[int x, int y]
    {
        get => cells[y, x];
        set => cells[y, x] = value;
    }

    /// <summary>
    /// Gets or sets the value of the cell at the specified point (x, y) in the grid.
    /// </summary>
    public T this[(int X, int Y) p]
    {
        get => cells[p.Y, p.X];
        set => cells[p.Y, p.X] = value;
    }

    /// <summary>
    /// Creates a new grid with the specified width and height, initializing all cells to their default value.
    /// </summary>
    public Grid(int width, int height, T fill) : this(width, height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                cells[y, x] = fill;
            }
        }
    }

    /// <summary>
    /// Creates a new grid that is a copy of the given grid.
    /// </summary>
    public Grid(Grid<T> grid) : this(grid.Width, grid.Height)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                cells[y, x] = grid.cells[y, x];
            }
        }
    }

    /// <summary>
    /// Checks if the given coordinates (x, y) are within the bounds of the grid.
    /// </summary>
    public bool InBounds(int x, int y)
    {
        return y >= 0 && y < Height && x >= 0 && x < Width;
    }

    /// <summary>
    /// Checks if the given point (x, y) is within the bounds of the grid.
    /// </summary>
    public bool InBounds((int X, int Y) p)
    {
        return InBounds(p.X, p.Y);
    }

    /// <summary>
    /// Tries to get the value of the cell at (x, y). Returns true if the coordinates are in bounds, false otherwise.
    /// </summary>
    public bool TryGet(int x, int y, [MaybeNullWhen(false)] out T result)
    {
        if (InBounds(x, y))
        {
            result = cells[y, x];
            return true;
        }

        result = default;
        return false;
    }

    #region Enumeration
    /// <summary>
    /// Returns an enumerable of all cells in the grid, along with their coordinates (x, y) and value.
    /// </summary>
    public IEnumerable<(int X, int Y, T Value)> Cells()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                yield return (x, y, cells[y, x]);
            }
        }
    }

    /// <summary>
    /// Returns the values of the cells in the specified row (y) of the grid.
    /// </summary>
    public IEnumerable<T> Row(int y)
    {
        for (int x = 0; x < Width; x++)
        {
            yield return cells[y, x];
        }
    }

    /// <summary>
    /// Returns the values of the cells in the specified column (x) of the grid.
    /// </summary>
    public IEnumerable<T> Column(int x)
    {
        for (int y = 0; y < Height; y++)
        {
            yield return cells[y, x];
        }
    }

    /// <summary>
    /// Counts the number of cells in the grid that match the specified predicate.
    /// </summary>
    public int Count(Predicate<T> match)
    {
        int n = 0;
        foreach (T cell in cells)
        {
            if (match(cell))
            {
                n++;
            }
        }

        return n;
    }

    /// <summary>
    /// Finds the coordinates of the first cell in the grid that matches the specified predicate. Returns null if no matching cell is found.
    /// </summary>
    public (int X, int Y)? Find(Predicate<T> match)
    {
        foreach (var (x, y, v) in Cells())
        {
            if (match(v))
            {
                return (x, y);
            }
        }

        return null;
    }
    #endregion

    #region Neighbours
    private static readonly (int dx, int dy)[] OrthogonalOffsets = [(0, 1), (1, 0), (0, -1), (-1, 0)];

    private static readonly (int dx, int dy)[] AllOffsets = [(0, 1), (1, 1), (1, 0), (1, -1), (0, -1), (-1, -1), (-1, 0), (-1, 1)];

    /// <summary>
    /// Returns the coordinates of the 4 orthogonal neighbouring cells (up, down, left, right) of the cell at (x, y) that are within the bounds of the grid.
    /// </summary>
    public IEnumerable<(int X, int Y, T Value)> Neighbours4(int x, int y)
    {
        return Step(x, y, OrthogonalOffsets);
    }

    /// <summary>
    /// Returns the coordinates of the 8 neighbouring cells (including diagonals) of the cell at (x, y) that are within the bounds of the grid.
    /// </summary>
    public IEnumerable<(int X, int Y, T Value)> Neighbours8(int x, int y)
    {
        return Step(x, y, AllOffsets);
    }

    private IEnumerable<(int X, int Y, T Value)> Step(int x, int y, (int dx, int dy)[] offsets)
    {
        foreach ((int dx, int dy) in offsets)
        {
            int nx = x + dx;
            int ny = y + dy;

            if (TryGet(nx, ny, out T? value))
            {
                yield return (nx, ny, value);
            }
        }
    }

    /// <summary>
    /// Returns the coordinates of the 4 orthogonal neighbouring cells (up, down, left, right) of the cell at (x, y) that are within the bounds of the grid.
    /// </summary>
    public int Neighbours4Count(int x, int y, Predicate<T> match)
    {
        return StepCount(x, y, match, OrthogonalOffsets);
    }

    /// <summary>
    /// Returns the coordinates of the 8 neighbouring cells (including diagonals) of the cell at (x, y) that are within the bounds of the grid.
    /// </summary>
    public int Neighbours8Count(int x, int y, Predicate<T> match)
    {
        return StepCount(x, y, match, AllOffsets);
    }

    private int StepCount(int x, int y, Predicate<T> match, (int dx, int dy)[] offsets)
    {
        int count = 0;

        foreach ((int dx, int dy) in offsets)
        {
            int nx = x + dx;
            int ny = y + dy;

            if (TryGet(nx, ny, out T? value) && match(value))
            {
                count++;
            }
        }

        return count;
    }
    #endregion

    #region Parsing
    /// <summary>
    /// Parses a grid from a string representation, where each line represents a row in the grid. The selector function is used to convert each character in the lines to the desired type T.
    /// </summary>
    public static Grid<T> Parse(string text, Func<char, T> selector)
    {
        return Grid<T>.Parse(SplitLines(text), selector);
    }

    /// <summary>
    /// Parses a grid from a list of strings, where each string represents a row in the grid. The selector function is used to convert each character in the strings to the desired type T.
    /// </summary>
    public static Grid<T> Parse(IReadOnlyList<string> lines, Func<char, T> selector)
    {
        if (lines.Count == 0)
            throw new ArgumentException("Cannot parse an empty grid.", nameof(lines));

        int height = lines.Count;
        int width = lines[0].Length;
        var grid = new Grid<T>(width, height);

        for (int y = 0; y < height; y++)
        {
            string line = lines[y];

            if (line.Length != width)
                throw new FormatException($"Ragged input: line {y} has length {line.Length}, expected {width}.");

            for (int x = 0; x < width; x++)
            {
                grid[x, y] = selector(line[x]);
            }
        }

        return grid;
    }

    private static string[] SplitLines(string text)
    {
        return text.ReplaceLineEndings("\n").Trim('\n').Split('\n');
    }
    #endregion

    #region Rendering (debugging)
    /// <summary>
    /// Renders the grid to a string using a selector function to convert each cell to a character.
    /// </summary>
    public string Render(Func<T, char> selector)
    {
        var sb = new StringBuilder(Height * (Width + 1));
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
                sb.Append(selector(cells[y, x]));

            if (y < Height - 1)
                sb.Append('\n');
        }
        return sb.ToString();
    }

    /// <summary>
    /// Renders the grid to a string using a selector function to convert each cell to a string, with an optional separator between cells.
    /// </summary>
    public string Render(Func<T, string> selector, string separator = " ")
    {
        var rendered = new string[Height, Width];
        int width = 0;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                string value = selector(cells[y, x]);
                rendered[y, x] = value;
                if (value.Length > width)
                    width = value.Length;
            }
        }

        var sb = new StringBuilder(Height * (Width * (width + separator.Length) + 1));
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (x > 0)
                    sb.Append(separator);

                sb.Append(rendered[y, x].PadLeft(width));
            }

            if (y < Height - 1)
                sb.Append('\n');
        }

        return sb.ToString();
    }
    #endregion
}

/// <summary>
/// A static helper class for creating grids of specific types.
/// </summary>
public static class Grid
{
    /// <summary>
    /// Creates a grid of characters from a string representation.
    /// </summary>
    public static Grid<char> OfChars(string text) => Grid<char>.Parse(text, c => c);

    /// <summary>
    /// Creates a grid of characters from a list of strings, where each string represents a row in the grid.
    /// </summary>
    public static Grid<char> OfChars(IReadOnlyList<string> lines) => Grid<char>.Parse(lines, c => c);

    /// <summary>
    /// Creates a grid of booleans from a string representation, where a specified character represents true and all other characters represent false.
    /// </summary>
    public static Grid<bool> OfBools(string text, char trueChar) => Grid<bool>.Parse(text, c => c == trueChar);

    /// <summary>
    /// Creates a grid of booleans from a list of strings, where each string represents a row in the grid and a specified character represents true and all other characters represent false.
    /// </summary>
    public static Grid<bool> OfBools(IReadOnlyList<string> lines, char trueChar) => Grid<bool>.Parse(lines, c => c == trueChar);

    /// <summary>
    /// Creates a grid of a specified type from a string representation, using a selector function to convert characters to the desired type.
    /// </summary>
    public static Grid<T> Of<T>(string text, Func<char, T> selector) => Grid<T>.Parse(text, selector);

    /// <summary>
    /// Creates a grid of a specified type from a list of strings, where each string represents a row in the grid, using a selector function to convert characters to the desired type.
    /// </summary>
    public static Grid<T> Of<T>(IReadOnlyList<string> lines, Func<char, T> selector) => Grid<T>.Parse(lines, selector);
}
