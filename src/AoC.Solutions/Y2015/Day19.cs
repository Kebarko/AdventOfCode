using KE.AoC.Core.Solution;

namespace KE.AoC.Solutions.Y2015;

[Solution(2015, 19)]
public sealed class Day19 : SolutionBase
{
    /// <summary>
    /// Calculates the number of distinct molecules that can be created by applying one replacement to the target molecule.
    /// </summary>
    /// <param name="input">The input string containing the replacements and the target molecule.</param>
    /// <returns>The number of distinct molecules that can be created.</returns>
    public override object PartOne(string input)
    {
        (List<(string, string)> replacements, string molecule) = ParseInput(input.AsSpan());

        ReadOnlySpan<char> span = molecule.AsSpan();
        HashSet<string> molecules = [];

        foreach ((string from, string to) in replacements)
        {
            int start = 0;
            int index;
            while ((index = span[start..].IndexOf(from, StringComparison.Ordinal)) >= 0)
            {
                int at = start + index;
                molecules.Add(string.Concat(span[..at], to, span[(at + from.Length)..]));

                start = at + 1;
            }
        }

        return molecules.Count;
    }

    /// <summary>
    /// Calculates the minimum number of steps required to transform the target molecule into the molecule "e" using the given replacements.
    /// </summary>
    /// <param name="input">The input string containing the replacements and the target molecule.</param>
    /// <returns>The minimum number of steps required.</returns>
    public override object PartTwo(string input)
    {
        (List<(string From, string To)> replacements, string molecule) = ParseInput(input.AsSpan());

        // Sort the replacements in descending order of the length of the "To" string to prioritize longer replacements first.
        // This is necessary for this specific input to ensure that we reduce the molecule in the fewest steps possible.
        (string From, string To)[] reverse = [.. replacements.OrderByDescending(r => r.To.Length)];

        string current = molecule;
        int steps = 0;

        while (current != "e")
        {
            foreach ((string from, string to) in reverse)
            {
                int index = current.IndexOf(to, StringComparison.Ordinal);
                if (index < 0)
                    continue;

                if (from == "e" && to.Length != current.Length)
                    continue;

                ReadOnlySpan<char> span = current.AsSpan();
                current = string.Concat(span[..index], from, span[(index + to.Length)..]);
                steps++;
                break;
            }
        }

        return steps;
    }

    /// <summary>
    /// Parses the input into a list of replacements and the target molecule.
    /// </summary>
    /// <param name="span">The input span containing the replacements and the target molecule.</param>
    /// <returns>A tuple containing the list of replacements and the target molecule.</returns>
    private static (List<(string, string)>, string) ParseInput(ReadOnlySpan<char> span)
    {
        List<(string, string)> replacements = [];
        string molecule = string.Empty;

        foreach (ReadOnlySpan<char> line in span.EnumerateLines())
        {
            if (line.IsEmpty)
                continue;

            int arrow = line.IndexOf(" => ", StringComparison.Ordinal);
            if (arrow < 0)
                molecule = line.ToString();
            else
                replacements.Add((line[..arrow].ToString(), line[(arrow + 4)..].ToString()));
        }

        return (replacements, molecule);
    }
}
