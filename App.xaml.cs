using Superete;
using System;
using System.Windows;

namespace GestionComerce
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // CHECK 1: Handle registration command from installer
            if (e.Args.Length > 0 && e.Args[0] == "/register")
            {
                try
                {
                    MachineLock.RegisterInstallation();
                    // Silent success - installer will continue
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Registration failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Current.Shutdown();
                return;
            }

            // CHECK 2: Handle database setup command from installer
            if (e.Args.Length > 0 && e.Args[0] == "/setupdb")
            {
                try
                {
                    DatabaseSetup.EnsureDatabaseExists();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Database setup failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Current.Shutdown();
                return;
            }

            // CHECK 3: Ensure database exists before starting app
            if (!DatabaseSetup.EnsureDatabaseExists())
            {
                MessageBox.Show("Cannot start application without database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
                return;
            }

            // CHECK 4: Normal startup - create main window
            MainWindow main = new MainWindow();

            // If MainWindow constructor failed (machine lock or expiry), don't continue
            if (main == null || Current.MainWindow == null)
            {
                return;
            }

            main.Show();

            // Create global button window
            GlobalButtonWindow globalButton = new GlobalButtonWindow
            {
                WindowStartupLocation = WindowStartupLocation.Manual,
                Topmost = true,
                ShowInTaskbar = false,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Background = System.Windows.Media.Brushes.Transparent
            };

            PositionBottomRight(main, globalButton);
            globalButton.Show();

            main.LocationChanged += (s, ev) => PositionBottomRight(main, globalButton);
            main.SizeChanged += (s, ev) => PositionBottomRight(main, globalButton);

            // Handle minimize / restore safely
            main.StateChanged += (s, ev) =>
            {
                if (!globalButton.IsLoaded) return; // prevent access after close
                if (main.WindowState == WindowState.Minimized)
                    globalButton.Hide();
                else
                    globalButton.Show();
            };

            // Close the floating window when the main window closes
            main.Closed += (s, ev) =>
            {
                if (globalButton.IsLoaded)
                {
                    globalButton.Close();
                }
            };
        }

        private void PositionBottomRight(Window main, Window globalButton)
        {
            // Get usable screen area (excludes taskbar)
            var workingArea = SystemParameters.WorkArea;
            if (main.WindowState == WindowState.Maximized)
            {
                // When maximized, use the working area coordinates instead of main.Left/Top
                globalButton.Left = workingArea.Right - globalButton.Width - 10;
                globalButton.Top = workingArea.Bottom - globalButton.Height - 10;
            }
            else
            {
                // Normal state – follow the main window's corner
                globalButton.Left = main.Left + main.Width - globalButton.Width - 10;
                globalButton.Top = main.Top + main.Height - globalButton.Height - 10;
            }
        }
    }
}