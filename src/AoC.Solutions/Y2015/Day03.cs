using KE.AoC.Core.Solution;

namespace KE.AoC.Solutions.Y2015;

[Solution(2015, 3)]
public sealed class Day03 : SolutionBase
{
    /// <summary>
    /// Calculates the number of unique positions visited by Santa based on the input string of directions.
    /// </summary>
    /// <param name="input">The input string of directions.</param>
    /// <returns>The number of unique positions visited.</returns>
    public override object PartOne(string input)
    {
        HashSet<(int, int)> posMap = [];

        (int X, int Y) pos = (0, 0);
        posMap.Add(pos);

        foreach (char c in input)
        {
            pos = Move(pos, c);

            posMap.Add(pos);
        }

        return posMap.Count;
    }

    /// <summary>
    /// Calculates the number of unique positions visited by Santa and Robo-Santa based on the input string of directions, where they take turns moving.
    /// </summary>
    /// <param name="input">The input string of directions.</param>
    /// <returns>The number of unique positions visited.</returns>
    public override object PartTwo(string input)
    {
        HashSet<(int, int)> posMap = [];

        (int X, int Y) pos1 = (0, 0);
        (int X, int Y) pos2 = pos1;
        posMap.Add(pos1);

        bool isRobo = false;
        foreach (char c in input)
        {
            if (isRobo)
            {
                pos2 = Move(pos2, c);
                posMap.Add(pos2);
            }
            else
            {
                pos1 = Move(pos1, c);
                posMap.Add(pos1);
            }

            isRobo = !isRobo;
        }

        return posMap.Count;
    }

    /// <summary>
    /// Moves the given position in the specified direction.
    /// </summary>
    /// <param name="pos">The current position.</param>
    /// <param name="dir">The direction to move.</param>
    /// <returns>The new position after the move.</returns>
    private static (int X, int Y) Move((int X, int Y) pos, char dir)
    {
        return dir switch
        {
            '>' => (pos.X + 1, pos.Y),
            '<' => (pos.X - 1, pos.Y),
            '^' => (pos.X, pos.Y + 1),
            'v' => (pos.X, pos.Y - 1),
            _ => pos
        };
    }
}
