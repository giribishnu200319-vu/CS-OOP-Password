using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace password.Models
{
    /// <summary>
    /// Performs brute-force password cracking using multiple threads.
    /// This is Stage 4 of the development - Multi-threaded brute force logic.
    /// 
    /// Key features:
    /// - Uses (CPU cores - 1) threads for parallel processing
    /// - Supports cancellation tokens for graceful shutdown
    /// - Thread-safe implementation using thread-safe collections
    /// - Stops all threads immediately when password is found
    /// </summary>
    public class MultiThreadedBruteForceCracker
    {
        private readonly BruteForceGenerator _generator;
        private readonly PasswordValidator _validator;
        private readonly int _threadCount;
        private CancellationTokenSource? _cancellationTokenSource;
        private bool _passwordFound = false;
        private string _foundPassword = string.Empty;
        private long _totalAttempts = 0;

        // Lock object for thread-safe operations
        private readonly object _lockObject = new object();

        public MultiThreadedBruteForceCracker(PasswordValidator validator)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _generator = new BruteForceGenerator();
            
            // Use (CPU cores - 1) threads as required
            _threadCount = Math.Max(1, Environment.ProcessorCount - 1);
        }

        /// <summary>
        /// Gets the number of threads that will be used.
        /// </summary>
        public int ThreadCount => _threadCount;

        /// <summary>
        /// Cracks the password using multiple threads.
        /// </summary>
        /// <param name="onProgress">Optional callback for progress updates</param>
        public CrackResult Crack(Action<long>? onProgress = null)
        {
            var result = new CrackResult
            {
                ThreadsUsed = _threadCount
            };

            _passwordFound = false;
            _foundPassword = string.Empty;
            _totalAttempts = 0;

            // Create cancellation token source
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Get all combinations
                var allCombinations = _generator.GenerateAllCombinations().ToList();

                // Split work among threads
                var tasks = new List<Task>();
                int combinationsPerThread = allCombinations.Count / _threadCount;
                int remainder = allCombinations.Count % _threadCount;

                for (int i = 0; i < _threadCount; i++)
                {
                    int start = i * combinationsPerThread + Math.Min(i, remainder);
                    int end = start + combinationsPerThread + (i < remainder ? 1 : 0);
                    
                    var threadCombinations = allCombinations.Skip(start).Take(end - start);

                    // Create task for this thread
                    var task = Task.Run(() => 
                        WorkerThread(threadCombinations, cancellationToken, onProgress),
                        cancellationToken);
                    
                    tasks.Add(task);
                }

                // Wait for all tasks to complete or password to be found
                try
                {
                    Task.WaitAll(tasks.ToArray(), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Password was found, tasks were cancelled
                }

                stopwatch.Stop();

                result.IsFound = _passwordFound;
                result.FoundPassword = _foundPassword;
                result.TotalAttempts = _totalAttempts;
                result.ElapsedTime = stopwatch.Elapsed;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                throw new InvalidOperationException("Error during multi-threaded brute force attack", ex);
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Worker thread function that tests combinations.
        /// </summary>
        private void WorkerThread(IEnumerable<string> combinations, CancellationToken cancellationToken, Action<long>? onProgress)
        {
            try
            {
                foreach (var combination in combinations)
                {
                    // Check if cancellation was requested
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    // Check if password was already found by another thread
                    if (_passwordFound)
                    {
                        break;
                    }

                    // Increment attempt counter (thread-safe)
                    lock (_lockObject)
                    {
                        _totalAttempts++;
                        
                        // Report progress every 100,000 attempts
                        if (_totalAttempts % 100000 == 0)
                        {
                            onProgress?.Invoke(_totalAttempts);
                        }
                    }

                    // Test if this combination is correct
                    if (_validator.IsPasswordCorrect(combination))
                    {
                        // Found it!
                        lock (_lockObject)
                        {
                            if (!_passwordFound) // Double-check to ensure thread safety
                            {
                                _passwordFound = true;
                                _foundPassword = combination;
                            }
                        }

                        // Signal cancellation to other threads
                        _cancellationTokenSource?.Cancel();
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when password is found
            }
        }

        /// <summary>
        /// Stops the brute-force attack immediately.
        /// </summary>
        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}