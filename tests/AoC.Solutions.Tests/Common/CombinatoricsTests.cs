using KE.AoC.Solutions.Common;
using System.Collections;
using System.Collections.ObjectModel;

namespace KE.AoC.Solutions.Tests.Common;

public static class CombinatoricsTests
{
    // =====================================================================
    // GetCombinations
    // =====================================================================

    public class GetCombinations
    {
        [Theory]
        [InlineData(1, 0, "")]              // one empty combination
        [InlineData(6, 0, "")]
        [InlineData(1, 1, "a")]
        [InlineData(4, 1, "a,b,c,d")]
        [InlineData(4, 2, "ab,ac,ad,bc,bd,cd")]
        [InlineData(4, 3, "abc,abd,acd,bcd")]
        [InlineData(4, 4, "abcd")]
        [InlineData(5, 2, "ab,ac,ad,ae,bc,bd,be,cd,ce,de")]
        [InlineData(5, 4, "abcd,abce,abde,acde,bcde")]
        [InlineData(6, 5, "abcde,abcdf,abcef,abdef,acdef,bcdef")]
        [InlineData(0, 0, null)]            // see SharedContract.EmptySource_YieldsNothing_ForEveryLength
        [InlineData(0, 1, null)]
        [InlineData(0, 5, null)]
        [InlineData(2, 3, null)]            // length > count -> C(n,k) == 0
        [InlineData(3, 100, null)]
        public void ProducesExpectedSequence(int n, int length, string? expected)
        {
            var actual = Render(Combinatorics.GetCombinations(Source(n), length));

            Assert.Equal(Expand(expected), actual);
        }

        [Theory]
        [MemberData(nameof(SizeAndLengthMatrix), MemberType = typeof(CombinatoricsTests))]
        public void MatchesReferenceImplementation(int n, int length)
        {
            var items = Source(n);
            var expected = ExpectedFromOracle(items, n, length, ReferenceCombinations);
            var actual = Render(Combinatorics.GetCombinations(items, length));

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(SizeAndLengthMatrix), MemberType = typeof(CombinatoricsTests))]
        public void CountEqualsBinomialCoefficient(int n, int length)
        {
            long expected = n == 0 ? 0 : Binomial(n, length);

            Assert.Equal(expected, Combinatorics.GetCombinations(Source(n), length).LongCount());
        }

        [Theory]
        [InlineData(20, 3, 1140)]
        [InlineData(20, 10, 184756)]
        [InlineData(12, 6, 924)]
        public void CountIsCorrectForLargerInputs(int n, int length, int expected)
        {
            var items = Enumerable.Range(0, n).ToList();

            Assert.Equal(expected, Combinatorics.GetCombinations(items, length).Count());
        }

        [Theory]
        [MemberData(nameof(SizeAndLengthMatrix), MemberType = typeof(CombinatoricsTests))]
        public void SelectionsAreStrictlyIncreasing(int n, int length)
        {
            // Source is 'a','b','c',... so ordinal order mirrors index order.
            Assert.All(
                Combinatorics.GetCombinations(Source(n), length),
                combination => Assert.True(IsOrdered(combination, strict: true)));
        }

        [Theory]
        [InlineData(4, 4, "abcd")]
        [InlineData(1, 1, "a")]
        [InlineData(6, 6, "abcdef")]
        public void LengthEqualToCount_YieldsWholeSourceOnce(int n, int length, string expected)
        {
            var actual = Render(Combinatorics.GetCombinations(Source(n), length));

            Assert.Equal(expected, Assert.Single(actual));
        }

        [Theory]
        [InlineData(3, 4)]
        [InlineData(3, 5)]
        [InlineData(1, 2)]
        [InlineData(5, int.MaxValue)]
        public void LengthGreaterThanCount_YieldsNothing(int n, int length)
        {
            Assert.Empty(Combinatorics.GetCombinations(Source(n), length));
        }
    }

    // =====================================================================
    // GetCombinationsWithRepetition
    // =====================================================================

    public class GetCombinationsWithRepetition
    {
        [Theory]
        [InlineData(3, 0, "")]
        [InlineData(1, 1, "a")]
        [InlineData(1, 4, "aaaa")]          // length may exceed the item count here
        [InlineData(3, 1, "a,b,c")]
        [InlineData(2, 2, "aa,ab,bb")]
        [InlineData(3, 2, "aa,ab,ac,bb,bc,cc")]
        [InlineData(2, 3, "aaa,aab,abb,bbb")]
        [InlineData(3, 3, "aaa,aab,aac,abb,abc,acc,bbb,bbc,bcc,ccc")]
        [InlineData(2, 4, "aaaa,aaab,aabb,abbb,bbbb")]
        [InlineData(0, 0, null)]
        [InlineData(0, 1, null)]
        [InlineData(0, 9, null)]
        public void ProducesExpectedSequence(int n, int length, string? expected)
        {
            var actual = Render(Combinatorics.GetCombinationsWithRepetition(Source(n), length));

            Assert.Equal(Expand(expected), actual);
        }

        [Theory]
        [MemberData(nameof(SizeAndLengthMatrix), MemberType = typeof(CombinatoricsTests))]
        public void MatchesReferenceImplementation(int n, int length)
        {
            var items = Source(n);
            var expected = ExpectedFromOracle(items, n, length, ReferenceCombinationsWithRepetition);
            var actual = Render(Combinatorics.GetCombinationsWithRepetition(items, length));

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(SizeAndLengthMatrix), MemberType = typeof(CombinatoricsTests))]
        public void CountEqualsMultisetCoefficient(int n, int length)
        {
            long expected = n == 0 ? 0 : length == 0 ? 1 : Binomial(n + length - 1, length);

            Assert.Equal(expected, Combinatorics.GetCombinationsWithRepetition(Source(n), length).LongCount());
        }

        [Theory]
        [InlineData(10, 4, 715)]
        [InlineData(5, 8, 495)]
        [InlineData(3, 10, 66)]
        public void CountIsCorrectForLargerInputs(int n, int length, int expected)
        {
            var items = Enumerable.Range(0, n).ToList();

            Assert.Equal(expected, Combinatorics.GetCombinationsWithRepetition(items, length).Count());
        }

        [Theory]
        [MemberData(nameof(SizeAndLengthMatrix), MemberType = typeof(CombinatoricsTests))]
        public void SelectionsAreNonDecreasing(int n, int length)
        {
            Assert.All(
                Combinatorics.GetCombinationsWithRepetition(Source(n), length),
                combination => Assert.True(IsOrdered(combination, strict: false)));
        }

        [Theory]
        [InlineData(1, 3, "aaa")]
        [InlineData(3, 2, "aa")]
        [InlineData(3, 4, "cccc")]
        public void IncludesUniformCombinations(int n, int length, string expected)
        {
            var actual = Render(Combinatorics.GetCombinationsWithRepetition(Source(n), length));

            Assert.Contains(expected, actual);
        }
    }

    // =====================================================================
    // GetVariations
    // =====================================================================

    public class GetVariations
    {
        [Theory]
        [InlineData(3, 0, "")]              // one empty variation
        [InlineData(1, 1, "a")]
        [InlineData(2, 1, "a,b")]
        [InlineData(2, 2, "ab,ba")]
        [InlineData(3, 1, "a,b,c")]
        [InlineData(3, 2, "ab,ac,ba,bc,ca,cb")]
        [InlineData(3, 3, "abc,acb,bac,bca,cab,cba")]
        [InlineData(4, 2, "ab,ac,ad,ba,bc,bd,ca,cb,cd,da,db,dc")]
        [InlineData(0, 0, null)]
        [InlineData(0, 1, null)]
        [InlineData(0, 4, null)]
        [InlineData(2, 3, null)]            // length > count -> P(n,k) == 0
        [InlineData(3, 4, null)]
        [InlineData(1, 100, null)]
        public void ProducesExpectedSequence(int n, int length, string? expected)
        {
            var actual = Render(Combinatorics.GetVariations(Source(n), length));

            Assert.Equal(Expand(expected), actual);
        }

        [Theory]
        [MemberData(nameof(SizeAndLengthMatrix), MemberType = typeof(CombinatoricsTests))]
        public void MatchesReferenceImplementation(int n, int length)
        {
            var items = Source(n);
            var expected = ExpectedFromOracle(items, n, length, ReferenceVariations);
            var actual = Render(Combinatorics.GetVariations(items, length));

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(SizeAndLengthMatrix), MemberType = typeof(CombinatoricsTests))]
        public void CountEqualsFallingFactorial(int n, int length)
        {
            long expected = n == 0 ? 0 : FallingFactorial(n, length);

            Assert.Equal(expected, Combinatorics.GetVariations(Source(n), length).LongCount());
        }

        [Theory]
        [InlineData(10, 3, 720)]
        [InlineData(8, 4, 1680)]
        [InlineData(20, 2, 380)]
        [InlineData(7, 7, 5040)]
        public void CountIsCorrectForLargerInputs(int n, int length, int expected)
        {
            var items = Enumerable.Range(0, n).ToList();

            Assert.Equal(expected, Combinatorics.GetVariations(items, length).Count());
        }

        [Theory]
        [MemberData(nameof(SizeAndLengthMatrix), MemberType = typeof(CombinatoricsTests))]
        public void NoItemIsUsedTwiceWithinAVariation(int n, int length)
        {
            Assert.All(
                Combinatorics.GetVariations(Source(n), length),
                variation => Assert.Equal(variation.Length, variation.Distinct().Count()));
        }

        [Theory]
        [MemberData(nameof(SizeAndLengthMatrix), MemberType = typeof(CombinatoricsTests))]
        public void ResultsAreDistinct(int n, int length)
        {
            var actual = Render(Combinatorics.GetVariations(Source(n), length));

            Assert.Equal(actual.Length, actual.Distinct().Count());
        }

        [Theory]
        [InlineData(3, 4)]
        [InlineData(1, 2)]
        [InlineData(6, 7)]
        [InlineData(5, 40)]
        public void LengthGreaterThanCount_YieldsNothing(int n, int length)
        {
            // The backtracking loop must unwind cleanly rather than throw.
            Assert.Empty(Combinatorics.GetVariations(Source(n), length));
        }

        [Theory]
        [InlineData(2, "aa,ab,aa,ab,ba,ba")]
        [InlineData(3, "aab,aba,aab,aba,baa,baa")]
        public void DuplicateSourceItems_AreTreatedPositionally(int length, string expected)
        {
            // Positions are unique even when values are not: no de-duplication.
            var items = new List<char> { 'a', 'a', 'b' };

            Assert.Equal(expected.Split(','), Render(Combinatorics.GetVariations(items, length)));
        }
    }

    // =====================================================================
    // GetVariationsWithRepetition
    // =====================================================================

    public class GetVariationsWithRepetition
    {
        [Theory]
        [InlineData(3, 0, "")]
        [InlineData(1, 1, "a")]
        [InlineData(1, 4, "aaaa")]
        [InlineData(2, 1, "a,b")]
        [InlineData(2, 2, "aa,ab,ba,bb")]
        [InlineData(2, 3, "aaa,aab,aba,abb,baa,bab,bba,bbb")]
        [InlineData(3, 1, "a,b,c")]
        [InlineData(3, 2, "aa,ab,ac,ba,bb,bc,ca,cb,cc")]
        [InlineData(0, 0, null)]
        [InlineData(0, 1, null)]
        [InlineData(0, 7, null)]
        public void ProducesExpectedSequence(int n, int length, string? expected)
        {
            var actual = Render(Combinatorics.GetVariationsWithRepetition(Source(n), length));

            Assert.Equal(Expand(expected), actual);
        }

        [Theory]
        [MemberData(nameof(SmallSizeAndLengthMatrix), MemberType = typeof(CombinatoricsTests))]
        public void MatchesReferenceImplementation(int n, int length)
        {
            var items = Source(n);
            var expected = ExpectedFromOracle(items, n, length, ReferenceVariationsWithRepetition);
            var actual = Render(Combinatorics.GetVariationsWithRepetition(items, length));

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(SmallSizeAndLengthMatrix), MemberType = typeof(CombinatoricsTests))]
        public void CountEqualsCountRaisedToLength(int n, int length)
        {
            long expected = n == 0 ? 0 : Pow(n, length);

            Assert.Equal(expected, Combinatorics.GetVariationsWithRepetition(Source(n), length).LongCount());
        }

        [Theory]
        [InlineData(10, 3, 1000)]
        [InlineData(2, 12, 4096)]
        [InlineData(6, 5, 7776)]
        public void CountIsCorrectForLargerInputs(int n, int length, int expected)
        {
            var items = Enumerable.Range(0, n).ToList();

            Assert.Equal(expected, Combinatorics.GetVariationsWithRepetition(items, length).Count());
        }

        [Theory]
        [MemberData(nameof(SmallSizeAndLengthMatrix), MemberType = typeof(CombinatoricsTests))]
        public void ResultsAreDistinct(int n, int length)
        {
            var actual = Render(Combinatorics.GetVariationsWithRepetition(Source(n), length));

            Assert.Equal(actual.Length, actual.Distinct().Count());
        }

        [Theory]
        [InlineData(2, 3, "aaa")]
        [InlineData(2, 3, "bbb")]
        [InlineData(3, 2, "cc")]
        public void IncludesUniformVariations(int n, int length, string expected)
        {
            var actual = Render(Combinatorics.GetVariationsWithRepetition(Source(n), length));

            Assert.Contains(expected, actual);
        }

        [Theory]
        [InlineData(2, "aa,aa,ab,aa,aa,ab,ba,ba,bb")]
        public void DuplicateSourceItems_AreTreatedPositionally(int length, string expected)
        {
            var items = new List<char> { 'a', 'a', 'b' };

            Assert.Equal(expected.Split(','), Render(Combinatorics.GetVariationsWithRepetition(items, length)));
        }

        [Theory]
        [InlineData(3, 4)]
        [InlineData(2, 10)]
        [InlineData(1, 6)]
        public void LengthGreaterThanCount_IsSupported(int n, int length)
        {
            Assert.Equal(Pow(n, length), Combinatorics.GetVariationsWithRepetition(Source(n), length).LongCount());
        }
    }

    // =====================================================================
    // GetPermutations
    // =====================================================================

    public class GetPermutations
    {
        [Theory]
        [InlineData(0, null)]
        [InlineData(1, "a")]
        [InlineData(2, "ab,ba")]
        [InlineData(3, "abc,acb,bac,bca,cab,cba")]
        public void ProducesExpectedSequence(int n, string? expected)
        {
            var actual = Render(Combinatorics.GetPermutations(Source(n)));

            Assert.Equal(Expand(expected), actual);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 6)]
        [InlineData(4, 24)]
        [InlineData(5, 120)]
        [InlineData(6, 720)]
        public void CountEqualsFactorial(int n, int expected)
        {
            Assert.Equal(expected, Combinatorics.GetPermutations(Source(n)).Count());
        }

        [Theory]
        [InlineData(8, 40320)]
        [InlineData(9, 362880)]
        public void CountIsCorrectForLargerInputs(int n, int expected)
        {
            var items = Enumerable.Range(0, n).ToList();

            Assert.Equal(expected, Combinatorics.GetPermutations(items).Count());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        public void EveryPermutationIsARearrangementOfTheSource(int n)
        {
            var items = Source(n);
            var expected = new string([.. items]);

            Assert.All(
                Combinatorics.GetPermutations(items),
                permutation => Assert.Equal(expected, new string([.. permutation.Order()])));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        public void ResultsAreDistinct(int n)
        {
            var actual = Render(Combinatorics.GetPermutations(Source(n)));

            Assert.Equal(actual.Length, actual.Distinct().Count());
        }

        [Theory]
        [InlineData(1, "a", "a")]
        [InlineData(2, "ab", "ba")]
        [InlineData(3, "abc", "cba")]
        [InlineData(4, "abcd", "dcba")]
        [InlineData(6, "abcdef", "fedcba")]
        public void ResultsAreInLexicographicOrder(int n, string first, string last)
        {
            var actual = Render(Combinatorics.GetPermutations(Source(n)));

            Assert.Equal(first, actual[0]);
            Assert.Equal(last, actual[^1]);
        }

        [Theory]
        [InlineData(0)]
        public void EmptySource_YieldsNothing(int n)
        {
            // Consistent with the rest of the class: an empty source produces no
            // results, so 0! evaluates to 0 rather than 1.
            Assert.Empty(Combinatorics.GetPermutations(Source(n)));
        }

        [Theory]
        [InlineData("items")]
        public void NullItems_ThrowsArgumentNullException(string expectedParamName)
        {
            // Requires a ThrowIfNull guard in GetPermutations itself: passing
            // items.Count as an argument dereferences before GetVariations runs.
            var ex = Assert.Throws<ArgumentNullException>(
                () => Combinatorics.GetPermutations<char>(null!));

            Assert.Equal(expectedParamName, ex.ParamName);
        }

        [Theory]
        [InlineData("aab,aba,aab,aba,baa,baa")]
        public void DuplicateSourceItems_AreTreatedPositionally(string expected)
        {
            var items = new List<char> { 'a', 'a', 'b' };

            Assert.Equal(expected.Split(','), Render(Combinatorics.GetPermutations(items)));
        }

        [Theory]
        [InlineData(20)]
        [InlineData(30)]
        public void IsLazyAndDoesNotMaterialiseEverything(int n)
        {
            // 20! is ~2.4e18: a non-lazy implementation would never return.
            var items = Enumerable.Range(0, n).ToList();

            var head = Combinatorics.GetPermutations(items).Take(3).ToList();

            Assert.Equal(3, head.Count);
            Assert.All(head, p => Assert.Equal(n, p.Length));
        }

        [Theory]
        [InlineData(4)]
        [InlineData(5)]
        public void EachYieldedArrayIsADistinctInstance(int n)
        {
            var permutations = Combinatorics.GetPermutations(Source(n)).ToList();

            Assert.Equal(
                permutations.Count,
                permutations.Cast<object>().Distinct(ReferenceEqualityComparer.Instance).Count());
        }

        [Theory]
        [InlineData("abc,acb,bac,bca,cab,cba")]
        public void WorksWithAnIndexerOnlySource(string expected)
        {
            var items = new IndexerOnlyList<char>('a', 'b', 'c');

            Assert.Equal(expected.Split(','), Render(Combinatorics.GetPermutations(items)));
        }

        [Theory]
        [InlineData(4)]
        public void SequenceCanBeEnumeratedMoreThanOnce(int n)
        {
            var sequence = Combinatorics.GetPermutations(Source(n));

            Assert.Equal(Render(sequence), Render(sequence));
        }
    }

    // =====================================================================
    // Argument validation
    // =====================================================================

    public class ArgumentValidation
    {
        [Theory]
        [MemberData(nameof(GeneratorsAndLengths), MemberType = typeof(CombinatoricsTests))]
        public void NullItems_ThrowsArgumentNullException(Generator generator, int length)
        {
            // No enumeration: the guards must run at the call site, not at the first MoveNext.
            var ex = Assert.Throws<ArgumentNullException>(
                () => Invoke<char>(generator, null!, length));

            Assert.Equal("items", ex.ParamName);
        }

        [Theory]
        [MemberData(nameof(GeneratorsAndNegativeLengths), MemberType = typeof(CombinatoricsTests))]
        public void NegativeLength_ThrowsArgumentOutOfRangeException(Generator generator, int length)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(
                () => Invoke(generator, Source(3), length));

            Assert.Equal("length", ex.ParamName);
        }

        [Theory]
        [MemberData(nameof(AllGenerators), MemberType = typeof(CombinatoricsTests))]
        public void NullItems_TakesPrecedenceOverNegativeLength(Generator generator)
        {
            Assert.Throws<ArgumentNullException>(() => Invoke<char>(generator, null!, -1));
        }

        [Theory]
        [MemberData(nameof(AllGenerators), MemberType = typeof(CombinatoricsTests))]
        public void ValidationIsEager_NotDeferredToEnumeration(Generator generator)
        {
            // The public methods are not iterator blocks, so a bad argument surfaces
            // at the call site rather than at the eventual foreach, possibly frames away.
            Assert.Throws<ArgumentNullException>(() => Invoke<char>(generator, null!, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => Invoke(generator, Source(3), -1));
        }
    }

    // =====================================================================
    // Contract shared by every generator
    // =====================================================================

    public class SharedContract
    {
        [Theory]
        [MemberData(nameof(AllGenerators), MemberType = typeof(CombinatoricsTests))]
        public void EachYieldedArrayIsADistinctInstance(Generator generator)
        {
            // No internal buffer reuse: callers must be able to retain the arrays.
            var results = Invoke(generator, Source(4), 2).ToList();

            Assert.Equal(
                results.Count,
                results.Cast<object>().Distinct(ReferenceEqualityComparer.Instance).Count());
        }

        [Theory]
        [MemberData(nameof(AllGenerators), MemberType = typeof(CombinatoricsTests))]
        public void MutatingAYieldedArray_DoesNotAffectSubsequentResults(Generator generator)
        {
            var sequence = Invoke(generator, Source(4), 2);
            var expected = Render(sequence);

            foreach (var result in sequence)
                Array.Fill(result, 'z');

            Assert.Equal(expected, Render(sequence));
        }

        [Theory]
        [MemberData(nameof(AllGenerators), MemberType = typeof(CombinatoricsTests))]
        public void SequenceCanBeEnumeratedMoreThanOnce(Generator generator)
        {
            var sequence = Invoke(generator, Source(4), 2);

            Assert.Equal(Render(sequence), Render(sequence));
        }

        [Theory]
        [MemberData(nameof(AllGenerators), MemberType = typeof(CombinatoricsTests))]
        public void SequenceIsLazyAndDoesNotMaterialiseEverything(Generator generator)
        {
            // Every generator over 60 items taken 30 at a time has an astronomical
            // result count: a non-lazy implementation would never return.
            var items = Enumerable.Range(0, 60).ToList();

            var head = Invoke(generator, items, 30).Take(3).ToList();

            Assert.Equal(3, head.Count);
            Assert.All(head, c => Assert.Equal(30, c.Length));
        }

        [Theory]
        [MemberData(nameof(AllGenerators), MemberType = typeof(CombinatoricsTests))]
        public void SourceCollectionIsNotMutated(Generator generator)
        {
            var items = Source(5);
            var snapshot = items.ToArray();

            _ = Invoke(generator, items, 3).ToList();

            Assert.Equal(snapshot, items);
        }

        [Theory]
        [MemberData(nameof(AllGenerators), MemberType = typeof(CombinatoricsTests))]
        public void FullEnumerationTerminatesWithoutThrowing(Generator generator)
        {
            // Regression guard: every odometer must stop when saturated instead of
            // walking off the left edge of its index array.
            for (int n = 0; n <= 5; n++)
            {
                for (int length = 0; length <= 5; length++)
                {
                    var sequence = Invoke(generator, Source(n), length);
                    var exception = Record.Exception(() => sequence.ToList());

                    Assert.Null(exception);
                }
            }
        }

        [Theory]
        [MemberData(nameof(GeneratorsAndSmallLengths), MemberType = typeof(CombinatoricsTests))]
        public void EnumeratingPastTheEndReturnsFalse(Generator generator, int length)
        {
            using var enumerator = Invoke(generator, Source(4), length).GetEnumerator();

            while (enumerator.MoveNext()) { }

            Assert.False(enumerator.MoveNext());
            Assert.False(enumerator.MoveNext());
        }

        [Theory]
        [MemberData(nameof(GeneratorsAndSmallLengths), MemberType = typeof(CombinatoricsTests))]
        public void ResultsHaveTheRequestedLength(Generator generator, int length)
        {
            Assert.All(Invoke(generator, Source(4), length), c => Assert.Equal(length, c.Length));
        }

        [Theory]
        [MemberData(nameof(GeneratorsAndListKinds), MemberType = typeof(CombinatoricsTests))]
        public void WorksWithAnyIListImplementation(Generator generator, string kind)
        {
            IList<char> items = kind switch
            {
                "array" => new[] { 'a', 'b', 'c' },
                "list" => new List<char> { 'a', 'b', 'c' },
                "collection" => new Collection<char> { 'a', 'b', 'c' },
                "readOnlyCollection" => new ReadOnlyCollection<char>(new[] { 'a', 'b', 'c' }),
                "indexerOnly" => new IndexerOnlyList<char>('a', 'b', 'c'),
                _ => throw new ArgumentOutOfRangeException(nameof(kind))
            };

            // IndexerOnlyList throws on GetEnumerator, proving only Count and the
            // indexer are touched - no hidden materialisation of the source.
            var expected = Render(Invoke(generator, new List<char> { 'a', 'b', 'c' }, 2));

            Assert.Equal(expected, Render(Invoke(generator, items, 2)));
        }

        [Theory]
        [MemberData(nameof(AllGenerators), MemberType = typeof(CombinatoricsTests))]
        public void NullElementsArePreserved(Generator generator)
        {
            var items = new List<string?> { "x", null, "y" };

            var results = Invoke(generator, items, 2).ToList();

            Assert.Contains(results, c => c.Any(item => item is null));
            Assert.All(results, c => Assert.Equal(2, c.Length));
        }

        [Theory]
        [MemberData(nameof(AllGenerators), MemberType = typeof(CombinatoricsTests))]
        public void ValueTypeElementsAreCopiedByValue(Generator generator)
        {
            var items = new List<Point> { new(0, 0), new(1, 1), new(2, 2), new(3, 3) };

            var results = Invoke(generator, items, 2).ToList();

            Assert.NotEmpty(results);
            Assert.All(results, c => Assert.All(c, p => Assert.Equal(p.X, p.Y)));
        }

        [Theory]
        [MemberData(nameof(AllGenerators), MemberType = typeof(CombinatoricsTests))]
        public void ReferenceTypeElementsAreNotCloned(Generator generator)
        {
            var first = new object();
            var second = new object();
            var items = new List<object> { first, second };

            var results = Invoke(generator, items, 2).ToList();

            Assert.All(results,
                c => Assert.All(c, o => Assert.True(ReferenceEquals(o, first) || ReferenceEquals(o, second))));
        }

        [Theory]
        [MemberData(nameof(GeneratorsAndSmallLengths), MemberType = typeof(CombinatoricsTests))]
        public void EmptySource_YieldsNothing_ForEveryLength(Generator generator, int length)
        {
            // Documented convention: an empty source yields nothing even for length 0,
            // so the degenerate count evaluates to 0 rather than 1. Flip the length == 0
            // case here (and the n == 0 carve-outs in the count tests and in
            // ExpectedFromOracle) if that convention changes.
            Assert.Empty(Invoke(generator, Array.Empty<char>(), length));
        }

        [Theory]
        [MemberData(nameof(AllGenerators), MemberType = typeof(CombinatoricsTests))]
        public void NonEmptySourceWithZeroLength_YieldsOneEmptyResult(Generator generator)
        {
            var result = Assert.Single(Invoke(generator, Source(3), 0));

            Assert.Empty(result);
        }
    }

    // =====================================================================
    // Relationships between the generators
    // =====================================================================

    public class Relationships
    {
        [Theory]
        [MemberData(nameof(SizeAndLengthMatrix), MemberType = typeof(CombinatoricsTests))]
        public void VariationCountEqualsCombinationCountTimesFactorial(int n, int length)
        {
            // P(n,k) == C(n,k) * k!
            long combinations = Combinatorics.GetCombinations(Source(n), length).LongCount();
            long variations = Combinatorics.GetVariations(Source(n), length).LongCount();

            Assert.Equal(combinations * Factorial(length), variations);
        }

        [Theory]
        [MemberData(nameof(SizeAndLengthMatrix), MemberType = typeof(CombinatoricsTests))]
        public void SortedVariationsAreExactlyTheCombinations(int n, int length)
        {
            var items = Source(n);
            var expected = Render(Combinatorics.GetCombinations(items, length)).ToHashSet();
            var actual = Render(Combinatorics.GetVariations(items, length))
                .Select(v => new string([.. v.Order()]))
                .ToHashSet();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(SmallSizeAndLengthMatrix), MemberType = typeof(CombinatoricsTests))]
        public void SortedVariationsWithRepetitionAreExactlyTheCombinationsWithRepetition(int n, int length)
        {
            var items = Source(n);
            var expected = Render(Combinatorics.GetCombinationsWithRepetition(items, length)).ToHashSet();
            var actual = Render(Combinatorics.GetVariationsWithRepetition(items, length))
                .Select(v => new string([.. v.Order()]))
                .ToHashSet();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(SmallSizeAndLengthMatrix), MemberType = typeof(CombinatoricsTests))]
        public void VariationsAreASubsetOfVariationsWithRepetition(int n, int length)
        {
            var items = Source(n);
            var strict = Render(Combinatorics.GetVariations(items, length));
            var repeating = Render(Combinatorics.GetVariationsWithRepetition(items, length)).ToHashSet();

            Assert.All(strict, v => Assert.Contains(v, repeating));
        }

        [Theory]
        [MemberData(nameof(SizeAndLengthMatrix), MemberType = typeof(CombinatoricsTests))]
        public void CombinationsAreASubsetOfVariations(int n, int length)
        {
            var items = Source(n);
            var combinations = Render(Combinatorics.GetCombinations(items, length));
            var variations = Render(Combinatorics.GetVariations(items, length)).ToHashSet();

            Assert.All(combinations, c => Assert.Contains(c, variations));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        public void PermutationsEqualVariationsOfFullLength(int n)
        {
            var items = Source(n);

            Assert.Equal(
                Render(Combinatorics.GetVariations(items, items.Count)),
                Render(Combinatorics.GetPermutations(items)));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void EveryCombinationExpandsToItsPermutations(int length)
        {
            // Each C(n,k) selection accounts for exactly k! of the P(n,k) variations.
            var items = Source(5);

            var groups = Render(Combinatorics.GetVariations(items, length))
                .GroupBy(v => new string([.. v.Order()]))
                .ToDictionary(g => g.Key, g => g.Count());

            var combinations = Render(Combinatorics.GetCombinations(items, length));

            Assert.Equal(combinations.Length, groups.Count);
            Assert.All(combinations, c => Assert.Equal(Factorial(length), groups[c]));
        }
    }

    // =====================================================================
    // Shared helpers
    // =====================================================================

    public enum Generator
    {
        Combinations,
        CombinationsWithRepetition,
        Variations,
        VariationsWithRepetition
    }

    private const string Alphabet = "abcdef";

    public static TheoryData<int, int> SizeAndLengthMatrix()
    {
        var data = new TheoryData<int, int>();
        for (int n = 0; n <= Alphabet.Length; n++)
            for (int length = 0; length <= Alphabet.Length + 1; length++)
                data.Add(n, length);
        return data;
    }

    /// <summary>Bounded matrix for the generator whose output grows as n^k.</summary>
    public static TheoryData<int, int> SmallSizeAndLengthMatrix()
    {
        var data = new TheoryData<int, int>();
        for (int n = 0; n <= 5; n++)
            for (int length = 0; length <= 5; length++)
                data.Add(n, length);
        return data;
    }

    public static TheoryData<Generator> AllGenerators()
    {
        var data = new TheoryData<Generator>();
        foreach (var generator in Enum.GetValues<Generator>())
            data.Add(generator);
        return data;
    }

    public static TheoryData<Generator, int> GeneratorsAndLengths() => Cross([0, 1, 2, 10]);

    public static TheoryData<Generator, int> GeneratorsAndSmallLengths() => Cross([0, 1, 2, 3, 4]);

    public static TheoryData<Generator, int> GeneratorsAndNegativeLengths() => Cross([-1, -7, int.MinValue]);

    public static TheoryData<Generator, string> GeneratorsAndListKinds()
    {
        var data = new TheoryData<Generator, string>();
        foreach (var generator in Enum.GetValues<Generator>())
            foreach (var kind in new[] { "array", "list", "collection", "readOnlyCollection", "indexerOnly" })
                data.Add(generator, kind);
        return data;
    }

    private static TheoryData<Generator, int> Cross(int[] lengths)
    {
        var data = new TheoryData<Generator, int>();
        foreach (var generator in Enum.GetValues<Generator>())
            foreach (int length in lengths)
                data.Add(generator, length);
        return data;
    }

    private static IList<char> Source(int n) => Alphabet.Take(n).ToList();

    private static IEnumerable<T[]> Invoke<T>(Generator generator, IList<T> items, int length) => generator switch
    {
        Generator.Combinations => Combinatorics.GetCombinations(items, length),
        Generator.CombinationsWithRepetition => Combinatorics.GetCombinationsWithRepetition(items, length),
        Generator.Variations => Combinatorics.GetVariations(items, length),
        Generator.VariationsWithRepetition => Combinatorics.GetVariationsWithRepetition(items, length),
        _ => throw new ArgumentOutOfRangeException(nameof(generator))
    };

    private static string[] Render(IEnumerable<char[]> results) =>
        results.Select(c => new string(c)).ToArray();

    private static string[] Render<T>(IEnumerable<T[]> results) =>
        results.Select(c => string.Concat(c.Select(x => x?.ToString() ?? "<null>"))).ToArray();

    /// <summary>Null means "no results"; "" means "one empty result".</summary>
    private static string[] Expand(string? expected) =>
        expected is null ? [] : expected.Split(',');

    private static IEnumerable<string> ExpectedFromOracle(
        IList<char> items, int n, int length, Func<int, int, List<int[]>> oracle) =>
        // The oracles yield the empty tuple for n == 0, length == 0; every generator
        // suppresses it for an empty source.
        n == 0 ? [] : oracle(n, length).Select(indices => new string([.. indices.Select(i => items[i])]));

    private static bool IsOrdered(char[] result, bool strict)
    {
        for (int i = 1; i < result.Length; i++)
        {
            if (strict ? result[i] <= result[i - 1] : result[i] < result[i - 1])
                return false;
        }

        return true;
    }

    private static long Binomial(int n, int k)
    {
        if (k < 0 || k > n)
            return 0;

        long result = 1;
        for (int i = 1; i <= k; i++)
            result = result * (n - k + i) / i;

        return result;
    }

    private static long FallingFactorial(int n, int k)
    {
        if (k < 0 || k > n)
            return 0;

        long result = 1;
        for (int i = 0; i < k; i++)
            result *= n - i;

        return result;
    }

    private static long Factorial(int n)
    {
        long result = 1;
        for (int i = 2; i <= n; i++)
            result *= i;

        return result;
    }

    private static long Pow(int value, int exponent)
    {
        long result = 1;
        for (int i = 0; i < exponent; i++)
            result *= value;

        return result;
    }

    // ---------------------------------------------------------------------
    // Independent recursive oracles (index tuples, lexicographic order)
    // ---------------------------------------------------------------------

    /// <summary>Strictly increasing tuples: combinations.</summary>
    private static List<int[]> ReferenceCombinations(int n, int k)
    {
        var results = new List<int[]>();
        var current = new int[k];

        void Recurse(int start, int depth)
        {
            if (depth == k)
            {
                results.Add([.. current]);
                return;
            }

            for (int i = start; i < n; i++)
            {
                current[depth] = i;
                Recurse(i + 1, depth + 1);
            }
        }

        Recurse(0, 0);
        return results;
    }

    /// <summary>Non-decreasing tuples: combinations with repetition.</summary>
    private static List<int[]> ReferenceCombinationsWithRepetition(int n, int k)
    {
        var results = new List<int[]>();
        var current = new int[k];

        void Recurse(int start, int depth)
        {
            if (depth == k)
            {
                results.Add([.. current]);
                return;
            }

            for (int i = start; i < n; i++)
            {
                current[depth] = i;
                Recurse(i, depth + 1);
            }
        }

        Recurse(0, 0);
        return results;
    }

    /// <summary>Tuples of distinct indices in any order: variations.</summary>
    private static List<int[]> ReferenceVariations(int n, int k)
    {
        var results = new List<int[]>();
        var current = new int[k];
        var used = new bool[n];

        void Recurse(int depth)
        {
            if (depth == k)
            {
                results.Add([.. current]);
                return;
            }

            for (int i = 0; i < n; i++)
            {
                if (used[i])
                    continue;

                used[i] = true;
                current[depth] = i;
                Recurse(depth + 1);
                used[i] = false;
            }
        }

        Recurse(0);
        return results;
    }

    /// <summary>Unrestricted tuples: variations with repetition.</summary>
    private static List<int[]> ReferenceVariationsWithRepetition(int n, int k)
    {
        var results = new List<int[]>();
        var current = new int[k];

        void Recurse(int depth)
        {
            if (depth == k)
            {
                results.Add([.. current]);
                return;
            }

            for (int i = 0; i < n; i++)
            {
                current[depth] = i;
                Recurse(depth + 1);
            }
        }

        Recurse(0);
        return results;
    }

    private readonly record struct Point(int X, int Y);

    /// <summary>An <see cref="IList{T}"/> that supports only <c>Count</c> and the getter indexer.</summary>
    private sealed class IndexerOnlyList<T>(params T[] items) : IList<T>
    {
        public T this[int index]
        {
            get => items[index];
            set => throw new NotSupportedException();
        }

        public int Count => items.Length;

        public bool IsReadOnly => true;

        public IEnumerator<T> GetEnumerator() =>
            throw new NotSupportedException("The source must be accessed by index only.");

        IEnumerator IEnumerable.GetEnumerator() =>
            throw new NotSupportedException("The source must be accessed by index only.");

        public void Add(T item) => throw new NotSupportedException();

        public void Clear() => throw new NotSupportedException();

        public bool Contains(T item) => throw new NotSupportedException();

        public void CopyTo(T[] array, int arrayIndex) => throw new NotSupportedException();

        public int IndexOf(T item) => throw new NotSupportedException();

        public void Insert(int index, T item) => throw new NotSupportedException();

        public bool Remove(T item) => throw new NotSupportedException();

        public void RemoveAt(int index) => throw new NotSupportedException();
    }
}
