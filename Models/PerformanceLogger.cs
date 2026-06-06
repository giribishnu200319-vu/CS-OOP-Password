using System;
using System.Text;

namespace password.Models
{
    /// <summary>
    /// Logs and compares performance metrics between single and multi-threaded brute force.
    /// This is Stage 6 of the development - Performance measurement and analysis.
    /// </summary>
    public class PerformanceLogger
    {
        private CrackResult? _singleThreadResult;
        private CrackResult? _multiThreadResult;
        private readonly StringBuilder _logBuffer = new StringBuilder();

        /// <summary>
        /// Logs the result from single-threaded brute force.
        /// </summary>
        public void LogSingleThreadResult(CrackResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            _singleThreadResult = result;

            var entry = "SINGLE-THREADED BRUTE FORCE RESULT\n" +
                        "Timestamp: " + DateTime.Now + "\n" +
                        "Password Found: " + result.IsFound + "\n" +
                        "Password: " + result.FoundPassword + "\n" +
                        "Total Attempts: " + result.TotalAttempts + "\n" +
                        "Elapsed Time: " + result.ElapsedTime.TotalSeconds.ToString("F2") + " seconds\n" +
                        "Attempts Per Second: " + result.AttemptsPerSecond.ToString("F0") + "\n" +
                        "Threads Used: " + result.ThreadsUsed + "\n";

            _logBuffer.AppendLine(entry);
        }

        /// <summary>
        /// Logs the result from multi-threaded brute force.
        /// </summary>
        public void LogMultiThreadResult(CrackResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            _multiThreadResult = result;

            var entry = "MULTI-THREADED BRUTE FORCE RESULT\n" +
                        "Timestamp: " + DateTime.Now + "\n" +
                        "Password Found: " + result.IsFound + "\n" +
                        "Password: " + result.FoundPassword + "\n" +
                        "Total Attempts: " + result.TotalAttempts + "\n" +
                        "Elapsed Time: " + result.ElapsedTime.TotalSeconds.ToString("F4") + "s\n" +
                        "Attempts/Second: " + result.AttemptsPerSecond.ToString("F0") + "\n" +
                        "Threads Used: " + result.ThreadsUsed + "\n";

            _logBuffer.AppendLine(entry);
        }

        /// <summary>
        /// Compares performance metrics between single and multi-threaded approaches.
        /// </summary>
        public string ComparePerformance()
        {
            if (_singleThreadResult == null || _multiThreadResult == null)
                return "Error: Both results needed for comparison.\n";

            // Calculate metrics
            double speedup = _singleThreadResult.ElapsedTime.TotalSeconds /
                             _multiThreadResult.ElapsedTime.TotalSeconds;
            double efficiency = (speedup / _multiThreadResult.ThreadsUsed) * 100;
            double timeReduction = ((1 - (_multiThreadResult.ElapsedTime.TotalSeconds /
                                    _singleThreadResult.ElapsedTime.TotalSeconds)) * 100);

            var comparison = "PERFORMANCE COMPARISON REPORT\n" +
                             "\n" +
                             "SINGLE-THREADED:\n" +
                             "Time: " + _singleThreadResult.ElapsedTime.TotalSeconds.ToString("F4") + "s\n" +
                             "Speed: " + _singleThreadResult.AttemptsPerSecond.ToString("F0") + " attempts/sec\n" +
                             "\n" +
                             "MULTI-THREADED (" + _multiThreadResult.ThreadsUsed + " threads):\n" +
                             "Time: " + _multiThreadResult.ElapsedTime.TotalSeconds.ToString("F4") + "s\n" +
                             "Speed: " + _multiThreadResult.AttemptsPerSecond.ToString("F0") + " attempts/sec\n" +
                             "\n" +
                             "IMPROVEMENT:\n" +
                             "Speedup: " + speedup.ToString("F2") + "x faster\n" +
                             "Efficiency: " + efficiency.ToString("F2") + "%\n" +
                             "Time Reduction: " + timeReduction.ToString("F2") + "%\n";

            _logBuffer.AppendLine(comparison);
            return comparison;
        }

        /// <summary>
        /// Gets the complete performance report.
        /// </summary>
        public string GetPerformanceReport()
        {
            return _logBuffer.ToString();
        }

        /// <summary>
        /// Clears all logged results.
        /// </summary>
        public void Clear()
        {
            _singleThreadResult = null;
            _multiThreadResult = null;
            _logBuffer.Clear();
        }

        /// <summary>
        /// Gets a summary of both results side-by-side.
        /// </summary>
        public string GetSummary()
        {
            if (_singleThreadResult == null && _multiThreadResult == null)
                return "No results logged yet.";

            var summary = new StringBuilder();

            if (_singleThreadResult != null)
                summary.AppendLine(_singleThreadResult.ToString());

            if (_multiThreadResult != null)
                summary.AppendLine(_multiThreadResult.ToString());

            return summary.ToString();
        }
    }
}