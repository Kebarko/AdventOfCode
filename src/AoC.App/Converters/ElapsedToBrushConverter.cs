using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace KE.AoC.App.Converters;

/// <summary>
/// A value converter that converts a TimeSpan value to a SolidColorBrush based on the elapsed time, using a logarithmic scale to determine the color hue.
/// </summary>
internal class ElapsedToBrushConverter : IValueConverter
{
    /// <summary>
    /// Gets or sets the minimum elapsed time in milliseconds for color mapping. Values below this threshold will be clamped to this minimum.
    /// </summary>
    public double Min { get; set; } = 1;

    /// <summary>
    /// Gets or sets the maximum elapsed time in milliseconds for color mapping. Values above this threshold will be clamped to this maximum.
    /// </summary>
    public double Max { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the default brush to use when the input value is not a valid TimeSpan. This brush will be returned for invalid or null inputs.
    /// </summary>
    public Brush DefaultBrush { get; set; } = Brushes.Gray;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not TimeSpan timeSpan)
            return DefaultBrush;

        double ms = Math.Clamp(timeSpan.TotalMilliseconds, Min, Max);

        double lo = Math.Log10(Min);
        double hi = Math.Log10(Max);
        double t = Math.Clamp((Math.Log10(ms) - lo) / (hi - lo), 0, 1);

        double hue = 120.0 * (1 - t);
        SolidColorBrush brush = new(FromHSV(hue, 1, 0.8));
        brush.Freeze();
        return brush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();

    private static Color FromHSV(double h, double s, double v)
    {
        double c = s * v;
        double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
        double m = v - c;

        (double r, double g, double b) =
            h < 60 ? (c, x, 0.0) :
            h < 120 ? (x, c, 0.0) :
            h < 180 ? (0.0, c, x) :
            h < 240 ? (0.0, x, c) :
            h < 300 ? (x, 0.0, c) :
                      (c, 0.0, x);

        return Color.FromRgb(
            ToByte(r + m),
            ToByte(g + m),
            ToByte(b + m));

        static byte ToByte(double channel)
            => (byte)Math.Clamp(Math.Round(channel * 255, MidpointRounding.AwayFromZero), 0, 255);
    }
}
