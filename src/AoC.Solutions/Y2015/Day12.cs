using KE.AoC.Core.Solution;
using System.Text.Json.Nodes;

namespace KE.AoC.Solutions.Y2015;

[Solution(2015, 12)]
public sealed class Day12 : SolutionBase
{
    /// <summary>
    /// Calculates the sum of all integer values in the JSON input, regardless of any specific property values.
    /// </summary>
    /// <param name="input">The JSON input string.</param>
    /// <returns>The sum of all integer values.</returns>
    public override object PartOne(string input)
    {
        JsonNode? node = JsonNode.Parse(input);
        return node != null ? GetSum(node) : 0;
    }

    /// <summary>
    /// Calculates the sum of all integer values in the JSON input, excluding any objects that contain a property with the value "red".
    /// </summary>
    /// <param name="input">The JSON input string.</param>
    /// <returns>The sum of all integer values.</returns>
    public override object PartTwo(string input)
    {
        JsonNode? node = JsonNode.Parse(input);
        return node != null ? GetSum(node, "red") : 0;
    }

    /// <summary>
    /// Gets the sum of the integer values in the given JsonNode, optionally excluding objects with a specific property value.
    /// </summary>
    /// <param name="jsonNode">The JsonNode to sum.</param>
    /// <param name="exclProp">The property value to exclude.</param>
    /// <returns>The sum of the integer values.</returns>
    private static int GetSum(JsonNode? jsonNode, string? exclProp = null)
    {
        return jsonNode switch
        {
            JsonValue jsonVal => GetSum(jsonVal),
            JsonArray jsonArr => GetSum(jsonArr, exclProp),
            JsonObject jsonObj => GetSum(jsonObj, exclProp),
            _ => 0
        };
    }

    /// <summary>
    /// Gets the sum of the integer values in the given JsonObject, optionally excluding objects with a specific property value.
    /// </summary>
    /// <param name="jsonObject">The JsonObject to sum.</param>
    /// <param name="exclProp">The property value to exclude.</param>
    /// <returns>The sum of the integer values.</returns>
    private static int GetSum(JsonObject jsonObject, string? exclProp = null)
    {
        if (!IsValid(jsonObject, exclProp))
            return 0;

        int result = 0;
        foreach (KeyValuePair<string, JsonNode?> property in jsonObject)
            result += GetSum(property.Value, exclProp);

        return result;
    }

    /// <summary>
    /// Gets the sum of the integer values in the given JsonArray, optionally excluding objects with a specific property value.
    /// </summary>
    /// <param name="jsonArray">The JsonArray to sum.</param>
    /// <param name="exclProp">The property value to exclude.</param>
    /// <returns>The sum of the integer values.</returns>
    private static int GetSum(JsonArray jsonArray, string? exclProp = null)
    {
        int result = 0;
        foreach (JsonNode? jsonNode in jsonArray)
            result += GetSum(jsonNode, exclProp);

        return result;
    }

    /// <summary>
    /// Gets the sum of the integer value of the given JsonValue.
    /// </summary>
    /// <param name="jsonValue">The JsonValue to sum.</param>
    /// <returns>The sum of the integer value.</returns>
    private static int GetSum(JsonValue jsonValue)
    {
        return jsonValue.TryGetValue(out int value) ? value : 0;
    }

    /// <summary>
    /// Checks if the given JsonObject is valid based on the exclusion property.
    /// </summary>
    /// <param name="jsonObject">The JsonObject to check.</param>
    /// <param name="exclProp">The property value to exclude.</param>
    /// <returns>True if the JsonObject is valid, false otherwise.</returns>
    private static bool IsValid(JsonObject jsonObject, string? exclProp = null)
    {
        if (exclProp == null)
            return true;

        foreach (KeyValuePair<string, JsonNode?> property in jsonObject)
        {
            if (property.Value is JsonValue jsonValue &&
                jsonValue.TryGetValue(out string? value) &&
                value == exclProp)
            {
                return false;
            }
        }

        return true;
    }
}
