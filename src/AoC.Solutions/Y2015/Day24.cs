using KE.AoC.Core.Solution;
using System.Globalization;

namespace KE.AoC.Solutions.Y2015;

/// <summary>
/// --- Day 24: It Hangs in the Balance ---
/// </summary>
[Solution(2015, 24)]
public sealed class Day24 : SolutionBase
{
    /// <summary>
    /// Calculates the minimum quantum entanglement (QE) for dividing the given weights
    /// into three groups with equal weight.
    /// The QE is defined as the product of the weights in the first group.
    /// </summary>
    /// <param name="input">The input string containing the weights.</param>
    /// <returns>The minimum quantum entanglement (QE).</returns>
    public override object PartOne(string input)
    {
        List<int> weights = ParseWeights(input.AsSpan());

        return CalculateMinQE(weights, 3);
    }

    /// <summary>
    /// Calculates the minimum quantum entanglement (QE) for dividing the given weights
    /// into four groups with equal weight.
    /// The QE is defined as the product of the weights in the first group.
    /// </summary>
    /// <param name="input">The input string containing the weights.</param>
    /// <returns>The minimum quantum entanglement (QE).</returns>
    public override object PartTwo(string input)
    {
        List<int> weights = ParseWeights(input.AsSpan());

        return CalculateMinQE(weights, 4);
    }

    /// <summary>
    /// Calculates the minimum quantum entanglement (QE) for dividing the given weights
    /// into the specified number of groups with equal weight.
    /// The QE is defined as the product of the weights in the first group.
    /// </summary>
    /// <param name="weights">The list of weights to divide.</param>
    /// <param name="groups">The number of groups to divide the weights into.</param>
    /// <returns>The minimum quantum entanglement (QE).</returns>
    private static long CalculateMinQE(List<int> weights, int groups)
    {
        int groupWeight = weights.Sum() / groups;

        return GetSmallestGroups(weights, groupWeight)
            .Min(g => g.Aggregate(1L, (x, y) => x * y));
    }

    /// <summary>
    /// Generates all combinations of the smallest size from the provided list of items
    /// that sum up to the specified target sum.
    /// </summary>
    /// <param name="items">The list of items to combine.</param>
    /// <param name="targetSum">The target sum to achieve.</param>
    /// <returns>An enumerable of lists containing the valid combinations.</returns>
    private static IEnumerable<List<int>> GetSmallestGroups(List<int> items, int targetSum)
    {
        List<int> sorted = items.OrderBy(x => x).ToList();

        for (int size = 1; size <= sorted.Count; size++)
        {
            bool found = false;

            foreach (List<int> group in Combine(sorted, 0, size, targetSum, []))
            {
                yield return group;
                found = true;
            }

            if (found)
                yield break;
        }

        static IEnumerable<List<int>> Combine(List<int> items, int index, int size, int remaining, List<int> current)
        {
            if (current.Count == size)
            {
                if (remaining == 0)
                    yield return current;
                yield break;
            }

            for (int i = index; i < items.Count; i++)
            {
                int value = items[i];
                if (value > remaining)
                    break;

                current.Add(value);
                foreach (List<int> result in Combine(items, i + 1, size, remaining - value, current))
                    yield return result;
                current.RemoveAt(current.Count - 1);
            }
        }
    }

    /// <summary>
    /// Parses the input span into a list of integer weights,
    /// where each line in the span represents a weight.
    /// </summary>
    /// <param name="span">The input span containing the weights.</param>
    /// <returns>A list of integer weights.</returns>
    private static List<int> ParseWeights(ReadOnlySpan<char> span)
    {
        List<int> weights = [];
        foreach (ReadOnlySpan<char> line in span.EnumerateLines())
        {
            weights.Add(int.Parse(line, NumberStyles.None, NumberFormatInfo.InvariantInfo));
        }

        return weights;
    }
}
