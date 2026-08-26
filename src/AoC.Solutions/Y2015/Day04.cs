using KE.AoC.Core.Solution;
using System.Security.Cryptography;
using System.Text;

namespace KE.AoC.Solutions.Y2015;

[Solution(2015, 4)]
public sealed class Day04 : SolutionBase
{
    /// <summary>
    /// Finds the lowest integer that, when appended to the input string and hashed with MD5, produces a hash with five leading zeroes.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The lowest integer that satisfies the condition.</returns>
    public override object PartOne(string input)
    {
        return Mine(input.AsSpan().Trim(), 5);
    }

    /// <summary>
    /// Finds the lowest integer that, when appended to the input string and hashed with MD5, produces a hash with six leading zeroes.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The lowest integer that satisfies the condition.</returns>
    public override object PartTwo(string input)
    {
        return Mine(input.AsSpan().Trim(), 6);
    }

    /// <summary>
    /// Mines the input string for the lowest integer that, when appended to the input and hashed with MD5, produces a hash with the specified number of leading zeroes.
    /// </summary>
    /// <param name="input">The input string to mine.</param>
    /// <param name="zeroDigits">The number of leading zeroes to look for.</param>
    /// <returns>The lowest integer that satisfies the condition.</returns>
    private static int Mine(ReadOnlySpan<char> input, int zeroDigits)
    {
        Span<byte> buffer = stackalloc byte[64];
        Span<byte> hash = stackalloc byte[MD5.HashSizeInBytes];

        int prefixLength = Encoding.UTF8.GetBytes(input, buffer);

        for (int i = 1; ; i++)
        {
            i.TryFormat(buffer[prefixLength..], out int digits);
            MD5.HashData(buffer[..(prefixLength + digits)], hash);

            if (HasLeadingZeroes(hash, zeroDigits))
                return i;
        }
    }

    /// <summary>
    /// Checks if the given hash has the specified number of leading zeroes.
    /// </summary>
    /// <param name="hash">The hash to check.</param>
    /// <param name="count">The number of leading zeroes to look for.</param>
    /// <returns>true if the hash has the specified number of leading zeroes; otherwise, false.</returns>
    private static bool HasLeadingZeroes(Span<byte> hash, int count)
    {
        // Hex char (0-F) = 4 bits (2^4 = 16)
        // Byte (8 bits) = 2 hex chars
        // Example: 5 zeroes = 2.5 bytes = 2 full bytes + 1 half byte
        //          6 zeroes = 3 full bytes

        for (int i = 0; i < count / 2; i++)
            if (hash[i] != 0)
                return false;

        if (count % 2 != 0)
            if ((hash[count / 2] & 0xF0) != 0)
                return false;

        return true;
    }
}
