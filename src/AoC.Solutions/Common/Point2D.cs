using System.Numerics;

namespace KE.AoC.Solutions.Common;

/// <summary>
/// A generic 2D point structure that can hold coordinates of any numeric type.
/// </summary>
internal readonly record struct Point2D<T>(T X, T Y) where T : IBinaryNumber<T>, IRootFunctions<T>
{
    /// <summary>
    /// Calculates the Euclidean distance between this point and another point.
    /// </summary>
    public T GetDistance(Point2D<T> other)
    {
        return GetDistance(this, other);
    }

    /// <summary>
    /// Calculates the Manhattan distance between this point and another point.
    /// </summary>
    public T GetManhattanDistance(Point2D<T> other)
    {
        return GetManhattanDistance(this, other);
    }

    /// <summary>
    /// Calculates the Euclidean distance between two points.
    /// </summary>
    public static T GetDistance(Point2D<T> point1, Point2D<T> point2)
    {
        return T.Hypot(point1.X - point2.X, point1.Y - point2.Y);
    }

    /// <summary>
    /// Calculates the Manhattan distance between two points.
    /// </summary>
    public static T GetManhattanDistance(Point2D<T> point1, Point2D<T> point2)
    {
        return T.Abs(point1.X - point2.X) + T.Abs(point1.Y - point2.Y);
    }
}
