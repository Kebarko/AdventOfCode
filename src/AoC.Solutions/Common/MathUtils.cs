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

    /// <summary>
    /// Calculates the divisors of the given value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to calculate the divisors for.</param>
    /// <returns>A list of divisors of the value.</returns>
    public static List<T> Divisors<T>(T value) where T : IBinaryInteger<T>
    {
        List<T> divisors = [];

        if (value == T.Zero)
            return divisors;

        value = T.Abs(value);

        for (T i = T.One; i <= value / i; i++)
        {
            if (value % i == T.Zero)
            {
                divisors.Add(i);

                T paired = value / i;
                if (i != paired)
                    divisors.Add(paired);
            }
        }

        divisors.Sort();

        return divisors;
    }
}
