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
            LoadUserManagement();
            foreach (Role r in lr)
            {
                if (r.RoleID == u.RoleID)
                {
                    if (r.ViewUsers == false && r.AddUsers==false)
                    {
                        UserManagementBtn.IsEnabled = false;
                        ResetButtonStyles();
                        ConfigurationBtn.Style = (Style)FindResource("ActiveNavigationItemStyle");
                        LoadFactureSettings();
                    }
                    if (r.ViewFactureSettings == false)
                    {
                        ConfigurationBtn.IsEnabled = false;

                    }
                    if (r.ViewUsers == false && r.AddUsers == false && r.ViewFactureSettings == false)
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
                    if (r.ViewPaymentMethod == false)
                    {
                        DatabaseSettingsBtn.IsEnabled = false;
                    }
                    if (r.ViewUsers == false && r.AddUsers == false && r.ViewFactureSettings == false && r.ViewPaymentMethod == false)
                    {
                        ResetButtonStyles();

                        // Set À propos button as active
                        AProposBtn.Style = (Style)FindResource("ActiveNavigationItemStyle");

                        // Load À propos de nous Control
                        LoadAProposDeNous();
                    }
                    if (r.ViewApropos == false)
                    {
                        AProposBtn.IsEnabled = false;
                    }
                }
            }
            // Load default view (User Management)
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

        private void PaimentMehode_Click(object sender, RoutedEventArgs e)
        {
            // Reset all button styles
            ResetButtonStyles();

            // Set Payment Method button as active
            DatabaseSettingsBtn.Style = (Style)FindResource("ActiveNavigationItemStyle");

            // Load Payment Method Settings
            ContentGrid.Children.Clear();
            Superete.Settings.PaymentMethodSettings PaimentSettings = new Superete.Settings.PaymentMethodSettings(this);
            PaimentSettings.HorizontalAlignment = HorizontalAlignment.Stretch;
            PaimentSettings.VerticalAlignment = VerticalAlignment.Stretch;
            PaimentSettings.Margin = new Thickness(32, 24, 32, 24);
            ContentGrid.Children.Add(PaimentSettings);
        }

        private void APropos_Click(object sender, RoutedEventArgs e)
        {
            // Reset all button styles
            ResetButtonStyles();

            // Set À propos button as active
            AProposBtn.Style = (Style)FindResource("ActiveNavigationItemStyle");

            // Load À propos de nous Control
            LoadAProposDeNous();
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

        private void LoadAProposDeNous()
        {
            ContentGrid.Children.Clear();
            CAproposDeNous aproposDeNous = new CAproposDeNous(this);
            aproposDeNous.HorizontalAlignment = HorizontalAlignment.Stretch;
            aproposDeNous.VerticalAlignment = VerticalAlignment.Stretch;
            aproposDeNous.Margin = new Thickness(32, 24, 32, 24);
            ContentGrid.Children.Add(aproposDeNous);
        }

        private void ResetButtonStyles()
        {
            Style normalStyle = (Style)FindResource("NavigationItemStyle");

            UserManagementBtn.Style = normalStyle;
            ConfigurationBtn.Style = normalStyle;
            DatabaseSettingsBtn.Style = normalStyle;
            AProposBtn.Style = normalStyle;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            main.load_main(u);
        }
    }
}