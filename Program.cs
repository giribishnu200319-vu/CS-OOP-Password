using System;
using Avalonia;

namespace password
{
    /// <summary>
    /// Application entry point.
    /// This is Stage 5 of the development - Application startup.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        /// <summary>
        /// Builds the Avalonia application with proper configuration.
        /// </summary>
        public static AppBuilder BuildAvaloniaApp() =>
            AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}