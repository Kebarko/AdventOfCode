using KE.AoC.Core.Solution;
using System.Numerics;

namespace KE.AoC.Solutions.Y2015;

[Solution(2015, 11)]
public sealed class Day11 : SolutionBase
{
    /// <summary>
    /// Finds the first valid password after the given password according to the specified rules.
    /// </summary>
    /// <param name="input">The input password.</param>
    /// <returns>The next valid password.</returns>
    public override object PartOne(string input)
    {
        return FindNextPassword(input.AsSpan());
    }

    /// <summary>
    /// Finds the second valid password after the given password according to the specified rules.
    /// </summary>
    /// <param name="input">The input password.</param>
    /// <returns>The next valid password.</returns>
    public override object PartTwo(string input)
    {
        return FindNextPassword(input.AsSpan(), 2);
    }

    /// <summary>
    /// Finds the next valid password after the given password by incrementing it until a valid password is found. It can also find the next valid password after a specified number of steps.
    /// </summary>
    /// <param name="password">The password to find the next valid password for.</param>
    /// <param name="steps">The number of steps to take.</param>
    /// <returns>The next valid password.</returns>
    private static string FindNextPassword(ReadOnlySpan<char> password, int steps = 1)
    {
        Validate(password);

        Span<char> buffer = stackalloc char[password.Length];
        password.CopyTo(buffer);

        SkipForbidden(buffer);

        for (int step = 0; step < steps; step++)
        {
            Advance(buffer);
        }

        return new(buffer);
    }

    /// <summary>
    /// Advances the given password to the next valid password by incrementing it until a valid password is found.
    /// </summary>
    /// <param name="password">The password to advance.</param>
    /// <exception cref="InvalidOperationException">Thrown when there is no valid password greater than the given one.</exception>
    private static void Advance(Span<char> password)
    {
        do
        {
            if (!Increment(password))
                throw new InvalidOperationException("There is no valid password greater than the given one.");
        }
        while (!IsValid(password));
    }

    /// <summary>
    /// Skips over any forbidden characters ('i', 'l', 'o') in the given password by incrementing them to the next valid character and filling the subsequent characters with 'a'.
    /// </summary>
    /// <param name="password"></param>
    private static void SkipForbidden(Span<char> password)
    {
        for (int i = 0; i < password.Length; i++)
        {
            if (password[i] is not ('i' or 'l' or 'o'))
            {
                continue;
            }

            password[i]++;
            password[(i + 1)..].Fill('a');
        }
    }

    /// <summary>
    /// Increments the given password by one, treating it as a base-26 number with 'a' as 0 and 'z' as 25.
    /// If the password reaches 'z', it wraps around to 'a' and carries over to the next character.
    /// The method also skips over forbidden characters ('i', 'l', 'o') by incrementing them to the next valid character.
    /// </summary>
    /// <param name="password">The password to increment.</param>
    /// <returns>True if the password was incremented successfully, false otherwise.</returns>
    private static bool Increment(Span<char> password)
    {
        for (int i = password.Length - 1; i >= 0; i--)
        {
            if (password[i] == 'z')
            {
                password[i] = 'a';
                continue;
            }

            password[i]++;

            if (password[i] is 'i' or 'l' or 'o')
            {
                password[i]++;
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if the given password is valid according to the specified rules:
    /// 1. It must contain at least one straight of three consecutive letters (e.g., abc, def).
    /// 2. It must not contain any of the letters i, l, or o.
    /// 3. It must contain at least two different, non-overlapping pairs of letters (e.g., aa, bb).
    /// </summary>
    /// <param name="password">The password to validate.</param>
    /// <returns>True if the password is valid, false otherwise.</returns>
    private static bool IsValid(ReadOnlySpan<char> password)
    {
        bool hasStraight = false;
        int pairs = 0;
        int last = password.Length - 1;

        for (int i = 0; i <= last; i++)
        {
            if (password[i] is < 'a' or > 'z' or 'i' or 'l' or 'o')
            {
                return false;
            }

            if (!hasStraight && i <= last - 2 && (password[i] + 1) == password[i + 1] && (password[i + 1] + 1) == password[i + 2])
            {
                hasStraight = true;
            }

            if (i <= last - 1 && password[i] == password[i + 1])
            {
                pairs |= 1 << password[i] - 'a';
            }
        }

        return hasStraight && BitOperations.PopCount((uint)pairs) >= 2;
    }

    /// <summary>
    /// Validates the input password to ensure it is not empty and consists of lowercase letters only.
    /// </summary>
    /// <param name="password">The password to validate.</param>
    /// <exception cref="ArgumentException">Thrown when the password is empty or contains invalid characters.</exception>
    private static void Validate(ReadOnlySpan<char> password)
    {
        if (password.IsEmpty)
            throw new ArgumentException("The password must not be empty.", nameof(password));

        foreach (char c in password)
        {
            if (c is < 'a' or > 'z')
                throw new ArgumentException($"The password must consist of lowercase letters only, but contains '{c}'.", nameof(password));
        }
    }
}
