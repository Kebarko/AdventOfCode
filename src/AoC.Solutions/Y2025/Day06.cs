using KE.AoC.Core.Solution;
using System.Globalization;
using System.Text;

namespace KE.AoC.Solutions.Y2025;

[Solution(2025, 6)]
public sealed class Day06 : SolutionBase
{
    /// <summary>
    /// Calculates the sum of the results of applying the specified operators to groups of values extracted from the input.
    /// </summary>
    public override object PartOne(string input)
    {
        var groups = new List<Group>();

        SpanLineEnumerator enumerator = input.EnumerateLines();
        if (enumerator.MoveNext())
        {
            int i;
            ReadOnlySpan<char> line = enumerator.Current;
            while (enumerator.MoveNext())
            {
                i = 0;
                foreach (Range range in line.SplitAny(ReadOnlySpan<char>.Empty))
                {
                    if (line[range].IsEmpty)
                        continue;

                    int value = int.Parse(line[range], NumberStyles.None, NumberFormatInfo.InvariantInfo);

                    if (groups.Count <= i)
                        groups.Add(new Group());

                    groups[i].Values.Add(value);

                    i++;
                }

                line = enumerator.Current;
            }

            i = 0;
            foreach (Range range in line.SplitAny(ReadOnlySpan<char>.Empty))
            {
                if (line[range].IsEmpty)
                    continue;

                groups[i].Operator = line[range][0];

                i++;
            }
        }

        return groups.Sum(group => group.Calculate());
    }

    /// <summary>
    /// Calculates the sum of the results of applying the specified operators to groups of values extracted from the input.
    /// </summary>
    public override object PartTwo(string input)
    {
        string[] lines = input.Split("\r\n");

        var groups = new List<Group>();
        var group = new Group();

        for (int col = lines.Max(l => l.Length) - 1; col >= 0; col--)
        {
            int value = 0;
            char op = '\0';
            for (int row = 0; row < lines.Length; row++)
            {
                char ch = lines[row][col];
                if (ch == '+' || ch == '*')
                {
                    op = ch;
                }
                else if (char.IsDigit(ch))
                {
                    value *= 10;
                    value += ch - '0';
                }
            }

            if (value != 0)
            {
                group.Values.Add(value);
            }

            if (op != '\0')
            {
                group.Operator = op;
                groups.Add(group);
                group = new();
            }
        }

        return groups.Sum(group => group.Calculate());
    }

    /// <summary>
    /// Represents a group of values and an operator to perform calculations on those values.
    /// </summary>
    private class Group
    {
        /// <summary>
        /// Gets the list of integer values associated with this group.
        /// </summary>
        public List<int> Values { get; } = [];

        /// <summary>
        /// Gets or sets the operator ('+' or '*') used for calculations in this group.
        /// </summary>
        public char Operator { get; set; }

        /// <summary>
        /// Calculates the result of applying the operator to the values in this group.
        /// </summary>
        public long Calculate()
        {
            long result = 0;

            switch (Operator)
            {
                case '+':
                    result += Values.Sum();
                    break;
                case '*':
                    result += Values.Aggregate(1L, (x, y) => x * y);
                    break;
            }

            return result;
        }
    }
}
