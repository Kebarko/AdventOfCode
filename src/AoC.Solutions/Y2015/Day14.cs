using KE.AoC.Core.Solution;
using System.Globalization;

namespace KE.AoC.Solutions.Y2015;

[Solution(2015, 14)]
public sealed class Day14 : SolutionBase
{
    private const int RaceDuration = 2503;

    /// <summary>
    /// Calculates the maximum distance traveled by any reindeer after the race duration.
    /// </summary>
    /// <param name="input">The input string containing the reindeer descriptions.</param>
    /// <returns>The maximum distance traveled by any reindeer.</returns>
    public override object PartOne(string input)
    {
        List<Reindeer> herd = ParseHerd(input.AsSpan());

        return herd.Max(r => r.CalculateDistance(RaceDuration));
    }

    /// <summary>
    /// Calculates the maximum points earned by any reindeer after the race duration.
    /// </summary>
    /// <param name="input">The input string containing the reindeer descriptions.</param>
    /// <returns>The maximum points earned by any reindeer.</returns>
    public override object PartTwo(string input)
    {
        List<Reindeer> herd = ParseHerd(input.AsSpan());

        int[] distances = new int[herd.Count];
        int[] points = new int[herd.Count];

        for (int time = 1; time <= RaceDuration; time++)
        {
            int maxDistance = 0;

            for (int i = 0; i < herd.Count; i++)
            {
                distances[i] = herd[i].CalculateDistance(time);

                if (distances[i] > maxDistance)
                    maxDistance = distances[i];
            }

            for (int i = 0; i < herd.Count; i++)
            {
                if (distances[i] == maxDistance)
                    points[i]++;
            }
        }

        return points.Max();
    }

    /// <summary>
    /// Parses the input span into a list of Reindeer objects.
    /// </summary>
    /// <param name="span">The input span containing the reindeer descriptions.</param>
    /// <returns>A list of Reindeer objects.</returns>
    private static List<Reindeer> ParseHerd(ReadOnlySpan<char> span)
    {
        List<Reindeer> herd = [];

        Span<Range> tokens = stackalloc Range[16];
        foreach (ReadOnlySpan<char> line in span.EnumerateLines())
        {
            if (line.Split(tokens, ' ') != 15)
                continue;

            herd.Add(new Reindeer(
                int.Parse(line[tokens[3]], NumberStyles.None, NumberFormatInfo.InvariantInfo),
                int.Parse(line[tokens[6]], NumberStyles.None, NumberFormatInfo.InvariantInfo),
                int.Parse(line[tokens[13]], NumberStyles.None, NumberFormatInfo.InvariantInfo)));
        }

        return herd;
    }

    /// <summary>
    /// Represents a reindeer with its speed, flying time, and resting time.
    /// </summary>
    /// <param name="Speed">The speed of the reindeer in km/s.</param>
    /// <param name="Fly">The duration in seconds the reindeer can fly before resting.</param>
    /// <param name="Rest">The duration in seconds the reindeer must rest after flying.</param>
    private readonly record struct Reindeer(int Speed, int Fly, int Rest)
    {
        public int CalculateDistance(int time)
        {
            int cycle = Fly + Rest;

            return (time / cycle * Fly + Math.Min(time % cycle, Fly)) * Speed;
        }
    }
}
