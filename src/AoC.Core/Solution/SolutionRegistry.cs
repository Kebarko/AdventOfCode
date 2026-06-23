using System.Reflection;

namespace KE.AoC.Core.Solution;

/// <summary>
/// Represents a descriptor for a solution, including its year, day, factory method for creating an instance, and whether it has a second part.
/// </summary>
/// <param name="Year">The year of the solution.</param>
/// <param name="Day">The day of the solution.</param>
/// <param name="SolutionFactory">A function that creates an instance of the solution.</param>
/// <param name="HasPartTwo">A value indicating whether the solution has a second part.</param>
public sealed record SolutionDescriptor(int Year, int Day, Func<ISolution> SolutionFactory, bool HasPartTwo);

public sealed class SolutionRegistry(params Assembly[] assemblies)
{
    /// <summary>
    /// Gets a read-only list of all solution descriptors found in the specified assemblies, ordered by year and day.
    /// </summary>
    public IReadOnlyList<SolutionDescriptor> All { get; } = assemblies
            .SelectMany(SafeGetTypes)
            .Where(t => typeof(ISolution).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .Select(TryDescribe)
            .Where(d => d != null)
            .Select(d => d!)
            .OrderBy(d => d.Year)
            .ThenBy(d => d.Day)
            .ToList();

    private static SolutionDescriptor? TryDescribe(Type type)
    {
        SolutionAttribute? attr = type.GetCustomAttribute<SolutionAttribute>();
        if (attr == null)
            return null;

        MethodInfo? partTwo = type.GetMethod(nameof(ISolution.PartTwo), [typeof(string)]);
        bool hasPartTwo = partTwo != null && partTwo.DeclaringType == type;

        return new SolutionDescriptor(attr.Year, attr.Day, () => (ISolution)Activator.CreateInstance(type)!, hasPartTwo);
    }

    private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t != null)!;
        }
    }
}
