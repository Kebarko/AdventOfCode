using System.Text.RegularExpressions;

namespace KE.AoC.Solutions.Common;

/// <summary>
/// Represents an interval of long integers with a start and end value. Boundaries are inclusive.
/// </summary>
internal readonly record struct LongInterval : IComparable<LongInterval>
{
    private static readonly Regex intervalRegex = new Regex(@"^\s*(-?\d+)\s*-\s*(-?\d+)\s*$", RegexOptions.Compiled);

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
    public ulong Length => checked((ulong)End - (ulong)Start + 1);

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
    public static LongInterval Parse(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            throw new ArgumentException("Interval string must not be null or empty.", nameof(str));

        Match match = intervalRegex.Match(str);
        if (!match.Success)
            throw new FormatException($"Invalid interval '{str}'. Expected format 'Start-End'.");

        long start = long.Parse(match.Groups[1].ValueSpan);
        long end = long.Parse(match.Groups[2].ValueSpan);

        return new LongInterval(start, end);
    }

    /// <summary>
    /// Tries to parse a string representation of an interval in the format "Start-End" into a <see cref="LongInterval"/> instance.
    /// </summary>
    public static bool TryParse(string str, out LongInterval result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(str))
            return false;

        Match match = intervalRegex.Match(str);
        if (!match.Success)
            return false;

        if (!long.TryParse(match.Groups[1].ValueSpan, out long start) ||
            !long.TryParse(match.Groups[2].ValueSpan, out long end))
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
