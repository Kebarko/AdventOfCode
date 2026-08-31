using KE.AoC.Core.Solution;

namespace KE.AoC.Solutions.Y2025;

[Solution(2025, 11)]
public sealed class Day11 : SolutionBase
{
    /// <summary>
    /// Counts the number of distinct paths from "you" to "out" in a directed graph.
    /// </summary>
    public override object PartOne(string input)
    {
        const string start = "you";
        const string target = "out";

        Dictionary<string, List<string>> graph = ParseGraph(input.AsSpan());
        Dictionary<string, long> cache = [];

        return Count(start);

        long Count(string node)
        {
            if (node == target)
                return 1;

            if (cache.TryGetValue(node, out long cached))
                return cached;

            long total = 0;
            if (graph.TryGetValue(node, out var nextNodes))
                foreach (string next in nextNodes)
                    total += Count(next);

            cache[node] = total;

            return total;
        }
    }

    /// <summary>
    /// Counts the number of distinct paths from "svr" to "out" in a directed graph, that visit both "dac" and "fft" at least once.
    /// </summary>
    public override object PartTwo(string input)
    {
        const string start = "svr";
        const string target = "out";
        const string dac = "dac";
        const string fft = "fft";

        Dictionary<string, List<string>> graph = ParseGraph(input.AsSpan());
        Dictionary<(string, bool, bool), long> cache = [];

        return Count(start, false, false);

        long Count(string node, bool visitDac, bool visitFft)
        {
            visitDac |= node == dac;
            visitFft |= node == fft;

            if (node == target)
                return (visitDac && visitFft) ? 1 : 0;

            var key = (node, visitDac, visitFft);
            if (cache.TryGetValue(key, out long cached))
                return cached;

            long total = 0;
            if (graph.TryGetValue(node, out var nextNodes))
                foreach (string next in nextNodes)
                    total += Count(next, visitDac, visitFft);

            cache[key] = total;

            return total;
        }
    }

    private static Dictionary<string, List<string>> ParseGraph(ReadOnlySpan<char> span)
    {
        Dictionary<string, List<string>> graph = [];

        foreach (ReadOnlySpan<char> line in span.EnumerateLines())
        {
            List<Range> ranges = [];
            foreach (Range range in line.SplitAny([':', ' ']))
            {
                ranges.Add(range);
            }

            if (ranges.Count > 1)
            {
                List<string> values = [];
                foreach (Range range in ranges.Skip(2))
                {
                    values.Add(line[range].ToString());
                }

                graph.Add(line[ranges[0]].ToString(), values);
            }
        }

        return graph;
    }
}
