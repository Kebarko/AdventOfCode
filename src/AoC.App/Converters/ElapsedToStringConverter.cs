using System.Globalization;
using System.Windows.Data;

namespace KE.AoC.App.Converters;

/// <summary>
/// A value converter that converts a TimeSpan value to a formatted string representation of the elapsed time.
/// </summary>
internal class ElapsedToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not TimeSpan timeSpan || timeSpan == TimeSpan.Zero)
            return string.Empty;

        if (timeSpan.TotalMicroseconds < 1000)
            return $"{timeSpan.TotalMicroseconds:F1} µs";

        if (timeSpan.TotalMilliseconds < 1000)
            return $"{timeSpan.TotalMilliseconds:F1} ms";

        return $"{timeSpan.TotalSeconds:F1} s";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
