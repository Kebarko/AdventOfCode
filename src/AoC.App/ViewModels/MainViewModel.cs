using KE.AoC.App.ViewModels.Common;
using KE.AoC.Core.Input;
using KE.AoC.Core.Solution;
using System.Collections.ObjectModel;

namespace KE.AoC.App.ViewModels;

/// <summary>
/// Represents a view model for the main application window, containing a collection of years and their corresponding days and parts.    
/// </summary>
public sealed class MainViewModel : ViewModelBase
{
    /// <summary>
    /// Gets the collection of years, each containing a collection of days and their corresponding parts.
    /// </summary>
    public ObservableCollection<YearViewModel> Years { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class with the specified solution registry, input provider, and solution runner.
    /// </summary>
    /// <param name="registry">The solution registry.</param>
    /// <param name="inputProvider">The input provider.</param>
    /// <param name="runner">The solution runner.</param>
    public MainViewModel(SolutionRegistry registry, IInputProvider inputProvider, SolutionRunner runner)
    {
        Years = new ObservableCollection<YearViewModel>(
            registry.All
                .GroupBy(descriptor => descriptor.Year)
                .OrderByDescending(group => group.Key)
                .Select(group => new YearViewModel(group.Key, group
                    .OrderBy(descriptor => descriptor.Day)
                    .Select(descriptor => new DayViewModel(descriptor, inputProvider, runner))
                    .ToList()))
        );
    }
}
