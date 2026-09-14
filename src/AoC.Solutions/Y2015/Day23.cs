using KE.AoC.Core.Solution;
using System.Globalization;

namespace KE.AoC.Solutions.Y2015;

/// <summary>
/// --- Day 23: Opening the Turing Lock ---
/// </summary>
[Solution(2015, 23)]
public sealed class Day23 : SolutionBase
{
    /// <summary>
    /// Executes the program defined in the input string with an initial value of 0 for register A,
    /// returning the final value of register B after execution.
    /// </summary>
    /// <param name="input">The input string containing the program.</param>
    /// <returns>The final value of register B after execution.</returns>
    public override object PartOne(string input)
    {
        return Run(input, 0);
    }

    /// <summary>
    /// Executes the program defined in the input string with an initial value of 1 for register A,
    /// returning the final value of register B after execution.
    /// </summary>
    /// <param name="input">The input string containing the program.</param>
    /// <returns>The final value of register B after execution.</returns>
    public override object PartTwo(string input)
    {
        return Run(input, 1);
    }

    /// <summary>
    /// Runs the program defined in the input string with the specified initial value for register A,
    /// returning the final value of register B after execution.
    /// </summary>
    /// <param name="input">The input string containing the program.</param>
    /// <param name="initA">The initial value for register A.</param>
    /// <returns>The final value of register B after execution.</returns>
    private static long Run(string input, long initA)
    {
        Instruction[] program = ParseProgram(input);

        Span<long> registers = stackalloc long[2];
        registers[0] = initA;

        Execute(program, registers);

        return registers[1];
    }

    /// <summary>
    /// Executes the provided program on the given registers, modifying the registers in place
    /// according to the instructions in the program.
    /// The execution continues until the instruction pointer goes out of bounds of the program array.
    /// </summary>
    /// <param name="program">The program to execute.</param>
    /// <param name="registers">The registers to use during execution.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when an invalid instruction is encountered.
    /// </exception>
    private static void Execute(ReadOnlySpan<Instruction> program, Span<long> registers)
    {
        int i = 0;
        while ((uint)i < (uint)program.Length)
        {
            Instruction ins = program[i];

            switch (ins.Type)
            {
                case InstructionType.Hlf:
                    registers[ins.Register] /= 2;
                    i++;
                    break;
                case InstructionType.Tpl:
                    registers[ins.Register] *= 3;
                    i++;
                    break;
                case InstructionType.Inc:
                    registers[ins.Register]++;
                    i++;
                    break;
                case InstructionType.Jmp:
                    i += ins.Offset;
                    break;
                case InstructionType.Jie:
                    i += long.IsEvenInteger(registers[ins.Register]) ? ins.Offset : 1;
                    break;
                case InstructionType.Jio:
                    i += registers[ins.Register] == 1 ? ins.Offset : 1;
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported instruction type '{ins.Type}'.");
            }
        }
    }

    /// <summary>
    /// Parses the entire program from the input string, splitting it into individual instructions
    /// and converting them into an array of Instruction objects.
    /// </summary>
    /// <param name="input">The input string containing the program.</param>
    /// <returns>An array of Instruction objects representing the parsed program.</returns>
    private static Instruction[] ParseProgram(string input)
    {
        List<Instruction> program = [];
        foreach (ReadOnlySpan<char> line in input.EnumerateLines())
        {
            program.Add(ParseInstruction(line));
        }

        return [.. program];
    }

    /// <summary>
    /// Parses a single instruction from a span of characters, determining its type,
    /// register (if applicable), and offset (if applicable).
    /// </summary>
    /// <param name="span">The span of characters containing the instruction.</param>
    /// <returns>The parsed instruction.</returns>
    /// <exception cref="FormatException">Thrown when the instruction is invalid.</exception>
    private static Instruction ParseInstruction(ReadOnlySpan<char> span) => span[..3] switch
    {
        "hlf" => new Instruction(InstructionType.Hlf, Register: span[4] - 'a'),
        "tpl" => new Instruction(InstructionType.Tpl, Register: span[4] - 'a'),
        "inc" => new Instruction(InstructionType.Inc, Register: span[4] - 'a'),
        "jmp" => new Instruction(InstructionType.Jmp, Offset: ParseOffset(span[4..])),
        "jie" => new Instruction(InstructionType.Jie, span[4] - 'a', ParseOffset(span[7..])),
        "jio" => new Instruction(InstructionType.Jio, span[4] - 'a', ParseOffset(span[7..])),
        _ => throw new FormatException($"Invalid instruction: {span}")
    };

    /// <summary>
    /// Parses the offset value from a span of characters, allowing for leading signs.
    /// </summary>
    /// <param name="span">The span of characters containing the offset value.</param>
    /// <returns>The parsed offset value.</returns>
    private static int ParseOffset(ReadOnlySpan<char> span)
        => int.Parse(span, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo);

    /// <summary>
    /// Represents a single instruction in the program, including its type,
    /// register (if applicable), and offset (if applicable).
    /// </summary>
    /// <param name="Type">The type of the instruction.</param>
    /// <param name="Register">The register affected by the instruction, if applicable.</param>
    /// <param name="Offset">The offset for jump instructions, if applicable.</param>
    private readonly record struct Instruction(InstructionType Type, int Register = -1, int Offset = -1);

    /// <summary>
    /// Represents the types of instructions supported by the program.
    /// </summary>
    private enum InstructionType { Hlf, Tpl, Inc, Jmp, Jie, Jio }
}
