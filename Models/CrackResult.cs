using System;

namespace password.Models
{
    /// <summary>
    /// Represents the result of a brute-force password cracking attempt.
    /// This is Stage 2 of the development - Data structure for results.
    /// </summary>
    public class CrackResult
    {
        /// <summary>
        /// The password that was found, or empty string if not found.
        /// </summary>
        public string FoundPassword { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the password was successfully found.
        /// </summary>
        public bool IsFound { get; set; } = false;

        /// <summary>
        /// Total time elapsed during the cracking attempt.
        /// </summary>
        public TimeSpan ElapsedTime { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Total number of password attempts made.
        /// </summary>
        public long TotalAttempts { get; set; } = 0;

        /// <summary>
        /// Number of threads used in the cracking attempt.
        /// 1 for single-threaded, >1 for multi-threaded.
        /// </summary>
        public int ThreadsUsed { get; set; } = 1;

        /// <summary>
        /// Attempts per second (calculated metric).
        /// </summary>
        public double AttemptsPerSecond
        {
            get
            {
                if (ElapsedTime.TotalSeconds == 0)
                    return 0;
                return TotalAttempts / ElapsedTime.TotalSeconds;
            }
        }

                public string GetPerformanceReport()
        {
            return $"Attempts: {TotalAttempts}, Time: {ElapsedTime.TotalSeconds:F2}s, Speed: {AttemptsPerSecond:F0} attempts/sec, Threads: {ThreadsUsed}";
        }
    }
}