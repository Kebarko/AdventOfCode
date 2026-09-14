using KE.AoC.Core.Solution;
using KE.AoC.Solutions.Common;
using System.Globalization;

namespace KE.AoC.Solutions.Y2025;

/// <summary>
/// --- Day 12: Christmas Tree Farm ---
/// </summary>
[Solution(2025, 12)]
public sealed class Day12 : SolutionBase
{
    /// <summary>
    /// Counts the number of regions that can be filled with the given shapes.
    /// </summary>
    public override object PartOne(string input)
    {
        (List<Grid<bool>> shapes, List<Region> regions) = Parse(input.AsSpan());

        return regions.Count(region => IsRegionValid(region, shapes));
    }

    /// <summary>
    /// Counts the number of regions that can be filled with the given shapes,
    /// considering only square shapes.
    /// </summary>
    /// <param name="region">The region to check.</param>
    /// <param name="shapes">The list of shapes to consider.</param>
    /// <returns>true if the region can be filled with the given shapes; otherwise, false.</returns>
    /// <exception cref="NotImplementedException">
    /// Thrown when the rest of the validation logic is not implemented.
    /// </exception>
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

    /// <summary>
    /// Parses the input string into a list of shapes and a list of regions.
    /// </summary>
    /// <param name="span">The span containing the input string.</param>
    /// <returns>A tuple containing the list of shapes and the list of regions.</returns>
    private static (List<Grid<bool>> Shapes, List<Region> Regions) Parse(ReadOnlySpan<char> span)
    {
        List<Grid<bool>> shapes = [];
        List<Region> regions = [];
        List<string> shapeLines = [];

        foreach (ReadOnlySpan<char> line in span.EnumerateLines())
        {
            if (line.IsEmpty)
                continue;

            if (!line.Contains(':'))
            {
                shapeLines.Add(line.ToString());
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

    /// <summary>
    /// Represents a rectangular region with a specified width, height,
    /// and a list of quantities for different shapes.
    /// </summary>
    /// <param name="Width">The width of the region.</param>
    /// <param name="Height">The height of the region.</param>
    /// <param name="Quantity">The list of quantities for different shapes.</param>
    private record Region(int Width, int Height, List<int> Quantity)
    {
        public int Area => Width * Height;

        /// <summary>
        /// Parses a span of characters representing a region
        /// in the format "WidthxHeight:Quantity1,Quantity2,...".
        /// </summary>
        /// <param name="span">The span containing the region definition.</param>
        /// <returns>The parsed region.</returns>
        public static Region Parse(ReadOnlySpan<char> span)
        {
            int x = span.IndexOf('x');
            int colon = span.IndexOf(':');

            int width = int.Parse(span[..x], NumberStyles.None, NumberFormatInfo.InvariantInfo);
            int height = int.Parse(span[(x + 1)..colon], NumberStyles.None, NumberFormatInfo.InvariantInfo);
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
