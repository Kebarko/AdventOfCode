using KE.AoC.Core.Solution;
using KE.AoC.Solutions.Common;

namespace KE.AoC.Solutions.to.Y015;

[Solution(2015, 6)]
public sealed class Day06 : SolutionBase
{
    /// <summary>
    /// The size of the grid of lights (1000x1000).
    /// </summary>
    private const int Size = 1000;

    /// <summary>
    /// Calculates the number of lights that are on after executing the instructions.
    /// </summary>
    /// <param name="input">The input string containing the instructions.</param>
    /// <returns>The number of lights that are on.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an invalid action is encountered.</exception>
    public override object PartOne(string input)
    {
        List<Instruction> instructions = ParseInstructions(input);

        bool[] grid = new bool[Size * Size];

        foreach (Instruction ins in instructions)
        {
            switch (ins.Action)
            {
                case LightAction.TurnOn:
                    Set(grid, ins.From, ins.To, true);
                    break;
                case LightAction.TurnOff:
                    Set(grid, ins.From, ins.To, false);
                    break;
                case LightAction.Toggle:
                    Toggle(grid, ins.From, ins.To);
                    break;
                default:
                    throw new InvalidOperationException($"Action {ins.Action} is not allowed!");
            }
        }

        return grid.Count(x => x);
    }

    /// <summary>
    /// Calculates the total brightness of all lights after executing the instructions.
    /// </summary>
    /// <param name="input">The input string containing the instructions.</param>
    /// <returns>The total brightness of all lights.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an invalid action is encountered.</exception>
    public override object PartTwo(string input)
    {
        List<Instruction> instructions = ParseInstructions(input);

        int[] grid = new int[Size * Size];

        foreach (Instruction ins in instructions)
        {
            switch (ins.Action)
            {
                case LightAction.TurnOn:
                    Adjust(grid, ins.From, ins.To, x => x + 1);
                    break;
                case LightAction.TurnOff:
                    Adjust(grid, ins.From, ins.To, x => Math.Max(0, x - 1));
                    break;
                case LightAction.Toggle:
                    Adjust(grid, ins.From, ins.To, x => x + 2);
                    break;
                default:
                    throw new InvalidOperationException($"Action {ins.Action} is not allowed!");
            }
        }

        return grid.Sum(c => c);
    }

    /// <summary>
    /// Sets the lights in the specified rectangular area of the grid to the given value (on or off).
    /// </summary>
    /// <param name="grid">The grid of lights.</param>
    /// <param name="from">The starting point of the rectangular area.</param>
    /// <param name="to">The ending point of the rectangular area.</param>
    /// <param name="value">The value to set the lights to.</param>
    private static void Set(bool[] grid, Point2D<int> from, Point2D<int> to, bool value)
    {
        for (int y = from.Y; y <= to.Y; y++)
        {
            Span<bool> row = grid.AsSpan(y * Size + from.X, to.X - from.X + 1);
            row.Fill(value);
        }
    }

    /// <summary>
    /// Toggles the lights in the specified rectangular area of the grid (turns on lights that are off and turns off lights that are on).
    /// </summary>
    /// <param name="grid">The grid of lights.</param>
    /// <param name="from">The starting point of the rectangular area.</param>
    /// <param name="to">The ending point of the rectangular area.</param>
    private static void Toggle(bool[] grid, Point2D<int> from, Point2D<int> to)
    {
        for (int y = from.Y; y <= to.Y; y++)
        {
            Span<bool> row = grid.AsSpan(y * Size + from.X, to.X - from.X + 1);
            for (int i = 0; i < row.Length; i++)
            {
                row[i] = !row[i];
            }
        }
    }

    /// <summary>
    /// Adjusts the brightness of the lights in the specified rectangular area of the grid using the provided function.
    /// </summary>
    /// <param name="grid">The grid of lights.</param>
    /// <param name="from">The starting point of the rectangular area.</param>
    /// <param name="to">The ending point of the rectangular area.</param>
    /// <param name="fn">The function to apply to each light's brightness.</param>
    private static void Adjust(int[] grid, Point2D<int> from, Point2D<int> to, Func<int, int> fn)
    {
        for (int y = from.Y; y <= to.Y; y++)
        {
            Span<int> row = grid.AsSpan(y * Size + from.X, to.X - from.X + 1);
            for (int i = 0; i < row.Length; i++)
            {
                row[i] = fn(row[i]);
            }
        }
    }

    /// <summary>
    /// Parses the input string into a list of Instruction records, each containing an action and the coordinates of the rectangular area to which the action applies.  
    /// </summary>
    /// <param name="input">The input string containing the instructions.</param>
    /// <returns>The list of parsed instructions.</returns>
    private static List<Instruction> ParseInstructions(string input)
    {
        List<Instruction> instructions = [];
        foreach (ReadOnlySpan<char> line in input.EnumerateLines())
        {
            instructions.Add(ParseInstruction(line));
        }

        return instructions;
    }

    /// <summary>
    /// Parses a single instruction from a string and returns an Instruction record containing the action and the coordinates of the rectangular area.
    /// </summary>
    /// <param name="span">The string containing the instruction.</param>
    /// <returns>The parsed instruction.</returns>
    /// <exception cref="FormatException">Thrown when the string is not in the correct format.</exception>
    private static Instruction ParseInstruction(ReadOnlySpan<char> span)
    {
        LightAction action;
        if (span.StartsWith("turn on "))
        {
            action = LightAction.TurnOn;
            span = span[8..];
        }
        else if (span.StartsWith("turn off "))
        {
            action = LightAction.TurnOff;
            span = span[9..];
        }
        else if (span.StartsWith("toggle "))
        {
            action = LightAction.Toggle;
            span = span[7..];
        }
        else
        {
            throw new FormatException($"Unrecognized instruction: {span}");
        }

        int separator = span.IndexOf(" through ");

        if (separator < 0)
            throw new FormatException($"Missing ' through ': {span}");

        Point2D<int> from = ParsePoint(span[..separator]);
        Point2D<int> to = ParsePoint(span[(separator + 9)..]);

        return new Instruction(action, from, to);
    }

    /// <summary>
    /// Parses a point from a string in the format "x,y" into a Point2D<int> structure.
    /// </summary>
    /// <param name="span">The string containing the point coordinates.</param>
    /// <returns>The parsed point.</returns>
    /// <exception cref="FormatException">Thrown when the string is not in the correct format.</exception>
    private static Point2D<int> ParsePoint(ReadOnlySpan<char> span)
    {
        int comma = span.IndexOf(',');

        if (comma < 0)
            throw new FormatException($"Malformed point: {span}");

        return new Point2D<int>(int.Parse(span[..comma]), int.Parse(span[(comma + 1)..]));
    }

    /// <summary>
    /// Represents the possible actions that can be performed on the lights in the grid.
    /// </summary>
    private enum LightAction { TurnOn, TurnOff, Toggle }

    /// <summary>
    /// Represents an instruction to perform a light action on a rectangular area of the grid.
    /// </summary>
    /// <param name="Action">The action to perform.</param>
    /// <param name="From">The starting point of the rectangular area.</param>
    /// <param name="To">The ending point of the rectangular area.</param>
    private readonly record struct Instruction(LightAction Action, Point2D<int> From, Point2D<int> To);
}
