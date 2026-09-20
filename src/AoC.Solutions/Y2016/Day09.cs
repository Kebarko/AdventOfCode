using KE.AoC.Core.Solution;
using System.Globalization;

namespace KE.AoC.Solutions.Y2016;

/// <summary>
/// --- Day 9: Explosives in Cyberspace ---
/// </summary>
[Solution(2016, 9)]
public sealed class Day09 : SolutionBase
{
    /// <summary>
    /// Calculates the decompressed length of the input string
    /// using a simplified version of the decompression algorithm.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The decompressed length.</returns>
    public override object PartOne(string input)
    {
        return GetDecompressedLengthV1(input);
    }

    /// <summary>
    /// Calculates the decompressed length of the input string
    /// using a recursive version of the decompression algorithm.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The decompressed length.</returns>
    public override object PartTwo(string input)
    {
        return GetDecompressedLengthV2(input);
    }

    /// <summary>
    /// Calculates the decompressed length of the input string
    /// using a simplified version of the decompression algorithm.
    /// </summary>
    /// <param name="span">The span containing the input string.</param>
    /// <returns>The decompressed length.</returns>
    /// <exception cref="FormatException">Thrown when the input string is malformed.</exception>
    private static long GetDecompressedLengthV1(ReadOnlySpan<char> span)
    {
        long total = 0;

        int i = 0;
        while (i < span.Length)
        {
            ReadOnlySpan<char> rest = span[i..];

            int open = rest.IndexOf('(');
            if (open == -1)
            {
                total += rest.Length;
                break;
            }

            int close = rest[open..].IndexOf(')');
            if (close == -1)
                throw new FormatException($"Unterminated marker.");

            close += open;

            Marker marker = ParseMarker(rest[(open + 1)..close]);

            int data = close + 1;
            if (marker.DataLength > rest.Length - data)
                throw new FormatException($"Marker runs past the end of the input.");

            total += open + (long)marker.DataLength * marker.Repeat;

            i += data + marker.DataLength;
        }

        return total;
    }

    /// <summary>
    /// Calculates the decompressed length of the input string
    /// using a recursive version of the decompression algorithm.
    /// </summary>
    /// <param name="span">The span containing the input string.</param>
    /// <returns>The decompressed length.</returns>
    /// <exception cref="FormatException">Thrown when the input string is malformed.</exception>
    private static long GetDecompressedLengthV2(ReadOnlySpan<char> span)
    {
        long total = 0;

        int i = 0;
        while (i < span.Length)
        {
            if (span[i] != '(')
            {
                total++;
                i++;
                continue;
            }

            total += ProcessMarker(span, ref i);
        }

        return total;
    }

    /// <summary>
    /// Processes a marker in the format AxB where A is the number of characters to take
    /// and B is the number of times to repeat them. It updates the index to point to the character
    /// after the marker and its data.
    /// </summary>
    /// <param name="span">The span containing the marker.</param>
    /// <param name="i">The index of the marker.</param>
    /// <returns>The length of the decompressed data.</returns>
    private static long ProcessMarker(ReadOnlySpan<char> span, ref int i)
    {
        int close = span[i..].IndexOf(')') + i;

        Marker marker = ParseMarker(span[(i + 1)..close]);

        i = close + 1 + marker.DataLength;

        return marker.Repeat * GetDecompressedLengthV2(span.Slice(close + 1, marker.DataLength));
    }

    /// <summary>
    /// Parses a marker in the format AxB where A is the number of characters to take
    /// and B is the number of times to repeat them.
    /// </summary>
    /// <param name="span">The span containing the marker.</param>
    /// <returns>The parsed marker.</returns>
    /// <exception cref="FormatException">Thrown when the marker is malformed.</exception>
    private static Marker ParseMarker(ReadOnlySpan<char> span)
    {
        int x = span.IndexOf('x');
        if (x == -1)
            throw new FormatException($"Malformed marker '{span}'.");

        return new(ParseValue(span[..x]), ParseValue(span[(x + 1)..]));

        static int ParseValue(ReadOnlySpan<char> span)
            => int.TryParse(span, NumberStyles.None, NumberFormatInfo.InvariantInfo, out int value)
                ? value
                : throw new FormatException($"Malformed marker value '{span}'.");
    }

    /// <summary>
    /// Represents a marker in the format AxB where A is the number of characters to take
    /// and B is the number of times to repeat them.
    /// </summary>
    /// <param name="DataLength">The number of characters to take.</param>
    /// <param name="Repeat">The number of times to repeat the characters.</param>
    private readonly record struct Marker(int DataLength, int Repeat);
}
