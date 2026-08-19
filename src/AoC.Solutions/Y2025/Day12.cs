using KE.AoC.Core.Solution;
using KE.AoC.Solutions.Common;

namespace KE.AoC.Solutions.Y2025;

[Solution(2025, 12)]
public sealed class Day12 : SolutionBase
{
    /// <summary>
    /// Counts the number of regions that can be filled with the given shapes.
    /// </summary>
    public override object PartOne(string input)
    {
        (List<Grid<bool>> shapes, List<Region> regions) = Parse(input);

        return regions.Count(region => IsRegionValid(region, shapes));
    }

    private static bool IsRegionValid(Region region, List<Grid<bool>> shapes)
    {
        if (shapes.All(s => s.Width == s.Height))
        {
            int shapeSize = shapes[0].Width;
            int shapesPerRow = region.Width / shapeSize;
            int shapesPerCol = region.Height / shapeSize;

            if (shapesPerRow * shapesPerCol >= region.Quantity.Sum())
            {
                return true;
            }

            int shapesNetArea = region.Quantity
                .Zip(shapes)
                .Sum(pair => pair.First * pair.Second.Count(c => c));

            if (shapesNetArea > region.Area)
            {
                return false;
            }
        }

        throw new NotImplementedException();
    }

    private static (List<Grid<bool>> Shapes, List<Region> Regions) Parse(string input)
    {
        List<Grid<bool>> shapes = [];
        List<Region> regions = [];
        List<string> shapeLines = [];

        foreach (string line in Lines(input))
        {
            if (!line.Contains(':'))
            {
                shapeLines.Add(line);
            }
            else if (!line.EndsWith(':'))
            {
                regions.Add(Region.Parse(line));
            }
            else
            {
                FlushShape();
            }
        }

        FlushShape();

        return (shapes, regions);

        void FlushShape()
        {
            if (shapeLines.Count == 0)
                return;

            shapes.Add(Grid.OfBools(shapeLines, '#'));
            shapeLines.Clear();
        }
    }

    private record Region(int Width, int Height, List<int> Quantity)
    {
        public int Area => Width * Height;

        public static Region Parse(ReadOnlySpan<char> span)
        {
            int x = span.IndexOf('x');
            int colon = span.IndexOf(':');

            int width = int.Parse(span[..x]);
            int height = int.Parse(span[(x + 1)..colon]);
            List<int> quantity = [];

            int value = 0;
            bool inNumber = false;

            foreach (char c in span[(colon + 1)..])
            {
                if (char.IsAsciiDigit(c))
                {
                    value = value * 10 + (c - '0');
                    inNumber = true;
                }
                else if (inNumber)
                {
                    quantity.Add(value);
                    value = 0;
                    inNumber = false;
                }
            }

            if (inNumber)
            {
                quantity.Add(value);
            }

            return new Region(width, height, quantity);
        }
    }
}
