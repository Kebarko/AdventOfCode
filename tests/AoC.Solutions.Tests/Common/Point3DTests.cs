using KE.AoC.Solutions.Common;
using System.Numerics;

namespace KE.AoC.Solutions.Tests.Common;

public static class Point3DTests
{
    #region Record semantics

    public class RecordSemantics
    {
        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(1, 2, 3)]
        [InlineData(-5, 7, -9)]
        [InlineData(int.MaxValue, int.MinValue, 0)]
        public void Constructor_AssignsCoordinates(int x, int y, int z)
        {
            var point = new Point3D<int>(x, y, z);

            Assert.Equal(x, point.X);
            Assert.Equal(y, point.Y);
            Assert.Equal(z, point.Z);
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(1, 2, 3)]
        [InlineData(-1, -2, -3)]
        public void Equality_SameCoordinates_AreEqual(int x, int y, int z)
        {
            var point1 = new Point3D<int>(x, y, z);
            var point2 = new Point3D<int>(x, y, z);

            Assert.Equal(point1, point2);
            Assert.True(point1 == point2);
            Assert.False(point1 != point2);
            Assert.Equal(point1.GetHashCode(), point2.GetHashCode());
        }

        [Theory]
        [InlineData(9, 2, 3)] // differs in X
        [InlineData(1, 9, 3)] // differs in Y
        [InlineData(1, 2, 9)] // differs in Z
        public void Equality_DifferentCoordinates_AreNotEqual(int x, int y, int z)
        {
            var point1 = new Point3D<int>(1, 2, 3);
            var point2 = new Point3D<int>(x, y, z);

            Assert.NotEqual(point1, point2);
            Assert.False(point1 == point2);
            Assert.True(point1 != point2);
        }

        [Fact]
        public void Default_EqualsOriginPoint()
        {
            Assert.Equal(new Point3D<int>(0, 0, 0), default(Point3D<int>));
        }

        [Fact]
        public void WithExpression_ReplacesSingleCoordinate()
        {
            var point = new Point3D<int>(1, 2, 3);

            var modified = point with { Y = 42 };

            Assert.Equal(new Point3D<int>(1, 42, 3), modified);
            Assert.Equal(new Point3D<int>(1, 2, 3), point); // original unchanged
        }

        [Fact]
        public void Deconstruct_YieldsCoordinatesInOrder()
        {
            var (x, y, z) = new Point3D<int>(1, 2, 3);

            Assert.Equal(1, x);
            Assert.Equal(2, y);
            Assert.Equal(3, z);
        }
    }

    #endregion

    #region GetDistance

    public class GetDistance
    {
        private const int Precision = 12;

        [Theory]
        [InlineData(0, 0, 0, 0, 0, 0, 0.0)]
        [InlineData(1, 2, 3, 1, 2, 3, 0.0)]              // identical non-origin points
        [InlineData(0, 0, 0, 5, 0, 0, 5.0)]              // axis-aligned X
        [InlineData(0, 0, 0, 0, 5, 0, 5.0)]              // axis-aligned Y
        [InlineData(0, 0, 0, 0, 0, 5, 5.0)]              // axis-aligned Z
        [InlineData(0, 0, 0, 1, 2, 2, 3.0)]              // (1,2,2) Pythagorean triple
        [InlineData(0, 0, 0, 2, 3, 6, 7.0)]              // (2,3,6) Pythagorean triple
        [InlineData(1, 1, 1, 3, 4, 7, 7.0)]              // translated triple
        [InlineData(0, 0, 0, -1, -2, -2, 3.0)]           // negative coordinates
        [InlineData(-1, -2, -2, 1, 2, 2, 6.0)]           // spanning the origin: (2,4,4)
        public void KnownIntPoints_ReturnsExpected(
            int x1, int y1, int z1, int x2, int y2, int z2, double expected)
        {
            var point1 = new Point3D<int>(x1, y1, z1);
            var point2 = new Point3D<int>(x2, y2, z2);

            Assert.Equal(expected, point1.GetDistance(point2), Precision);
        }

        [Theory]
        [InlineData(0.5, 0.5, 0.5, 1.5, 2.5, 3.5, 3.7416573867739413)] // sqrt(14)
        [InlineData(0.0, 0.0, 0.0, 0.1, 0.2, 0.2, 0.3)]
        public void KnownDoublePoints_ReturnsExpected(
            double x1, double y1, double z1, double x2, double y2, double z2, double expected)
        {
            var point1 = new Point3D<double>(x1, y1, z1);
            var point2 = new Point3D<double>(x2, y2, z2);

            Assert.Equal(expected, point1.GetDistance(point2), Precision);
        }

        [Theory]
        [InlineData(0u, 0u, 0u, 2u, 3u, 6u, 7.0)]
        [InlineData(10u, 10u, 10u, 8u, 7u, 4u, 7.0)] // point1 > point2, must not underflow
        public void UnsignedCoordinates_ReturnsExpected(
            uint x1, uint y1, uint z1, uint x2, uint y2, uint z2, double expected)
        {
            var point1 = new Point3D<uint>(x1, y1, z1);
            var point2 = new Point3D<uint>(x2, y2, z2);

            Assert.Equal(expected, point1.GetDistance(point2), Precision);
        }

        [Theory]
        [InlineData(1, 2, 3, -4, 5, -6)]
        [InlineData(0, 0, 0, 7, 8, 9)]
        public void IsSymmetric(int x1, int y1, int z1, int x2, int y2, int z2)
        {
            var point1 = new Point3D<int>(x1, y1, z1);
            var point2 = new Point3D<int>(x2, y2, z2);

            Assert.Equal(point1.GetDistance(point2), point2.GetDistance(point1));
        }

        [Fact]
        public void InstanceAndStatic_ReturnSameResult()
        {
            var point1 = new Point3D<int>(1, 2, 3);
            var point2 = new Point3D<int>(4, 6, 8);

            Assert.Equal(
                Point3D<int>.GetDistance(point1, point2),
                point1.GetDistance(point2));
        }

        [Fact]
        public void HugeDoubleDeltas_DoNotOverflowToInfinity()
        {
            // Naive sqrt(dx² + dy² + dz²) would overflow (1e200² = 1e400 > double.MaxValue).
            // double.Hypot avoids intermediate overflow.
            var origin = new Point3D<double>(0, 0, 0);
            var far = new Point3D<double>(1e200, 1e200, 1e200);

            double distance = origin.GetDistance(far);
            double expected = 1e200 * Math.Sqrt(3);

            Assert.False(double.IsInfinity(distance));
            Assert.Equal(expected, distance, tolerance: expected * 1e-12);
        }

        [Fact]
        public void LongCoordinatesAbove2Pow53_LosePrecision()
        {
            // Documents a known limitation: double.CreateChecked guards range, not precision.
            // 2^53 + 1 is not representable as a double and rounds to 2^53,
            // so two distinct points appear to coincide.
            var point1 = new Point3D<long>(1L << 53, 0, 0);
            var point2 = new Point3D<long>((1L << 53) + 1, 0, 0);

            Assert.Equal(0.0, point1.GetDistance(point2));
        }
    }

    #endregion

    #region GetSquaredDistance

    public class GetSquaredDistance
    {
        [Theory]
        [InlineData(0, 0, 0, 0, 0, 0, 0)]
        [InlineData(1, 2, 3, 1, 2, 3, 0)]        // identical points
        [InlineData(0, 0, 0, 1, 2, 3, 14)]
        [InlineData(0, 0, 0, 2, 3, 6, 49)]
        [InlineData(0, 0, 0, -1, -2, -3, 14)]    // negative coordinates
        [InlineData(-1, -2, -3, 1, 2, 3, 56)]    // spanning the origin: 4 + 16 + 36
        [InlineData(0, 0, 0, 5, 0, 0, 25)]       // axis-aligned
        public void KnownIntPoints_ReturnsExpected(
            int x1, int y1, int z1, int x2, int y2, int z2, int expected)
        {
            var point1 = new Point3D<int>(x1, y1, z1);
            var point2 = new Point3D<int>(x2, y2, z2);

            Assert.Equal(expected, point1.GetSquaredDistance<int>(point2));
        }

        [Fact]
        public void WiderResultType_AvoidsOverflowOfSourceType()
        {
            // Per-axis squared delta is 2.5e9 (> int.MaxValue); deltas must be
            // converted to TResult before arithmetic for this to work.
            var point1 = new Point3D<int>(0, 0, 0);
            var point2 = new Point3D<int>(50_000, 50_000, 50_000);

            Assert.Equal(7_500_000_000L, point1.GetSquaredDistance<long>(point2));
        }

        [Fact]
        public void DoubleResultType_WithIntCoordinates_ReturnsExpected()
        {
            var point1 = new Point3D<int>(0, 0, 0);
            var point2 = new Point3D<int>(1, 2, 3);

            Assert.Equal(14.0, point1.GetSquaredDistance<double>(point2));
        }

        [Fact]
        public void CoordinateOutsideResultTypeRange_ThrowsOverflowException()
        {
            // TResult.CreateChecked throws when a coordinate cannot be represented.
            var point1 = new Point3D<long>(3_000_000_000L, 0, 0);
            var point2 = new Point3D<long>(0, 0, 0);

            Assert.Throws<OverflowException>(() => point1.GetSquaredDistance<int>(point2));
        }

        [Fact]
        public void UnsignedResultType_WithReversedOperands_StillCorrect()
        {
            // dx = 0u - 3u wraps in unsigned arithmetic, but squaring is congruent
            // modulo 2^32, so the final result is still exact as long as the true
            // squared distance fits in TResult. This test pins that behavior.
            var point1 = new Point3D<uint>(0, 0, 0);
            var point2 = new Point3D<uint>(3, 4, 0);

            Assert.Equal(25u, point1.GetSquaredDistance<uint>(point2));
        }

        [Theory]
        [InlineData(1, 2, 3, -4, 5, -6)]
        [InlineData(0, 0, 0, 7, 8, 9)]
        public void IsSymmetric(int x1, int y1, int z1, int x2, int y2, int z2)
        {
            var point1 = new Point3D<int>(x1, y1, z1);
            var point2 = new Point3D<int>(x2, y2, z2);

            Assert.Equal(
                point1.GetSquaredDistance<long>(point2),
                point2.GetSquaredDistance<long>(point1));
        }

        [Fact]
        public void InstanceAndStatic_ReturnSameResult()
        {
            var point1 = new Point3D<int>(1, 2, 3);
            var point2 = new Point3D<int>(4, 6, 8);

            Assert.Equal(
                Point3D<int>.GetSquaredDistance<long>(point1, point2),
                point1.GetSquaredDistance<long>(point2));
        }

        public static TheoryData<Point3D<Int128>, Point3D<Int128>, Int128> Int128Cases => new()
        {
            {
                new Point3D<Int128>(0, 0, 0),
                new Point3D<Int128>(long.MaxValue, 0, 0),
                (Int128)long.MaxValue * long.MaxValue
            },
            {
                new Point3D<Int128>((Int128)long.MaxValue + 10, 5, 5),
                new Point3D<Int128>(long.MaxValue, 5, 5),
                100
            },
        };

        [Theory]
        [MemberData(nameof(Int128Cases))]
        public void Int128Coordinates_ReturnsExpected(
            Point3D<Int128> point1, Point3D<Int128> point2, Int128 expected)
        {
            Assert.Equal(expected, point1.GetSquaredDistance<Int128>(point2));
        }

        public static TheoryData<Point3D<BigInteger>, Point3D<BigInteger>, BigInteger> BigIntegerCases => new()
        {
            {
                new Point3D<BigInteger>(0, 0, 0),
                new Point3D<BigInteger>(1, 2, 3),
                14
            },
            {
                new Point3D<BigInteger>(BigInteger.Pow(10, 30), 0, 0),
                new Point3D<BigInteger>(0, 0, 0),
                BigInteger.Pow(10, 60)
            },
        };

        [Theory]
        [MemberData(nameof(BigIntegerCases))]
        public void BigIntegerCoordinates_ReturnsExpected(
            Point3D<BigInteger> point1, Point3D<BigInteger> point2, BigInteger expected)
        {
            Assert.Equal(expected, point1.GetSquaredDistance<BigInteger>(point2));
        }
    }

    #endregion

    #region GetManhattanDistance

    public class GetManhattanDistance
    {
        [Theory]
        [InlineData(0, 0, 0, 0, 0, 0, 0)]
        [InlineData(1, 2, 3, 1, 2, 3, 0)]         // identical points
        [InlineData(0, 0, 0, 1, 2, 3, 6)]
        [InlineData(1, 2, 3, 0, 0, 0, 6)]         // reversed operands
        [InlineData(0, 0, 0, -1, -2, -3, 6)]      // negative coordinates
        [InlineData(-1, -2, -3, 1, 2, 3, 12)]     // spanning the origin
        [InlineData(10, 0, 0, 0, 0, 0, 10)]       // axis-aligned
        [InlineData(-5, 3, -7, 2, -4, 6, 27)]     // mixed signs: 7 + 7 + 13
        public void KnownIntPoints_ReturnsExpected(
            int x1, int y1, int z1, int x2, int y2, int z2, int expected)
        {
            var point1 = new Point3D<int>(x1, y1, z1);
            var point2 = new Point3D<int>(x2, y2, z2);

            Assert.Equal(expected, point1.GetManhattanDistance(point2));
        }

        [Theory]
        [InlineData((byte)5, (byte)10, (byte)15, (byte)20, (byte)3, (byte)15, (byte)22)] // 15 + 7 + 0
        [InlineData((byte)0, (byte)0, (byte)0, (byte)255, (byte)0, (byte)0, (byte)255)]  // full byte range on one axis
        public void ByteCoordinates_BranchBasedAbsDiff_DoesNotUnderflow(
            byte x1, byte y1, byte z1, byte x2, byte y2, byte z2, byte expected)
        {
            // T.Abs(a - b) would underflow for unsigned types when a < b;
            // the branch-based AbsDiff must handle both operand orders.
            var point1 = new Point3D<byte>(x1, y1, z1);
            var point2 = new Point3D<byte>(x2, y2, z2);

            Assert.Equal(expected, point1.GetManhattanDistance(point2));
            Assert.Equal(expected, point2.GetManhattanDistance(point1));
        }

        [Fact]
        public void UIntCoordinates_NearMaxValue_ReturnsExpected()
        {
            var point1 = new Point3D<uint>(uint.MaxValue, 0, 0);
            var point2 = new Point3D<uint>(uint.MaxValue - 5, 3, 0);

            Assert.Equal(8u, point1.GetManhattanDistance(point2));
        }

        [Fact]
        public void IntCoordinates_NearExtremes_ReturnsExpected()
        {
            // T.Abs would throw for a delta of int.MinValue; branch-based AbsDiff
            // never negates, so deltas near the extremes are safe as long as the
            // true per-axis difference fits in T.
            var point1 = new Point3D<int>(int.MaxValue, int.MinValue, 0);
            var point2 = new Point3D<int>(int.MaxValue - 7, int.MinValue + 9, 0);

            Assert.Equal(16, point1.GetManhattanDistance(point2));
        }

        [Theory]
        [InlineData(0.5, 0.5, 0.5, 1.5, 2.5, 3.5, 6.0)]
        [InlineData(-0.25, 0.0, 0.75, 0.25, -1.0, 0.25, 2.0)]
        public void DoubleCoordinates_ReturnsExpected(
            double x1, double y1, double z1, double x2, double y2, double z2, double expected)
        {
            var point1 = new Point3D<double>(x1, y1, z1);
            var point2 = new Point3D<double>(x2, y2, z2);

            Assert.Equal(expected, point1.GetManhattanDistance(point2), 12);
        }

        [Theory]
        [InlineData(1, 2, 3, -4, 5, -6)]
        [InlineData(0, 0, 0, 7, 8, 9)]
        public void IsSymmetric(int x1, int y1, int z1, int x2, int y2, int z2)
        {
            var point1 = new Point3D<int>(x1, y1, z1);
            var point2 = new Point3D<int>(x2, y2, z2);

            Assert.Equal(
                point1.GetManhattanDistance(point2),
                point2.GetManhattanDistance(point1));
        }

        [Fact]
        public void InstanceAndStatic_ReturnSameResult()
        {
            var point1 = new Point3D<int>(1, 2, 3);
            var point2 = new Point3D<int>(4, 6, 8);

            Assert.Equal(
                Point3D<int>.GetManhattanDistance(point1, point2),
                point1.GetManhattanDistance(point2));
        }

        public static TheoryData<Point3D<Int128>, Point3D<Int128>, Int128> Int128Cases => new()
        {
            {
                new Point3D<Int128>((Int128)long.MaxValue + 10, 0, 0),
                new Point3D<Int128>(long.MaxValue, 0, 0),
                10
            },
            {
                new Point3D<Int128>(0, 0, 0),
                new Point3D<Int128>((Int128)long.MaxValue + 1, 1, 1),
                (Int128)long.MaxValue + 3
            },
        };

        [Theory]
        [MemberData(nameof(Int128Cases))]
        public void Int128Coordinates_ReturnsExpected(
            Point3D<Int128> point1, Point3D<Int128> point2, Int128 expected)
        {
            Assert.Equal(expected, point1.GetManhattanDistance(point2));
        }

        public static TheoryData<Point3D<BigInteger>, Point3D<BigInteger>, BigInteger> BigIntegerCases => new()
        {
            {
                new Point3D<BigInteger>(BigInteger.Pow(10, 30), 0, 0),
                new Point3D<BigInteger>(0, BigInteger.Pow(10, 30), 0),
                2 * BigInteger.Pow(10, 30)
            },
        };

        [Theory]
        [MemberData(nameof(BigIntegerCases))]
        public void BigIntegerCoordinates_ReturnsExpected(
            Point3D<BigInteger> point1, Point3D<BigInteger> point2, BigInteger expected)
        {
            Assert.Equal(expected, point1.GetManhattanDistance(point2));
        }
    }

    #endregion
}
