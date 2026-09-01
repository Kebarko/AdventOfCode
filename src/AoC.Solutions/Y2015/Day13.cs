using KE.AoC.Core.Solution;
using KE.AoC.Solutions.Common;

namespace KE.AoC.Solutions.Y2015;

[Solution(2015, 13)]
public sealed class Day13 : SolutionBase
{
    /// <summary>
    /// Calculates the maximum happiness for all possible arrangements of family members based on the input.
    /// </summary>
    /// <param name="input">The input string containing the family information.</param>
    /// <returns>The maximum happiness value.</returns>
    public override object PartOne(string input)
    {
        return CalculateMaxHappiness(input.AsSpan());
    }

    /// <summary>
    /// Calculates the maximum happiness for all possible arrangements of family members based on the input including a neutral member.
    /// </summary>
    /// <param name="input">The input string containing the family information.</param>
    /// <returns>The maximum happiness value.</returns>
    public override object PartTwo(string input)
    {
        return CalculateMaxHappiness(input.AsSpan(), true);
    }

    /// <summary>
    /// Calculates the maximum happiness for all possible arrangements of family members.
    /// </summary>
    /// <param name="span">The input span containing the family information.</param>
    /// <param name="addNeutralMember">Indicates whether to add a neutral member to the family.</param>
    /// <returns>The maximum happiness value.</returns>
    private static int CalculateMaxHappiness(ReadOnlySpan<char> span, bool addNeutralMember = false)
    {
        Family family = ParseFamily(span, addNeutralMember);

        int maxHappiness = 0;
        foreach (string[] arrangement in Combinatorics.GetPermutations(family.Members.Keys.ToList()))
        {
            int happiness = CalculateHappiness(family, arrangement);
            if (happiness > maxHappiness)
                maxHappiness = happiness;
        }

        return maxHappiness;
    }

    /// <summary>
    /// Calculates the total happiness for a given arrangement of family members.
    /// </summary>
    /// <param name="family">The family object containing member information and happiness values.</param>
    /// <param name="arrangement">The arrangement of family members.</param>
    /// <returns>The total happiness for the given arrangement.</returns>
    private static int CalculateHappiness(Family family, string[] arrangement)
    {
        int result = 0;
        for (int i = 0; i < arrangement.Length; i++)
        {
            int member1Idx = family.Members[arrangement[i]];
            int member2Idx = family.Members[arrangement[(i + 1) % arrangement.Length]];

            result += family.Happiness[member1Idx, member2Idx];
            result += family.Happiness[member2Idx, member1Idx];
        }

        return result;
    }

    /// <summary>
    /// Parses the input span to create a Family object containing members and their happiness values.
    /// </summary>
    /// <param name="span">The input span containing the family information.</param>
    /// <param name="addNeutralMember">Indicates whether to add a neutral member to the family.</param>
    /// <returns>The parsed Family object.</returns>
    private static Family ParseFamily(ReadOnlySpan<char> span, bool addNeutralMember = false)
    {
        Dictionary<string, int> indexByName = new(StringComparer.Ordinal);
        Dictionary<string, int>.AlternateLookup<ReadOnlySpan<char>> lookup =
            indexByName.GetAlternateLookup<ReadOnlySpan<char>>();

        List<(int From, int To, int Value)> edges = [];

        Span<Range> tokens = stackalloc Range[12];
        foreach (ReadOnlySpan<char> line in span.EnumerateLines())
        {
            if (line.Split(tokens, ' ') != 11)
                continue;

            int value = int.Parse(line[tokens[3]]);
            if (line[tokens[2]].SequenceEqual("lose"))
                value = -value;

            edges.Add((GetIndex(line[tokens[0]]), GetIndex(line[tokens[10]][..^1]), value));
        }

        if (addNeutralMember)
            GetIndex(string.Empty);

        int[,] happiness = new int[indexByName.Count, indexByName.Count];
        foreach ((int from, int to, int value) in edges)
            happiness[from, to] = value;

        return new Family(indexByName, happiness);

        int GetIndex(ReadOnlySpan<char> span)
        {
            if (!lookup.TryGetValue(span, out int index))
            {
                string name = span.ToString();
                index = indexByName.Count;
                indexByName.Add(name, index);
            }

            return index;
        }
    }

    /// <summary>
    /// Represents a family with members and their happiness values towards each other.
    /// </summary>
    /// <param name="Members">The dictionary mapping member names to their indices.</param>
    /// <param name="Happiness">The 2D array representing happiness values between members.</param>
    private record class Family(Dictionary<string, int> Members, int[,] Happiness);
}
