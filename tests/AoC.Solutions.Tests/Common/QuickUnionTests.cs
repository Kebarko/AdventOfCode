using KE.AoC.Solutions.Common;

namespace KE.AoC.Solutions.Tests.Common;

public sealed class QuickUnionTests
{
    #region Test data

    /// <summary>
    /// Element counts that are valid but interesting: empty, singleton, small, larger.
    /// </summary>
    public static TheoryData<int> ElementCounts => new() { 0, 1, 2, 3, 10, 257 };

    /// <summary>
    /// Non-empty element counts (at least one valid index exists).
    /// </summary>
    public static TheoryData<int> NonEmptyElementCounts => new() { 1, 2, 3, 10, 257 };

    /// <summary>
    /// (n, invalidIndex) pairs covering below-range, at-range and extreme values.
    /// </summary>
    public static TheoryData<int, int> InvalidIndices => new()
    {
        { 0, 0 },
        { 0, -1 },
        { 0, 1 },
        { 0, int.MaxValue },
        { 0, int.MinValue },
        { 1, 1 },
        { 1, -1 },
        { 1, int.MaxValue },
        { 1, int.MinValue },
        { 10, 10 },
        { 10, 11 },
        { 10, -1 },
        { 10, int.MinValue },
    };

    /// <summary>
    /// (n, invalidIndex) pairs restricted to non-empty instances, so that index 0 is always a valid partner.
    /// </summary>
    public static TheoryData<int, int> InvalidIndicesNonEmpty => new()
    {
        { 1, 1 },
        { 1, -1 },
        { 1, int.MaxValue },
        { 1, int.MinValue },
        { 10, 10 },
        { 10, 11 },
        { 10, -1 },
        { 10, int.MinValue },
    };

    /// <summary>
    /// (n, edges, expectedComponents) graph cases, including duplicate edges, self-edges and cycles.
    /// </summary>
    public static TheoryData<int, int[][], int> GraphCases => new()
    {
        // No edges at all.
        { 1, [], 1 },
        { 5, [], 5 },

        // Single edge.
        { 2, [[0, 1]], 1 },
        { 5, [[0, 4]], 4 },

        // Self-edges are no-ops.
        { 5, [[0, 0], [3, 3]], 5 },

        // Duplicate edges (both orders) are no-ops after the first.
        { 5, [[0, 1], [0, 1], [1, 0]], 4 },

        // Chain.
        { 5, [[0, 1], [1, 2], [2, 3], [3, 4]], 1 },

        // Cycle: the closing edge is redundant.
        { 5, [[0, 1], [1, 2], [2, 0]], 3 },

        // Two disjoint pairs.
        { 6, [[0, 1], [2, 3]], 4 },

        // Three disjoint pairs.
        { 6, [[0, 1], [2, 3], [4, 5]], 3 },

        // Two chains, then bridged into one component.
        { 6, [[0, 1], [1, 2], [3, 4], [4, 5], [2, 3]], 1 },

        // Star.
        { 7, [[3, 0], [3, 1], [3, 2], [3, 4], [3, 5], [3, 6]], 1 },

        // Interleaved, unsorted, redundant edges.
        { 8, [[7, 1], [3, 5], [1, 7], [5, 3], [0, 2], [2, 0], [4, 6]], 4 },
    };

    /// <summary>
    /// (seed, n, operationCount) cases for the randomized comparison against a naive reference.
    /// </summary>
    public static TheoryData<int, int, int> RandomCases => new()
    {
        { 1, 2, 50 },
        { 2, 5, 100 },
        { 3, 16, 200 },
        { 4, 64, 500 },
        { 5, 200, 1_000 },
        { 6, 1_000, 3_000 },
    };

    #endregion

    #region Construction

    [Theory]
    [InlineData(-1)]
    [InlineData(-2)]
    [InlineData(-1_000)]
    [InlineData(int.MinValue)]
    public void Constructor_WithNegativeElementCount_Throws(int n)
    {
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() => new QuickUnion(n));

        Assert.Equal("n", exception.ParamName);
    }

    [Theory]
    [MemberData(nameof(ElementCounts))]
    public void Constructor_SetsComponentsToElementCount(int n)
    {
        QuickUnion uf = new(n);

        Assert.Equal(n, uf.Components);
    }

    [Theory]
    [MemberData(nameof(NonEmptyElementCounts))]
    public void NewInstance_EveryElementIsItsOwnSingletonRoot(int n)
    {
        QuickUnion uf = new(n);

        for (int i = 0; i < n; i++)
        {
            Assert.Equal(i, uf.Find(i));
            Assert.Equal(1, uf.SizeOf(i));
            Assert.True(uf.Connected(i, i));
        }
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(10)]
    public void NewInstance_DistinctElementsAreNotConnected(int n)
    {
        QuickUnion uf = new(n);

        for (int p = 0; p < n; p++)
        {
            for (int q = p + 1; q < n; q++)
            {
                Assert.False(uf.Connected(p, q));
                Assert.False(uf.Connected(q, p));
            }
        }
    }

    #endregion

    #region Index validation

    [Theory]
    [MemberData(nameof(InvalidIndices))]
    public void SizeOf_WithIndexOutOfRange_Throws(int n, int index)
    {
        QuickUnion uf = new(n);

        Assert.Throws<ArgumentOutOfRangeException>(() => uf.SizeOf(index));
    }

    [Theory]
    [MemberData(nameof(InvalidIndices))]
    public void Find_WithIndexOutOfRange_Throws(int n, int index)
    {
        QuickUnion uf = new(n);

        Assert.Throws<ArgumentOutOfRangeException>(() => uf.Find(index));
    }

    [Theory]
    [MemberData(nameof(InvalidIndices))]
    public void Connected_WithFirstIndexOutOfRange_Throws(int n, int index)
    {
        QuickUnion uf = new(n);

        Assert.Throws<ArgumentOutOfRangeException>(() => uf.Connected(index, index));
    }

    [Theory]
    [MemberData(nameof(InvalidIndicesNonEmpty))]
    public void Connected_WithSecondIndexOutOfRange_Throws(int n, int index)
    {
        QuickUnion uf = new(n);

        Assert.Throws<ArgumentOutOfRangeException>(() => uf.Connected(0, index));
    }

    [Theory]
    [MemberData(nameof(InvalidIndices))]
    public void Union_WithFirstIndexOutOfRange_Throws(int n, int index)
    {
        QuickUnion uf = new(n);

        Assert.Throws<ArgumentOutOfRangeException>(() => uf.Union(index, index));
    }

    [Theory]
    [MemberData(nameof(InvalidIndicesNonEmpty))]
    public void Union_WithSecondIndexOutOfRange_Throws(int n, int index)
    {
        QuickUnion uf = new(n);

        Assert.Throws<ArgumentOutOfRangeException>(() => uf.Union(0, index));
    }

    [Theory]
    [MemberData(nameof(InvalidIndicesNonEmpty))]
    public void Union_WithIndexOutOfRange_LeavesStateUnchanged(int n, int index)
    {
        QuickUnion uf = new(n);

        Assert.Throws<ArgumentOutOfRangeException>(() => uf.Union(0, index));
        Assert.Throws<ArgumentOutOfRangeException>(() => uf.Union(index, 0));

        Assert.Equal(n, uf.Components);
        Assert.Equal(1, uf.SizeOf(0));
        Assert.Equal(0, uf.Find(0));
    }

    #endregion

    #region Union semantics

    [Theory]
    [InlineData(2, 0, 1)]
    [InlineData(5, 0, 4)]
    [InlineData(5, 3, 1)]
    [InlineData(257, 256, 0)]
    public void Union_OfElementsInDifferentComponents_MergesThem(int n, int p, int q)
    {
        QuickUnion uf = new(n);

        Assert.True(uf.Union(p, q));

        Assert.Equal(n - 1, uf.Components);
        Assert.True(uf.Connected(p, q));
        Assert.True(uf.Connected(q, p));
        Assert.Equal(uf.Find(p), uf.Find(q));
        Assert.Equal(2, uf.SizeOf(p));
        Assert.Equal(2, uf.SizeOf(q));
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(5, 0)]
    [InlineData(5, 2)]
    [InlineData(5, 4)]
    public void Union_OfElementWithItself_ReturnsFalseAndLeavesStateUnchanged(int n, int p)
    {
        QuickUnion uf = new(n);

        Assert.False(uf.Union(p, p));

        Assert.Equal(n, uf.Components);
        Assert.Equal(1, uf.SizeOf(p));
        Assert.Equal(p, uf.Find(p));
    }

    [Theory]
    [InlineData(2, 0, 1)]
    [InlineData(6, 5, 2)]
    [InlineData(6, 0, 3)]
    public void Union_RepeatedForTheSamePair_ReturnsFalseAfterTheFirstCall(int n, int p, int q)
    {
        QuickUnion uf = new(n);

        Assert.True(uf.Union(p, q));

        int componentsAfterMerge = uf.Components;
        int rootAfterMerge = uf.Find(p);

        Assert.False(uf.Union(p, q));
        Assert.False(uf.Union(q, p));
        Assert.False(uf.Union(p, p));

        Assert.Equal(componentsAfterMerge, uf.Components);
        Assert.Equal(rootAfterMerge, uf.Find(p));
        Assert.Equal(rootAfterMerge, uf.Find(q));
        Assert.Equal(2, uf.SizeOf(p));
    }

    [Theory]
    [InlineData(2, 0, 1)]
    [InlineData(9, 7, 2)]
    public void Union_IsSymmetricInItsArguments(int n, int p, int q)
    {
        QuickUnion forward = new(n);
        QuickUnion reverse = new(n);

        Assert.Equal(forward.Union(p, q), reverse.Union(q, p));

        Assert.Equal(forward.Components, reverse.Components);
        Assert.Equal(forward.SizeOf(p), reverse.SizeOf(p));
        Assert.Equal(forward.SizeOf(q), reverse.SizeOf(q));
        Assert.Equal(forward.Find(p) == forward.Find(q), reverse.Find(p) == reverse.Find(q));
    }

    [Theory]
    [InlineData(3, 0, 1, 2)]
    [InlineData(5, 4, 2, 0)]
    [InlineData(5, 1, 3, 4)]
    public void Union_IsTransitive(int n, int a, int b, int c)
    {
        QuickUnion uf = new(n);

        uf.Union(a, b);
        uf.Union(b, c);

        Assert.True(uf.Connected(a, c));
        Assert.True(uf.Connected(c, a));
        Assert.Equal(n - 2, uf.Components);
        Assert.Equal(3, uf.SizeOf(a));
        Assert.Equal(3, uf.SizeOf(b));
        Assert.Equal(3, uf.SizeOf(c));
    }

    [Theory]
    [InlineData(6, 3, 4, 5)]
    [InlineData(8, 0, 1, 7)]
    public void Union_DoesNotConnectUnrelatedElements(int n, int p, int q, int outsider)
    {
        QuickUnion uf = new(n);

        uf.Union(p, q);

        Assert.False(uf.Connected(p, outsider));
        Assert.False(uf.Connected(outsider, q));
        Assert.Equal(1, uf.SizeOf(outsider));
        Assert.Equal(outsider, uf.Find(outsider));
    }

    [Theory]
    [MemberData(nameof(NonEmptyElementCounts))]
    public void UnionOfAllElements_ProducesOneComponentOfFullSize(int n)
    {
        QuickUnion uf = new(n);

        for (int i = 1; i < n; i++)
            Assert.True(uf.Union(0, i));

        Assert.Equal(1, uf.Components);

        int root = uf.Find(0);

        for (int i = 0; i < n; i++)
        {
            Assert.Equal(root, uf.Find(i));
            Assert.Equal(n, uf.SizeOf(i));
            Assert.True(uf.Connected(0, i));
        }
    }

    [Theory]
    [InlineData(4)]
    [InlineData(16)]
    public void UnionOnFullyMergedInstance_ReturnsFalseAndKeepsSingleComponent(int n)
    {
        QuickUnion uf = new(n);

        for (int i = 1; i < n; i++)
            uf.Union(0, i);

        for (int p = 0; p < n; p++)
        {
            for (int q = 0; q < n; q++)
                Assert.False(uf.Union(p, q));
        }

        Assert.Equal(1, uf.Components);
        Assert.Equal(n, uf.SizeOf(0));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Union_AttachesTheSmallerComponentToTheLargerComponentsRoot(bool largerFirst)
    {
        QuickUnion uf = new(6);

        // Build a component of size 3: {0, 1, 2}.
        uf.Union(0, 1);
        uf.Union(0, 2);

        int largeRoot = uf.Find(0);

        // Merge the singleton {5} into it, in either argument order.
        Assert.True(largerFirst ? uf.Union(0, 5) : uf.Union(5, 0));

        // Union by size must keep the larger component's root.
        Assert.Equal(largeRoot, uf.Find(5));
        Assert.Equal(largeRoot, uf.Find(0));
        Assert.Equal(4, uf.SizeOf(largeRoot));
        Assert.Equal(3, uf.Components);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Union_OfEquallySizedComponents_KeepsOneOfTheTwoRoots(bool reversed)
    {
        QuickUnion uf = new(4);

        uf.Union(0, 1);
        uf.Union(2, 3);

        int leftRoot = uf.Find(0);
        int rightRoot = uf.Find(2);

        Assert.True(reversed ? uf.Union(2, 0) : uf.Union(0, 2));

        int mergedRoot = uf.Find(0);

        // Which root survives a tie is implementation-defined; that it is one of them is not.
        Assert.Contains(mergedRoot, new[] { leftRoot, rightRoot });
        Assert.Equal(mergedRoot, uf.Find(3));
        Assert.Equal(4, uf.SizeOf(mergedRoot));
        Assert.Equal(1, uf.Components);
    }

    #endregion

    #region Component structure invariants

    [Theory]
    [MemberData(nameof(GraphCases))]
    public void AfterUnions_ComponentsEqualsTheNumberOfDistinctRoots(int n, int[][] edges, int expectedComponents)
    {
        QuickUnion uf = Build(n, edges);

        int distinctRoots = Roots(uf, n).Distinct().Count();

        Assert.Equal(expectedComponents, uf.Components);
        Assert.Equal(expectedComponents, distinctRoots);
    }

    [Theory]
    [MemberData(nameof(GraphCases))]
    public void AfterUnions_SizeOfMatchesActualComponentSizeForEveryElement(int n, int[][] edges, int expectedComponents)
    {
        _ = expectedComponents;

        QuickUnion uf = Build(n, edges);
        int[] roots = Roots(uf, n);

        var groups = Enumerable.Range(0, n).GroupBy(i => roots[i]).ToArray();

        foreach (var group in groups)
        {
            int expectedSize = group.Count();

            // Includes non-root members, whose own slot in the internal size array is stale.
            foreach (int element in group)
                Assert.Equal(expectedSize, uf.SizeOf(element));
        }

        int totalSize = groups.Sum(group => group.Count());

        Assert.Equal(n, totalSize);
    }

    [Theory]
    [MemberData(nameof(GraphCases))]
    public void AfterUnions_EveryRootIsItsOwnRoot(int n, int[][] edges, int expectedComponents)
    {
        _ = expectedComponents;

        QuickUnion uf = Build(n, edges);

        foreach (int root in Roots(uf, n).Distinct())
            Assert.Equal(root, uf.Find(root));
    }

    [Theory]
    [MemberData(nameof(GraphCases))]
    public void AfterUnions_ConnectedAgreesWithRootEquality(int n, int[][] edges, int expectedComponents)
    {
        _ = expectedComponents;

        QuickUnion uf = Build(n, edges);
        int[] roots = Roots(uf, n);

        for (int p = 0; p < n; p++)
        {
            for (int q = 0; q < n; q++)
                Assert.Equal(roots[p] == roots[q], uf.Connected(p, q));
        }
    }

    [Theory]
    [MemberData(nameof(GraphCases))]
    public void Find_IsStableAcrossRepeatedCalls(int n, int[][] edges, int expectedComponents)
    {
        _ = expectedComponents;

        QuickUnion uf = Build(n, edges);

        int[] first = Roots(uf, n);
        int[] second = Roots(uf, n);
        int[] third = Roots(uf, n);

        // Path halving mutates the internal parent array but must never change the reported root.
        Assert.Equal(first, second);
        Assert.Equal(first, third);
    }

    [Theory]
    [MemberData(nameof(GraphCases))]
    public void ReadOperations_DoNotChangeComponents(int n, int[][] edges, int expectedComponents)
    {
        _ = expectedComponents;

        QuickUnion uf = Build(n, edges);
        int before = uf.Components;

        for (int i = 0; i < n; i++)
        {
            _ = uf.Find(i);
            _ = uf.SizeOf(i);
            _ = uf.Connected(i, 0);
        }

        Assert.Equal(before, uf.Components);
    }

    [Theory]
    [MemberData(nameof(GraphCases))]
    public void SuccessfulUnions_EqualElementCountMinusComponents(int n, int[][] edges, int expectedComponents)
    {
        _ = expectedComponents;

        QuickUnion uf = new(n);
        int merges = 0;

        foreach (int[] edge in edges)
        {
            if (uf.Union(edge[0], edge[1]))
                merges++;
        }

        Assert.Equal(n - uf.Components, merges);
    }

    #endregion

    #region Scale and degenerate shapes

    [Theory]
    [InlineData(1_000)]
    [InlineData(100_000)]
    public void Find_OnLongChain_ResolvesToASingleRootForEveryElement(int n)
    {
        QuickUnion uf = new(n);

        for (int i = 1; i < n; i++)
            uf.Union(i - 1, i);

        Assert.Equal(1, uf.Components);

        int root = uf.Find(0);

        for (int i = 0; i < n; i++)
        {
            Assert.Equal(root, uf.Find(i));
            Assert.Equal(n, uf.SizeOf(i));
        }
    }

    [Theory]
    [InlineData(1_024)]
    [InlineData(65_536)]
    public void PairwiseDoublingMerges_ProduceASingleComponent(int n)
    {
        QuickUnion uf = new(n);

        // Merge equal-sized components level by level, the worst case for tree depth under union by size.
        for (int step = 1; step < n; step *= 2)
        {
            for (int i = 0; i + step < n; i += step * 2)
                Assert.True(uf.Union(i, i + step));
        }

        Assert.Equal(1, uf.Components);
        Assert.Equal(n, uf.SizeOf(n - 1));

        int root = uf.Find(0);

        for (int i = 0; i < n; i++)
            Assert.Equal(root, uf.Find(i));
    }

    [Theory]
    [InlineData(10_000)]
    public void SelfContainedComponents_RemainSeparate(int n)
    {
        QuickUnion uf = new(n);

        // Pair every even element with its odd successor: n / 2 components of size 2.
        for (int i = 0; i + 1 < n; i += 2)
            uf.Union(i, i + 1);

        Assert.Equal(n / 2, uf.Components);

        for (int i = 0; i + 1 < n; i += 2)
        {
            Assert.True(uf.Connected(i, i + 1));
            Assert.Equal(2, uf.SizeOf(i));

            if (i + 2 < n)
                Assert.False(uf.Connected(i, i + 2));
        }
    }

    #endregion

    #region Randomized comparison against a naive reference

    [Theory]
    [MemberData(nameof(RandomCases))]
    public void RandomOperationSequence_MatchesNaiveReferenceImplementation(int seed, int n, int operations)
    {
        Random random = new(seed);

        QuickUnion actual = new(n);
        NaiveUnionFind expected = new(n);

        for (int step = 0; step < operations; step++)
        {
            int p = random.Next(n);
            int q = random.Next(n);

            Assert.Equal(expected.Connected(p, q), actual.Connected(p, q));
            Assert.Equal(expected.Union(p, q), actual.Union(p, q));
            Assert.Equal(expected.Components, actual.Components);
            Assert.Equal(expected.SizeOf(p), actual.SizeOf(p));
            Assert.Equal(expected.SizeOf(q), actual.SizeOf(q));
            Assert.True(actual.Connected(p, q));
        }

        for (int i = 0; i < n; i++)
        {
            Assert.Equal(expected.SizeOf(i), actual.SizeOf(i));

            for (int j = 0; j < n; j++)
                Assert.Equal(expected.Connected(i, j), actual.Connected(i, j));
        }
    }

    #endregion

    #region Helpers

    private static QuickUnion Build(int n, int[][] edges)
    {
        QuickUnion uf = new(n);

        foreach (int[] edge in edges)
            uf.Union(edge[0], edge[1]);

        return uf;
    }

    private static int[] Roots(QuickUnion uf, int n) => [.. Enumerable.Range(0, n).Select(uf.Find)];

    /// <summary>
    /// Obviously correct O(n) reference implementation used as a test oracle.
    /// </summary>
    private sealed class NaiveUnionFind
    {
        private readonly int[] label;

        public NaiveUnionFind(int n)
        {
            label = new int[n];

            for (int i = 0; i < n; i++)
                label[i] = i;
        }

        public int Components => label.Distinct().Count();

        public bool Connected(int p, int q) => label[p] == label[q];

        public int SizeOf(int p) => label.Count(l => l == label[p]);

        public bool Union(int p, int q)
        {
            if (label[p] == label[q])
                return false;

            int from = label[q];
            int to = label[p];

            for (int i = 0; i < label.Length; i++)
            {
                if (label[i] == from)
                    label[i] = to;
            }

            return true;
        }
    }

    #endregion
}
