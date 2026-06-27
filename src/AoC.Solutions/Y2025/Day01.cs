using KE.AoC.Core.Solution;

namespace KE.AoC.Solutions.Y2025
{
    [Solution(2025, 1)]
    public sealed class Day01 : SolutionBase
    {
        public override object PartOne(string input)
        {
            string[] rotations = Lines(input);

            const int total = 100;
            int current = 50;
            int zeros = 0;
            foreach (string rotation in rotations)
            {
                char direction = rotation[0];
                int distance = int.Parse(rotation[1..]);

                switch (direction)
                {
                    case 'R':
                        current = (current + distance) % total;
                        break;

                    case 'L':
                        current = ((current - distance) % total + total) % total;
                        break;
                }

                if (current == 0)
                {
                    zeros++;
                }
            }

            return zeros;
        }

        public override object PartTwo(string input)
        {
            string[] rotations = Lines(input);

            const int total = 100;
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
                            int distToZero = total - current;
                            if (distance >= distToZero)
                            {
                                zeros += 1 + (distance - distToZero) / total;
                            }

                            current = (current + distance) % total;
                            break;
                        }
                    case 'L':
                        {
                            int distToZero = current == 0 ? total : current;

                            if (distance >= distToZero)
                            {
                                zeros += 1 + (distance - distToZero) / total;
                            }

                            current = ((current - distance) % total + total) % total;
                            break;
                        }
                }
            }

            return zeros;
        }
    }
}
