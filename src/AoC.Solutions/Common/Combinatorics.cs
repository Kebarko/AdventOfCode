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

    /// <summary>
    /// Generates all variations of a specified length from the given collection of items.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="items">The collection of items to generate variations from.</param>
    /// <param name="length">The length of each variation.</param>
    /// <returns>An enumerable of variations, each represented as an array of items.</returns>
    public static IEnumerable<T[]> GetVariations<T>(IList<T> items, int length)
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
            var used = new bool[items.Count];
            for (int i = 0; i < length; i++)
                indices[i] = -1;

            var level = 0;
            while (level >= 0)
            {
                if (level == length)
                {
                    var result = new T[length];
                    for (var i = 0; i < length; i++)
                        result[i] = items[indices[i]];
                    yield return result;

                    level--;
                    continue;
                }

                if (indices[level] >= 0)
                    used[indices[level]] = false;

                indices[level]++;
                while (indices[level] < items.Count && used[indices[level]])
                    indices[level]++;

                if (indices[level] < items.Count)
                {
                    used[indices[level]] = true;
                    level++;
                    if (level < length)
                        indices[level] = -1;
                }
                else
                {
                    indices[level] = -1;
                    level--;
                }
            }
        }
    }

    /// <summary>
    /// Generates all variations of a specified length from the given collection of items,
    /// allowing for repetition of items in the variations.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="items">The collection of items to generate variations from.</param>
    /// <param name="length">The length of each variation.</param>
    /// <returns>An enumerable of variations, each represented as an array of items.</returns>
    public static IEnumerable<T[]> GetVariationsWithRepetition<T>(IList<T> items, int length)
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
                var variation = new T[length];
                for (int i = 0; i < length; i++)
                    variation[i] = items[indices[i]];
                yield return variation;

                int pos = length - 1;
                while (pos >= 0 && indices[pos] == items.Count - 1)
                {
                    indices[pos] = 0;
                    pos--;
                }

                if (pos < 0)
                    yield break;

                indices[pos]++;
            }
        }
    }

    /// <summary>
    /// Generates all permutations of the given collection of items.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="items">The collection of items to generate permutations from.</param>
    /// <returns>An enumerable of permutations, each represented as an array of items.</returns>
    public static IEnumerable<T[]> GetPermutations<T>(IList<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        return GetVariations(items, items.Count);
    }
}
