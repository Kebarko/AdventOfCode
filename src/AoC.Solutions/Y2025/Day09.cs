using KE.AoC.Core.Solution;
using KE.AoC.Solutions.Common;

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

        ValidatePolygon(points);

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

        ValidatePolygon(points);

        return points
            .SelectMany((p, i) => points.Skip(i + 1), Rectangle.FromCorners)
            .OrderByDescending(rectangle => rectangle.Area)
            .First(rectangle => IsRectangleInsidePolygon(rectangle, points))
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
    /// Validates that the given polygon is a rectilinear polygon with at least 4 vertices.
    /// </summary>
    private static void ValidatePolygon(List<Point2D<int>> polygon)
    {
        int n = polygon.Count;

        if (n < 4)
            throw new ArgumentException("Polygon needs at least 4 vertices.");

        for (int i = 0; i < n; i++)
        {
            Edge edge = new(polygon[i], polygon[(i + 1) % n]);

            if (!edge.IsHorizontal && !edge.IsVertical)
                throw new ArgumentException($"Polygon edge {i} is not axis-aligned. Polygon must be rectilinear.");
        }
    }

    /// <summary>
    /// Checks if a given rectangle is entirely contained within a polygon.
    /// </summary>
    private static bool IsRectangleInsidePolygon(Rectangle rectangle, List<Point2D<int>> polygon)
    {
        // Every corner must be inside or on the polygon boundary.
        foreach (Point2D<int> corner in rectangle.AllCorners)
        {
            if (!IsVertexInPolygon(corner, polygon))
                return false;
        }

        // No polygon edge may cut throught the rectangle.
        int n = polygon.Count;
        for (int i = 0; i < n; i++)
        {
            if (EdgeCrossesRectangleInterior(new Edge(polygon[i], polygon[(i + 1) % n]), rectangle))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if a given vertex is inside or on the boundary of a polygon.
    /// </summary>
    private static bool IsVertexInPolygon(Point2D<int> vertex, List<Point2D<int>> polygon)
    {
        int n = polygon.Count;
        bool inside = false;
        for (int i = 0; i < n; i++)
        {
            Edge edge = new(polygon[i], polygon[(i + 1) % n]);

            // Check if the vertex lies exactly on the edge. If it does, it's considered inside.
            if (IsVertexOnEdge(vertex, edge))
                return true;

            // Check if the edge is either completely above or below the vertex.
            // If it is, it cannot intersect with a horizontal ray extending to the right from the vertex.
            if (vertex.Y < Math.Min(edge.Vertex1.Y, edge.Vertex2.Y) ||
                vertex.Y > Math.Max(edge.Vertex1.Y, edge.Vertex2.Y))
                continue;

            // Check if the ray cross this edge to the right of the vertex.
            long dy = edge.Vertex1.Y - edge.Vertex2.Y;
            long dx = edge.Vertex1.X - edge.Vertex2.X;
            long lhs = (vertex.X - edge.Vertex1.X) * dy;
            long rhs = (vertex.Y - edge.Vertex1.Y) * dx;
            bool intersectsToTheRight = dy > 0 ? lhs < rhs : lhs > rhs;

            // If the ray intersects the edge to the right of the vertex, toggle the inside status.
            if (intersectsToTheRight)
                inside = !inside;
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
            return vertex.Y == edge.Vertex1.Y &&
                vertex.X >= Math.Min(edge.Vertex1.X, edge.Vertex2.X) &&
                vertex.X <= Math.Max(edge.Vertex1.X, edge.Vertex2.X);
        }

        if (edge.IsVertical)
        {
            // Check if the vertex's x-coordinate matches the edge's x-coordinate
            // and if the vertex's y-coordinate is within the edge's y-range.
            return vertex.X == edge.Vertex1.X &&
                vertex.Y >= Math.Min(edge.Vertex1.Y, edge.Vertex2.Y) &&
                vertex.Y <= Math.Max(edge.Vertex1.Y, edge.Vertex2.Y);
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
            if (edge.Vertex1.Y <= rectangle.MinY ||
                edge.Vertex1.Y >= rectangle.MaxY)
                return false;

            // Check if the edge's x-range overlaps with the rectangle's x-range.
            return Math.Min(edge.Vertex1.X, edge.Vertex2.X) < rectangle.MaxX &&
                Math.Max(edge.Vertex1.X, edge.Vertex2.X) > rectangle.MinX;

        }

        if (edge.IsVertical)
        {
            // Check if the edge is either to the left or right of the rectangle's interior.
            // If it is, it cannot cross the rectangle's interior.
            if (edge.Vertex1.X <= rectangle.MinX ||
                edge.Vertex1.X >= rectangle.MaxX)
                return false;

            // Check if the edge's y-range overlaps with the rectangle's y-range.
            return Math.Min(edge.Vertex1.Y, edge.Vertex2.Y) < rectangle.MaxY &&
                Math.Max(edge.Vertex1.Y, edge.Vertex2.Y) > rectangle.MinY;
        }

        throw new ArgumentException("Edge must be either horizontal or vertical.");
    }

    /// <summary>
    /// Represents an edge defined by two vertices in 2D space.
    /// </summary>
    private readonly record struct Edge(Point2D<int> Vertex1, Point2D<int> Vertex2)
    {
        /// <summary>
        /// Indicates whether the edge is horizontal (i.e., both vertices have the same Y-coordinate).
        /// </summary>
        public bool IsHorizontal => Vertex1.Y == Vertex2.Y;

        /// <summary>
        /// Indicates whether the edge is vertical (i.e., both vertices have the same X-coordinate).
        /// </summary>
        public bool IsVertical => Vertex1.X == Vertex2.X;
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
        public long Area => (MaxX - MinX + 1L) * (MaxY - MinY + 1L);

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
