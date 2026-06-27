using System.Windows.Input;

namespace KE.AoC.App.ViewModels.Common;

/// <summary>
/// Represents an synchronous command that can be bound to UI elements in a WPF application.
/// </summary>
public class RelayCommand(Action execute, Func<bool>? canExecute = null) : ICommand
{
    /// <summary>
    /// Event that is raised when the ability of the command to execute changes.
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>
    /// Determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">The parameter passed to the command.</param>
    /// <returns>true if the command can execute; otherwise, false.</returns>
    public bool CanExecute(object? parameter)
    {
        return canExecute?.Invoke() ?? true;
    }

    /// <summary>
    /// Executes the command synchronously.
    /// </summary>
    /// <param name="parameter">The parameter passed to the command.</param>
    public void Execute(object? parameter)
    {
        execute();
    }
}
