using KE.AoC.App.ViewModels.Common;
using KE.AoC.Core.Input;
using KE.AoC.Core.Solution;

namespace KE.AoC.App.ViewModels;

/// <summary>
/// Represents a view model for a specific day, containing the corresponding parts for that day.
/// </summary>
public sealed class DayViewModel : ViewModelBase
{
    /// <summary>
    /// Gets the day represented by this view model.
    /// </summary>
    public int Day { get; }

    /// <summary>
    /// Gets the view model for part 1 of the specified day.
    /// </summary>
    public PartViewModel Part1 { get; }

    /// <summary>
    /// Gets the view model for part 2 of the specified day, or null if part 2 is not available.
    /// </summary>
    public PartViewModel? Part2 { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DayViewModel"/> class with the specified solution descriptor, input provider, and solution runner.
    /// </summary>
    /// <param name="descriptor">The solution descriptor for the specified day.</param>
    /// <param name="inputProvider">The input provider.</param>
    /// <param name="runner">The solution runner.</param>
    public DayViewModel(SolutionDescriptor descriptor, IInputProvider inputProvider, SolutionRunner runner)
    {
        Day = descriptor.Day;
        Part1 = new PartViewModel(descriptor, inputProvider, runner, 1);
        if (descriptor.HasPartTwo)
            Part2 = (PartViewModel?)new PartViewModel(descriptor, inputProvider, runner, 2);
    }
}
