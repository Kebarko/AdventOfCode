namespace KE.AoC.Solutions.Common;

/// <summary>
/// Represents an interval of long integers with a start and end value. Boundaries are inclusive.
/// </summary>
public readonly partial record struct LongInterval : IComparable<LongInterval>
{
    /// <summary>
    /// Gets the start value of the interval.
    /// </summary>
    public long Start { get; }

    /// <summary>
    /// Gets the end value of the interval.
    /// </summary>
    public long End { get; }

    /// <summary>
    /// Gets the length of the interval.
    /// </summary>
    public ulong Length => checked(unchecked((ulong)End - (ulong)Start) + 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="LongInterval"/> struct with the specified start and end values.
    /// </summary>
    public LongInterval(long start, long end)
    {
        if (start > end)
        {
            throw new ArgumentException($"Invalid interval: start ({start}) must be less than or equal to end ({end}).");
        }

        Start = start;
        End = end;
    }

    /// <summary>
    /// Determines whether the current interval overlaps with another interval.
    /// </summary>
    public bool Overlaps(LongInterval other)
    {
        return Start <= other.End && End >= other.Start;
    }

    /// <summary>
    /// Determines whether the current interval is adjacent to another interval.
    /// </summary>
    public bool IsAdjacentTo(LongInterval other)
    {
        return (End != long.MaxValue && End + 1 == other.Start) ||
               (other.End != long.MaxValue && other.End + 1 == Start);
    }

    /// <summary>
    /// Compares the current interval with another interval for ordering.
    /// </summary>
    public int CompareTo(LongInterval other)
    {
        if (Start == other.Start)
        {
            return End.CompareTo(other.End);
        }

        return Start.CompareTo(other.Start);
    }

    /// <summary>
    /// Parses a string representation of an interval in the format "Start-End" into a <see cref="LongInterval"/> instance.
    /// </summary>
    public static LongInterval Parse(ReadOnlySpan<char> span)
    {
        if (span.IsEmpty)
            throw new ArgumentException("Interval string must not be null or empty.", nameof(span));

        int dash = span.IndexOf('-');
        if (dash < 0)
            throw new FormatException($"Invalid interval '{span}'. Expected format 'Start-End'.");

        long start = long.Parse(span[..dash]);
        long end = long.Parse(span[(dash + 1)..]);

        return new LongInterval(start, end);
    }

    /// <summary>
    /// Tries to parse a string representation of an interval in the format "Start-End" into a <see cref="LongInterval"/> instance.
    /// </summary>
    public static bool TryParse(ReadOnlySpan<char> span, out LongInterval result)
    {
        result = default;

        if (span.IsEmpty)
            return false;

        int dash = span.IndexOf('-');
        if (dash < 0)
            return false;

        if (!long.TryParse(span[..dash], out long start) ||
            !long.TryParse(span[(dash + 1)..], out long end))
            return false;

        if (start > end)
            return false;

        result = new LongInterval(start, end);
        return true;
    }

    /// <summary>
    /// Merges a collection of intervals into a list of non-overlapping and non-adjacent intervals.
    /// </summary>
    public static IReadOnlyList<LongInterval> Merge(IEnumerable<LongInterval> intervals)
    {
        List<LongInterval> ordered = [.. intervals];

        if (ordered.Count == 0)
            return [];

        ordered.Sort();

        var result = new List<LongInterval>();
        LongInterval tmp = ordered[0];

        foreach (LongInterval interval in ordered.Skip(1))
        {
            if (tmp.Overlaps(interval) || tmp.IsAdjacentTo(interval))
            {
                tmp = new LongInterval(tmp.Start, Math.Max(tmp.End, interval.End));
            }
            else
            {
                result.Add(tmp);
                tmp = interval;
            }
        }

        result.Add(tmp);
        return result;
    }
}
