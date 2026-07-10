using KE.AoC.Solutions.Common;

namespace KE.AoC.Solutions.Tests.Common;

public sealed class GridTests
{
    // ---- Construction ------------------------------------------------------

    [Theory]
    [InlineData(1, 1)]
    [InlineData(3, 2)]   // wider than tall — catches width/height swaps
    [InlineData(2, 3)]   // taller than wide
    [InlineData(10, 1)]
    [InlineData(1, 10)]
    public void Constructor_SetsDimensions(int width, int height)
    {
        var grid = new Grid<int>(width, height);

        Assert.Equal(width, grid.Width);
        Assert.Equal(height, grid.Height);
    }

    [Fact]
    public void Constructor_InitializesCellsToDefault()
    {
        var ints = new Grid<int>(3, 2);
        var strings = new Grid<string>(3, 2);

        Assert.All(ints.Cells(), c => Assert.Equal(0, c.Value));
        Assert.All(strings.Cells(), c => Assert.Null(c.Value));
    }

    [Theory]
    [InlineData(-42)]
    [InlineData(0)]
    [InlineData(7)]
    public void FillConstructor_InitializesAllCellsToFillValue(int fill)
    {
        var grid = new Grid<int>(3, 2, fill);

        Assert.All(grid.Cells(), c => Assert.Equal(fill, c.Value));
    }

    [Fact]
    public void CopyConstructor_CopiesDimensionsAndValues()
    {
        var source = MakeNumberedGrid(3, 2);

        var copy = new Grid<int>(source);

        Assert.Equal(source.Width, copy.Width);
        Assert.Equal(source.Height, copy.Height);
        Assert.Equal(source.Cells(), copy.Cells());
    }

    [Fact]
    public void CopyConstructor_CreatesIndependentCopy()
    {
        var source = new Grid<int>(2, 2, 1);
        var copy = new Grid<int>(source);

        source[0, 0] = 99;
        copy[1, 1] = -1;

        Assert.Equal(1, copy[0, 0]);
        Assert.Equal(1, source[1, 1]);
    }

    // ---- Indexers ----------------------------------------------------------

    [Theory]
    [InlineData(0, 0)]
    [InlineData(2, 0)]
    [InlineData(0, 1)]
    [InlineData(2, 1)]
    public void Indexer_RoundTripsValues(int x, int y)
    {
        var grid = new Grid<int>(3, 2);

        grid[x, y] = 42;

        Assert.Equal(42, grid[x, y]);
    }

    [Fact]
    public void Indexer_DistinguishesXAndY()
    {
        // In a non-square grid, a transposed implementation would
        // either throw or write to the wrong cell here.
        var grid = new Grid<int>(3, 2);

        grid[2, 1] = 7;

        Assert.Equal(7, grid[2, 1]);
        Assert.Equal(0, grid[1, 1]);
        Assert.Equal(0, grid[2, 0]);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(3, 0)]
    [InlineData(0, 2)]
    [InlineData(3, 2)]
    public void Indexer_OutOfBounds_Throws(int x, int y)
    {
        var grid = new Grid<int>(3, 2);

        Assert.Throws<IndexOutOfRangeException>(() => grid[x, y]);
        Assert.Throws<IndexOutOfRangeException>(() => grid[x, y] = 1);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(2, 1)]
    public void TupleIndexer_RoundTripsValues(int x, int y)
    {
        var grid = new Grid<int>(3, 2);

        grid[(x, y)] = 42;

        Assert.Equal(42, grid[(x, y)]);
        Assert.Equal(42, grid[x, y]); // both indexers address the same cell
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(3, 0)]
    [InlineData(0, 2)]
    public void TupleIndexer_OutOfBounds_Throws(int x, int y)
    {
        var grid = new Grid<int>(3, 2);

        Assert.Throws<IndexOutOfRangeException>(() => grid[(x, y)]);
        Assert.Throws<IndexOutOfRangeException>(() => grid[(x, y)] = 1);
    }

    // ---- Bounds checking ---------------------------------------------------

    [Theory]
    [InlineData(0, 0, true)]
    [InlineData(2, 1, true)]   // last valid cell in a 3x2 grid
    [InlineData(1, 0, true)]
    [InlineData(-1, 0, false)]
    [InlineData(0, -1, false)]
    [InlineData(3, 0, false)]  // x == Width
    [InlineData(0, 2, false)]  // y == Height
    [InlineData(3, 2, false)]
    [InlineData(int.MinValue, 0, false)]
    [InlineData(0, int.MaxValue, false)]
    public void InBounds_ChecksBothAxes(int x, int y, bool expected)
    {
        var grid = new Grid<int>(3, 2);

        Assert.Equal(expected, grid.InBounds(x, y));
        Assert.Equal(expected, grid.InBounds((x, y)));
    }

    [Theory]
    [InlineData(0, 0, true)]
    [InlineData(2, 1, true)]
    [InlineData(-1, 0, false)]
    [InlineData(3, 0, false)]
    [InlineData(0, 2, false)]
    public void TryGet_ReturnsValueOnlyWhenInBounds(int x, int y, bool expected)
    {
        var grid = MakeNumberedGrid(3, 2);

        bool success = grid.TryGet(x, y, out int value);

        Assert.Equal(expected, success);
        Assert.Equal(expected ? grid[x, y] : default, value);
    }

    [Fact]
    public void TryGet_OutOfBounds_ReturnsDefaultForReferenceType()
    {
        var grid = new Grid<string>(2, 2, "filled");

        bool success = grid.TryGet(-1, 0, out string? value);

        Assert.False(success);
        Assert.Null(value);
    }

    // ---- Enumeration -------------------------------------------------------

    [Fact]
    public void Cells_EnumeratesAllCellsInRowMajorOrder()
    {
        var grid = MakeNumberedGrid(3, 2);

        var expected = new (int X, int Y, int Value)[]
        {
            (0, 0, 0), (1, 0, 1), (2, 0, 2),
            (0, 1, 10), (1, 1, 11), (2, 1, 12),
        };

        Assert.Equal(expected, grid.Cells());
    }

    [Fact]
    public void Cells_OnSingleCellGrid_YieldsOneCell()
    {
        var grid = new Grid<char>(1, 1, 'x');

        Assert.Equal([(0, 0, 'x')], grid.Cells());
    }

    [Theory]
    [InlineData(0, new[] { 0, 1, 2 })]
    [InlineData(1, new[] { 10, 11, 12 })]
    public void Row_YieldsValuesLeftToRight(int y, int[] expected)
    {
        var grid = MakeNumberedGrid(3, 2);

        Assert.Equal(expected, grid.Row(y));
    }

    [Theory]
    [InlineData(0, new[] { 0, 10 })]
    [InlineData(1, new[] { 1, 11 })]
    [InlineData(2, new[] { 2, 12 })]
    public void Column_YieldsValuesTopToBottom(int x, int[] expected)
    {
        var grid = MakeNumberedGrid(3, 2);

        Assert.Equal(expected, grid.Column(x));
    }

    // ---- Count / Find ------------------------------------------------------

    [Theory]
    [InlineData(-1, 6)]  // all cells
    [InlineData(1, 4)]   // 2, 10, 11, 12
    [InlineData(11, 1)]  // 12
    [InlineData(12, 0)]  // none
    public void Count_CountsMatchingCells(int threshold, int expected)
    {
        var grid = MakeNumberedGrid(3, 2);

        Assert.Equal(expected, grid.Count(v => v > threshold));
    }

    [Fact]
    public void Find_ReturnsFirstMatchInRowMajorOrder()
    {
        var grid = Grid.OfChars("..#\n#..");

        // '#' exists at (2, 0) and (0, 1); row-major order finds (2, 0) first.
        Assert.Equal((2, 0), grid.Find(c => c == '#'));
    }

    [Fact]
    public void Find_ReturnsNullWhenNoMatch()
    {
        var grid = Grid.OfChars("...\n...");

        Assert.Null(grid.Find(c => c == '#'));
    }

    // ---- Neighbours --------------------------------------------------------

    [Theory]
    [InlineData(1, 1, 4)]  // centre
    [InlineData(0, 0, 2)]  // corner
    [InlineData(2, 2, 2)]  // opposite corner
    [InlineData(1, 0, 3)]  // edge
    [InlineData(0, 1, 3)]  // edge
    public void Neighbours4_ClipsToBounds(int x, int y, int expectedCount)
    {
        var grid = new Grid<int>(3, 3);

        Assert.Equal(expectedCount, grid.Neighbours4(x, y).Count());
    }

    [Theory]
    [InlineData(1, 1, 8)]  // centre
    [InlineData(0, 0, 3)]  // corner
    [InlineData(2, 2, 3)]  // opposite corner
    [InlineData(1, 0, 5)]  // edge
    [InlineData(0, 1, 5)]  // edge
    public void Neighbours8_ClipsToBounds(int x, int y, int expectedCount)
    {
        var grid = new Grid<int>(3, 3);

        Assert.Equal(expectedCount, grid.Neighbours8(x, y).Count());
    }

    public static TheoryData<int, int, (int, int)[]> Neighbours4Coordinates => new()
    {
        { 1, 1, [(0, 1), (2, 1), (1, 0), (1, 2)] },
        { 0, 0, [(1, 0), (0, 1)] },
        { 2, 0, [(1, 0), (2, 1)] },
    };

    [Theory]
    [MemberData(nameof(Neighbours4Coordinates))]
    public void Neighbours4_YieldsExpectedCoordinates(int x, int y, (int, int)[] expected)
    {
        var grid = new Grid<int>(3, 3);

        var actual = grid.Neighbours4(x, y).Select(n => (n.X, n.Y));

        Assert.Equal(expected.Order(), actual.Order());
    }

    public static TheoryData<int, int, (int, int)[]> Neighbours8Coordinates => new()
    {
        { 0, 0, [(1, 0), (0, 1), (1, 1)] },
        { 1, 1, [(0, 0), (1, 0), (2, 0), (0, 1), (2, 1), (0, 2), (1, 2), (2, 2)] },
    };

    [Theory]
    [MemberData(nameof(Neighbours8Coordinates))]
    public void Neighbours8_YieldsExpectedCoordinates(int x, int y, (int, int)[] expected)
    {
        var grid = new Grid<int>(3, 3);

        var actual = grid.Neighbours8(x, y).Select(n => (n.X, n.Y));

        Assert.Equal(expected.Order(), actual.Order());
    }

    [Fact]
    public void Neighbours_ReturnCellValues()
    {
        var grid = MakeNumberedGrid(3, 3);

        Assert.All(grid.Neighbours8(1, 1), n => Assert.Equal(grid[n.X, n.Y], n.Value));
    }

    [Fact]
    public void Neighbours_OnSingleCellGrid_AreEmpty()
    {
        var grid = new Grid<int>(1, 1);

        Assert.Empty(grid.Neighbours4(0, 0));
        Assert.Empty(grid.Neighbours8(0, 0));
    }

    // ---- Parsing -----------------------------------------------------------

    [Theory]
    [InlineData("ab\ncd")]        // Unix line endings
    [InlineData("ab\r\ncd")]      // Windows line endings
    [InlineData("ab\ncd\n")]      // trailing newline
    [InlineData("\nab\ncd")]      // leading newline
    [InlineData("\r\nab\r\ncd\r\n")]
    public void Parse_String_NormalizesLineEndings(string text)
    {
        var grid = Grid<char>.Parse(text, c => c);

        Assert.Equal(2, grid.Width);
        Assert.Equal(2, grid.Height);
        Assert.Equal("ab\ncd", grid.Render(c => c));
    }

    [Fact]
    public void Parse_String_AppliesSelector()
    {
        var grid = Grid<int>.Parse("12\n34", c => c - '0');

        Assert.Equal(1, grid[0, 0]);
        Assert.Equal(2, grid[1, 0]);
        Assert.Equal(3, grid[0, 1]);
        Assert.Equal(4, grid[1, 1]);
    }

    [Fact]
    public void Parse_Lines_ProducesExpectedGrid()
    {
        var grid = Grid<char>.Parse(["abc", "def"], c => c);

        Assert.Equal(3, grid.Width);
        Assert.Equal(2, grid.Height);
        Assert.Equal('a', grid[0, 0]);
        Assert.Equal('f', grid[2, 1]);
    }

    [Theory]
    [InlineData("z")]
    [InlineData("a\nb\nc")]  // single column
    public void Parse_SingleColumnOrCell_Works(string text)
    {
        var grid = Grid<char>.Parse(text, c => c);

        Assert.Equal(1, grid.Width);
        Assert.Equal(text.Count(c => c == '\n') + 1, grid.Height);
    }

    [Fact]
    public void Parse_EmptyLineList_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Grid<char>.Parse([], c => c));
    }

    [Theory]
    [InlineData("abc\nde")]    // second line shorter
    [InlineData("ab\nabc")]    // second line longer
    [InlineData("a\nbc\nd")]   // middle line ragged
    public void Parse_RaggedInput_ThrowsFormatException(string text)
    {
        Assert.Throws<FormatException>(() => Grid<char>.Parse(text, c => c));
    }

    // ---- Rendering ---------------------------------------------------------

    [Fact]
    public void Render_Char_RoundTripsWithParse()
    {
        const string text = "#.#\n.#.\n###";

        var grid = Grid.OfChars(text);

        Assert.Equal(text, grid.Render(c => c));
    }

    [Fact]
    public void Render_Char_HasNoTrailingNewline()
    {
        var grid = new Grid<char>(2, 2, '.');

        Assert.DoesNotContain('\n', grid.Render(c => c).Split('\n')[^1]);
        Assert.False(grid.Render(c => c).EndsWith('\n'));
    }

    [Fact]
    public void Render_Char_SingleRow_HasNoNewline()
    {
        var grid = Grid.OfChars("abc");

        Assert.Equal("abc", grid.Render(c => c));
    }

    [Fact]
    public void Render_String_PadsToWidestValueAndUsesSeparator()
    {
        var grid = new Grid<int>(2, 2);
        grid[0, 0] = 1;
        grid[1, 0] = 10;
        grid[0, 1] = 100;
        grid[1, 1] = 2;

        string rendered = grid.Render(v => v.ToString());

        Assert.Equal("  1  10\n100   2", rendered);
    }

    [Fact]
    public void Render_String_HonoursCustomSeparator()
    {
        var grid = new Grid<int>(2, 1);
        grid[0, 0] = 1;
        grid[1, 0] = 2;

        Assert.Equal("1|2", grid.Render(v => v.ToString(), "|"));
    }

    // ---- Static factory (Grid) ---------------------------------------------

    [Fact]
    public void OfChars_FromString_PreservesCharacters()
    {
        var grid = Grid.OfChars("ab\ncd");

        Assert.Equal([(0, 0, 'a'), (1, 0, 'b'), (0, 1, 'c'), (1, 1, 'd')], grid.Cells());
    }

    [Fact]
    public void OfChars_FromLines_PreservesCharacters()
    {
        var grid = Grid.OfChars(["ab", "cd"]);

        Assert.Equal('a', grid[0, 0]);
        Assert.Equal('d', grid[1, 1]);
    }

    [Theory]
    [InlineData('#', 3)]
    [InlineData('.', 3)]
    [InlineData('x', 0)]
    public void OfBools_FromString_MapsTrueChar(char trueChar, int expectedTrueCount)
    {
        var grid = Grid.OfBools("#.#\n.#.", trueChar);

        Assert.Equal(expectedTrueCount, grid.Count(v => v));
    }

    [Fact]
    public void OfBools_FromLines_MapsTrueChar()
    {
        var grid = Grid.OfBools(["#.", ".#"], '#');

        Assert.True(grid[0, 0]);
        Assert.False(grid[1, 0]);
        Assert.False(grid[0, 1]);
        Assert.True(grid[1, 1]);
    }

    [Fact]
    public void Of_FromString_AppliesSelector()
    {
        var grid = Grid.Of("19\n28", c => c - '0');

        Assert.Equal(1, grid[0, 0]);
        Assert.Equal(8, grid[1, 1]);
    }

    [Fact]
    public void Of_FromLines_AppliesSelector()
    {
        var grid = Grid.Of(["19", "28"], c => c - '0');

        Assert.Equal(9, grid[1, 0]);
        Assert.Equal(2, grid[0, 1]);
    }

    // ---- Helpers -----------------------------------------------------------

    /// <summary>
    /// Creates a grid where each cell holds x + 10 * y, so every cell value
    /// encodes its own coordinates.
    /// </summary>
    private static Grid<int> MakeNumberedGrid(int width, int height)
    {
        var grid = new Grid<int>(width, height);
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                grid[x, y] = x + 10 * y;

        return grid;
    }
}
