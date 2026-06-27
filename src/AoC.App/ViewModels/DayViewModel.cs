using KE.AoC.App.ViewModels.Common;
using KE.AoC.Core.Input;
using KE.AoC.Core.Solution;
using System.Diagnostics;

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
    /// Gets the URL for the specified day on the Advent of Code website.
    /// </summary>
    public Uri Url { get; }

    /// <summary>
    /// Gets the command to open the puzzle for the specified day in the default web browser.
    /// </summary>
    public RelayCommand OpenPuzzleCommand { get; }

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
        Url = new($"https://adventofcode.com/{descriptor.Year}/day/{Day}");
        OpenPuzzleCommand = new RelayCommand(OpenPuzzle);
    }

    private void OpenPuzzle()
    {
        try
        {
            Process.Start(new ProcessStartInfo(Url.AbsoluteUri) { UseShellExecute = true });
        }
        catch
        {
            Debug.WriteLine($"Failed to open URL: {Url.AbsoluteUri}");
        }
    }
}
