using KE.AoC.Core.Solution;

namespace KE.AoC.Solutions.Y2015;

[Solution(2015, 7)]
public sealed class Day07 : SolutionBase
{
    /// <summary>
    /// Runs the circuit simulation for part one of the problem, returning the value of wire "a" based on the provided input string.
    /// </summary>
    /// <param name="input">The input string representing the circuit.</param>
    /// <returns>The value of wire "a".</returns>
    public override object PartOne(string input)
    {
        return Run(input, "a", []);
    }

    /// <summary>
    /// Runs the circuit simulation for part two of the problem, returning the value of wire "a" based on the provided input string, with wire "b" overridden to have a value of 956.
    /// </summary>
    /// <param name="input">The input string representing the circuit.</param>
    /// <returns>The value of wire "a".</returns>
    public override object PartTwo(string input)
    {
        return Run(input, "a", new() { { "b", 956 } });
    }

    /// <summary>
    /// Runs the circuit simulation based on the input string and returns the value of the specified wire.
    /// </summary>
    /// <param name="input">The input string representing the circuit.</param>
    /// <param name="wire">The wire whose value is to be returned.</param>
    /// <param name="cache">A dictionary used to cache computed wire values.</param>
    /// <returns>The value of the specified wire.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static ushort Run(string input, string wire, Dictionary<string, ushort> cache)
    {
        Dictionary<string, Gate> gateMap = ParseGates(input).ToDictionary(g => g.Output);

        return Evaluate(wire);

        ushort Evaluate(string wire)
        {
            if (cache.TryGetValue(wire, out ushort cached))
            {
                return cached;
            }

            ushort result = gateMap[wire] switch
            {
                Assign(Operand value, _) => Resolve(value),
                Not(Operand input, _) => (ushort)~Resolve(input),
                Binary(BinaryOp.And, Operand left, Operand right, _) => (ushort)(Resolve(left) & Resolve(right)),
                Binary(BinaryOp.Or, Operand left, Operand right, _) => (ushort)(Resolve(left) | Resolve(right)),
                Binary(BinaryOp.LShift, Operand left, Operand right, _) => (ushort)(Resolve(left) << Resolve(right)),
                Binary(BinaryOp.RShift, Operand left, Operand right, _) => (ushort)(Resolve(left) >> Resolve(right)),
                _ => throw new InvalidOperationException("Invalid gate!")
            };

            cache[wire] = result;
            return result;
        }

        ushort Resolve(Operand operand) => operand switch
        {
            Literal(ushort value) => value,
            WireRef(string name) => Evaluate(name),
            _ => throw new InvalidOperationException("Invalid operand!")
        };
    }

    /// <summary>
    /// Parses the input string into a list of Gate objects, representing the circuit described by the input.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The list of parsed gates.</returns>
    /// <exception cref="FormatException">Thrown when the input string is malformed.</exception>
    private static List<Gate> ParseGates(string input)
    {
        Span<Range> tokens = stackalloc Range[5];

        List<Gate> gates = [];
        foreach (ReadOnlySpan<char> line in input.EnumerateLines())
        {
            if (line.IsEmpty)
                continue;

            int count = line.Split(tokens, ' ');
            string output = line[tokens[count - 1]].ToString();

            Gate gate = count switch
            {
                3 => new Assign(ParseOperand(line[tokens[0]]), output),
                4 => new Not(ParseOperand(line[tokens[1]]), output),
                5 => new Binary(ParseBinaryOp(line[tokens[1]]), ParseOperand(line[tokens[0]]), ParseOperand(line[tokens[2]]), output),
                _ => throw new FormatException($"Malformed gate: {line}"),
            };

            gates.Add(gate);
        }

        return gates;
    }

    /// <summary>
    /// Parses an operand from a span of characters, returning either a Literal or a WireRef depending on the content of the span.
    /// </summary>
    /// <param name="span">The span of characters to parse.</param>
    /// <returns>The parsed operand.</returns>
    private static Operand ParseOperand(ReadOnlySpan<char> span)
    {
        if (ushort.TryParse(span, out ushort result))
        {
            return new Literal(result);
        }

        return new WireRef(span.ToString());
    }

    /// <summary>
    /// Parses a binary operation from a span of characters, returning the corresponding BinaryOp enum value.
    /// </summary>
    /// <param name="span">The span of characters to parse.</param>
    /// <returns>The corresponding BinaryOp enum value.</returns>
    private static BinaryOp ParseBinaryOp(ReadOnlySpan<char> span)
    {
        return Enum.Parse<BinaryOp>(span, true);
    }

    /// <summary>
    /// Represents a gate in the circuit, which can be an assignment, a NOT operation, or a binary operation.
    /// </summary>
    /// <param name="Output">The output wire of the gate.</param>
    private abstract record class Gate(string Output);

    /// <summary>
    /// Represents an assignment gate in the circuit, which assigns a value to an output wire.
    /// </summary>
    /// <param name="Value">The value to assign.</param>
    /// <param name="Output">The output wire of the gate.</param>
    private sealed record class Assign(Operand Value, string Output) : Gate(Output);

    /// <summary>
    /// Represents a NOT gate in the circuit, which performs a bitwise NOT operation on its input and assigns the result to an output wire.
    /// </summary>
    /// <param name="Input">The input wire of the gate.</param>
    /// <param name="Output">The output wire of the gate.</param>
    private sealed record class Not(Operand Input, string Output) : Gate(Output);

    /// <summary>
    /// Represents a binary gate in the circuit, which performs a binary operation (AND, OR, LSHIFT, RSHIFT) on its two input operands and assigns the result to an output wire.
    /// </summary>
    /// <param name="Op">The binary operation to perform.</param>
    /// <param name="Left">The left input operand.</param>
    /// <param name="Right">The right input operand.</param>
    /// <param name="Output">The output wire of the gate.</param>
    private sealed record class Binary(BinaryOp Op, Operand Left, Operand Right, string Output) : Gate(Output);

    /// <summary>
    /// Represents an operand in the circuit, which can be either a literal value or a reference to another wire.
    /// </summary>
    private abstract record class Operand;

    /// <summary>
    /// Represents a literal value operand in the circuit.
    /// </summary>
    /// <param name="Value">The literal value of the operand.</param>
    private sealed record class Literal(ushort Value) : Operand;

    /// <summary>
    /// Represents a reference to another wire in the circuit.
    /// </summary>
    /// <param name="Name">The name of the wire.</param>
    private sealed record class WireRef(string Name) : Operand;

    /// <summary>
    /// Represents the binary operations that can be performed in the circuit.
    /// </summary>
    private enum BinaryOp { And, Or, LShift, RShift }
}
