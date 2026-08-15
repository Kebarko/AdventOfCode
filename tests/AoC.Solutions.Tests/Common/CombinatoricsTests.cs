using KE.AoC.Solutions.Common;
using System.Collections;
using System.Collections.ObjectModel;

namespace KE.AoC.Solutions.Tests.Common;

public sealed class CombinatoricsTests
{
    // ---------------------------------------------------------------------
    // Argument validation
    // ---------------------------------------------------------------------

    [Theory]
    [InlineData(false, 0)]
    [InlineData(false, 1)]
    [InlineData(false, 10)]
    [InlineData(true, 0)]
    [InlineData(true, 1)]
    [InlineData(true, 10)]
    public void NullItems_ThrowsArgumentNullException(bool withRepetition, int length)
    {
        // No enumeration: the guards must run at the call site, not at the first MoveNext.
        var ex = Assert.Throws<ArgumentNullException>(
            () => Invoke<char>(withRepetition, null!, length));

        Assert.Equal("items", ex.ParamName);
    }

    [Theory]
    [InlineData(false, -1)]
    [InlineData(false, -7)]
    [InlineData(false, int.MinValue)]
    [InlineData(true, -1)]
    [InlineData(true, -7)]
    [InlineData(true, int.MinValue)]
    public void NegativeLength_ThrowsArgumentOutOfRangeException(bool withRepetition, int length)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => Invoke(withRepetition, Source(3), length));

        Assert.Equal("length", ex.ParamName);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void NullItems_TakesPrecedenceOverNegativeLength(bool withRepetition)
    {
        Assert.Throws<ArgumentNullException>(() => Invoke<char>(withRepetition, null!, -1));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ArgumentValidation_IsEager_NotDeferredToEnumeration(bool withRepetition)
    {
        // The public method is not an iterator block, so a bad argument surfaces at
        // the call site rather than at the eventual foreach, possibly frames away.
        Assert.Throws<ArgumentNullException>(() => Invoke<char>(withRepetition, null!, 2));
        Assert.Throws<ArgumentOutOfRangeException>(() => Invoke(withRepetition, Source(3), -1));
    }

    // ---------------------------------------------------------------------
    // GetCombinations - exact expected output
    // ---------------------------------------------------------------------

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
    [InlineData(0, 0, null)]            // see EmptySource_YieldsNothing_ForEveryLength
    [InlineData(0, 1, null)]
    [InlineData(0, 5, null)]
    [InlineData(2, 3, null)]            // length > count -> C(n,k) == 0
    [InlineData(3, 100, null)]
    public void GetCombinations_ProducesExpectedSequence(int n, int length, string? expected)
    {
        var actual = Render(Combinatorics.GetCombinations(Source(n), length));

        Assert.Equal(Expand(expected), actual);
    }

    // ---------------------------------------------------------------------
    // GetCombinationsWithRepetition - exact expected output
    // ---------------------------------------------------------------------

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
    public void GetCombinationsWithRepetition_ProducesExpectedSequence(int n, int length, string? expected)
    {
        var actual = Render(Combinatorics.GetCombinationsWithRepetition(Source(n), length));

        Assert.Equal(Expand(expected), actual);
    }

    // ---------------------------------------------------------------------
    // Cross-check against an independent recursive oracle
    // ---------------------------------------------------------------------

    [Theory]
    [MemberData(nameof(SizeAndLengthMatrix))]
    public void GetCombinations_MatchesReferenceImplementation(int n, int length)
    {
        var items = Source(n);
        var expected = ExpectedCombinations(items, n, length, ReferenceCombinations);
        var actual = Render(Combinatorics.GetCombinations(items, length));

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(SizeAndLengthMatrix))]
    public void GetCombinationsWithRepetition_MatchesReferenceImplementation(int n, int length)
    {
        var items = Source(n);
        var expected = ExpectedCombinations(items, n, length, ReferenceCombinationsWithRepetition);
        var actual = Render(Combinatorics.GetCombinationsWithRepetition(items, length));

        Assert.Equal(expected, actual);
    }

    // ---------------------------------------------------------------------
    // Cardinality
    // ---------------------------------------------------------------------

    [Theory]
    [MemberData(nameof(SizeAndLengthMatrix))]
    public void GetCombinations_CountEqualsBinomialCoefficient(int n, int length)
    {
        long expected = n == 0 ? 0 : Binomial(n, length);

        Assert.Equal(expected, Combinatorics.GetCombinations(Source(n), length).LongCount());
    }

    [Theory]
    [MemberData(nameof(SizeAndLengthMatrix))]
    public void GetCombinationsWithRepetition_CountEqualsMultisetCoefficient(int n, int length)
    {
        long expected = n == 0 ? 0 : length == 0 ? 1 : Binomial(n + length - 1, length);

        Assert.Equal(expected, Combinatorics.GetCombinationsWithRepetition(Source(n), length).LongCount());
    }

    [Theory]
    [InlineData(20, 3, 1140)]
    [InlineData(20, 10, 184756)]
    [InlineData(12, 6, 924)]
    public void GetCombinations_CountIsCorrectForLargerInputs(int n, int length, int expected)
    {
        var items = Enumerable.Range(0, n).ToList();

        Assert.Equal(expected, Combinatorics.GetCombinations(items, length).Count());
    }

    [Theory]
    [InlineData(10, 4, 715)]
    [InlineData(5, 8, 495)]
    [InlineData(3, 10, 66)]
    public void GetCombinationsWithRepetition_CountIsCorrectForLargerInputs(int n, int length, int expected)
    {
        var items = Enumerable.Range(0, n).ToList();

        Assert.Equal(expected, Combinatorics.GetCombinationsWithRepetition(items, length).Count());
    }

    // ---------------------------------------------------------------------
    // Structural invariants
    // ---------------------------------------------------------------------

    [Theory]
    [MemberData(nameof(SizeAndLengthMatrix))]
    public void GetCombinations_SelectionsAreStrictlyIncreasing(int n, int length)
    {
        // Source is 'a','b','c',... so ordinal order mirrors index order.
        Assert.All(
            Combinatorics.GetCombinations(Source(n), length),
            combination => Assert.True(IsOrdered(combination, strict: true)));
    }

    [Theory]
    [MemberData(nameof(SizeAndLengthMatrix))]
    public void GetCombinationsWithRepetition_SelectionsAreNonDecreasing(int n, int length)
    {
        Assert.All(
            Combinatorics.GetCombinationsWithRepetition(Source(n), length),
            combination => Assert.True(IsOrdered(combination, strict: false)));
    }

    [Theory]
    [MemberData(nameof(SizeAndLengthMatrix))]
    public void BothMethods_YieldCombinationsOfRequestedLength(int n, int length)
    {
        Assert.All(Combinatorics.GetCombinations(Source(n), length),
            c => Assert.Equal(length, c.Length));
        Assert.All(Combinatorics.GetCombinationsWithRepetition(Source(n), length),
            c => Assert.Equal(length, c.Length));
    }

    [Theory]
    [MemberData(nameof(SizeAndLengthMatrix))]
    public void BothMethods_YieldNoDuplicateCombinations(int n, int length)
    {
        var strict = Render(Combinatorics.GetCombinations(Source(n), length));
        var repeating = Render(Combinatorics.GetCombinationsWithRepetition(Source(n), length));

        Assert.Equal(strict.Length, strict.Distinct().Count());
        Assert.Equal(repeating.Length, repeating.Distinct().Count());
    }

    [Theory]
    [InlineData(4, 2)]
    [InlineData(5, 3)]
    [InlineData(3, 3)]
    [InlineData(6, 1)]
    [InlineData(4, 0)]
    public void WithRepetition_IsSupersetOfStrictCombinations(int n, int length)
    {
        var strict = Render(Combinatorics.GetCombinations(Source(n), length));
        var repeating = Render(Combinatorics.GetCombinationsWithRepetition(Source(n), length)).ToHashSet();

        Assert.All(strict, s => Assert.Contains(s, repeating));
    }

    [Theory]
    [InlineData(1, 3, "aaa")]
    [InlineData(3, 2, "aa")]
    [InlineData(3, 4, "cccc")]
    public void WithRepetition_IncludesUniformCombinations(int n, int length, string expected)
    {
        var actual = Render(Combinatorics.GetCombinationsWithRepetition(Source(n), length));

        Assert.Contains(expected, actual);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FullEnumeration_TerminatesWithoutThrowing(bool withRepetition)
    {
        // Regression guard: the odometer must stop when every position is saturated
        // instead of walking off the left edge of the index array.
        for (int n = 0; n <= Alphabet.Length; n++)
        {
            for (int length = 0; length <= Alphabet.Length; length++)
            {
                var sequence = Invoke(withRepetition, Source(n), length);
                var exception = Record.Exception(() => sequence.ToList());

                Assert.Null(exception);
            }
        }
    }

    [Theory]
    [InlineData(false, 4, 4)]
    [InlineData(false, 1, 1)]
    [InlineData(false, 6, 6)]
    [InlineData(false, 5, 2)]
    [InlineData(true, 4, 1)]
    [InlineData(true, 1, 1)]
    [InlineData(true, 3, 3)]
    public void EnumeratingPastTheEnd_ReturnsFalse(bool withRepetition, int n, int length)
    {
        using var enumerator = Invoke(withRepetition, Source(n), length).GetEnumerator();

        while (enumerator.MoveNext()) { }

        Assert.False(enumerator.MoveNext());
        Assert.False(enumerator.MoveNext());
    }

    [Theory]
    [InlineData(4, 4, "abcd")]
    [InlineData(1, 1, "a")]
    [InlineData(6, 6, "abcdef")]
    public void GetCombinations_LengthEqualToCount_YieldsWholeSourceOnce(int n, int length, string expected)
    {
        var actual = Render(Combinatorics.GetCombinations(Source(n), length));

        Assert.Equal(expected, Assert.Single(actual));
    }

    [Theory]
    [InlineData(3, 4)]
    [InlineData(3, 5)]
    [InlineData(1, 2)]
    [InlineData(5, int.MaxValue)]
    public void GetCombinations_LengthGreaterThanCount_YieldsNothing(int n, int length)
    {
        Assert.Empty(Combinatorics.GetCombinations(Source(n), length));
    }

    // ---------------------------------------------------------------------
    // Contract of the returned sequence
    // ---------------------------------------------------------------------

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EachYieldedArrayIsADistinctInstance(bool withRepetition)
    {
        // No internal buffer reuse: callers must be able to retain the arrays.
        var combinations = Invoke(withRepetition, Source(4), 2).ToList();

        Assert.Equal(
            combinations.Count,
            combinations.Cast<object>().Distinct(ReferenceEqualityComparer.Instance).Count());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void MutatingAYieldedArray_DoesNotAffectSubsequentResults(bool withRepetition)
    {
        var sequence = Invoke(withRepetition, Source(4), 2);
        var expected = Render(sequence);

        foreach (var combination in sequence)
            Array.Fill(combination, 'z');

        Assert.Equal(expected, Render(sequence));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Sequence_CanBeEnumeratedMoreThanOnce(bool withRepetition)
    {
        var sequence = Invoke(withRepetition, Source(4), 2);

        Assert.Equal(Render(sequence), Render(sequence));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Sequence_IsLazyAndDoesNotMaterialiseEverything(bool withRepetition)
    {
        // C(60, 30) is ~1.18e17: a non-lazy implementation would never return.
        var items = Enumerable.Range(0, 60).ToList();

        var head = Invoke(withRepetition, items, 30).Take(3).ToList();

        Assert.Equal(3, head.Count);
        Assert.All(head, c => Assert.Equal(30, c.Length));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SourceCollection_IsNotMutated(bool withRepetition)
    {
        var items = Source(5);
        var snapshot = items.ToArray();

        _ = Invoke(withRepetition, items, 3).ToList();

        Assert.Equal(snapshot, items);
    }

    [Theory]
    [InlineData("array")]
    [InlineData("list")]
    [InlineData("collection")]
    [InlineData("readOnlyCollection")]
    [InlineData("indexerOnly")]
    public void WorksWithAnyIListImplementation(string kind)
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
        Assert.Equal(new[] { "ab", "ac", "bc" },
            Render(Combinatorics.GetCombinations(items, 2)));
        Assert.Equal(new[] { "aa", "ab", "ac", "bb", "bc", "cc" },
            Render(Combinatorics.GetCombinationsWithRepetition(items, 2)));
    }

    // ---------------------------------------------------------------------
    // Element semantics
    // ---------------------------------------------------------------------

    [Theory]
    [InlineData(false, 2, "aa,ab,ab")]
    [InlineData(true, 2, "aa,aa,ab,aa,ab,bb")]
    public void DuplicateSourceItems_AreTreatedPositionally(bool withRepetition, int length, string expected)
    {
        // Combinations are over positions, not over values: no de-duplication.
        var items = new List<char> { 'a', 'a', 'b' };

        Assert.Equal(expected.Split(','), Render(Invoke(withRepetition, items, length)));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void NullElements_ArePreserved(bool withRepetition)
    {
        var items = new List<string?> { "x", null, "y" };

        var combinations = Invoke(withRepetition, items, 2).ToList();

        Assert.Contains(combinations, c => c.Any(item => item is null));
        Assert.All(combinations, c => Assert.Equal(2, c.Length));
    }

    [Theory]
    [InlineData(false, 6)]
    [InlineData(true, 10)]
    public void ValueTypeElements_AreCopiedByValue(bool withRepetition, int expectedCount)
    {
        var items = new List<Point> { new(0, 0), new(1, 1), new(2, 2), new(3, 3) };

        var combinations = Invoke(withRepetition, items, 2).ToList();

        Assert.Equal(expectedCount, combinations.Count);
        Assert.All(combinations, c => Assert.All(c, p => Assert.Equal(p.X, p.Y)));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ReferenceTypeElements_AreNotCloned(bool withRepetition)
    {
        var first = new object();
        var second = new object();
        var items = new List<object> { first, second };

        var combinations = Invoke(withRepetition, items, 2).ToList();

        Assert.All(combinations,
            c => Assert.All(c, o => Assert.True(ReferenceEquals(o, first) || ReferenceEquals(o, second))));
    }

    [Theory]
    [InlineData(false, 0)]
    [InlineData(false, 1)]
    [InlineData(false, 5)]
    [InlineData(true, 0)]
    [InlineData(true, 1)]
    [InlineData(true, 5)]
    public void EmptySource_YieldsNothing_ForEveryLength(bool withRepetition, int length)
    {
        // Documented convention: an empty source yields nothing even for length 0,
        // so C(0,0) evaluates to 0 rather than 1. Flip the length == 0 case here
        // (and the n == 0 carve-outs in the count and oracle tests) if that changes.
        Assert.Empty(Invoke(withRepetition, Array.Empty<char>(), length));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void NonEmptySource_WithZeroLength_YieldsOneEmptyCombination(bool withRepetition)
    {
        var combination = Assert.Single(Invoke(withRepetition, Source(3), 0));

        Assert.Empty(combination);
    }

    // ---------------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------------

    private const string Alphabet = "abcdef";

    public static TheoryData<int, int> SizeAndLengthMatrix()
    {
        var data = new TheoryData<int, int>();
        for (int n = 0; n <= Alphabet.Length; n++)
            for (int length = 0; length <= Alphabet.Length + 1; length++)
                data.Add(n, length);
        return data;
    }

    private static IList<char> Source(int n) => Alphabet.Take(n).ToList();

    private static IEnumerable<T[]> Invoke<T>(bool withRepetition, IList<T> items, int length) =>
        withRepetition
            ? Combinatorics.GetCombinationsWithRepetition(items, length)
            : Combinatorics.GetCombinations(items, length);

    private static string[] Render(IEnumerable<char[]> combinations) =>
        combinations.Select(c => new string(c)).ToArray();

    private static string[] Render<T>(IEnumerable<T[]> combinations) =>
        combinations.Select(c => string.Concat(c.Select(x => x?.ToString() ?? "<null>"))).ToArray();

    /// <summary>Null means "no combinations"; "" means "one empty combination".</summary>
    private static string[] Expand(string? expected) =>
        expected is null ? [] : expected.Split(',');

    private static IEnumerable<string> ExpectedCombinations(
        IList<char> items, int n, int length, Func<int, int, List<int[]>> oracle) =>
        // The oracle yields the empty tuple for n == 0, length == 0; the
        // implementation suppresses it for every empty source.
        n == 0 ? [] : oracle(n, length).Select(indices => new string(indices.Select(i => items[i]).ToArray()));

    private static bool IsOrdered(char[] combination, bool strict)
    {
        for (int i = 1; i < combination.Length; i++)
        {
            if (strict ? combination[i] <= combination[i - 1] : combination[i] < combination[i - 1])
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

    /// <summary>Independent recursive oracle for strictly increasing index tuples.</summary>
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

    /// <summary>Independent recursive oracle for non-decreasing index tuples.</summary>
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
