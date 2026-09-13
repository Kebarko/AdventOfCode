using KE.AoC.Core.Solution;
using System.Globalization;

namespace KE.AoC.Solutions.Y2015;

[Solution(2015, 22)]
public sealed class Day22 : SolutionBase
{
    private static ReadOnlySpan<int> Costs => [53, 73, 113, 173, 229];

    /// <summary>
    /// Calculates the least amount of mana spent to defeat the boss in normal mode, given the player's and boss's initial hit points and damage. Returns 0 if no winning sequence is found.
    /// </summary>
    /// <param name="input">The input string containing the player's and boss's initial hit points and damage.</param>
    /// <returns>The least amount of mana spent to defeat the boss in normal mode, or 0 if no winning sequence is found.</returns>
    public override object PartOne(string input)
    {
        (int HitPoints, int Damage) = ParseBoss(input);

        return Solve(50, 500, HitPoints, Damage, hardMode: false);
    }

    /// <summary>
    /// Calculates the least amount of mana spent to defeat the boss in hard mode, given the player's and boss's initial hit points and damage. Returns 0 if no winning sequence is found.
    /// </summary>
    /// <param name="input">The input string containing the boss's hit points and damage.</param>
    /// <returns>The least amount of mana spent to defeat the boss in hard mode, or 0 if no winning sequence is found.</returns>
    public override object PartTwo(string input)
    {
        (int HitPoints, int Damage) = ParseBoss(input);

        return Solve(50, 500, HitPoints, Damage, hardMode: true);
    }

    /// <summary>
    /// Solves the game by simulating all possible sequences of spell casts and boss attacks, returning the minimum amount of mana spent to defeat the boss. If no winning sequence is found, returns 0.
    /// </summary>
    /// <param name="playerHitPoints">The hit points of the player.</param>
    /// <param name="mana">The amount of mana available.</param>
    /// <param name="bossHitPoints">The hit points of the boss.</param>
    /// <param name="bossDamage">The damage dealt by the boss.</param>
    /// <param name="hardMode">Indicates whether hard mode is enabled.</param>
    /// <returns>The least amount of mana spent to defeat the boss, or 0 if no winning sequence is found.</returns>
    private static int Solve(int playerHitPoints, int mana, int bossHitPoints, int bossDamage, bool hardMode)
    {
        State state = new()
        {
            PlayerHitPoints = playerHitPoints,
            Mana = mana,
            BossHitPoints = bossHitPoints,
        };

        int best = int.MaxValue;
        PlayerTurn(state, bossDamage, hardMode, 0, ref best);

        return best == int.MaxValue ? 0 : best;
    }

    /// <summary>
    /// Simulates the player's turn in the game, exploring all possible spell casts and recursively simulating the boss's turn. Updates the best mana spent if a winning path is found.
    /// </summary>
    /// <param name="state">The current game state.</param>
    /// <param name="bossDamage">The damage dealt by the boss.</param>
    /// <param name="hardMode">Indicates whether hard mode is enabled.</param>
    /// <param name="spent">The amount of mana spent so far.</param>
    /// <param name="best">The best mana spent found so far.</param>
    private static void PlayerTurn(State state, int bossDamage, bool hardMode, int spent, ref int best)
    {
        // Hard mode drains a hit point before anything else, and can lose the fight outright.
        if (hardMode && --state.PlayerHitPoints <= 0)
            return;

        ApplyEffects(ref state);

        if (state.BossHitPoints <= 0)
        {
            best = Math.Min(best, spent);
            return;
        }

        for (int i = 0; i < Costs.Length; i++)
        {
            int total = spent + Costs[i];

            if (Costs[i] > state.Mana || total >= best)
                continue;

            // Effects have already ticked, so a timer that has just reached zero is recastable.
            State next = state;

            if (!TryCast((SpellType)i, ref next))
                continue;

            if (next.BossHitPoints <= 0)
            {
                best = Math.Min(best, total);
                continue;
            }

            // --- Boss turn ---
            ApplyEffects(ref next);

            if (next.BossHitPoints <= 0)
            {
                best = Math.Min(best, total);
                continue;
            }

            next.PlayerHitPoints -= Math.Max(1, bossDamage - (next.ShieldTimer > 0 ? 7 : 0));

            if (next.PlayerHitPoints <= 0)
                continue;

            PlayerTurn(next, bossDamage, hardMode, total, ref best);
        }
    }

    /// <summary>
    /// Applies the effects of active spells to the current game state, updating timers and hit points as necessary.
    /// </summary>
    /// <param name="state">The current game state.</param>
    private static void ApplyEffects(ref State state)
    {
        if (state.ShieldTimer > 0)
            state.ShieldTimer--;

        if (state.PoisonTimer > 0)
        {
            state.BossHitPoints -= 3;
            state.PoisonTimer--;
        }

        if (state.RechargeTimer > 0)
        {
            state.Mana += 101;
            state.RechargeTimer--;
        }
    }

    /// <summary>
    /// Attempts to cast a spell, updating the game state accordingly. Returns false if the spell cannot be cast (e.g., due to insufficient mana or an active effect).
    /// </summary>
    /// <param name="spell">The spell to cast.</param>
    /// <param name="state">The current game state.</param>
    /// <returns>true if the spell was cast successfully; otherwise, false.</returns>
    private static bool TryCast(SpellType spell, ref State state)
    {
        switch (spell)
        {
            case SpellType.MagicMissile:
                state.BossHitPoints -= 4;
                break;

            case SpellType.Drain:
                state.BossHitPoints -= 2;
                state.PlayerHitPoints += 2;
                break;

            case SpellType.Shield:
                if (state.ShieldTimer > 0)
                    return false;

                state.ShieldTimer = 6;
                break;

            case SpellType.Poison:
                if (state.PoisonTimer > 0)
                    return false;

                state.PoisonTimer = 6;
                break;

            case SpellType.Recharge:
                if (state.RechargeTimer > 0)
                    return false;

                state.RechargeTimer = 5;
                break;
        }

        state.Mana -= Costs[(int)spell];

        return true;
    }

    /// <summary>
    /// Parses the boss's hit points and damage from the input string.
    /// </summary>
    /// <param name="span">The input span containing the boss's information.</param>
    /// <returns>A tuple containing the boss's hit points and damage.</returns>
    /// <exception cref="FormatException">Thrown when the input format is invalid.</exception>
    private static (int HitPoints, int Damage) ParseBoss(ReadOnlySpan<char> span)
    {
        int? hitPoints = null;
        int? damage = null;

        Span<Range> tokens = stackalloc Range[3];

        foreach (ReadOnlySpan<char> line in span.EnumerateLines())
        {
            if (line.Split(tokens, ": ") != 2)
                continue;

            switch (line[tokens[0]])
            {
                case "Hit Points":
                    hitPoints = int.Parse(line[tokens[1]], NumberStyles.None, NumberFormatInfo.InvariantInfo);
                    break;
                case "Damage":
                    damage = int.Parse(line[tokens[1]], NumberStyles.None, NumberFormatInfo.InvariantInfo);
                    break;
            }
        }

        if (hitPoints is null || damage is null)
            throw new FormatException("Invalid boss input format. Missing required properties.");

        return ((int)hitPoints, (int)damage);
    }

    /// <summary>
    /// Represents the state of the game at any given point.
    /// </summary>
    private struct State
    {
        /// <summary>
        /// The hit points of the player. When this reaches zero or below, the player loses.
        /// </summary>
        public int PlayerHitPoints;

        /// <summary>
        /// The amount of mana the player has available to cast spells.
        /// </summary>
        public int Mana;

        /// <summary>
        /// The hit points of the boss. When this reaches zero or below, the player wins.
        /// </summary>
        public int BossHitPoints;

        /// <summary>
        /// The timer for the Shield spell, which provides temporary armor to the player.
        /// </summary>
        public int ShieldTimer;

        /// <summary>
        /// The timer for the Poison spell, which deals damage to the boss over time.
        /// </summary>
        public int PoisonTimer;

        /// <summary>
        /// The timer for the Recharge spell, which increases the player's mana over time.
        /// </summary>
        public int RechargeTimer;
    }

    /// <summary>
    /// Represents the different types of spells available to the player.
    /// </summary>
    private enum SpellType
    {
        MagicMissile,
        Drain,
        Shield,
        Poison,
        Recharge
    }
}
