using KE.AoC.Core.Solution;

namespace KE.AoC.Solutions.Y2015;

[Solution(2015, 15)]
public sealed class Day15 : SolutionBase
{
    private const int Teaspoons = 100;
    private const int TargetCalories = 500;

    /// <summary>
    /// Calculates the maximum score for the optimal combination of teaspoons.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The maximum score.</returns>
    public override object PartOne(string input)
    {
        List<Ingredient> ingredients = ParseIngredients(input.AsSpan());

        return FindMaxScore(ingredients, Teaspoons);
    }

    /// <summary>
    /// Calculates the maximum score for the optimal combination of teaspoons with a target calorie count.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The maximum score.</returns>
    public override object PartTwo(string input)
    {
        List<Ingredient> ingredients = ParseIngredients(input.AsSpan());

        return FindMaxScore(ingredients, Teaspoons, TargetCalories);
    }

    /// <summary>
    /// Finds the maximum score for the given ingredients and teaspoons, optionally considering a target calorie count.
    /// </summary>
    /// <param name="ingredients">The list of ingredients.</param>
    /// <param name="teaspoons">The number of teaspoons.</param>
    /// <param name="targetCalories">The optional target calorie count.</param>
    /// <returns>The maximum score.</returns>
    private static long FindMaxScore(List<Ingredient> ingredients, int teaspoons, int? targetCalories = null)
    {
        ArgumentNullException.ThrowIfNull(ingredients);
        if (ingredients.Count == 0)
            return 0;

        return FindMaxScore(ingredients, teaspoons, ingredients.Count, new int[ingredients.Count], 0, targetCalories);
    }

    /// <summary>
    /// Recursively finds the maximum score for the given ingredients, remaining teaspoons, remaining ingredients, quantities, index, and optional target calories.
    /// </summary>
    /// <param name="ingredients">The list of ingredients.</param>
    /// <param name="remTsps">The remaining teaspoons.</param>
    /// <param name="remIngrdnts">The remaining ingredients.</param>
    /// <param name="quantities">The quantities of each ingredient.</param>
    /// <param name="index">The current index.</param>
    /// <param name="targetCalories">The optional target calorie count.</param>
    /// <returns>The maximum score.</returns>
    private static long FindMaxScore(List<Ingredient> ingredients, int remTsps, int remIngrdnts, int[] quantities, int index, int? targetCalories)
    {
        if (remIngrdnts == 1)
        {
            quantities[index] = remTsps;
            return CalculateTotalScore(ingredients, quantities, targetCalories);
        }

        long max = 0;
        for (int tsps = 0; tsps <= remTsps; tsps++)
        {
            quantities[index] = tsps;
            long score = FindMaxScore(ingredients, remTsps - tsps, remIngrdnts - 1, quantities, index + 1, targetCalories);

            if (score > max)
                max = score;
        }

        return max;
    }

    /// <summary>
    /// Calculates the total calories based on the given ingredients and their quantities.
    /// </summary>
    /// <param name="ingredients">The list of ingredients.</param>
    /// <param name="quantities">The quantities of each ingredient.</param>
    /// <returns>The total calories.</returns>
    private static long CalculateCalories(List<Ingredient> ingredients, int[] quantities)
    {
        int calories = 0;

        for (int i = 0; i < ingredients.Count; i++)
        {
            calories += ingredients[i].Calories * quantities[i];
        }

        return calories;
    }

    /// <summary>
    /// Calculates the total score based on the given ingredients, their quantities, and an optional target calorie count.
    /// </summary>
    /// <param name="ingredients">The list of ingredients.</param>
    /// <param name="quantities">The quantities of each ingredient.</param>
    /// <param name="targetCalories">The optional target calorie count.</param>
    /// <returns>The total score.</returns>
    private static long CalculateTotalScore(List<Ingredient> ingredients, int[] quantities, int? targetCalories)
    {
        int capacity = 0, durability = 0, flavor = 0, texture = 0, calories = 0;

        for (int i = 0; i < ingredients.Count; i++)
        {
            capacity += ingredients[i].Capacity * quantities[i];
            durability += ingredients[i].Durability * quantities[i];
            flavor += ingredients[i].Flavor * quantities[i];
            texture += ingredients[i].Texture * quantities[i];
            calories += ingredients[i].Calories * quantities[i];
        }

        if (targetCalories != null && calories != targetCalories)
            return 0;

        return Math.Max(capacity, 0) * Math.Max(durability, 0) * Math.Max(flavor, 0) * Math.Max(texture, 0);
    }

    /// <summary>
    /// Parses the input span to extract ingredient information and returns a list of Ingredient records.
    /// </summary>
    /// <param name="span">The input span containing ingredient information.</param>
    /// <returns>A list of Ingredient records.</returns>
    private static List<Ingredient> ParseIngredients(ReadOnlySpan<char> span)
    {
        List<Ingredient> ingredients = [];

        Span<Range> tokens = stackalloc Range[12];
        foreach (ReadOnlySpan<char> line in span.EnumerateLines())
        {
            if (line.Split(tokens, ' ') != 11)
                continue;

            ingredients.Add(new Ingredient(
                int.Parse(line[tokens[2]][..^1]),
                int.Parse(line[tokens[4]][..^1]),
                int.Parse(line[tokens[6]][..^1]),
                int.Parse(line[tokens[8]][..^1]),
                int.Parse(line[tokens[10]])));
        }

        return ingredients;
    }

    /// <summary>
    /// Represents an ingredient with its properties: capacity, durability, flavor, texture, and calories.
    /// </summary>
    /// <param name="Capacity">The capacity of the ingredient.</param>
    /// <param name="Durability">The durability of the ingredient.</param>
    /// <param name="Flavor">The flavor of the ingredient.</param>
    /// <param name="Texture">The texture of the ingredient.</param>
    /// <param name="Calories">The calories of the ingredient.</param>
    private readonly record struct Ingredient(int Capacity, int Durability, int Flavor, int Texture, int Calories);
}
