using KE.AoC.Core.Solution;

namespace KE.AoC.Solutions.Y2025;

[Solution(2025, 6)]
public sealed class Day06 : SolutionBase
{
    /// <summary>
    /// Calculates the sum of the results of applying the specified operators to groups of values extracted from the input.
    /// </summary>
    public override object PartOne(string input)
    {
        string[] lines = Lines(input);

        var groups = new List<Group>();

        foreach (string line in lines[0..^1])
        {
            int[] values = Ints(line);
            for (int j = 0; j < values.Length; j++)
            {
                if (groups.Count <= j)
                    groups.Add(new Group());

                groups[j].Values.Add(values[j]);
            }
        }

        char[] operators = Chars(lines[^1]);
        for (int i = 0; i < groups.Count; i++)
        {
            groups[i].Operator = operators[i];
        }

        return groups.Sum(group => group.Calculate());
    }

    /// <summary>
    /// Calculates the sum of the results of applying the specified operators to groups of values extracted from the input.
    /// </summary>
    public override object PartTwo(string input)
    {
        string[] lines = Lines(input, StringSplitOptions.RemoveEmptyEntries);

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
