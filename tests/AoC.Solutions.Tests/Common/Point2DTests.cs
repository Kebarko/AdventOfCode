using KE.AoC.Solutions.Common;
using System.Numerics;

namespace KE.AoC.Solutions.Tests.Common;

public static class Point2DTests
{
    private const double Tolerance = 1e-9;

    public class GetDistance
    {
        [Theory]
        [InlineData(0, 0, 0, 0, 0.0)]
        [InlineData(0, 0, 3, 4, 5.0)]
        [InlineData(3, 4, 0, 0, 5.0)]
        [InlineData(-3, -4, 0, 0, 5.0)]
        [InlineData(-2, -3, 4, 5, 10.0)]
        [InlineData(2, 3, 9, 3, 7.0)]   // horizontal
        [InlineData(2, 3, 2, 10, 7.0)]  // vertical
        public void Int_ReturnsEuclideanDistance(int x1, int y1, int x2, int y2, double expected)
        {
            var point1 = new Point2D<int>(x1, y1);
            var point2 = new Point2D<int>(x2, y2);

            Assert.Equal(expected, point1.GetDistance(point2), Tolerance);
        }

        [Theory]
        [InlineData(0.0, 0.0, 1.0, 1.0, 1.4142135623730951)] // sqrt(2)
        [InlineData(0.5, 0.5, 2.5, 2.0, 2.5)]
        [InlineData(-1.5, -2.5, 1.5, 1.5, 5.0)]
        public void Double_ReturnsEuclideanDistance(double x1, double y1, double x2, double y2, double expected)
        {
            var point1 = new Point2D<double>(x1, y1);
            var point2 = new Point2D<double>(x2, y2);

            Assert.Equal(expected, point1.GetDistance(point2), Tolerance);
        }

        // Safe for unsigned types in either argument order, because coordinates
        // are converted to double before subtraction.
        [Theory]
        [InlineData(0u, 0u, 3u, 4u, 5.0)]
        [InlineData(3u, 4u, 0u, 0u, 5.0)]
        [InlineData(uint.MaxValue, 0u, uint.MaxValue, 0u, 0.0)]
        public void UInt_NoUnderflow(uint x1, uint y1, uint x2, uint y2, double expected)
        {
            var point1 = new Point2D<uint>(x1, y1);
            var point2 = new Point2D<uint>(x2, y2);

            Assert.Equal(expected, point1.GetDistance(point2), Tolerance);
        }

        // double.Hypot avoids intermediate overflow (dx * dx would be Infinity)
        // and underflow (dx * dx would flush to zero).
        [Theory]
        [InlineData(1e200, 1e200, 1.4142135623730951e200)]
        [InlineData(1e-200, 1e-200, 1.4142135623730951e-200)]
        public void Double_ExtremeMagnitudes_HypotAvoidsOverflowAndUnderflow(double x, double y, double expected)
        {
            var origin = new Point2D<double>(0.0, 0.0);
            var point = new Point2D<double>(x, y);

            double actual = origin.GetDistance(point);

            Assert.True(double.IsFinite(actual));
            Assert.Equal(expected, actual, expected * 1e-12);
        }

        [Theory]
        [InlineData(double.NaN, 0.0)]
        [InlineData(0.0, double.NaN)]
        public void Double_NaNCoordinate_PropagatesNaN(double x, double y)
        {
            var point1 = new Point2D<double>(x, y);
            var point2 = new Point2D<double>(0.0, 0.0);

            Assert.True(double.IsNaN(point1.GetDistance(point2)));
        }

        [Theory]
        [InlineData(double.PositiveInfinity, 0.0)]
        [InlineData(0.0, double.NegativeInfinity)]
        public void Double_InfiniteCoordinate_ReturnsPositiveInfinity(double x, double y)
        {
            var point1 = new Point2D<double>(x, y);
            var point2 = new Point2D<double>(0.0, 0.0);

            Assert.Equal(double.PositiveInfinity, point1.GetDistance(point2));
        }

        [Theory]
        [InlineData(0, 0, 3, 4)]
        [InlineData(1, 2, -7, 5)]
        [InlineData(-10, -20, -30, -40)]
        public void Int_IsSymmetric(int x1, int y1, int x2, int y2)
        {
            var point1 = new Point2D<int>(x1, y1);
            var point2 = new Point2D<int>(x2, y2);

            Assert.Equal(
                Point2D<int>.GetDistance(point1, point2),
                Point2D<int>.GetDistance(point2, point1));
        }

        [Theory]
        [InlineData(0, 0, 3, 4)]
        [InlineData(-2, -3, 4, 5)]
        public void InstanceMethod_MatchesStaticMethod(int x1, int y1, int x2, int y2)
        {
            var point1 = new Point2D<int>(x1, y1);
            var point2 = new Point2D<int>(x2, y2);

            Assert.Equal(Point2D<int>.GetDistance(point1, point2), point1.GetDistance(point2));
        }
    }

    public class GetSquaredDistance
    {
        [Theory]
        [InlineData(0, 0, 0, 0, 0)]
        [InlineData(0, 0, 3, 4, 25)]
        [InlineData(3, 4, 0, 0, 25)]
        [InlineData(-3, -4, 3, 4, 100)]
        [InlineData(1, 2, 4, 6, 25)]
        public void Int_ToInt_ReturnsSquaredDistance(int x1, int y1, int x2, int y2, int expected)
        {
            var point1 = new Point2D<int>(x1, y1);
            var point2 = new Point2D<int>(x2, y2);

            Assert.Equal(expected, point1.GetSquaredDistance<int>(point2));
        }

        // TResult wider than T: the squared value would overflow int,
        // but conversion happens before arithmetic, so long is safe.
        [Theory]
        [InlineData(0, 0, 100_000, 100_000, 20_000_000_000L)]
        [InlineData(-50_000, 0, 50_000, 0, 10_000_000_000L)]
        [InlineData(int.MaxValue, 0, 0, 0, 4_611_686_014_132_420_609L)] // (2^31 - 1)^2
        public void Int_ToLong_WidensBeforeArithmetic(int x1, int y1, int x2, int y2, long expected)
        {
            var point1 = new Point2D<int>(x1, y1);
            var point2 = new Point2D<int>(x2, y2);

            Assert.Equal(expected, point1.GetSquaredDistance<long>(point2));
        }

        // Unsigned T with a signed TResult: safe in either argument order,
        // because conversion precedes subtraction.
        [Theory]
        [InlineData(1u, 1u, 4u, 5u, 25)]
        [InlineData(4u, 5u, 1u, 1u, 25)]
        public void UInt_ToInt_NoUnderflow(uint x1, uint y1, uint x2, uint y2, int expected)
        {
            var point1 = new Point2D<uint>(x1, y1);
            var point2 = new Point2D<uint>(x2, y2);

            Assert.Equal(expected, point1.GetSquaredDistance<int>(point2));
        }

        // Unsigned TResult with point1 < point2: dx wraps around, but squaring
        // in modular arithmetic yields the correct result as long as the true
        // squared distance fits in TResult: ((a - b) mod 2^32)^2 mod 2^32 == (a - b)^2.
        [Theory]
        [InlineData(1u, 1u, 3u, 3u, 8u)]
        [InlineData(0u, 10u, 5u, 0u, 125u)]
        public void UInt_ToUInt_WrapAroundStillYieldsCorrectResult(uint x1, uint y1, uint x2, uint y2, uint expected)
        {
            var point1 = new Point2D<uint>(x1, y1);
            var point2 = new Point2D<uint>(x2, y2);

            Assert.Equal(expected, point1.GetSquaredDistance<uint>(point2));
        }

        [Theory]
        [InlineData(0.5, 0.5, 1.5, 2.5, 5.0)]
        [InlineData(-1.5, 0.0, 1.5, 0.0, 9.0)]
        public void Double_ToDouble_HandlesFractionalCoordinates(double x1, double y1, double x2, double y2, double expected)
        {
            var point1 = new Point2D<double>(x1, y1);
            var point2 = new Point2D<double>(x2, y2);

            Assert.Equal(expected, point1.GetSquaredDistance<double>(point2), Tolerance);
        }

        // CreateChecked throws when a coordinate does not fit into TResult.
        [Theory]
        [InlineData(long.MaxValue, 0L)]
        [InlineData(0L, long.MinValue)]
        public void Long_ToInt_CoordinateOutOfRange_ThrowsOverflowException(long x, long y)
        {
            var point1 = new Point2D<long>(x, y);
            var point2 = new Point2D<long>(0, 0);

            Assert.Throws<OverflowException>(() => point1.GetSquaredDistance<int>(point2));
        }

        public static TheoryData<long, long, long, long, Int128> Int128Cases => new()
        {
            // 1.6e19 + 9e18 = 2.5e19, exceeds both long.MaxValue and ulong.MaxValue
            { 0L, 0L, 4_000_000_000L, 3_000_000_000L, Int128.Parse("25000000000000000000") },
            { -4_000_000_000L, 0L, 0L, 3_000_000_000L, Int128.Parse("25000000000000000000") },
        };

        [Theory]
        [MemberData(nameof(Int128Cases))]
        public void Long_ToInt128_ResultExceedsLongRange(long x1, long y1, long x2, long y2, Int128 expected)
        {
            var point1 = new Point2D<long>(x1, y1);
            var point2 = new Point2D<long>(x2, y2);

            Assert.Equal(expected, point1.GetSquaredDistance<Int128>(point2));
        }

        public static TheoryData<long, long, long, long, BigInteger> BigIntegerCases => new()
        {
            {
                long.MaxValue, long.MaxValue, 0L, 0L,
                2 * BigInteger.Pow(long.MaxValue, 2)
            },
            {
                // dx spans the entire long range (2^64 - 1); exact only because
                // conversion to BigInteger happens before the subtraction.
                long.MinValue, 0L, long.MaxValue, 0L,
                BigInteger.Pow((BigInteger)long.MaxValue - long.MinValue, 2)
            },
        };

        [Theory]
        [MemberData(nameof(BigIntegerCases))]
        public void Long_ToBigInteger_IsExactForFullLongRange(long x1, long y1, long x2, long y2, BigInteger expected)
        {
            var point1 = new Point2D<long>(x1, y1);
            var point2 = new Point2D<long>(x2, y2);

            Assert.Equal(expected, point1.GetSquaredDistance<BigInteger>(point2));
        }

        [Theory]
        [InlineData(0, 0, 3, 4)]
        [InlineData(-2, -3, 4, 5)]
        [InlineData(7, -1, -8, 12)]
        public void SqrtOfSquaredDistance_MatchesGetDistance(int x1, int y1, int x2, int y2)
        {
            var point1 = new Point2D<int>(x1, y1);
            var point2 = new Point2D<int>(x2, y2);

            Assert.Equal(
                point1.GetDistance(point2),
                Math.Sqrt(point1.GetSquaredDistance<long>(point2)),
                Tolerance);
        }

        [Theory]
        [InlineData(0, 0, 3, 4)]
        [InlineData(-2, -3, 4, 5)]
        public void InstanceMethod_MatchesStaticMethod(int x1, int y1, int x2, int y2)
        {
            var point1 = new Point2D<int>(x1, y1);
            var point2 = new Point2D<int>(x2, y2);

            Assert.Equal(
                Point2D<int>.GetSquaredDistance<long>(point1, point2),
                point1.GetSquaredDistance<long>(point2));
        }
    }

    public class GetManhattanDistance
    {
        [Theory]
        [InlineData(0, 0, 0, 0, 0)]
        [InlineData(0, 0, 3, 4, 7)]
        [InlineData(3, 4, 0, 0, 7)]
        [InlineData(-2, -3, 4, 5, 14)]
        [InlineData(5, 5, 2, 9, 7)]
        [InlineData(int.MinValue, 0, -1, 0, int.MaxValue)] // AbsDiff avoids Abs(MinValue) overflow
        [InlineData(int.MinValue, int.MinValue, int.MinValue, int.MinValue, 0)]
        public void Int_ReturnsManhattanDistance(int x1, int y1, int x2, int y2, int expected)
        {
            var point1 = new Point2D<int>(x1, y1);
            var point2 = new Point2D<int>(x2, y2);

            Assert.Equal(expected, point1.GetManhattanDistance(point2));
        }

        // The branch-based AbsDiff must not underflow for unsigned types,
        // regardless of argument order.
        [Theory]
        [InlineData(1u, 2u, 10u, 20u, 27u)]
        [InlineData(10u, 20u, 1u, 2u, 27u)]
        [InlineData(0u, 0u, uint.MaxValue, 0u, uint.MaxValue)]
        [InlineData(uint.MaxValue, 0u, 0u, 0u, uint.MaxValue)]
        public void UInt_NoUnderflow(uint x1, uint y1, uint x2, uint y2, uint expected)
        {
            var point1 = new Point2D<uint>(x1, y1);
            var point2 = new Point2D<uint>(x2, y2);

            Assert.Equal(expected, point1.GetManhattanDistance(point2));
        }

        [Theory]
        [InlineData(long.MaxValue, 0L, long.MaxValue - 5L, 0L, 5L)]
        [InlineData(long.MinValue, 0L, long.MinValue + 5L, 0L, 5L)]
        public void Long_ExtremeCoordinatesWithSmallDifference(long x1, long y1, long x2, long y2, long expected)
        {
            var point1 = new Point2D<long>(x1, y1);
            var point2 = new Point2D<long>(x2, y2);

            Assert.Equal(expected, point1.GetManhattanDistance(point2));
        }

        [Theory]
        [InlineData(0.5, 0.5, 2.0, 1.0, 2.0)]
        [InlineData(-1.5, -2.5, 1.5, 1.5, 7.0)]
        public void Double_HandlesFractionalCoordinates(double x1, double y1, double x2, double y2, double expected)
        {
            var point1 = new Point2D<double>(x1, y1);
            var point2 = new Point2D<double>(x2, y2);

            Assert.Equal(expected, point1.GetManhattanDistance(point2), Tolerance);
        }

        // NaN fails both branches' comparisons but propagates through the
        // subtraction either way.
        [Theory]
        [InlineData(double.NaN, 0.0)]
        [InlineData(0.0, double.NaN)]
        public void Double_NaNCoordinate_PropagatesNaN(double x, double y)
        {
            var point1 = new Point2D<double>(x, y);
            var point2 = new Point2D<double>(0.0, 0.0);

            Assert.True(double.IsNaN(point1.GetManhattanDistance(point2)));
        }

        [Theory]
        [InlineData(0, 0, 3, 4)]
        [InlineData(1, 2, -7, 5)]
        [InlineData(-10, -20, -30, -40)]
        public void Int_IsSymmetric(int x1, int y1, int x2, int y2)
        {
            var point1 = new Point2D<int>(x1, y1);
            var point2 = new Point2D<int>(x2, y2);

            Assert.Equal(
                Point2D<int>.GetManhattanDistance(point1, point2),
                Point2D<int>.GetManhattanDistance(point2, point1));
        }

        [Theory]
        [InlineData(0, 0, 3, 4)]
        [InlineData(-2, -3, 4, 5)]
        public void InstanceMethod_MatchesStaticMethod(int x1, int y1, int x2, int y2)
        {
            var point1 = new Point2D<int>(x1, y1);
            var point2 = new Point2D<int>(x2, y2);

            Assert.Equal(
                Point2D<int>.GetManhattanDistance(point1, point2),
                point1.GetManhattanDistance(point2));
        }
    }

    public class RecordSemantics
    {
        [Theory]
        [InlineData(1, 2, 1, 2, true)]
        [InlineData(0, 0, 0, 0, true)]
        [InlineData(1, 2, 2, 1, false)] // swapped coordinates are not equal
        [InlineData(1, 2, 1, 3, false)]
        [InlineData(-1, 2, 1, 2, false)]
        public void ValueEquality(int x1, int y1, int x2, int y2, bool expected)
        {
            var point1 = new Point2D<int>(x1, y1);
            var point2 = new Point2D<int>(x2, y2);

            Assert.Equal(expected, point1 == point2);
            Assert.Equal(expected, point1.Equals(point2));
            Assert.Equal(!expected, point1 != point2);

            if (expected)
            {
                Assert.Equal(point1.GetHashCode(), point2.GetHashCode());
            }
        }

        [Fact]
        public void Default_IsOrigin()
        {
            Assert.Equal(new Point2D<int>(0, 0), default);
        }

        [Fact]
        public void Deconstruct_YieldsCoordinates()
        {
            var (x, y) = new Point2D<int>(3, -7);

            Assert.Equal(3, x);
            Assert.Equal(-7, y);
        }
    }
}
