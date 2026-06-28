using KE.AoC.Core.Solution;

namespace KE.AoC.Solutions.Y2025;

[Solution(2025, 1)]
public sealed class Day01 : SolutionBase
{
    /// <summary>
    /// Calculates the number of times the pointer lands on zero after performing a series of rotations.
    /// </summary>
    public override object PartOne(string input)
    {
        string[] rotations = Lines(input);

        const int size = 100;
        int current = 50;
        int zeros = 0;
        foreach (string rotation in rotations)
        {
            char direction = rotation[0];
            int distance = int.Parse(rotation[1..]);

            switch (direction)
            {
                case 'R':
                    current = (current + distance) % size;
                    break;

                case 'L':
                    current = ((current - distance) % size + size) % size;
                    break;
            }

            if (current == 0)
            {
                zeros++;
            }
        }

        return zeros;
    }

    /// <summary>
    /// Calculates the number of times the pointer lands on zero after performing a series of rotations, taking into account multiple full rotations.
    /// </summary>
    public override object PartTwo(string input)
    {
        string[] rotations = Lines(input);

        const int size = 100;
        int current = 50;
        int zeros = 0;
        foreach (string rotation in rotations)
        {
            char direction = rotation[0];
            int distance = int.Parse(rotation[1..]);

            switch (direction)
            {
                case 'R':
                    {
                        int distToZero = size - current;
                        if (distance >= distToZero)
                        {
                            zeros += 1 + (distance - distToZero) / size;
                        }

                        current = (current + distance) % size;
                        break;
                    }
                case 'L':
                    {
                        int distToZero = current == 0 ? size : current;

                        if (distance >= distToZero)
                        {
                            zeros += 1 + (distance - distToZero) / size;
                        }

                        current = ((current - distance) % size + size) % size;
                        break;
                    }
            }
        }

        return zeros;
    }
}
