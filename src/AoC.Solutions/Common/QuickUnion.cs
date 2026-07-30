namespace KE.AoC.Solutions.Common;

/// <summary>
/// A simple implementation of the Union-Find data structure using the Weighted Quick-Union algorithm with path halving optimization.
/// </summary>
public sealed class QuickUnion
{
    /// <summary>
    /// Array to hold the parent of each element.
    /// </summary>
    /// <remarks>
    /// The index represents the element, and the value at that index represents the parent of that element.
    /// </remarks>
    private readonly int[] parent;

    /// <summary>
    /// Array to hold the size of each component.
    /// </summary>
    /// <remarks>
    /// The index represents the root of the component, and the value at that index represents the number of elements of that component.
    /// </remarks>
    private readonly int[] size;

    /// <summary>
    /// Gets the current number of components (disjoint sets).
    /// </summary>
    public int Components { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QuickUnion"/> class with the specified number of elements.
    /// </summary>
    /// <param name="n">The number of elements.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="n"/> is negative.</exception>
    public QuickUnion(int n)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(n);

        parent = new int[n];
        size = new int[n];

        for (int i = 0; i < n; i++)
        {
            parent[i] = i;
            size[i] = 1;
        }

        Components = n;
    }

    /// <summary>
    /// Returns the size of the component containing element <paramref name="p"/>.
    /// </summary>
    /// <param name="p">The index of the element.</param>
    /// <returns>The size of the component containing <paramref name="p"/>.</returns>
    public int SizeOf(int p)
    {
        // Check that the index is within the valid range.
        CheckIndex(p);

        // Return the size of the component containing the p element.
        return size[FindRoot(p)];
    }

    /// <summary>
    /// Determines whether two elements are in the same component.
    /// </summary>
    /// <param name="p">The index of the first element.</param>
    /// <param name="q">The index of the second element.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="p"/> and <paramref name="q"/>
    /// are in the same component; otherwise <see langword="false"/>.
    /// </returns>
    public bool Connected(int p, int q)
    {
        // Check that the indices are within the valid range.
        CheckIndex(p);
        CheckIndex(q);

        // Two elements are connected if they have a common root.
        return FindRoot(p) == FindRoot(q);
    }

    /// <summary>
    /// Finds the root of the component containing element <paramref name="p"/>.
    /// </summary>
    /// <param name="p">The index of the element.</param>
    /// <returns>
    /// The root of the component containing <paramref name="p"/>. Two elements
    /// are in the same component if and only if their roots are equal. The root
    /// is stable between <see cref="Union"/> calls, but a successful union may
    /// change which element is the root of the merged component.
    /// </returns>
    public int Find(int p)
    {
        // Check that the index is within the valid range.
        CheckIndex(p);

        // Find the root of the p element.
        return FindRoot(p);
    }

    /// <summary>
    /// Unites the components containing elements <paramref name="p"/> and
    /// <paramref name="q"/>.
    /// </summary>
    /// <param name="p">The index of the first element.</param>
    /// <param name="q">The index of the second element.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="p"/> and <paramref name="q"/>
    /// were in different components, which are now merged (decreasing
    /// <see cref="Components"/> by one); <see langword="false"/> if they were
    /// already in the same component and no change was made.
    /// </returns>
    public bool Union(int p, int q)
    {
        // Check that the indices are within the valid range.
        CheckIndex(p);
        CheckIndex(q);

        // Find the roots of the p and q elements.
        int rootP = FindRoot(p);
        int rootQ = FindRoot(q);

        // If they are already in the same component, no union is needed.
        if (rootP == rootQ)
            return false;

        // Make the smaller tree point to the larger tree to keep the tree flat.
        if (size[rootP] < size[rootQ])
        {
            parent[rootP] = rootQ;
            size[rootQ] += size[rootP];
        }
        else
        {
            parent[rootQ] = rootP;
            size[rootP] += size[rootQ];
        }

        // Decrease the number of components since we merged two components into one.
        Components--;

        return true;
    }

    /// <summary>
    /// Finds the root of the component containing element <paramref name="p"/>.
    /// </summary>
    /// <param name="p">The index of the element.</param>
    /// <returns>The root of the component containing <paramref name="p"/>.</returns>
    private int FindRoot(int p)
    {
        // Return the root of the component containing the element.
        while (p != parent[p])
        {
            parent[p] = parent[parent[p]]; // path halving
            p = parent[p];
        }

        return p;
    }

    /// <summary>
    /// Checks if the given index is within the valid range of [0, parent.Length).
    /// </summary>
    /// <param name="i">Index to check.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of bounds.</exception>
    private void CheckIndex(int i)
    {
        if ((uint)i >= (uint)parent.Length)
            throw new ArgumentOutOfRangeException(nameof(i), i, $"Index must be in [0, {parent.Length}).");
    }
}
