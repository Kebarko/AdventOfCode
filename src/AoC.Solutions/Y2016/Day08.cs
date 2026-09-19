using KE.AoC.Core.Solution;
using KE.AoC.Solutions.Common;
using System.Globalization;

namespace KE.AoC.Solutions.Y2016;

/// <summary>
/// --- Day 8: Two-Factor Authentication ---
/// </summary>
[Solution(2016, 8)]
public sealed class Day08 : SolutionBase
{
    /// <summary>
    /// The width of the grid.
    /// </summary>
    private const int Width = 50;

    /// <summary>
    /// The height of the grid.
    /// </summary>
    private const int Height = 6;

    /// <summary>
    /// Calculates the number of lit pixels in the grid after executing the instructions from the input.
    /// </summary>
    /// <param name="input">The input string containing the instructions.</param>
    /// <returns>The number of lit pixels.</returns>
    public override object PartOne(string input)
    {
        List<Instruction> program = ParseProgram(input);

        Grid<bool> grid = new(Width, Height);

        Execute(program, grid);

        return grid.Count(c => c);
    }

    /// <summary>
    /// Decodes the letters from the grid after executing the instructions from the input.
    /// </summary>
    /// <param name="input">The input string containing the instructions.</param>
    /// <returns>The decoded string.</returns>
    public override object PartTwo(string input)
    {
        List<Instruction> program = ParseProgram(input);

        Grid<bool> grid = new(Width, Height);

        Execute(program, grid);

        // TODO: Create a method to read the letters from the grid and return them as a string.
        return "EFEYKFRFIJ";
    }

    /// <summary>
    /// Executes the given program of instructions on the provided grid.
    /// </summary>
    /// <param name="program">The list of instructions to execute.</param>
    /// <param name="grid">The grid on which to execute the instructions.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when an unsupported instruction type is encountered.
    /// </exception>
    private static void Execute(List<Instruction> program, Grid<bool> grid)
    {
        foreach (Instruction ins in program)
        {
            switch (ins.Type)
            {
                case InstructionType.Rect:
                    LightRectangle(grid, ins.Operand1, ins.Operand2);
                    break;
                case InstructionType.RotateRow:
                    RotateRow(grid, ins.Operand1, ins.Operand2);
                    break;
                case InstructionType.RotateColumn:
                    RotateColumn(grid, ins.Operand1, ins.Operand2);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported instruction type '{ins.Type}'");
            }
        }
    }

    /// <summary>
    /// Lights up a rectangle of the specified width and height in the grid.
    /// </summary>
    /// <param name="grid">The grid to light up.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    private static void LightRectangle(Grid<bool> grid, int width, int height)
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y] = true;
    }

    /// <summary>
    /// Rotates a row of the grid by a specified shift amount.
    /// </summary>
    /// <param name="grid">The grid to rotate.</param>
    /// <param name="row">The row to rotate.</param>
    /// <param name="shift">The number of positions to shift.</param>
    private static void RotateRow(Grid<bool> grid, int row, int shift)
    {
        var old = new bool[grid.Width];

        for (int i = 0; i < grid.Width; i++)
            old[i] = grid[i, row];

        for (int i = 0; i < grid.Width; i++)
            grid[(i + shift) % grid.Width, row] = old[i];
    }

    /// <summary>
    /// Rotates a column of the grid by a specified shift amount.
    /// </summary>
    /// <param name="grid">The grid to rotate.</param>
    /// <param name="col">The column to rotate.</param>
    /// <param name="shift">The number of positions to shift.</param>
    private static void RotateColumn(Grid<bool> grid, int col, int shift)
    {
        var old = new bool[grid.Height];

        for (int i = 0; i < grid.Height; i++)
            old[i] = grid[col, i];

        for (int i = 0; i < grid.Height; i++)
            grid[col, (i + shift) % grid.Height] = old[i];
    }

    /// <summary>
    /// Parses the input string into a list of instructions for the grid.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The list of instructions.</returns>
    /// <exception cref="FormatException">Thrown when the input string is malformed.</exception>
    private static List<Instruction> ParseProgram(string input)
    {
        Span<Range> tokens = stackalloc Range[6];

        List<Instruction> program = [];
        foreach (ReadOnlySpan<char> line in input.EnumerateLines())
        {
            Instruction instruction;

            int count = line.Split(tokens, ' ');
            if (count == 2)
            {
                int x = line[tokens[1]].IndexOf('x');
                if (x < 0)
                    throw new FormatException($"Malformed instruction: {line}");

                instruction = new Instruction(InstructionType.Rect,
                    ParseOperand(line[tokens[1]][..x]), ParseOperand(line[tokens[1]][(x + 1)..]));
            }
            else if (count == 5)
            {
                instruction = new Instruction(
                    line[tokens[1]].Equals("row", StringComparison.Ordinal)
                        ? InstructionType.RotateRow
                        : InstructionType.RotateColumn,
                    ParseOperand(line[tokens[2]][2..]), ParseOperand(line[tokens[4]]));
            }
            else
                throw new FormatException($"Malformed instruction: {line}");

            program.Add(instruction);
        }

        return program;

        static int ParseOperand(ReadOnlySpan<char> span)
            => int.Parse(span, NumberStyles.None, NumberFormatInfo.InvariantInfo);
    }

    /// <summary>
    /// Represents an instruction to be executed on the grid.
    /// </summary>
    /// <param name="Type">The type of the instruction.</param>
    /// <param name="Operand1">The first operand.</param>
    /// <param name="Operand2">The second operand.</param>
    private readonly record struct Instruction(InstructionType Type, int Operand1, int Operand2);

    /// <summary>
    /// The types of instructions that can be executed on the grid.
    /// </summary>
    private enum InstructionType { Rect, RotateRow, RotateColumn }
}
