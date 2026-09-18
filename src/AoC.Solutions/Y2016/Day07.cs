
using KE.AoC.Core.Solution;

namespace KE.AoC.Solutions.Y2016;

/// <summary>
/// --- Day 7: Internet Protocol Version 7 ---
/// </summary>
[Solution(2016, 7)]
public sealed class Day07 : SolutionBase
{
    /// <summary>
    /// Calculates the number of IP addresses that support TLS (Transport Layer Snooping) from the given input.
    /// </summary>
    /// <param name="input">The input lines representing IP addresses.</param>
    /// <returns>The number of IP addresses that support TLS.</returns>
    public override object PartOne(string input)
    {
        int count = 0;
        foreach (ReadOnlySpan<char> line in input.EnumerateLines())
        {
            if (SupportsTls(line))
                count++;
        }

        return count;
    }

    /// <summary>
    /// Calculates the number of IP addresses that support SSL (Super-Secret Listening) from the given input.
    /// </summary>
    /// <param name="input">The input lines representing IP addresses.</param>
    /// <returns>The number of IP addresses that support SSL.</returns>
    public override object PartTwo(string input)
    {
        int count = 0;
        foreach (ReadOnlySpan<char> line in input.EnumerateLines())
        {
            if (SupportsSsl(line))
                count++;
        }

        return count;
    }

    /// <summary>
    /// Determines if the given IP address supports TLS (Transport Layer Snooping).
    /// </summary>
    /// <param name="span">The IP address to check.</param>
    /// <returns><c>true</c> if the IP address supports TLS; otherwise, <c>false</c>.</returns>
    private static bool SupportsTls(ReadOnlySpan<char> span)
    {
        bool abbaOutside = false;
        bool abbaInside = false;

        int i = 0;
        while (i < span.Length)
        {
            int open = span.Slice(i).IndexOf('[');
            ReadOnlySpan<char> supernet = open == -1 ? span.Slice(i) : span.Slice(i, open);

            if (HasAbba(supernet))
                abbaOutside = true;

            if (open == -1)
                break;

            i += open + 1;

            int close = span.Slice(i).IndexOf(']');
            ReadOnlySpan<char> hypernet = span.Slice(i, close);

            if (HasAbba(hypernet))
                abbaInside = true;

            i += close + 1;
        }

        return abbaOutside && !abbaInside;

        static bool HasAbba(ReadOnlySpan<char> span)
        {
            for (int i = 0; i < span.Length - 3; i++)
                if (span[i] != span[i + 1] && span[i] == span[i + 3] && span[i + 1] == span[i + 2])
                    return true;

            return false;
        }
    }

    /// <summary>
    /// Determines if the given IP address supports SSL (Super-Secret Listening).
    /// </summary>
    /// <param name="span">The IP address to check.</param>
    /// <returns><c>true</c> if the IP address supports SSL; otherwise, <c>false</c>.</returns>
    private static bool SupportsSsl(ReadOnlySpan<char> span)
    {
        int i = 0;
        while (i < span.Length)
        {
            int open = span.Slice(i).IndexOf('[');
            ReadOnlySpan<char> supernet = open == -1 ? span.Slice(i) : span.Slice(i, open);

            for (int j = 0; j < supernet.Length - 2; j++)
                if (supernet[j] != supernet[j + 1] && supernet[j] == supernet[j + 2])
                    if (HasBab(span, supernet[j], supernet[j + 1]))
                        return true;

            if (open == -1)
                break;

            i += open + 1;

            int close = span.Slice(i).IndexOf(']');
            i += close + 1;
        }

        return false;

        static bool HasBab(ReadOnlySpan<char> span, char a, char b)
        {
            int i = 0;
            while (i < span.Length)
            {
                int open = span.Slice(i).IndexOf('[');
                if (open == -1)
                    break;

                i += open + 1;

                int close = span.Slice(i).IndexOf(']');
                ReadOnlySpan<char> hypernet = span.Slice(i, close);

                for (int j = 0; j < hypernet.Length - 2; j++)
                    if (hypernet[j] == b && hypernet[j + 1] == a && hypernet[j + 2] == b)
                        return true;

                i += close + 1;
            }

            return false;
        }
    }
}
