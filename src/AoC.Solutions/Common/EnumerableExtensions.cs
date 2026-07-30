using System.Diagnostics;

namespace KE.AoC.Solutions.Common
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Trace<T>(this IEnumerable<T> source, string label, Func<T, string>? formatter = null)
        {
#if DEBUG
            List<T> list = source.ToList();
            string text = formatter is null
                ? string.Join(", ", list)
                : string.Join(", ", list.Select(formatter));
            Debug.WriteLine($"{label} ({list.Count}): {text}");
            return list;
#else
            return source;
#endif
        }
    }
}
