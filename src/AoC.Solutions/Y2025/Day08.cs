using KE.AoC.Core.Solution;
using KE.AoC.Solutions.Common;
using System.Globalization;

namespace KE.AoC.Solutions.Y2025;

[Solution(2025, 8)]
public sealed class Day08 : SolutionBase
{
    /// <summary>
    /// Calculates the product of the sizes of the three largest connected components formed by connecting specified number of shortest edges between points in 3D space.
    /// </summary>
    public override object PartOne(string input)
    {
        List<Point3D<int>> points = ParsePoints(input.AsSpan());

        var uf = new QuickUnion(points.Count);

        foreach ((int p, int q) in ShortestEdges(points, 1000))
        {
            uf.Union(p, q);
        }

        return Enumerable.Range(0, points.Count)
            .Where(p => uf.Find(p) == p)
            .Select(uf.SizeOf)
            .OrderByDescending(x => x)
            .Take(3)
            .Aggregate(1L, (x, y) => x * y);
    }

    /// <summary>
    /// Calculates the product of the sizes of the three largest connected components formed by connecting all shortest edges between points in 3D space until only one connected component remains.
    /// </summary>
    public override object PartTwo(string input)
    {
        List<Point3D<int>> points = ParsePoints(input.AsSpan());

        var uf = new QuickUnion(points.Count);

        long result = 0;
        foreach ((int p, int q) in AllEdges(points))
        {
            uf.Union(p, q);

            if (uf.Components == 1)
            {
                result = (long)points[p].X * points[q].X;
                break;
            }
        }

        return result;
    }

    /// <summary>
    /// Parses the input string into a list of 3D points represented by Point3D<int> objects.
    /// </summary>
    private static List<Point3D<int>> ParsePoints(ReadOnlySpan<char> span)
    {
        List<Point3D<int>> result = [];

        foreach (ReadOnlySpan<char> line in span.EnumerateLines())
        {
            List<Range> ranges = [];
            foreach (Range range in line.Split(','))
                ranges.Add(range);

            if (ranges.Count == 3)
            {
                result.Add(new Point3D<int>(
                    int.Parse(line[ranges[0]], NumberStyles.None, NumberFormatInfo.InvariantInfo),
                    int.Parse(line[ranges[1]], NumberStyles.None, NumberFormatInfo.InvariantInfo),
                    int.Parse(line[ranges[2]], NumberStyles.None, NumberFormatInfo.InvariantInfo)));
            }
        }

        return result;
    }

    /// <summary>
    /// Finds the specified number of shortest edges between points in a 3D space and returns them as a collection of tuples representing the indices of the connected points.
    /// </summary>
    private static IEnumerable<(int I, int J)> ShortestEdges(List<Point3D<int>> points, int count)
    {
        var queue = new PriorityQueue<(int I, int J), long>(Comparer<long>.Create((x, y) => y.CompareTo(x)));

        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                long distance = Point3D<int>.GetSquaredDistance<long>(points[i], points[j]);

                if (queue.Count < count)
                {
                    queue.Enqueue((i, j), distance);
                }
                else
                {
                    queue.EnqueueDequeue((i, j), distance);
                }
            }
        }

        return queue.UnorderedItems.Select(item => item.Element);
    }

    /// <summary>
    /// Finds all edges between points in a 3D space and returns them as a collection of tuples representing the indices of the connected points, ordered by their squared distances.
    /// </summary>
    private static IEnumerable<(int I, int J)> AllEdges(List<Point3D<int>> points)
    {
        var list = new List<(int I, int J, long Distance)>();

        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                long distance = Point3D<int>.GetSquaredDistance<long>(points[i], points[j]);

                list.Add((i, j, distance));
            }
        }

        return list.OrderBy(x => x.Distance).Select(x => (x.I, x.J));
    }
}
