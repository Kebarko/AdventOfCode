using KE.AoC.Core.Solution;
using System.Security.Cryptography;
using System.Text;

namespace KE.AoC.Solutions.Y2016;

/// <summary>
/// --- Day 5: How About a Nice Game of Chess? ---
/// </summary>
[Solution(2016, 5)]
public sealed class Day05 : SolutionBase
{
    /// <summary>
    /// The length of the password to be generated.
    /// </summary>
    private const int PassLength = 8;

    /// <summary>
    /// The number of leading zeroes required in the hash to consider it valid for password generation.
    /// </summary>
    private const int ZeroDigits = 5;

    /// <summary>
    /// Finds the password by mining the input string for hashes with five leading zeroes,
    /// using the sixth character of the hash as the next character of the password.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The resulting password.</returns>
    public override object PartOne(string input)
    {
        const int passCharIdx = 5;

        Span<char> password = stackalloc char[PassLength];
        int index = 0;
        for (int i = 0; i < PassLength; i++)
        {
            string hash = Mine(input.AsSpan(), ref index, ZeroDigits);

            password[i] = hash[passCharIdx];
            index++;
        }

        return new string(password);
    }

    /// <summary>
    /// Finds the password by mining the input string for hashes with five leading zeroes,
    /// using the sixth character of the hash as the position and the seventh character as the value.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The resulting password.</returns>
    public override object PartTwo(string input)
    {
        const int passPosIdx = 5;
        const int passCharIdx = 6;

        Span<char> password = stackalloc char[PassLength];
        Span<bool> isSet = stackalloc bool[PassLength];
        int index = 0;

        do
        {
            string hash = Mine(input.AsSpan(), ref index, ZeroDigits);
            int passPos = Convert.ToInt32(hash[passPosIdx].ToString(), 16);
            if (passPos >= 0 && passPos <= 7 && !isSet[passPos])
            {
                password[passPos] = hash[passCharIdx];
                isSet[passPos] = true;
            }

            index++;
        }
        while (isSet.IndexOf(false) >= 0);

        return new string(password);
    }

    /// <summary>
    /// Mines the input string for the lowest integer that,
    /// when appended to the input and hashed with MD5,
    /// produces a hash with the specified number of leading zeroes.
    /// </summary>
    /// <param name="input">The input string to mine.</param>
    /// <param name="index">The starting index for mining.</param>
    /// <param name="zeroDigits">The number of leading zeroes to look for.</param>
    /// <returns>The hexadecimal string representation of the hash.</returns>
    private static string Mine(ReadOnlySpan<char> input, ref int index, int zeroDigits)
    {
        Span<byte> buffer = stackalloc byte[64];
        Span<byte> hash = stackalloc byte[MD5.HashSizeInBytes];

        int prefixLength = Encoding.UTF8.GetBytes(input, buffer);

        do
        {
            index.TryFormat(buffer[prefixLength..], out int digits);
            MD5.HashData(buffer[..(prefixLength + digits)], hash);

            index++;
        }
        while (!HasLeadingZeroes(hash, zeroDigits));

        return Convert.ToHexStringLower(hash);
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
