using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using password.Models;
using Avalonia.Threading;

namespace password.ViewModels
{
    /// <summary>
    /// ViewModel for the main window using MVVM pattern.
    /// This is Stage 5 of the development - GUI logic and state management.
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        // Core components
        private readonly PasswordHasher _passwordHasher = new PasswordHasher();
        private readonly PasswordGenerator _passwordGenerator = new PasswordGenerator();
        private readonly PasswordValidator _passwordValidator = new PasswordValidator();
        private readonly PerformanceLogger _performanceLogger = new PerformanceLogger();

        // State
        private string _currentPassword = string.Empty;
        private string _targetHash = string.Empty;
        private int _progress = 0;
        private string _elapsedTime = "00:00:00";
        private string _resultOutput = "Ready to start...";
        private bool _isRunning = false;
        private bool _isMultiThreaded = true;
        private string _passwordLength = "0";
        private string _statusMessage = "Ready";

        // Task management
        private Task? _attackTask;
        private CancellationTokenSource? _cancellationTokenSource;
        private DateTime _startTime;

        // Events
        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindowViewModel()
        {
            InitializeTimer();
        }

        #region Properties

        public string CurrentPassword
        {
            get => _currentPassword;
            set => SetProperty(ref _currentPassword, value);
        }

        public string TargetHash
        {
            get => _targetHash;
            set => SetProperty(ref _targetHash, value);
        }

        public int Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public string ElapsedTime
        {
            get => _elapsedTime;
            set => SetProperty(ref _elapsedTime, value);
        }

        public string ResultOutput
        {
            get => _resultOutput;
            set => SetProperty(ref _resultOutput, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public bool IsMultiThreaded
        {
            get => _isMultiThreaded;
            set => SetProperty(ref _isMultiThreaded, value);
        }

        public string PasswordLength
        {
            get => _passwordLength;
            set => SetProperty(ref _passwordLength, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        #endregion

        #region Commands

        public void GeneratePassword()
        {
            try
            {
                // Generate a new password
                CurrentPassword = _passwordGenerator.GeneratePassword();
                PasswordLength = _passwordGenerator.GetRandomLength().ToString();

                // Hash it
                TargetHash = _passwordHasher.GenerateHash(CurrentPassword);

                // Set it in validator
                _passwordValidator.SetTargetHash(TargetHash);

                StatusMessage = $"Password generated: {CurrentPassword} (length: {PasswordLength})";
                ResultOutput = "Password created. Click 'Start Brute Force' to begin attack...";
                Progress = 0;
                ElapsedTime = "00:00:00";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                ResultOutput = $"Error generating password: {ex.Message}";
            }
        }

        public void StartBruteForce()
        {
            if (string.IsNullOrEmpty(TargetHash))
            {
                StatusMessage = "Please generate a password first!";
                return;
            }

            if (IsRunning)
                return;

            IsRunning = true;
            Progress = 0;
            _startTime = DateTime.Now;
            ResultOutput = "Attack in progress...";
            StatusMessage = $"Starting {(IsMultiThreaded ? "multi-threaded" : "single-threaded")} brute force...";

            // Run attack in background
            _cancellationTokenSource = new CancellationTokenSource();
            _attackTask = Task.Run(() => PerformBruteForceAttack(_cancellationTokenSource.Token));
        }

        public void StopBruteForce()
        {
            if (!IsRunning)
                return;

            _cancellationTokenSource?.Cancel();
            StatusMessage = "Stopping attack...";
        }

        #endregion

        #region Private Methods

        private void PerformBruteForceAttack(CancellationToken cancellationToken)
        {
            try
            {
                CrackResult result;

                if (IsMultiThreaded)
                {
                    var multiCracker = new MultiThreadedBruteForceCracker(_passwordValidator);
                    result = multiCracker.Crack(progress =>
                    {
                        // Update progress based on percentage
                        long totalCombinations = BruteForceGenerator.CalculateTotalCombinations();
                        int progressPercent = (int)((progress * 100) / totalCombinations);
                        if (progressPercent > 100) progressPercent = 100;
                        Progress = progressPercent;
                    });
                    _performanceLogger.LogMultiThreadResult(result);
                }
                else
                {
                    var singleCracker = new SingleThreadedBruteForceCracker(_passwordValidator);
                    result = singleCracker.Crack();
                    _performanceLogger.LogSingleThreadResult(result);
                }

                // Update UI with results
                IsRunning = false;
                Progress = 100;

                if (result.IsFound)
                {
                    StatusMessage = $"✓ Password found: {result.FoundPassword}";
                    ResultOutput = result.ToString();
                }
                else
                {
                    StatusMessage = "✗ Password not found";
                    ResultOutput = "Attack completed but password not found in the search space.";
                }

                // Show performance report
                ResultOutput += "\n" + _performanceLogger.GetPerformanceReport();
            }
            catch (OperationCanceledException)
            {
                IsRunning = false;
                StatusMessage = "Attack stopped by user";
            }
            catch (Exception ex)
            {
                IsRunning = false;
                StatusMessage = $"Error: {ex.Message}";
                ResultOutput = $"Error during brute force: {ex.Message}";
            }
        }

        private void InitializeTimer()
        {
            // Use DispatcherTimer which runs on UI thread
            var timer = new Avalonia.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000); // Update every 1000ms

            timer.Tick += (s, e) =>
            {
                if (IsRunning)
                {
                    TimeSpan elapsed = DateTime.Now - _startTime;
                    ElapsedTime = elapsed.Hours.ToString("D2") + ":" +
                                 elapsed.Minutes.ToString("D2") + ":" +
                                 elapsed.Seconds.ToString("D2");
                }
            };

            timer.Start();
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}