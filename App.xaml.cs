using System;
using System.Windows;

namespace GestionComerce
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow main = new MainWindow();
            main.Show();

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

            // ✅ Handle minimize / restore safely
            main.StateChanged += (s, ev) =>
            {
                if (!globalButton.IsLoaded) return; // prevent access after close

                if (main.WindowState == WindowState.Minimized)
                    globalButton.Hide();
                else
                    globalButton.Show();
            };

            // ✅ Close the floating window when the main window closes
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
                // Normal state – follow the main window’s corner
                globalButton.Left = main.Left + main.Width - globalButton.Width - 10;
                globalButton.Top = main.Top + main.Height - globalButton.Height - 10;
            }
        }
    }
}
