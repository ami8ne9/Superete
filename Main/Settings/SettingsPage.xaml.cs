using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Superete.Main.Settings
{
    /// <summary>
    /// Logique d'interaction pour SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        public User u;
         List<User> lu;
        public MainWindow main;
        public List<Role> lr;
        List<Famille> lf;

        public SettingsPage(User u, List<User> lu, List<Role> lr, List<Famille> lf, MainWindow main)
        {
            InitializeComponent();
            this.u = u;
            this.lu = lu;
            this.lr = lr;
            this.lf = lf;
            this.main = main;

            // Load default view (User Management)
            LoadUserManagement();
        }

        private void NavigationItem_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            // Reset all button styles to normal
            ResetButtonStyles();

            // Set clicked button as active
            if (clickedButton != null)
            {
                clickedButton.Style = (Style)FindResource("ActiveNavigationItemStyle");
            }

            // Handle navigation based on button name
            if (clickedButton == UserManagementBtn)
            {
                LoadUserManagement();
            }
            else if (clickedButton == DatabaseSettingsBtn)
            {
                // TODO: Load Database Settings
                LoadPlaceholder("Database Settings");
            }
            else if (clickedButton == SecurityBtn)
            {
                // TODO: Load Security Settings
                LoadPlaceholder("Security");
            }
            else if (clickedButton == NotificationsBtn)
            {
                // TODO: Load Notifications Settings
                LoadPlaceholder("Notifications");
            }
            else if (clickedButton == ReportsBtn)
            {
                // TODO: Load Reports Settings
                LoadPlaceholder("Reports");
            }
            else if (clickedButton == SystemLogsBtn)
            {
                // TODO: Load System Logs
                LoadPlaceholder("System Logs");
            }
            else if (clickedButton == PreferencesBtn)
            {
                // TODO: Load Preferences
                LoadPlaceholder("Preferences");
            }
        }

        private void FactureSettings_Click(object sender, RoutedEventArgs e)
        {
            // Reset all button styles
            ResetButtonStyles();

            // Set Facture Settings button as active
            ConfigurationBtn.Style = (Style)FindResource("ActiveNavigationItemStyle");

            // Load Facture Settings Control
            LoadFactureSettings();
        }

        private void LoadUserManagement()
        {
            ContentGrid.Children.Clear();
            CUserManagment UserM = new CUserManagment(u, lu, lr, this);
            UserM.HorizontalAlignment = HorizontalAlignment.Stretch;
            UserM.VerticalAlignment = VerticalAlignment.Stretch;
            UserM.Margin = new Thickness(0);
            ContentGrid.Children.Add(UserM);
        }

        private void LoadFactureSettings()
        {
            ContentGrid.Children.Clear();
            Superete.Settings.CFactureSettings factureSettings = new Superete.Settings.CFactureSettings(this);
            factureSettings.HorizontalAlignment = HorizontalAlignment.Stretch;
            factureSettings.VerticalAlignment = VerticalAlignment.Stretch;
            factureSettings.Margin = new Thickness(32, 24, 32, 24);
            ContentGrid.Children.Add(factureSettings);
        }

        private void LoadPlaceholder(string title)
        {
            ContentGrid.Children.Clear();

            // Create a placeholder view
            Border placeholder = new Border
            {
                Background = new SolidColorBrush(Colors.White),
                CornerRadius = new CornerRadius(8),
                Margin = new Thickness(20),
                Padding = new Thickness(40)
            };

            StackPanel stack = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock titleBlock = new TextBlock
            {
                Text = title,
                FontSize = 32,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F2937")),
                Margin = new Thickness(0, 0, 0, 10)
            };

            TextBlock messageBlock = new TextBlock
            {
                Text = "Cette section est en cours de développement",
                FontSize = 16,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"))
            };

            stack.Children.Add(titleBlock);
            stack.Children.Add(messageBlock);
            placeholder.Child = stack;

            ContentGrid.Children.Add(placeholder);
        }

        private void ResetButtonStyles()
        {
            Style normalStyle = (Style)FindResource("NavigationItemStyle");

            UserManagementBtn.Style = normalStyle;
            ConfigurationBtn.Style = normalStyle;
            DatabaseSettingsBtn.Style = normalStyle;
            SecurityBtn.Style = normalStyle;
            NotificationsBtn.Style = normalStyle;
            ReportsBtn.Style = normalStyle;
            SystemLogsBtn.Style = normalStyle;
            PreferencesBtn.Style = normalStyle;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            main.load_main(u);
        }

        private void PaimentMehode_Click(object sender, RoutedEventArgs e)
        {
            {

                ResetButtonStyles();
                DatabaseSettingsBtn.Style = (Style)FindResource("ActiveNavigationItemStyle");
                ContentGrid.Children.Clear();
                Superete.Settings.PaymentMethodSettings PaimentSettings = new Superete.Settings.PaymentMethodSettings(this);
                PaimentSettings.HorizontalAlignment = HorizontalAlignment.Stretch;
                PaimentSettings.VerticalAlignment = VerticalAlignment.Stretch;
                PaimentSettings.Margin = new Thickness(32, 24, 32, 24);
                ContentGrid.Children.Add(PaimentSettings);
            }
        }
    }
}