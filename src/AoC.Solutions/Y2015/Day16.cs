using KE.AoC.Core.Solution;
using System.Globalization;

namespace KE.AoC.Solutions.Y2015;

[Solution(2015, 16)]
public sealed class Day16 : SolutionBase
{
    private static readonly Dictionary<string, int> Target = new()
    {
        { "children", 3 },
        { "cats", 7 },
        { "samoyeds", 2 },
        { "pomeranians", 3 },
        { "akitas", 0 },
        { "vizslas", 0 },
        { "goldfish", 5 },
        { "trees", 3 },
        { "cars", 2 },
        { "perfumes", 1 },
    };

    /// <summary>
    /// Finds the first valid Aunt Sue based on the provided input.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The ID of the first valid Aunt Sue, or 0 if none is found.</returns>
    public override object PartOne(string input)
    {
        List<Aunt> aunts = ParseAunt(input.AsSpan());

        return FindValidAunt(aunts);
    }

    /// <summary>
    /// Finds the first valid Aunt Sue based on the provided input and specific comparison rules.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The ID of the first valid Aunt Sue, or 0 if none is found.</returns>
    public override object PartTwo(string input)
    {
        Dictionary<string, Func<int, int, bool>> comparers = new()
        {
            { "cats", (aunt, target) => aunt > target },
            { "trees", (aunt, target) => aunt > target },
            { "pomeranians", (aunt, target) => aunt < target },
            { "goldfish", (aunt, target) => aunt < target },
        };

        List<Aunt> aunts = ParseAunt(input.AsSpan());

        return FindValidAunt(aunts, comparers);
    }

    /// <summary>
    /// Finds the first valid Aunt Sue based on the provided list of aunts and optional comparers.
    /// </summary>
    /// <param name="aunts">The list of Aunt Sue objects to check.</param>
    /// <param name="comparers">The optional comparers.</param>
    /// <returns>The ID of the first valid Aunt Sue, or 0 if none is found.</returns>
    private static int FindValidAunt(List<Aunt> aunts, Dictionary<string, Func<int, int, bool>>? comparers = null)
    {
        foreach (Aunt aunt in aunts)
        {
            if (IsAuntValid(aunt, comparers))
            {
                return aunt.Id;
            }
        }

        return 0;
    }

    /// <summary>
    /// Checks if the given Aunt Sue is valid based on the target properties and optional comparers.
    /// </summary>
    /// <param name="aunt">The Aunt Sue to check.</param>
    /// <param name="comparers">The optional comparers.</param>
    /// <returns><c>true</c> if the Aunt Sue is valid; otherwise, <c>false</c>.</returns>
    private static bool IsAuntValid(Aunt aunt, Dictionary<string, Func<int, int, bool>>? comparers)
    {
        foreach (KeyValuePair<string, int> auntProp in aunt.Props)
        {
            Func<int, int, bool> comparer = comparers?.TryGetValue(auntProp.Key, out var cmp) == true
                ? cmp
                : ((aunt, target) => aunt == target);

            if (!Target.TryGetValue(auntProp.Key, out int targetValue) || !comparer(auntProp.Value, targetValue))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Parses the input span into a list of Aunt objects.
    /// </summary>
    /// <param name="span">The input span.</param>
    /// <returns>The list of Aunt objects.</returns>
    private static List<Aunt> ParseAunt(ReadOnlySpan<char> span)
    {
        List<Aunt> aunts = [];

        Span<Range> tokens = stackalloc Range[9];
        foreach (ReadOnlySpan<char> line in span.EnumerateLines())
        {
            if (line.Split(tokens, ' ') != 8)
                continue;

            Aunt aunt = new(int.Parse(line[tokens[1]][..^1], NumberStyles.None, NumberFormatInfo.InvariantInfo));
            aunt.Props.Add(line[tokens[2]][..^1].ToString(), int.Parse(line[tokens[3]][..^1], NumberStyles.None, NumberFormatInfo.InvariantInfo));
            aunt.Props.Add(line[tokens[4]][..^1].ToString(), int.Parse(line[tokens[5]][..^1], NumberStyles.None, NumberFormatInfo.InvariantInfo));
            aunt.Props.Add(line[tokens[6]][..^1].ToString(), int.Parse(line[tokens[7]], NumberStyles.None, NumberFormatInfo.InvariantInfo));

            aunts.Add(aunt);
        }

        return aunts;
    }

    /// <summary>
    /// Represents an Aunt Sue with an ID and a set of properties.
    /// </summary>
    /// <param name="id">The unique identifier of the Aunt Sue.</param>
    private class Aunt(int id)
    {
        /// <summary>
        /// Gets the unique identifier of the Aunt Sue.
        /// </summary>
        public int Id { get; } = id;

        /// <summary>
        /// Gets the properties of the Aunt Sue.
        /// </summary>
        public Dictionary<string, int> Props { get; } = [];
    }
}
