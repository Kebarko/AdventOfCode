using System.Diagnostics;

namespace KE.AoC.Core.Solution;

/// <summary>
/// Represents the result of running a part of a solution, including success status, output, elapsed time, and any error message.
/// </summary>
public sealed record PartResult(bool Success, object? Output, TimeSpan? Elapsed, string? Error);

/// <summary>
/// Provides functionality to run a specific part of a solution asynchronously, capturing the result and any exceptions that may occur during execution.
/// </summary>
public sealed class SolutionRunner
{
    /// <summary>
    /// Runs the specified part of a solution asynchronously, capturing the result and any exceptions that may occur during execution.
    /// </summary>
    /// <param name="inputFactory">A function that provides the input for the solution.</param>
    /// <param name="solutionFactory">A function that creates an instance of the solution.</param>
    /// <param name="part">The part of the solution to run (1 or 2).</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation and its result.</returns>
    public static Task<PartResult> RunAsync(Func<string> inputFactory, Func<ISolution> solutionFactory, int part, CancellationToken ct)
    {
        return Task.Run(() =>
        {
            try
            {
                string input = inputFactory();
                ISolution solution = solutionFactory();

                var stopwatch = Stopwatch.StartNew();

                object? output = part switch
                {
                    1 => solution.PartOne(input),
                    2 => solution.PartTwo(input),
                    _ => throw new ArgumentOutOfRangeException(nameof(part), "Part must be 1 or 2.")
                };

                stopwatch.Stop();

                return new PartResult(true, output, stopwatch.Elapsed, null);
            }
            catch (Exception ex)
            {
                return new PartResult(false, null, null, ex.Message);
            }
        }, ct);
    }
}
