using KE.AoC.Core.Solution;
using System.Numerics;

namespace KE.AoC.Solutions.Y2025;

[Solution(2025, 10)]
public sealed class Day10 : SolutionBase
{
    /// <summary>
    /// Calculates the fewest button presses needed to achieve the target values for all machines described in the input.
    /// </summary>
    public override object PartOne(string input)
    {
        List<Machine> machines = [];
        foreach (ReadOnlySpan<char> span in input.EnumerateLines())
        {
            machines.Add(ParseMachine(span));
        }

        int result = 0;

        foreach (Machine machine in machines)
        {
            if (machine.Target == 0)
                continue;

            var visited = new HashSet<int> { 0 };
            var queue = new Queue<(int State, int Presses)>();
            queue.Enqueue((0, 0));

            bool found = false;
            while (!found && queue.Count > 0)
            {
                (int state, int presses) = queue.Dequeue();

                foreach (int button in machine.Buttons)
                {
                    int next = state ^ button;
                    if (next == machine.Target)
                    {
                        result += presses + 1;
                        found = true;
                        break;
                    }
                    if (visited.Add(next))
                        queue.Enqueue((next, presses + 1));
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Calculates the fewest button presses needed to archive the joltage values for all machines described in the input.
    /// </summary>
    public override object PartTwo(string input)
    {
        List<Machine> machines = [];
        foreach (ReadOnlySpan<char> span in input.EnumerateLines())
        {
            machines.Add(ParseMachine(span));
        }

        long result = 0;

        foreach (Machine machine in machines)
        {
            int buttons = machine.Buttons.Length;
            int joltages = machine.Joltage.Length;

            var subsetContrib = new int[1 << buttons][];
            subsetContrib[0] = new int[joltages];
            for (int s = 1; s < (1 << buttons); s++)
            {
                int low = BitOperations.TrailingZeroCount(s);
                int[] prev = subsetContrib[s & (s - 1)];
                int[] contrib = (int[])prev.Clone();
                int button = machine.Buttons[low];
                for (int j = 0; j < joltages; j++)
                    if ((button & (1 << j)) != 0)
                        contrib[j]++;
                subsetContrib[s] = contrib;
            }

            var memo = new Dictionary<string, long>();

            result += Solve(machine.Joltage);

            long Solve(int[] target)
            {
                if (target.All(v => v == 0))
                    return 0;

                string key = string.Join(',', target);
                if (memo.TryGetValue(key, out long cached))
                    return cached;

                long best = long.MaxValue;

                for (int s = 0; s < (1 << buttons); s++)
                {
                    int[] c = subsetContrib[s];

                    bool ok = true;
                    for (int j = 0; j < joltages; j++)
                    {
                        if ((c[j] & 1) != (target[j] & 1) || c[j] > target[j])
                        {
                            ok = false;
                            break;
                        }
                    }
                    if (!ok)
                        continue;

                    int[] half = new int[joltages];
                    for (int j = 0; j < joltages; j++)
                        half[j] = (target[j] - c[j]) >> 1;

                    long sub = Solve(half);
                    if (sub != long.MaxValue)
                        best = Math.Min(best, BitOperations.PopCount((uint)s) + 2 * sub);
                }

                memo[key] = best;
                return best;
            }
        }

        return result;
    }

    /// <summary>
    /// Parses a ReadOnlySpan of characters representing a machine configuration into a Machine object.
    /// </summary>
    private static Machine ParseMachine(ReadOnlySpan<char> span)
    {
        int target = 0;
        int[] buttons = new int[span.Count('(')];
        int[] joltage = [];

        int b = 0;
        foreach (Range range in span.Split(' '))
        {
            ReadOnlySpan<char> token = span[range];
            ReadOnlySpan<char> body = token[1..^1];

            switch (token[0])
            {
                case '[': target = ParseTarget(body); break;
                case '(': buttons[b++] = ParseButton(body); break;
                case '{': joltage = ParseJoltage(body); break;
            }
        }

        return new Machine(target, buttons.ToArray(), joltage);
    }

    /// <summary>
    /// Parses a ReadOnlySpan of characters representing a target value with '#' characters into an integer mask.
    /// </summary>
    private static int ParseTarget(ReadOnlySpan<char> span)
    {
        int mask = 0;

        for (int i = 0; i < span.Length; i++)
            if (span[i] == '#')
                mask |= 1 << i;

        return mask;
    }

    /// <summary>
    /// Parses a ReadOnlySpan of characters representing button values separated by commas into an integer mask.
    /// </summary>
    private static int ParseButton(ReadOnlySpan<char> span)
    {
        int mask = 0;

        foreach (Range range in span.Split(','))
            mask |= 1 << int.Parse(span[range]);

        return mask;
    }

    /// <summary>
    /// Parses a ReadOnlySpan of characters representing joltage values separated by commas into an array of integers.
    /// </summary>
    private static int[] ParseJoltage(ReadOnlySpan<char> span)
    {
        int[] values = new int[span.Count(',') + 1];

        int i = 0;
        foreach (Range range in span.Split(','))
        {
            values[i++] = int.Parse(span[range]);
        }

        return values;
    }

    /// <summary>
    /// Represents a machine with a target value, a list of button values, and an array of joltage values.
    /// </summary>
    private sealed record Machine(int Target, int[] Buttons, int[] Joltage) { }
}
