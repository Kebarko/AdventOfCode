using KE.AoC.Core.Solution;
using System.Diagnostics;

namespace KE.AoC.Solutions.Y2016;

/// <summary>
/// --- Day 4: Security Through Obscurity ---
/// </summary>
[Solution(2016, 4)]
public sealed class Day04 : SolutionBase
{
    /// <summary>
    /// Calculates the sum of the sector IDs of all real rooms from the input.
    /// </summary>
    /// <param name="input">The input string containing the room information.</param>
    /// <returns>The sum of the sector IDs of all real rooms.</returns>
    public override object PartOne(string input)
    {
        int result = 0;
        foreach (ReadOnlySpan<char> line in input.EnumerateLines())
        {
            Room room = ParseRoom(line);
            if (room.IsRealRoom())
                result += room.SectorId;
        }

        return result;
    }

    /// <summary>
    /// Finds the sector ID of the room that decrypts to "northpole object storage" from the input.
    /// </summary>
    /// <param name="input">The input string containing the room information.</param>
    /// <returns>The sector ID of the matching room.</returns>
    /// <exception cref="UnreachableException">Thrown when no matching room is found.</exception>
    public override object PartTwo(string input)
    {
        const string target = "northpole object storage";
        foreach (ReadOnlySpan<char> line in input.EnumerateLines())
        {
            Room room = ParseRoom(line);
            if (room.Matches(target))
                return room.SectorId;
        }

        throw new UnreachableException("No matching room found");
    }

    /// <summary>
    /// Parses a line of input into a <see cref="Room"/> struct.
    /// </summary>
    /// <param name="span">The line of input to parse.</param>
    /// <returns>The parsed <see cref="Room"/> struct.</returns>
    private static Room ParseRoom(ReadOnlySpan<char> span)
    {
        int lastDash = span.LastIndexOf('-');
        int openBracket = span.LastIndexOf('[');

        ReadOnlySpan<char> name = span[..lastDash];
        int sectorId = int.Parse(span[(lastDash + 1)..openBracket]);
        ReadOnlySpan<char> checksum = span[(openBracket + 1)..^1];

        return new Room(name, sectorId, checksum);
    }

    /// <summary>
    /// Represents a room with its encrypted name, sector ID, and checksum.
    /// </summary>
    /// <param name="name">The encrypted name of the room.</param>
    /// <param name="sectorId">The sector ID of the room.</param>
    /// <param name="checksum">The checksum of the room.</param>
    private readonly ref struct Room(ReadOnlySpan<char> name, int sectorId, ReadOnlySpan<char> checksum)
    {
        /// <summary>
        /// Gets the encrypted name of the room.
        /// </summary>
        public ReadOnlySpan<char> Name { get; } = name;

        /// <summary>
        /// Gets the sector ID of the room.
        /// </summary>
        public int SectorId { get; } = sectorId;

        /// <summary>
        /// Gets the checksum of the room.
        /// </summary>
        public ReadOnlySpan<char> Checksum { get; } = checksum;

        /// <summary>
        /// Verifies that <see cref="Checksum"/> is exactly the most frequent letters of
        /// <see cref="Name"/>, in descending count order with ties broken alphabetically.
        /// </summary>
        public bool IsRealRoom()
        {
            // keys[i] packs both ranking criteria into one comparable int:
            //   bits 31..5 -> occurrence count, bits 4..0 -> alphabetical tiebreak.
            // A single '>' then yields the full ordering, so no sort is needed.
            Span<int> keys = stackalloc int[26];

            // Seed the tiebreak field with the inverted letter index ('a' -> 25 ... 'z' -> 0),
            // so a higher key means an earlier letter. Max seed is 25, i.e. it always fits
            // in the low 5 bits and counts added later never carry into it.
            for (int i = 0; i < 26; i++)
                keys[i] = 25 - i;

            // Tally the name. Each occurrence is worth 32, which is strictly greater than the
            // largest possible tiebreak spread (25), so the tiebreak can never overturn a
            // genuine count difference. Dashes are separators and are not counted.
            foreach (char c in Name)
                if (c != '-')
                    keys[c - 'a'] += 1 << 5;

            // Greedily pick the highest-ranked remaining letter once per checksum position and
            // compare it in place, rather than materializing the expected checksum.
            for (int i = 0; i < Checksum.Length; i++)
            {
                // Linear scan over the 26 slots: 5 passes total, branch-predictable, no allocation.
                int best = 0;
                for (int k = 1; k < 26; k++)
                    if (keys[k] > keys[best])
                        best = k;

                // A zero count field means the greedy pick is a letter absent from the name,
                // which happens once fewer than Checksum.Length distinct letters remain. Without
                // this guard a short name would match leftover tiebreak seeds by coincidence
                // (e.g. name "abc" against checksum "abcde").
                if (keys[best] < (1 << 5) || Checksum[i] != (char)('a' + best))
                    return false;

                // Retire the chosen letter. Safe as a sentinel because every live key is >= 0,
                // and it also makes the method total for checksums longer than 26 characters.
                keys[best] = int.MinValue;
            }

            return true;
        }

        /// <summary>
        /// Tests whether decrypting <see cref="Name"/> with the shift cipher keyed by
        /// <see cref="SectorId"/> yields exactly <paramref name="target"/>.
        /// </summary>
        public bool Matches(ReadOnlySpan<char> target)
        {
            // Decryption is character-for-character (dashes become single spaces), so the
            // plaintext length always equals the ciphertext length. This discards nearly every
            // room before any arithmetic runs.
            if (Name.Length != target.Length)
                return false;

            // Rotating by the sector ID is the same as rotating by its remainder mod 26.
            // Non-negative because the parser rejects signs, so the index below stays in range.
            int shift = SectorId % 26;

            // Compare in place instead of building the decrypted string first: the loop exits at
            // the first divergence and never allocates.
            for (int i = 0; i < Name.Length; i++)
            {
                // Dashes map to spaces; letters rotate forward with wraparound. Both operands of
                // the '%' are non-negative ('a'..'z' minus 'a' gives 0..25, plus a shift of 0..25),
                // so the result needs no sign correction.
                char decoded = Name[i] == '-' ? ' ' : (char)('a' + (Name[i] - 'a' + shift) % 26);

                if (decoded != target[i])
                    return false;
            }

            return true;
        }
    }
}
