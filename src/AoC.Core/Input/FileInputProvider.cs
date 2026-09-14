using System.Text;

namespace KE.AoC.Core.Input;

/// <summary>
/// Provides input data for Advent of Code puzzles from files located in a specified root directory.
/// </summary>
/// <param name="root">The root directory containing the input files.</param>
public sealed class FileInputProvider(string root) : IInputProvider
{
    /// <summary>
    /// Retrieves the input data for a specific year and day of the Advent of Code puzzle from a file following the convention <c>inputs/{year}/{day:D2}.txt</c>, e.g. <c>inputs/2024/01.txt</c>.
    /// </summary>
    /// <param name="year">The year of the puzzle.</param>
    /// <param name="day">The day of the puzzle.</param>
    /// <returns>The input data for the puzzle.</returns>
    public string GetInput(int year, int day)
    {
        string path = Path.Combine(root, year.ToString(), $"{day:D2}.txt");
        if (!File.Exists(path))
            throw new FileNotFoundException($"Input not found. Create the file at:\n{path}");

        return Normalize(File.ReadAllText(path, Encoding.UTF8));
    }

    /// <summary>
    /// Normalizes the raw input data by trimming any trailing carriage return and newline characters.
    /// </summary>
    /// <param name="raw">The raw input data.</param>
    /// <returns>The normalized input data.</returns>
    private static string Normalize(string raw) => raw.TrimEnd('\r', '\n');

    /// <summary>
    /// Locates the root directory containing the "inputs" folder by traversing up the directory tree from the application's base directory.
    /// </summary>
    /// <returns>The path to the root directory containing the input files.</returns>
    public static string LocateInputsRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            string inputsPath = Path.Combine(dir.FullName, "inputs");
            if (Directory.Exists(inputsPath))
                return inputsPath;

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the 'inputs' directory.");
    }
}
