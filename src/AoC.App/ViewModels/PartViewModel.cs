using KE.AoC.App.ViewModels.Common;
using KE.AoC.Core.Input;
using KE.AoC.Core.Solution;

namespace KE.AoC.App.ViewModels;

/// <summary>
/// Represents a view model for a specific part of a solution, providing functionality to run the solution and display the result, elapsed time, and error state.
/// </summary>
public sealed class PartViewModel : ViewModelBase
{
    private readonly SolutionDescriptor descriptor;
    private readonly IInputProvider inputProvider;
    private readonly SolutionRunner runner;
    private bool isRunning;
    private string result = string.Empty;
    private TimeSpan elapsed = TimeSpan.Zero;
    private bool hasError;

    /// <summary>
    /// Gets the part number represented by this view model (1 or 2).
    /// </summary>
    public int Part { get; }

    /// <summary>
    /// Gets the command to run the solution for the specified part.
    /// </summary>
    public AsyncRelayCommand RunCommand { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the solution is currently running.
    /// </summary>
    public bool IsRunning
    {
        get => isRunning;
        set { if (SetProperty(ref isRunning, value)) RunCommand.RaiseCanExecuteChanged(); }
    }

    /// <summary>
    /// Gets or sets the result of the solution execution.
    /// </summary>
    public string Result
    {
        get => result;
        set { SetProperty(ref result, value); }
    }

    /// <summary>
    /// Gets or sets the elapsed time for the solution execution.
    /// </summary>
    public TimeSpan Elapsed
    {
        get => elapsed;
        set { SetProperty(ref elapsed, value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether an error occurred during the solution execution.
    /// </summary>
    public bool HasError
    {
        get => hasError;
        set { SetProperty(ref hasError, value); }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PartViewModel"/> class with the specified solution descriptor, input provider, solution runner, and part number.
    /// </summary>
    /// <param name="descriptor">The solution descriptor.</param>
    /// <param name="inputProvider">The input provider.</param>
    /// <param name="runner">The solution runner.</param>
    /// <param name="part">The part number (1 or 2).</param>
    public PartViewModel(SolutionDescriptor descriptor, IInputProvider inputProvider, SolutionRunner runner, int part)
    {
        this.descriptor = descriptor;
        this.inputProvider = inputProvider;
        this.runner = runner;
        Part = part;
        RunCommand = new AsyncRelayCommand(RunAsync, () => !IsRunning);
    }

    /// <summary>
    /// Runs the solution for the specified part asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RunAsync()
    {
        if (IsRunning)
            return;

        IsRunning = true;
        Result = "...";
        Elapsed = TimeSpan.Zero;
        HasError = false;

        PartResult result = await runner.RunAsync(() => inputProvider.GetInput(descriptor.Year, descriptor.Day), descriptor.SolutionFactory, Part);

        if (result.Success)
        {
            Result = result.Output?.ToString() ?? "<empty>";
            if (result.Elapsed is not null)
                Elapsed = result.Elapsed.Value;
        }
        else
        {
            Result = result.Error ?? "Unknown error";
            HasError = true;
        }

        IsRunning = false;
    }
}
