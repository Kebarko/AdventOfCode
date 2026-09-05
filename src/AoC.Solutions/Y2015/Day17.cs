using KE.AoC.Core.Solution;

namespace KE.AoC.Solutions.Y2015;

[Solution(2015, 17)]
public sealed class Day17 : SolutionBase
{
    private const int TargetVolume = 150;

    /// <summary>
    /// Counts the number of combinations of containers that can fill the target volume.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The number of combinations.</returns>
    public override object PartOne(string input)
    {
        List<int> containers = ParseContainers(input.AsSpan());

        Span<int> countsByUsed = stackalloc int[containers.Count + 1];
        CountCombinations(containers, 0, TargetVolume, 0, countsByUsed);

        int result = 0;
        foreach (int count in countsByUsed)
            result += count;

        return result;
    }

    /// <summary>
    /// Counts the number of combinations of containers that can fill the target volume using the minimum number of containers.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The number of combinations.</returns>
    public override object PartTwo(string input)
    {
        List<int> containers = ParseContainers(input.AsSpan());

        Span<int> countsByUsed = stackalloc int[containers.Count + 1];
        CountCombinations(containers, 0, TargetVolume, 0, countsByUsed);

        for (int used = 1; used < countsByUsed.Length; used++)
            if (countsByUsed[used] > 0)
                return countsByUsed[used];

        return 0;
    }

    /// <summary>
    /// Counts the combinations of containers that can fill the target volume.
    /// </summary>
    /// <param name="containers">The list of container sizes.</param>
    /// <param name="startIndex">The index to start considering containers from.</param>
    /// <param name="remainingVolume">The remaining volume to fill.</param>
    /// <param name="usedCount">The number of containers used.</param>
    /// <param name="countsByUsed">The array to store the counts of combinations by the number of used containers.</param>
    private static void CountCombinations(List<int> containers, int startIndex, int remainingVolume, int usedCount, Span<int> countsByUsed)
    {
        if (remainingVolume == 0)
        {
            countsByUsed[usedCount]++;
            return;
        }

        for (int i = startIndex; i < containers.Count; i++)
        {
            int volume = containers[i];
            if (volume <= remainingVolume)
            {
                CountCombinations(containers, i + 1, remainingVolume - volume, usedCount + 1, countsByUsed);
            }
        }
    }

    /// <summary>
    /// Parses the input span into a list of container sizes.
    /// </summary>
    /// <param name="span">The input span.</param>
    /// <returns>The list of container sizes.</returns>
    private static List<int> ParseContainers(ReadOnlySpan<char> span)
    {
        List<int> containers = [];

        foreach (ReadOnlySpan<char> line in span.EnumerateLines())
        {
            containers.Add(int.Parse(line));
        }

        return containers;
    }
}
