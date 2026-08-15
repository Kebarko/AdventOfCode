namespace KE.AoC.Solutions.Common;

/// <summary>
/// Provides methods for generating combinations of items from a collection.
/// </summary>
public static class Combinatorics
{
    /// <summary>
    /// Generates all combinations of a specified length from the given collection of items.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="items">The collection of items to generate combinations from.</param>
    /// <param name="length">The length of each combination.</param>
    /// <returns>An enumerable of combinations, each represented as an array of items.</returns>
    public static IEnumerable<T[]> GetCombinations<T>(IList<T> items, int length)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        return Iterate(items, length);

        static IEnumerable<T[]> Iterate(IList<T> items, int length)
        {
            if (items.Count == 0 || items.Count < length)
                yield break;

            if (length == 0)
            {
                yield return [];
                yield break;
            }

            var indices = new int[length];
            for (int i = 0; i < length; i++)
                indices[i] = i;

            while (true)
            {
                var combination = new T[length];
                for (int i = 0; i < length; i++)
                    combination[i] = items[indices[i]];
                yield return combination;

                int pos = length - 1;
                while (pos >= 0 && indices[pos] == items.Count - length + pos)
                    pos--;

                if (pos < 0)
                    yield break;

                indices[pos]++;
                for (int i = pos + 1; i < length; i++)
                    indices[i] = indices[i - 1] + 1;
            }
        }
    }

    /// <summary>
    /// Generates all combinations of a specified length from the given collection of items,
    /// allowing for repetition of items in the combinations.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="items">The collection of items to generate combinations from.</param>
    /// <param name="length">The length of each combination.</param>
    /// <returns>An enumerable of combinations, each represented as an array of items.</returns>
    public static IEnumerable<T[]> GetCombinationsWithRepetition<T>(IList<T> items, int length)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        return Iterate(items, length);

        static IEnumerable<T[]> Iterate(IList<T> items, int length)
        {
            if (items.Count == 0)
                yield break;

            if (length == 0)
            {
                yield return [];
                yield break;
            }

            var indices = new int[length];

            while (true)
            {
                var combination = new T[length];
                for (int i = 0; i < length; i++)
                    combination[i] = items[indices[i]];
                yield return combination;

                int pos = length - 1;
                while (pos >= 0 && indices[pos] == items.Count - 1)
                    pos--;

                if (pos < 0)
                    yield break;

                indices[pos]++;
                for (int i = pos + 1; i < length; i++)
                    indices[i] = indices[pos];
            }
        }
    }
}
