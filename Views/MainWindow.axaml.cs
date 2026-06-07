using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using password.ViewModels;

namespace password.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel? _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            
            _viewModel = new MainWindowViewModel();
            
            // Subscribe to property changes
            _viewModel.PropertyChanged += (s, e) =>
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    switch (e.PropertyName)
                    {
                        case nameof(MainWindowViewModel.CurrentPassword):
                            PasswordTextBox.Text = _viewModel.CurrentPassword;
                            break;
                            
                        case nameof(MainWindowViewModel.TargetHash):
                            HashTextBox.Text = _viewModel.TargetHash;
                            break;
                            
                        case nameof(MainWindowViewModel.Progress):
                            ProgressBar.Value = _viewModel.Progress;
                            ProgressText.Text = _viewModel.Progress + "% Complete";
                            break;
                            
                        case nameof(MainWindowViewModel.ElapsedTime):
                            ElapsedTimeText.Text = _viewModel.ElapsedTime;
                            break;
                            
                        case nameof(MainWindowViewModel.StatusMessage):
                            StatusText.Text = _viewModel.StatusMessage;
                            break;
                            
                        case nameof(MainWindowViewModel.ResultOutput):
                            ResultsTextBox.Text = _viewModel.ResultOutput;
                            ResultsTextBox.CaretIndex = ResultsTextBox.Text?.Length ?? 0;
                            break;
                            
                        case nameof(MainWindowViewModel.IsRunning):
                            UpdateButtonStates();
                            break;
                            
                        case nameof(MainWindowViewModel.IsMultiThreaded):
                            UpdateRadioButtons();
                            break;
                    }
                });
            };
        }

        private void GeneratePasswordClick(object? sender, RoutedEventArgs e)
        {
            _viewModel?.GeneratePassword();
        }

        private void StartBruteForceClick(object? sender, RoutedEventArgs e)
        {
            _viewModel?.StartBruteForce();
        }

        private void StopBruteForceClick(object? sender, RoutedEventArgs e)
        {
            _viewModel?.StopBruteForce();
        }

        //Radio button handlers
        private void SingleThreadRadio_Click(object? sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.IsMultiThreaded = false;
                System.Console.WriteLine("Switched to: Single-Threaded");
            }
        }

        private void MultiThreadRadio_Click(object? sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.IsMultiThreaded = true;
                System.Console.WriteLine("Switched to: Multi-Threaded");
            }
        }

        private void UpdateButtonStates()
        {
            if (_viewModel != null)
            {
                StartButton.IsEnabled = !_viewModel.IsRunning;
                StopButton.IsEnabled = _viewModel.IsRunning;
            }
        }

        private void UpdateRadioButtons()
        {
            if (_viewModel != null)
            {
                SingleThreadRadio.IsChecked = !_viewModel.IsMultiThreaded;
                MultiThreadRadio.IsChecked = _viewModel.IsMultiThreaded;
            }
        }
    }
}