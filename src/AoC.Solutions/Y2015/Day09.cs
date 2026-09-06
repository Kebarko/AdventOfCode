using KE.AoC.Core.Solution;
using KE.AoC.Solutions.Common;
using System.Globalization;

namespace KE.AoC.Solutions.Y2015;

[Solution(2015, 9)]
public sealed class Day09 : SolutionBase
{
    /// <summary>
    /// Calculates the shortest path length that visits all locations in the graph.
    /// </summary>
    /// <param name="input">The input string containing distances between locations.</param>
    /// <returns>The shortest path length.</returns>
    public override object PartOne(string input)
    {
        Graph graph = ParseGraph(input.AsSpan());

        ushort minLength = ushort.MaxValue;
        foreach (string[] perm in Combinatorics.GetPermutations(graph.Vertices))
        {
            ushort length = CalculatePathLength(graph, perm);
            if (length < minLength)
                minLength = length;
        }

        return minLength;
    }

    /// <summary>
    /// Calculates the longest path length that visits all locations in the graph.
    /// </summary>
    /// <param name="input">The input string containing distances between locations.</param>
    /// <returns>The longest path length.</returns>
    public override object PartTwo(string input)
    {
        Graph graph = ParseGraph(input.AsSpan());

        ushort maxLength = ushort.MinValue;
        foreach (string[] perm in Combinatorics.GetPermutations(graph.Vertices))
        {
            ushort length = CalculatePathLength(graph, perm);
            if (length > maxLength)
                maxLength = length;
        }

        return maxLength;
    }

    /// <summary>
    /// Calculates the total length of the given path in the graph.
    /// </summary>
    /// <param name="graph">The graph.</param>
    /// <param name="path">The path.</param>
    /// <returns>The total length of the path.</returns>
    private static ushort CalculatePathLength(Graph graph, string[] path)
    {
        ushort length = 0;
        for (int i = 0; i < path.Length - 1; i++)
        {
            length += graph.Edges[(path[i], path[i + 1])];
        }

        return length;
    }

    /// <summary>
    /// Parses the input string to create a graph representation.
    /// </summary>
    /// <param name="span">The input string containing distances between locations.</param>
    /// <returns>The graph representation.</returns>
    private static Graph ParseGraph(ReadOnlySpan<char> span)
    {
        Dictionary<(string, string), ushort> distances = [];
        HashSet<string> locations = [];

        Span<Range> tokens = stackalloc Range[5];
        foreach (ReadOnlySpan<char> line in span.EnumerateLines())
        {
            int count = line.Split(tokens, ' ');
            if (count != 5)
                continue;

            string from = line[tokens[0]].ToString();
            string to = line[tokens[2]].ToString();
            ushort dist = ushort.Parse(line[tokens[4]], NumberStyles.None, NumberFormatInfo.InvariantInfo);

            distances.Add((from, to), dist);
            distances.Add((to, from), dist);

            locations.Add(from);
            locations.Add(to);
        }

        return new Graph(locations.ToList(), distances);
    }

    /// <summary>
    /// Represents a graph with vertices and edges.
    /// </summary>
    /// <param name="Vertices">The vertices of the graph.</param>
    /// <param name="Edges">The edges of the graph with their corresponding distances.</param>
    private record class Graph(IList<string> Vertices, IDictionary<(string, string), ushort> Edges);
}
