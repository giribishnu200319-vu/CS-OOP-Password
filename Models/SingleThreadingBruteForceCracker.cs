using System;
using System.Diagnostics;

namespace password.Models
{
    /// <summary>
    /// Performs a brute-force password cracking attack using a single thread.
    /// This is Stage 3 of the development - Single-threaded brute force logic.
    /// 
    /// This serves as the baseline for performance comparison with multi-threaded version.
    /// </summary>
    public class SingleThreadedBruteForceCracker
    {
        private readonly BruteForceGenerator _generator;
        private readonly PasswordValidator _validator;

        public SingleThreadedBruteForceCracker(PasswordValidator validator)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _generator = new BruteForceGenerator();
        }

        /// <summary>
        /// Cracks the password by trying all combinations sequentially.
        /// </summary>
        /// <remarks>
        /// This method runs synchronously on the calling thread.
        /// It generates and tests combinations sequentially, starting from length 1.
        /// </remarks>
        public CrackResult Crack()
        {
            var result = new CrackResult
            {
                ThreadsUsed = 1
            };

            // Start timer
            var stopwatch = Stopwatch.StartNew();
            long attemptCount = 0;

            try
            {
                // Generate all combinations starting from length 1 to 6
                foreach (var combination in _generator.GenerateAllCombinations())
                {
                    attemptCount++;

                    // Test if this combination is the correct password
                    if (_validator.IsPasswordCorrect(combination))
                    {
                        // Password found!
                        stopwatch.Stop();
                        result.IsFound = true;
                        result.FoundPassword = combination;
                        result.TotalAttempts = attemptCount;
                        result.ElapsedTime = stopwatch.Elapsed;
                        return result;
                    }
                }

                // If we get here, password was not found
                stopwatch.Stop();
                result.IsFound = false;
                result.TotalAttempts = attemptCount;
                result.ElapsedTime = stopwatch.Elapsed;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                throw new InvalidOperationException("Error during single-threaded brute force attack", ex);
            }

            return result;
        }
    }
}