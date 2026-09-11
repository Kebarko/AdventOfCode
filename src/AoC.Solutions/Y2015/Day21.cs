using KE.AoC.Core.Solution;
using System.Globalization;

namespace KE.AoC.Solutions.Y2015;

[Solution(2015, 21)]
public sealed class Day21 : SolutionBase
{
    private static readonly Item[] Weapons =
    [
        new Item("Dagger",      8, 4, 0),
        new Item("Shortsword", 10, 5, 0),
        new Item("Warhammer",  25, 6, 0),
        new Item("Longsword",  40, 7, 0),
        new Item("Greataxe",   74, 8, 0),
    ];

    private static readonly Item[] Armor =
    [
        new Item("Leather",     13, 0, 1),
        new Item("Chainmail",   31, 0, 2),
        new Item("Splintmail",  53, 0, 3),
        new Item("Bandedmail",  75, 0, 4),
        new Item("Platemail",  102, 0, 5),
    ];

    private static readonly Item[] Rings =
    [
        new Item("Damage +1",   25, 1, 0),
        new Item("Damage +2",   50, 2, 0),
        new Item("Damage +3",  100, 3, 0),
        new Item("Defense +1",  20, 0, 1),
        new Item("Defense +2",  40, 0, 2),
        new Item("Defense +3",  80, 0, 3),
    ];

    /// <summary>
    /// Calculates the minimum cost of gear required for the player to defeat the boss.
    /// </summary>
    /// <param name="input">The input string containing the boss's properties.</param>
    /// <returns>The minimum cost of gear required.</returns>
    public override object PartOne(string input)
    {
        Boss boss = ParseBoss(input.AsSpan());

        foreach (Gear gear in GetAllValidGears().OrderBy(g => g.TotalCost))
        {
            IPlayer player1 = new Me(gear);
            IPlayer player2 = new Boss(boss);

            Simulate(player1, player2);

            if (player1.HitPoints > 0)
                return gear.TotalCost;
        }

        return 0;
    }

    /// <summary>
    /// Calculates the maximum cost of gear that can be spent while still losing to the boss.
    /// </summary>
    /// <param name="input">The input string containing the boss's properties.</param>
    /// <returns>The maximum cost of gear that can be spent while still losing.</returns>
    public override object PartTwo(string input)
    {
        Boss boss = ParseBoss(input.AsSpan());

        foreach (Gear gear in GetAllValidGears().OrderByDescending(g => g.TotalCost))
        {
            IPlayer player1 = new Me(gear);
            IPlayer player2 = new Boss(boss);

            Simulate(player1, player2);

            if (player2.HitPoints > 0)
                return gear.TotalCost;
        }

        return 0;
    }

    /// <summary>
    /// Simulates a turn-based battle between two players, where each player takes turns attacking the other until one player's hit points reach zero or below. The damage dealt is calculated based on the attacker's damage and the defender's armor, with a minimum of 1 damage per attack. The method continues until one player is defeated.
    /// </summary>
    /// <param name="player1">The first player.</param>
    /// <param name="player2">The second player.</param>
    private static void Simulate(IPlayer player1, IPlayer player2)
    {
        IPlayer striker = player1;
        IPlayer defender = player2;

        while (player1.HitPoints > 0 && player2.HitPoints > 0)
        {
            int damage = striker.Damage - defender.Armor;
            if (damage <= 0)
                damage = 1;

            defender.HitPoints -= damage;

            (striker, defender) = (defender, striker);
        }
    }

    /// <summary>
    /// Generates all valid combinations of gear that can be equipped by the player, including one weapon, optional armor, and up to two rings.
    /// </summary>
    /// <returns>All valid gear combinations.</returns>
    private static IEnumerable<Gear> GetAllValidGears()
    {
        foreach (Item weapon in Weapons)
            foreach (Item? armor in Armor.Cast<Item?>().Prepend(null))
                foreach (Item[] rings in GetRingCombinations())
                    yield return new Gear(weapon, armor, rings);

        static IEnumerable<Item[]> GetRingCombinations()
        {
            yield return Array.Empty<Item>();

            foreach (Item ring in Rings)
                yield return new[] { ring };

            foreach (Item ring1 in Rings)
                foreach (Item ring2 in Rings)
                    yield return new[] { ring1, ring2 };
        }
    }

    /// <summary>
    /// Parses the boss's properties from the input string and creates a new Boss instance with the extracted values.
    /// </summary>
    /// <param name="span">The input string containing the boss's properties.</param>
    /// <returns>The created Boss instance.</returns>
    /// <exception cref="FormatException">Thrown when the input string is not in the expected format.</exception>
    private static Boss ParseBoss(ReadOnlySpan<char> span)
    {
        int? hitPoints = null;
        int? damage = null;
        int? armor = null;

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
                case "Armor":
                    armor = int.Parse(line[tokens[1]], NumberStyles.None, NumberFormatInfo.InvariantInfo);
                    break;
            }
        }

        if (hitPoints is null || damage is null || armor is null)
            throw new FormatException("Invalid boss input format. Missing required properties.");

        return new Boss((int)hitPoints, (int)damage, (int)armor);
    }

    /// <summary>
    /// Represents a player in the game, which can be either the player character or the boss.
    /// </summary>
    private interface IPlayer
    {
        /// <summary>
        /// Gets or sets the current hit points of the player. When the hit points reach zero or below, the player is defeated.
        /// </summary>
        int HitPoints { get; set; }

        /// <summary>
        /// Gets the damage value of the player, which determines how much damage they can inflict on their opponent.
        /// </summary>
        int Damage { get; }

        /// <summary>
        /// Gets the armor value of the player, which reduces the amount of damage they take from their opponent's attacks.
        /// </summary>
        int Armor { get; }
    }

    /// <summary>
    /// Represents the player character in the game, which is equipped with a specific combination of gear (weapon, optional armor, and rings).
    /// The player's hit points start at 100, and their damage and armor values are determined by the equipped gear.
    /// </summary>
    /// <param name="gear">The gear equipped by the player.</param>
    private sealed class Me(Gear gear) : IPlayer
    {
        /// <summary>
        /// Gets or sets the current hit points of the player character. The player starts with 100 hit points, and when the hit points reach zero or below, the player is defeated.
        /// </summary>
        public int HitPoints { get; set; } = 100;

        /// <summary>
        /// Gets the damage value of the player character, which is determined by the equipped gear (weapon and rings). This value represents how much damage the player can inflict on their opponent.
        /// </summary>
        public int Damage { get; } = gear.TotalDamage;

        /// <summary>
        /// Gets the armor value of the player character, which is determined by the equipped gear (optional armor and rings). This value represents how much damage the player can mitigate from their opponent's attacks.
        /// </summary>
        public int Armor { get; } = gear.TotalArmor;
    }

    /// <summary>
    /// Represents the boss character in the game, which has specific hit points, damage, and armor values.
    /// The boss's properties are initialized based on the input provided, and they can be copied to create a new instance of the boss with the same attributes.
    /// </summary>
    /// <param name="hitpoints">The hit points of the boss.</param>
    /// <param name="damage">The damage value of the boss.</param>
    /// <param name="armor">The armor value of the boss.</param>
    private sealed class Boss(int hitpoints, int damage, int armor) : IPlayer
    {
        /// <summary>
        /// Gets or sets the current hit points of the boss. The boss's hit points are initialized based on the input provided, and when the hit points reach zero or below, the boss is defeated.
        /// </summary>
        public int HitPoints { get; set; } = hitpoints;

        /// <summary>
        /// Gets the damage value of the boss, which is initialized based on the input provided. This value represents how much damage the boss can inflict on their opponent.
        /// </summary>
        public int Damage { get; } = damage;

        /// <summary>
        /// Gets the armor value of the boss, which is initialized based on the input provided. This value represents how much damage the boss can mitigate from their opponent's attacks.
        /// </summary>
        public int Armor { get; } = armor;

        /// <summary>
        /// Initializes a new instance of the Boss class by copying the properties of an existing boss instance. This constructor allows for creating a new boss with the same hit points, damage, and armor values as the provided boss.
        /// </summary>
        /// <param name="boss">The boss instance to copy.</param>
        public Boss(Boss boss)
             : this(boss.HitPoints, boss.Damage, boss.Armor)
        {
        }
    }

    /// <summary>
    /// Represents a combination of gear that can be equipped by the player, including a weapon, optional armor, and up to two rings.
    /// </summary>
    /// <param name="Weapon">The weapon to be equipped.</param>
    /// <param name="Armor">The optional armor to be equipped.</param>
    /// <param name="Rings">The rings to be equipped.</param>
    private sealed record class Gear(Item Weapon, Item? Armor, Item[] Rings)
    {
        /// <summary>
        /// Calculates the total cost of the gear, including the cost of the weapon, optional armor, and rings.
        /// </summary>
        public int TotalCost => Weapon.Cost + (Armor?.Cost ?? 0) + Rings.Sum(r => r.Cost);

        /// <summary>
        /// Calculates the total damage provided by the gear, including the damage from the weapon and any rings.
        /// </summary>
        public int TotalDamage => Weapon.Damage + Rings.Sum(r => r.Damage);

        /// <summary>
        /// Calculates the total armor provided by the gear, including the armor from the optional armor and any rings.
        /// </summary>
        public int TotalArmor => (Armor?.Armor ?? 0) + Rings.Sum(r => r.Armor);
    }

    /// <summary>
    /// Represents an item that can be equipped by the player or boss, such as a weapon, armor, or ring.
    /// </summary>
    /// <param name="Name">The name of the item.</param>
    /// <param name="Cost">The cost of the item.</param>
    /// <param name="Damage">The damage provided by the item.</param>
    /// <param name="Armor">The armor provided by the item.</param>
    private sealed record class Item(string Name, int Cost, int Damage, int Armor);
}
