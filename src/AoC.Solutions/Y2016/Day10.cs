using KE.AoC.Core.Solution;
using System.Globalization;
using System.Text.RegularExpressions;

namespace KE.AoC.Solutions.Y2016;

/// <summary>
/// --- Day 10: Balance Bots ---
/// </summary>
[Solution(2016, 10)]
public sealed partial class Day10 : SolutionBase
{
    /// <summary>
    /// Processes the input instructions to determine which bot is responsible for comparing chips 17 and 61.
    /// </summary>
    /// <param name="input">The input string containing the instructions.</param>
    /// <returns>The ID of the bot responsible for comparing chips 17 and 61.</returns>
    public override object PartOne(string input)
    {
        return Simulate(input).Item1;
    }

    /// <summary>
    /// Processes the input instructions to calculate the product of the first chips in outputs 0, 1, and 2.
    /// </summary>
    /// <param name="input">The input string containing the instructions.</param>
    /// <returns>The product of the first chips in outputs 0, 1, and 2.</returns>
    public override object PartTwo(string input)
    {
        return Simulate(input).Item2;
    }

    /// <summary>
    /// Simulates the chip distribution process based on the input instructions,
    /// returning the bot responsible for comparing chips 17 and 61,
    /// as well as the product of the first chips in outputs 0, 1, and 2.
    /// </summary>
    /// <param name="input">The input string containing the instructions.</param>
    /// <returns>
    /// A tuple containing the ID of the bot responsible for comparing chips 17 and 61,
    /// and the product of the first chips in outputs 0, 1, and 2.
    /// </returns>
    private static (int, int) Simulate(string input)
    {
        (Dictionary<int, List<int>> bots, Dictionary<int, BotRule> botRules) = ParseInstructions(input);
        Dictionary<int, List<int>> outputs = [];

        int wantedBot = -1;
        List<int> botsToProceed = [];
        while ((botsToProceed = GetBotsToProceed()).Count > 0)
        {
            foreach (int botId in botsToProceed)
            {
                List<int> chips = bots[botId];

                if (chips.Count == 2)
                {
                    (int lowChip, int highChip) = chips[0] < chips[1]
                        ? (chips[0], chips[1])
                        : (chips[1], chips[0]);
                    chips.Clear();

                    if (lowChip == 17 && highChip == 61)
                        wantedBot = botId;

                    BotRule botRule = botRules[botId];

                    if (botRule.Low.IsBot)
                        AddChipToBot(botRule.Low.Id, lowChip);
                    else
                        AddChipToOutput(botRule.Low.Id, lowChip);

                    if (botRule.High.IsBot)
                        AddChipToBot(botRule.High.Id, highChip);
                    else
                        AddChipToOutput(botRule.High.Id, highChip);
                }
            }
        }

        return (wantedBot, outputs[0][0] * outputs[1][0] * outputs[2][0]);

        List<int> GetBotsToProceed() =>
            [.. bots.Where(kvp => kvp.Value.Count == 2).Select(kvp => kvp.Key)];

        void AddChipToBot(int botId, int chipId)
        {
            if (!bots.ContainsKey(botId))
                bots.Add(botId, []);

            bots[botId].Add(chipId);
        }

        void AddChipToOutput(int outputId, int chipId)
        {
            if (!outputs.ContainsKey(outputId))
                outputs.Add(outputId, []);

            outputs[outputId].Add(chipId);
        }
    }

    /// <summary>
    /// Parses the input instructions to extract the initial chip assignments to bots
    /// and the rules for how each bot distributes its chips.
    /// </summary>
    /// <param name="input">The input string containing the instructions.</param>
    /// <returns>A tuple containing the initial chip assignments and the bot rules.</returns>
    private static (Dictionary<int, List<int>>, Dictionary<int, BotRule>) ParseInstructions(string input)
    {
        Dictionary<int, List<int>> bots = [];
        Dictionary<int, BotRule> botRules = [];

        foreach (ReadOnlySpan<char> line in input.EnumerateLines())
        {
            Match match = ChipAssignRegex().Match(line.ToString());
            if (match.Success)
            {
                int botId = ParseInt(match.Groups[2].ValueSpan);
                int chipId = ParseInt(match.Groups[1].ValueSpan);

                if (!bots.ContainsKey(botId))
                    bots.Add(botId, []);

                bots[botId].Add(chipId);

                continue;
            }

            match = BotRuleRegex().Match(line.ToString());
            if (match.Success)
            {
                int botId = ParseInt(match.Groups[1].ValueSpan);
                Target low = new(ParseInt(match.Groups[3].ValueSpan), match.Groups[2].Value == "bot");
                Target high = new(ParseInt(match.Groups[5].ValueSpan), match.Groups[4].Value == "bot");

                botRules.Add(botId, new(low, high));
            }
        }

        return (bots, botRules);
    }

    /// <summary>
    /// Parses an integer from a ReadOnlySpan of characters using invariant culture and no number styles.
    /// </summary>
    /// <param name="span">The span of characters to parse.</param>
    /// <returns>The parsed integer.</returns>
    private static int ParseInt(ReadOnlySpan<char> span) =>
        int.Parse(span, NumberStyles.None, NumberFormatInfo.InvariantInfo);

    /// <summary>
    /// Represents the regex pattern for matching chip assignment instructions in the input.
    /// </summary>
    /// <returns>The regex pattern.</returns>
    [GeneratedRegex(@"value (\d+) goes to bot (\d+)")]
    private static partial Regex ChipAssignRegex();

    /// <summary>
    /// Represents the regex pattern for matching bot rules in the input instructions.
    /// </summary>
    /// <returns>The regex pattern.</returns>
    [GeneratedRegex(@"bot (\d+) gives low to (bot|output) (\d+) and high to (bot|output) (\d+)")]
    private static partial Regex BotRuleRegex();

    /// <summary>
    /// Represents the rules for a bot's chip distribution, specifying the targets for the low and high chips.
    /// </summary>
    /// <param name="Low">The target for the low chip.</param>
    /// <param name="High">The target for the high chip.</param>
    private readonly record struct BotRule(Target Low, Target High);

    /// <summary>
    /// Represents a target for a bot's chip distribution, which can be either another bot or an output bin.
    /// </summary>
    /// <param name="Id">The identifier of the target.</param>
    /// <param name="IsBot">Indicates whether the target is a bot (true) or an output bin (false).</param>
    private readonly record struct Target(int Id, bool IsBot);
}
