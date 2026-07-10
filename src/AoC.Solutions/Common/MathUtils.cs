using System.Numerics;

namespace KE.AoC.Solutions.Common;

/// <summary>
/// Provides common mathematical utility methods.
/// </summary>
public static class MathUtils
{
    /// <summary>
    /// Calculates the number of digits in the given value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to calculate the number of digits for.</param>
    /// <returns>The number of digits in the value.</returns>
    public static int Digits<T>(T value) where T : IBinaryInteger<T>
    {
        T ten = T.CreateChecked(10);

        int count = 0;
        do
        {
            value /= ten;
            count++;
        }
        while (value != T.Zero);

        return count;
    }
}
