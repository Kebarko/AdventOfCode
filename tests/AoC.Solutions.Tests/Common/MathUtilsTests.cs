using KE.AoC.Solutions.Common;
using System.Numerics;

namespace KE.AoC.Solutions.Tests.Common;

public sealed class MathUtilsTests
{
    // ---------------------------------------------------------------
    // Int32 — boundaries around every power of ten, zero, negatives,
    // and both extremes (MinValue works because the implementation
    // never negates the value; integer division truncates toward zero).
    // ---------------------------------------------------------------

    [Theory]
    [InlineData(0, 1)]                    // zero is a single digit
    [InlineData(1, 1)]
    [InlineData(9, 1)]                    // last 1-digit value
    [InlineData(10, 2)]                   // first 2-digit value
    [InlineData(11, 2)]
    [InlineData(99, 2)]
    [InlineData(100, 3)]
    [InlineData(101, 3)]
    [InlineData(999, 3)]
    [InlineData(1_000, 4)]
    [InlineData(9_999, 4)]
    [InlineData(10_000, 5)]
    [InlineData(99_999, 5)]
    [InlineData(100_000, 6)]
    [InlineData(999_999, 6)]
    [InlineData(1_000_000, 7)]
    [InlineData(9_999_999, 7)]
    [InlineData(10_000_000, 8)]
    [InlineData(99_999_999, 8)]
    [InlineData(100_000_000, 9)]
    [InlineData(999_999_999, 9)]
    [InlineData(1_000_000_000, 10)]
    [InlineData(int.MaxValue, 10)]        // 2 147 483 647
    [InlineData(-1, 1)]
    [InlineData(-9, 1)]
    [InlineData(-10, 2)]
    [InlineData(-99, 2)]
    [InlineData(-100, 3)]
    [InlineData(-999, 3)]
    [InlineData(-1_000, 4)]
    [InlineData(-1_000_000_000, 10)]
    [InlineData(int.MinValue, 10)]        // -2 147 483 648 — no Abs, so no overflow
    public void Digits_Int32_ReturnsExpectedCount(int value, int expected)
    {
        Assert.Equal(expected, MathUtils.Digits(value));
    }

    // ---------------------------------------------------------------
    // Int64 — values beyond the Int32 range and both extremes.
    // ---------------------------------------------------------------

    [Theory]
    [InlineData(0L, 1)]
    [InlineData(9_999_999_999L, 10)]
    [InlineData(10_000_000_000L, 11)]
    [InlineData(999_999_999_999_999_999L, 18)]
    [InlineData(1_000_000_000_000_000_000L, 19)]
    [InlineData(long.MaxValue, 19)]       //  9 223 372 036 854 775 807
    [InlineData(-10_000_000_000L, 11)]
    [InlineData(long.MinValue, 19)]       // -9 223 372 036 854 775 808
    public void Digits_Int64_ReturnsExpectedCount(long value, int expected)
    {
        Assert.Equal(expected, MathUtils.Digits(value));
    }

    // ---------------------------------------------------------------
    // Systematic power-of-ten boundary sweep for Int64:
    // 10^k has k+1 digits; 10^k − 1 has k digits (for k ≥ 1).
    // Catches any off-by-one at every boundary without listing
    // each case by hand.
    // ---------------------------------------------------------------

    public static TheoryData<long, int> PowerOfTenBoundaries()
    {
        var data = new TheoryData<long, int>();
        long power = 1;
        for (int k = 1; k <= 18; k++)
        {
            power *= 10;                  // power == 10^k
            data.Add(power - 1, k);       // e.g. 999 → 3
            data.Add(power, k + 1);       // e.g. 1000 → 4
        }
        return data;
    }

    [Theory]
    [MemberData(nameof(PowerOfTenBoundaries))]
    public void Digits_Int64_PowerOfTenBoundaries(long value, int expected)
    {
        Assert.Equal(expected, MathUtils.Digits(value));
    }

    // ---------------------------------------------------------------
    // UInt64 — including the region above long.MaxValue.
    // ---------------------------------------------------------------

    [Theory]
    [InlineData(0UL, 1)]
    [InlineData(9UL, 1)]
    [InlineData(10UL, 2)]
    [InlineData(9_999_999_999_999_999_999UL, 19)]
    [InlineData(10_000_000_000_000_000_000UL, 20)]
    [InlineData(ulong.MaxValue, 20)]      // 18 446 744 073 709 551 615
    public void Digits_UInt64_ReturnsExpectedCount(ulong value, int expected)
    {
        Assert.Equal(expected, MathUtils.Digits(value));
    }

    // ---------------------------------------------------------------
    // Small integer types — verifies the generic constraint works for
    // narrow types and that CreateChecked(10) fits in all of them.
    // ---------------------------------------------------------------

    [Theory]
    [InlineData((byte)0, 1)]
    [InlineData((byte)9, 1)]
    [InlineData((byte)10, 2)]
    [InlineData((byte)99, 2)]
    [InlineData((byte)100, 3)]
    [InlineData(byte.MaxValue, 3)]        // 255
    public void Digits_Byte_ReturnsExpectedCount(byte value, int expected)
    {
        Assert.Equal(expected, MathUtils.Digits(value));
    }

    [Theory]
    [InlineData((sbyte)0, 1)]
    [InlineData(sbyte.MaxValue, 3)]       //  127
    [InlineData((sbyte)-99, 2)]
    [InlineData(sbyte.MinValue, 3)]       // -128
    public void Digits_SByte_ReturnsExpectedCount(sbyte value, int expected)
    {
        Assert.Equal(expected, MathUtils.Digits(value));
    }

    [Theory]
    [InlineData((short)0, 1)]
    [InlineData((short)9_999, 4)]
    [InlineData((short)10_000, 5)]
    [InlineData(short.MaxValue, 5)]       //  32 767
    [InlineData(short.MinValue, 5)]       // -32 768
    public void Digits_Int16_ReturnsExpectedCount(short value, int expected)
    {
        Assert.Equal(expected, MathUtils.Digits(value));
    }

    // ---------------------------------------------------------------
    // Int128 / UInt128 — attribute arguments can't express these
    // constants, so MemberData is used instead.
    // ---------------------------------------------------------------

    public static TheoryData<Int128, int> Int128Cases() => new()
    {
        { Int128.Zero, 1 },
        { (Int128)long.MaxValue, 19 },
        { (Int128)long.MaxValue + 1, 19 },                 // 9 223 372 036 854 775 808
        { Int128.MaxValue, 39 },                           //  ≈ 1.7 × 10^38
        { Int128.MinValue, 39 },                           // no Abs → MinValue is safe
        { -((Int128)long.MaxValue + 1), 19 },
    };

    [Theory]
    [MemberData(nameof(Int128Cases))]
    public void Digits_Int128_ReturnsExpectedCount(Int128 value, int expected)
    {
        Assert.Equal(expected, MathUtils.Digits(value));
    }

    public static TheoryData<UInt128, int> UInt128Cases() => new()
    {
        { UInt128.Zero, 1 },
        { (UInt128)ulong.MaxValue, 20 },
        { (UInt128)ulong.MaxValue + 1, 20 },               // 18 446 744 073 709 551 616
        { UInt128.MaxValue, 39 },                          //  ≈ 3.4 × 10^38
    };

    [Theory]
    [MemberData(nameof(UInt128Cases))]
    public void Digits_UInt128_ReturnsExpectedCount(UInt128 value, int expected)
    {
        Assert.Equal(expected, MathUtils.Digits(value));
    }

    // ---------------------------------------------------------------
    // BigInteger — arbitrary precision, including values far beyond
    // any fixed-width type. Expected counts derive from construction
    // (10^k has k+1 digits), not from string formatting, so the test
    // doesn't share a failure mode with the code under test.
    // ---------------------------------------------------------------

    public static TheoryData<BigInteger, int> BigIntegerCases() => new()
    {
        { BigInteger.Zero, 1 },
        { new BigInteger(-1), 1 },
        { BigInteger.Pow(10, 20), 21 },
        { BigInteger.Pow(10, 20) - 1, 20 },
        { BigInteger.Pow(10, 100), 101 },
        { BigInteger.Pow(10, 100) - 1, 100 },
        { -BigInteger.Pow(10, 100), 101 },
        { BigInteger.Parse("123456789012345678901234567890"), 30 },
    };

    [Theory]
    [MemberData(nameof(BigIntegerCases))]
    public void Digits_BigInteger_ReturnsExpectedCount(BigInteger value, int expected)
    {
        Assert.Equal(expected, MathUtils.Digits(value));
    }

    // ---------------------------------------------------------------
    // Cross-check against string length as an independent oracle,
    // sampled across several types. Trims the '-' sign for negatives.
    // ---------------------------------------------------------------

    public static TheoryData<long> OracleSamples() => new()
    {
        0, 1, -1, 7, 42, -42, 999, 1_000, -1_001, 123_456,
        int.MaxValue, int.MinValue, long.MaxValue, long.MinValue,
    };

    [Theory]
    [MemberData(nameof(OracleSamples))]
    public void Digits_MatchesStringLengthOracle(long value)
    {
        int expected = value.ToString().TrimStart('-').Length;

        Assert.Equal(expected, MathUtils.Digits(value));
    }
}
