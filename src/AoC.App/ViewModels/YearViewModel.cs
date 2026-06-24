using KE.AoC.App.ViewModels.Common;
using System.Collections.ObjectModel;

namespace KE.AoC.App.ViewModels;

/// <summary>
/// Represents a view model for a specific year, containing a collection of days and their corresponding parts.
/// </summary>
public sealed class YearViewModel : ViewModelBase
{
    /// <summary>
    /// Gets the year represented by this view model.
    /// </summary>
    public int Year { get; }

    /// <summary>
    /// Gets the collection of days for the specified year, each containing their corresponding parts.
    /// </summary>
    public ObservableCollection<DayViewModel> Days { get; }

    /// <summary>
    /// Gets the command to run all parts for the specified year.
    /// </summary>
    public AsyncRelayCommand RunAllCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="YearViewModel"/> class with the specified year and collection of days.
    /// </summary>
    /// <param name="year">The year represented by this view model.</param>
    /// <param name="days">The collection of days for the specified year.</param>
    public YearViewModel(int year, IEnumerable<DayViewModel> days)
    {
        Year = year;
        Days = new ObservableCollection<DayViewModel>(days);
        RunAllCommand = new AsyncRelayCommand(RunAllAsync);
    }

    private async Task RunAllAsync()
    {
        IEnumerable<Task> parts = Days
            .SelectMany(day => new[] { day.Part1, day.Part2 })
            .Where(part => part is not null)
            .Select(part => part!.RunAsync());

        await Task.WhenAll(parts);
    }
}
