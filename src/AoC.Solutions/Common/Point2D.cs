using System.Numerics;

namespace KE.AoC.Solutions.Common;

/// <summary>
/// A generic 2D point structure that can hold coordinates of any numeric type.
/// </summary>
internal readonly record struct Point2D<T>(T X, T Y) where T : INumber<T>
{
    /// <summary>
    /// Calculates the Euclidean distance between this point and another point.
    /// </summary>
    public double GetDistance(Point2D<T> other)
    {
        return GetDistance(this, other);
    }

    /// <summary>
    /// Calculates the squared Euclidean distance between this point and another point.
    /// </summary>
    public TResult GetSquaredDistance<TResult>(Point2D<T> other) where TResult : INumber<TResult>
    {
        return GetSquaredDistance<TResult>(this, other);
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
    public static double GetDistance(Point2D<T> point1, Point2D<T> point2)
    {
        double dx = double.CreateChecked(point1.X) - double.CreateChecked(point2.X);
        double dy = double.CreateChecked(point1.Y) - double.CreateChecked(point2.Y);

        return double.Hypot(dx, dy);
    }

    /// <summary>
    /// Calculates the squared Euclidean distance between two points.
    /// </summary>
    public static TResult GetSquaredDistance<TResult>(Point2D<T> point1, Point2D<T> point2) where TResult : INumber<TResult>
    {
        TResult dx = TResult.CreateChecked(point1.X) - TResult.CreateChecked(point2.X);
        TResult dy = TResult.CreateChecked(point1.Y) - TResult.CreateChecked(point2.Y);

        return dx * dx + dy * dy;
    }

    /// <summary>
    /// Calculates the Manhattan distance between two points.
    /// </summary>
    public static T GetManhattanDistance(Point2D<T> point1, Point2D<T> point2)
    {
        T dx = AbsDiff(point1.X, point2.X);
        T dy = AbsDiff(point1.Y, point2.Y);

        return dx + dy;

        static T AbsDiff(T a, T b) => a >= b ? a - b : b - a;
    }
}
