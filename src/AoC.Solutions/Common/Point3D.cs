using System.Numerics;

namespace KE.AoC.Solutions.Common;

/// <summary>
/// A generic 3D point structure that can hold coordinates of any numeric type.
/// </summary>
internal readonly record struct Point3D<T>(T X, T Y, T Z) where T : IBinaryNumber<T>, IRootFunctions<T>
{
    /// <summary>
    /// Calculates the Euclidean distance between this point and another point.
    /// </summary>
    public T GetDistance(Point3D<T> other)
    {
        return GetDistance(this, other);
    }

    /// <summary>
    /// Calculates the Manhattan distance between this point and another point.
    /// </summary>
    public T GetManhattanDistance(Point3D<T> other)
    {
        return GetManhattanDistance(this, other);
    }

    /// <summary>
    /// Calculates the Euclidean distance between two points.
    /// </summary>
    public static T GetDistance(Point3D<T> point1, Point3D<T> point2)
    {
        return T.Hypot(T.Hypot(point1.X - point2.X, point1.Y - point2.Y), point1.Z - point2.Z);
    }

    /// <summary>
    /// Calculates the Manhattan distance between two points.
    /// </summary>
    public static T GetManhattanDistance(Point3D<T> point1, Point3D<T> point2)
    {
        return T.Abs(point1.X - point2.X) + T.Abs(point1.Y - point2.Y) + T.Abs(point1.Z - point2.Z);
    }
}
