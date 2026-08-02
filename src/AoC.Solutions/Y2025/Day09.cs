using KE.AoC.Core.Solution;
using KE.AoC.Solutions.Common;
using System.Collections;

namespace KE.AoC.Solutions.Y2025;

[Solution(2025, 9)]
public sealed class Day09 : SolutionBase
{
    /// <summary>
    /// Calculates the maximum area of a rectangle that can be formed by any two points from the input string.
    /// </summary>
    public override object PartOne(string input)
    {
        List<Point2D<int>> points = ParsePoints(input);

        return points
            .SelectMany((p, i) => points.Skip(i + 1), Rectangle.FromCorners)
            .Max(rectangle => rectangle.Area);
    }

    /// <summary>
    /// Calculates the maximum area of a rectangle that can be formed by any two points from the input string,
    /// such that the rectangle is entirely contained within the polygon defined by the points.
    /// </summary>
    public override object PartTwo(string input)
    {
        List<Point2D<int>> points = ParsePoints(input);

        Polygon polygon = new(points);

        return points
            .SelectMany((p, i) => points.Skip(i + 1), Rectangle.FromCorners)
            .OrderByDescending(rectangle => rectangle.Area)
            .First(rectangle => IsRectangleInsidePolygon(rectangle, polygon))
            .Area;
    }

    /// <summary>
    /// Parses the input string into a list of 2D points represented by Point2D<int> objects.
    /// </summary>
    private static List<Point2D<int>> ParsePoints(string input)
    {
        return Lines(input)
            .Select(line => line.Split(','))
            .Where(coords => coords.Length == 2)
            .Select(coords => new Point2D<int>(
                int.Parse(coords[0]),
                int.Parse(coords[1])))
            .ToList();
    }

    /// <summary>
    /// Checks if a given rectangle is entirely contained within a polygon.
    /// </summary>
    private static bool IsRectangleInsidePolygon(Rectangle rectangle, Polygon polygon)
    {
        // Every corner must be inside or on the polygon boundary.
        foreach (Point2D<int> corner in rectangle.AllCorners)
        {
            if (!IsVertexInPolygon(corner, polygon))
                return false;
        }

        // No polygon edge may cut throught the rectangle.
        foreach (Edge edge in polygon)
        {
            if (EdgeCrossesRectangleInterior(edge, rectangle))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if a given vertex is inside or on the boundary of a polygon.
    /// </summary>
    private static bool IsVertexInPolygon(Point2D<int> vertex, Polygon polygon)
    {
        bool inside = false;

        foreach (Edge edge in polygon)
        {
            // Check if the vertex lies exactly on the edge. If it does, it's considered inside.
            if (IsVertexOnEdge(vertex, edge))
                return true;

            // Check if the edge is either completely above or below the vertex.
            // If it is, it cannot intersect with a horizontal ray extending to the right from the vertex.
            if (vertex.Y < edge.MinY || vertex.Y > edge.MaxY)
                continue;

            // Check if the ray cross this edge to the right of the vertex.
            if (edge.IsHorizontal)
            {
                if (vertex.Y == edge.Vertex1.Y && vertex.X < edge.MinX)
                    inside = !inside;
            }
            else if (edge.IsVertical)
            {
                if (vertex.X < edge.Vertex1.X)
                    inside = !inside;
            }
        }

        return inside;
    }

    /// <summary>
    /// Checks if a given vertex lies on a specified edge. The edge must be either horizontal or vertical.
    /// </summary>
    private static bool IsVertexOnEdge(Point2D<int> vertex, Edge edge)
    {
        if (edge.IsHorizontal)
        {
            // Check if the vertex's y-coordinate matches the edge's y-coordinate
            // and if the vertex's x-coordinate is within the edge's x-range.
            return vertex.Y == edge.Vertex1.Y && vertex.X >= edge.MinX && vertex.X <= edge.MaxX;
        }

        if (edge.IsVertical)
        {
            // Check if the vertex's x-coordinate matches the edge's x-coordinate
            // and if the vertex's y-coordinate is within the edge's y-range.
            return vertex.X == edge.Vertex1.X && vertex.Y >= edge.MinY && vertex.Y <= edge.MaxY;
        }

        throw new ArgumentException("Edge must be either horizontal or vertical.");
    }

    /// <summary>
    /// Checks if a given edge crosses the interior of a specified rectangle.
    /// </summary>
    private static bool EdgeCrossesRectangleInterior(Edge edge, Rectangle rectangle)
    {
        if (edge.IsHorizontal)
        {
            // Check if the edge is either above or below the rectangle's interior.
            // If it is, it cannot cross the rectangle's interior.
            if (edge.Vertex1.Y <= rectangle.MinY || edge.Vertex1.Y >= rectangle.MaxY)
                return false;

            // Check if the edge's x-range overlaps with the rectangle's x-range.
            return edge.MinX < rectangle.MaxX && edge.MaxX > rectangle.MinX;

        }

        if (edge.IsVertical)
        {
            // Check if the edge is either to the left or right of the rectangle's interior.
            // If it is, it cannot cross the rectangle's interior.
            if (edge.Vertex1.X <= rectangle.MinX || edge.Vertex1.X >= rectangle.MaxX)
                return false;

            // Check if the edge's y-range overlaps with the rectangle's y-range.
            return edge.MinY < rectangle.MaxY && edge.MaxY > rectangle.MinY;
        }

        throw new ArgumentException("Edge must be either horizontal or vertical.");
    }

    /// <summary>
    /// Represents a rectilinear polygon defined by a list of vertices in 2D space.
    /// </summary>
    private sealed class Polygon : IEnumerable<Edge>
    {
        private readonly List<Edge> edges;

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class with a list of vertices.
        /// </summary>
        public Polygon(List<Point2D<int>> vertices)
        {
            int n = vertices.Count;
            if (n < 4)
                throw new ArgumentException("Polygon needs at least 4 vertices.");

            edges = new List<Edge>(n);

            for (int i = 0; i < n; i++)
            {
                Edge edge = new(vertices[i], vertices[(i + 1) % n]);

                if (!edge.IsHorizontal && !edge.IsVertical)
                    throw new ArgumentException($"Polygon edge {i} is not axis-aligned. Polygon must be rectilinear.");

                edges.Add(edge);
            }
        }

        public IEnumerator<Edge> GetEnumerator()
        {
            return edges.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// Represents an edge defined by two vertices in 2D space.
    /// </summary>
    private sealed class Edge(Point2D<int> vertex1, Point2D<int> vertex2)
    {
        /// <summary>
        /// Gets the first vertex of the edge.
        /// </summary>
        public Point2D<int> Vertex1 { get; } = vertex1;

        /// <summary>
        /// Gets the second vertex of the edge.
        /// </summary>
        public Point2D<int> Vertex2 { get; } = vertex2;

        /// <summary>
        /// Gets the minimum X-coordinate of the edge (the smaller X-coordinate of the two vertices).
        /// </summary>
        public int MinX { get; } = Math.Min(vertex1.X, vertex2.X);

        /// <summary>
        /// Gets the minimum Y-coordinate of the edge (the smaller Y-coordinate of the two vertices).
        /// </summary>
        public int MinY { get; } = Math.Min(vertex1.Y, vertex2.Y);

        /// <summary>
        /// Gets the maximum X-coordinate of the edge (the larger X-coordinate of the two vertices).
        /// </summary>
        public int MaxX { get; } = Math.Max(vertex1.X, vertex2.X);

        /// <summary>
        /// Gets the maximum Y-coordinate of the edge (the larger Y-coordinate of the two vertices).
        /// </summary>
        public int MaxY { get; } = Math.Max(vertex1.Y, vertex2.Y);

        /// <summary>
        /// Indicates whether the edge is horizontal (i.e., both vertices have the same Y-coordinate).
        /// </summary>
        public bool IsHorizontal { get; } = vertex1.Y == vertex2.Y;

        /// <summary>
        /// Indicates whether the edge is vertical (i.e., both vertices have the same X-coordinate).
        /// </summary>
        public bool IsVertical { get; } = vertex1.X == vertex2.X;
    }

    /// <summary>
    /// Represents a rectangle defined by its minimum and maximum X and Y coordinates.
    /// </summary>
    private readonly record struct Rectangle(int MinX, int MinY, int MaxX, int MaxY)
    {
        /// <summary>
        /// Gets an array containing all four corners of the rectangle as Point2D<int> objects.
        /// </summary>
        public Point2D<int>[] AllCorners =>
        [
            new Point2D<int>(MinX, MinY),
            new Point2D<int>(MinX, MaxY),
            new Point2D<int>(MaxX, MinY),
            new Point2D<int>(MaxX, MaxY)
        ];

        /// <summary>
        /// Gets the area of the rectangle.
        /// </summary>
        public long Area { get; } = (MaxX - MinX + 1L) * (MaxY - MinY + 1L);

        /// <summary>
        /// Creates a rectangle from two corner points, ensuring that the minimum and maximum coordinates are correctly assigned.
        /// </summary>
        public static Rectangle FromCorners(Point2D<int> corner1, Point2D<int> corner2)
        {
            return new(
                Math.Min(corner1.X, corner2.X), Math.Min(corner1.Y, corner2.Y),
                Math.Max(corner1.X, corner2.X), Math.Max(corner1.Y, corner2.Y));
        }
    }
}
