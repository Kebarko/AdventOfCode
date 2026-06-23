using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KE.AoC.App.ViewModels.Common;

/// <summary>
/// Base class for view models that implements INotifyPropertyChanged to support data binding in MVVM architecture.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    /// <summary>
    /// Event that is raised when a property value changes. This event is part of the INotifyPropertyChanged interface and is used to notify the UI of changes in the view model's properties.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event for the specified property name. This method is called whenever a property value changes, allowing the UI to update accordingly.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Sets the value of a property and raises the PropertyChanged event if the value has changed. This method is used to simplify property setters in view models, ensuring that the UI is notified of changes.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="field">The backing field for the property.</param>
    /// <param name="value">The new value for the property.</param>
    /// <param name="propertyName">The name of the property that changed.</param>
    /// <returns>true if the property value changed; otherwise, false.</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);

        return true;
    }
}
