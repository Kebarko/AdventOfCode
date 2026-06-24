using System.Windows.Input;

namespace KE.AoC.App.ViewModels.Common;

/// <summary>
/// Represents an asynchronous command that can be bound to UI elements in a WPF application.
/// </summary>
public class AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null) : ICommand
{
    private bool isExecuting;

    /// <summary>
    /// Event that is raised when the ability of the command to execute changes.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">The parameter passed to the command.</param>
    /// <returns>true if the command can execute; otherwise, false.</returns>
    public bool CanExecute(object? parameter)
    {
        return !isExecuting && (canExecute?.Invoke() ?? true);
    }

    /// <summary>
    /// Executes the command asynchronously.
    /// </summary>
    /// <param name="parameter">The parameter passed to the command.</param>
    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
            return;

        isExecuting = true;
        RaiseCanExecuteChanged();

        try
        {
            await execute();
        }
        finally
        {
            isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }

    /// <summary>
    /// Raises the CanExecuteChanged event to notify that the command's ability to execute has changed.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
