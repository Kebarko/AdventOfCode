using KE.AoC.Solutions.Common;

namespace KE.AoC.Solutions.Tests.Common;

public class LongIntervalTests
{
    #region Constructor

    [Theory]
    [InlineData(0L, 0L)]
    [InlineData(1L, 1L)]
    [InlineData(-5L, -3L)]
    [InlineData(-10L, 10L)]
    [InlineData(long.MinValue, long.MinValue)]
    [InlineData(long.MaxValue, long.MaxValue)]
    [InlineData(long.MinValue, long.MaxValue)]
    public void Constructor_ValidBounds_SetsProperties(long start, long end)
    {
        var interval = new LongInterval(start, end);

        Assert.Equal(start, interval.Start);
        Assert.Equal(end, interval.End);
    }

    [Theory]
    [InlineData(1L, 0L)]
    [InlineData(0L, -1L)]
    [InlineData(long.MaxValue, long.MinValue)]
    [InlineData(1L, -1L)]
    public void Constructor_StartGreaterThanEnd_ThrowsArgumentException(long start, long end)
    {
        Assert.Throws<ArgumentException>(() => new LongInterval(start, end));
    }

    [Fact]
    public void DefaultInstance_IsZeroZero()
    {
        LongInterval interval = default;

        Assert.Equal(0L, interval.Start);
        Assert.Equal(0L, interval.End);
        Assert.Equal(1UL, interval.Length);
    }

    #endregion

    #region Length

    [Theory]
    [InlineData(0L, 0L, 1UL)]
    [InlineData(1L, 3L, 3UL)]
    [InlineData(-5L, -3L, 3UL)]
    [InlineData(long.MinValue, long.MinValue, 1UL)]
    [InlineData(long.MaxValue, long.MaxValue, 1UL)]
    [InlineData(0L, long.MaxValue, (ulong)long.MaxValue + 1)]
    [InlineData(long.MinValue, -1L, 9223372036854775808UL)] // 2^63 points
    [InlineData(-1L, 0L, 2UL)]
    [InlineData(-10L, 10L, 21UL)]
    [InlineData(long.MinValue + 1, long.MaxValue, ulong.MaxValue)]
    public void Length_ReturnsInclusivePointCount(long start, long end, ulong expected)
    {
        var interval = new LongInterval(start, end);

        Assert.Equal(expected, interval.Length);
    }

    [Fact]
    public void Length_FullRange_ThrowsOverflowException()
    {
        // The full range contains 2^64 points, which is not representable in ulong.
        var interval = new LongInterval(long.MinValue, long.MaxValue);

        Assert.Throws<OverflowException>(() => _ = interval.Length);
    }

    #endregion

    #region Overlaps

    public static TheoryData<LongInterval, LongInterval, bool> OverlapsCases => new()
    {
        // Identical intervals
        { new LongInterval(1, 5), new LongInterval(1, 5), true },
        // Single-point identical
        { new LongInterval(3, 3), new LongInterval(3, 3), true },
        // Full containment
        { new LongInterval(0, 10), new LongInterval(3, 7), true },
        // Partial overlap
        { new LongInterval(0, 5), new LongInterval(3, 8), true },
        // Touching at a shared endpoint (inclusive bounds => overlap)
        { new LongInterval(0, 5), new LongInterval(5, 10), true },
        // Single point inside a range
        { new LongInterval(0, 10), new LongInterval(4, 4), true },
        // Adjacent but not overlapping
        { new LongInterval(0, 5), new LongInterval(6, 10), false },
        // Clearly disjoint
        { new LongInterval(0, 5), new LongInterval(100, 200), false },
        // Disjoint negative/positive
        { new LongInterval(-10, -5), new LongInterval(5, 10), false },
        // Overlap across zero
        { new LongInterval(-5, 2), new LongInterval(0, 10), true },
        // Extremes: full range overlaps everything
        { new LongInterval(long.MinValue, long.MaxValue), new LongInterval(0, 0), true },
        // Extremes: disjoint single points at both ends
        { new LongInterval(long.MinValue, long.MinValue), new LongInterval(long.MaxValue, long.MaxValue), false },
    };

    [Theory]
    [MemberData(nameof(OverlapsCases))]
    public void Overlaps_IsCorrectAndSymmetric(LongInterval a, LongInterval b, bool expected)
    {
        Assert.Equal(expected, a.Overlaps(b));
        Assert.Equal(expected, b.Overlaps(a));
    }

    #endregion

    #region IsAdjacentTo

    public static TheoryData<LongInterval, LongInterval, bool> AdjacencyCases => new()
    {
        // Directly adjacent
        { new LongInterval(0, 5), new LongInterval(6, 10), true },
        // Adjacent single points
        { new LongInterval(3, 3), new LongInterval(4, 4), true },
        // Adjacent across zero
        { new LongInterval(-5, -1), new LongInterval(0, 5), true },
        // Gap of one
        { new LongInterval(0, 5), new LongInterval(7, 10), false },
        // Touching at a shared endpoint (overlapping, not adjacent)
        { new LongInterval(0, 5), new LongInterval(5, 10), false },
        // Overlapping
        { new LongInterval(0, 5), new LongInterval(3, 8), false },
        // Identical
        { new LongInterval(1, 5), new LongInterval(1, 5), false },
        // Adjacency where one interval ends at long.MaxValue (guard against overflow)
        { new LongInterval(5, long.MaxValue), new LongInterval(long.MinValue, 4), true },
        // long.MaxValue endpoint cannot be adjacent "upward"
        { new LongInterval(0, long.MaxValue), new LongInterval(0, long.MaxValue), false },
        // Full range is not adjacent to anything (including itself)
        { new LongInterval(long.MinValue, long.MaxValue), new LongInterval(long.MinValue, long.MaxValue), false },
        // Adjacent at the low extreme
        { new LongInterval(long.MinValue, long.MinValue), new LongInterval(long.MinValue + 1, 0), true },
    };

    [Theory]
    [MemberData(nameof(AdjacencyCases))]
    public void IsAdjacentTo_IsCorrectAndSymmetric(LongInterval a, LongInterval b, bool expected)
    {
        Assert.Equal(expected, a.IsAdjacentTo(b));
        Assert.Equal(expected, b.IsAdjacentTo(a));
    }

    #endregion

    #region CompareTo

    public static TheoryData<LongInterval, LongInterval, int> CompareCases => new()
    {
        // Ordered by start
        { new LongInterval(0, 5), new LongInterval(1, 2), -1 },
        { new LongInterval(10, 20), new LongInterval(-5, 100), 1 },
        // Same start: tie-break on end
        { new LongInterval(0, 5), new LongInterval(0, 10), -1 },
        { new LongInterval(0, 10), new LongInterval(0, 5), 1 },
        // Equal
        { new LongInterval(3, 7), new LongInterval(3, 7), 0 },
        // Extremes
        { new LongInterval(long.MinValue, 0), new LongInterval(long.MaxValue, long.MaxValue), -1 },
    };

    [Theory]
    [MemberData(nameof(CompareCases))]
    public void CompareTo_OrdersByStartThenEnd(LongInterval a, LongInterval b, int expectedSign)
    {
        Assert.Equal(expectedSign, Math.Sign(a.CompareTo(b)));
        Assert.Equal(-expectedSign, Math.Sign(b.CompareTo(a)));
    }

    [Fact]
    public void CompareTo_SortsListDeterministically()
    {
        List<LongInterval> intervals =
        [
            new(5, 10),
            new(0, 100),
            new(0, 5),
            new(-3, -1),
            new(5, 7),
        ];

        intervals.Sort();

        List<LongInterval> expected =
        [
            new(-3, -1),
            new(0, 5),
            new(0, 100),
            new(5, 7),
            new(5, 10),
        ];

        Assert.Equal(expected, intervals);
    }

    #endregion

    #region Parse

    [Theory]
    [InlineData("1-5", 1L, 5L)]
    [InlineData("0-0", 0L, 0L)]
    [InlineData("-5--3", -5L, -3L)]
    [InlineData("-10-10", -10L, 10L)]
    [InlineData("-9223372036854775808-9223372036854775807", long.MinValue, long.MaxValue)]
    public void Parse_ValidInput_ReturnsInterval(string input, long expectedStart, long expectedEnd)
    {
        LongInterval interval = LongInterval.Parse(input);

        Assert.Equal(expectedStart, interval.Start);
        Assert.Equal(expectedEnd, interval.End);
    }

    [Theory]
    [InlineData("")]
    public void Parse_NullOrWhitespace_ThrowsArgumentException(string input)
    {
        Assert.Throws<ArgumentException>(() => LongInterval.Parse(input));
    }

    [Theory]
    [InlineData("   ")]
    public void Parse_NullOrWhitespace_ThrowsFormatException(string input)
    {
        Assert.Throws<FormatException>(() => LongInterval.Parse(input));
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("5")]
    [InlineData("5-")]
    [InlineData("-5")]
    [InlineData("1-2-3")]
    [InlineData("1..5")]
    [InlineData("1.5-3")]
    [InlineData("- 5-3")]
    [InlineData("  3 - 7  ")]
    public void Parse_InvalidFormat_ThrowsFormatException(string input)
    {
        Assert.Throws<FormatException>(() => LongInterval.Parse(input));
    }

    [Theory]
    [InlineData("5-3")]
    [InlineData("0--1")]
    public void Parse_StartGreaterThanEnd_ThrowsArgumentException(string input)
    {
        Assert.Throws<ArgumentException>(() => LongInterval.Parse(input));
    }

    [Theory]
    [InlineData("99999999999999999999-0")]
    [InlineData("0-99999999999999999999")]
    public void Parse_ValueOutOfLongRange_ThrowsOverflowException(string input)
    {
        Assert.Throws<OverflowException>(() => LongInterval.Parse(input));
    }

    #endregion

    #region TryParse

    [Theory]
    [InlineData("1-5", 1L, 5L)]
    [InlineData("0-0", 0L, 0L)]
    [InlineData("-5--3", -5L, -3L)]
    [InlineData("-9223372036854775808-9223372036854775807", long.MinValue, long.MaxValue)]
    public void TryParse_ValidInput_ReturnsTrueWithResult(string input, long expectedStart, long expectedEnd)
    {
        bool success = LongInterval.TryParse(input, out LongInterval interval);

        Assert.True(success);
        Assert.Equal(expectedStart, interval.Start);
        Assert.Equal(expectedEnd, interval.End);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("5")]
    [InlineData("1-2-3")]
    [InlineData("  3 - 7  ")]
    [InlineData("5-3")]                     // start > end
    [InlineData("99999999999999999999-0")]  // overflow
    [InlineData("0-99999999999999999999")]  // overflow
    public void TryParse_InvalidInput_ReturnsFalseWithDefault(string? input)
    {
        bool success = LongInterval.TryParse(input!, out LongInterval interval);

        Assert.False(success);
        Assert.Equal(default, interval);
    }

    #endregion

    #region Merge

    public static TheoryData<LongInterval[], LongInterval[]> MergeCases => new()
    {
        // Empty input
        { Array.Empty<LongInterval>(), Array.Empty<LongInterval>() },
        // Single interval passes through
        { [new(1, 5)], [new(1, 5)] },
        // Overlapping pair
        { [new(1, 5), new(3, 8)], [new(1, 8)] },
        // Touching at a shared endpoint
        { [new(1, 5), new(5, 10)], [new(1, 10)] },
        // Adjacent pair
        { [new(1, 5), new(6, 10)], [new(1, 10)] },
        // Disjoint intervals stay separate
        { [new(1, 5), new(7, 10)], [new(1, 5), new(7, 10)] },
        // Unsorted input gets sorted first
        { [new(7, 10), new(1, 5), new(4, 8)], [new(1, 10)] },
        // Contained interval is absorbed
        { [new(0, 100), new(10, 20)], [new(0, 100)] },
        // Exact duplicates collapse
        { [new(1, 5), new(1, 5), new(1, 5)], [new(1, 5)] },
        // Chain of adjacency merges into one
        { [new(1, 2), new(3, 4), new(5, 6), new(7, 8)], [new(1, 8)] },
        // Mixed: two clusters
        { [new(1, 3), new(4, 6), new(10, 12), new(11, 15)], [new(1, 6), new(10, 15)] },
        // Negative and zero-crossing intervals
        { [new(-10, -6), new(-5, 0), new(1, 3)], [new(-10, 3)] },
        // Later interval fully inside the accumulated one (End must stay the max)
        { [new(0, 10), new(2, 3), new(5, 6)], [new(0, 10)] },
        // Adjacency at the long.MaxValue boundary (guard must not overflow)
        { [new(5, long.MaxValue), new(long.MinValue, 4)], [new(long.MinValue, long.MaxValue)] },
        // Interval ending at long.MaxValue merging via overlap
        { [new(0, long.MaxValue), new(100, 200)], [new(0, long.MaxValue)] },
        // Extremes that must remain separate
        {
            [new(long.MinValue, long.MinValue), new(long.MaxValue, long.MaxValue)],
            [new(long.MinValue, long.MinValue), new(long.MaxValue, long.MaxValue)]
        },
    };

    [Theory]
    [MemberData(nameof(MergeCases))]
    public void Merge_ProducesNonOverlappingNonAdjacentSortedIntervals(
        LongInterval[] input, LongInterval[] expected)
    {
        IReadOnlyList<LongInterval> result = LongInterval.Merge(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Merge_ResultInvariantsHold()
    {
        LongInterval[] input =
        [
            new(14, 18), new(1, 3), new(2, 6), new(30, 40),
            new(19, 25), new(-5, -1), new(50, 50),
        ];

        IReadOnlyList<LongInterval> result = LongInterval.Merge(input);

        for (int i = 1; i < result.Count; i++)
        {
            Assert.True(result[i - 1].CompareTo(result[i]) < 0, "Result must be strictly sorted.");
            Assert.False(result[i - 1].Overlaps(result[i]), "Result must not contain overlaps.");
            Assert.False(result[i - 1].IsAdjacentTo(result[i]), "Result must not contain adjacent intervals.");
        }
    }

    [Fact]
    public void Merge_DoesNotDependOnInputOrder()
    {
        LongInterval[] intervals = [new(1, 3), new(2, 6), new(8, 10), new(11, 15)];
        LongInterval[] reversed = [.. intervals.Reverse()];

        Assert.Equal(LongInterval.Merge(intervals), LongInterval.Merge(reversed));
    }

    #endregion

    #region Equality (record struct semantics)

    [Fact]
    public void Equality_SameBounds_AreEqual()
    {
        var a = new LongInterval(1, 5);
        var b = new LongInterval(1, 5);

        Assert.Equal(a, b);
        Assert.True(a == b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Theory]
    [InlineData(1L, 5L, 1L, 6L)]
    [InlineData(1L, 5L, 2L, 5L)]
    [InlineData(0L, 0L, 1L, 1L)]
    public void Equality_DifferentBounds_AreNotEqual(long aStart, long aEnd, long bStart, long bEnd)
    {
        var a = new LongInterval(aStart, aEnd);
        var b = new LongInterval(bStart, bEnd);

        Assert.NotEqual(a, b);
        Assert.True(a != b);
    }

    #endregion
}
