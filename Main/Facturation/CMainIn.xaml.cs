using GestionComerce;
using GestionComerce.Main.Facturation.CreateFacture;
using GestionComerce.Main.Facturation.HistoriqueFacture;
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

namespace GestionComerce.Main.Facturation
{
    /// <summary>
    /// Interaction logic for CMainI.xaml
    /// </summary>
    public partial class CMainIn : UserControl
    {
        public CMainIn(User u, MainWindow main, Operation op)
        {
            InitializeComponent();
            this.main = main;
            this.user = u;
            this.operation = op;

            ContentContainer.Children.Clear();

            // Pass the operation to CMainFa so it can be pre-selected
            CMainFa loginPage = new CMainFa(u, main, this, op);
            loginPage.HorizontalAlignment = HorizontalAlignment.Stretch;
            loginPage.VerticalAlignment = VerticalAlignment.Stretch;
            loginPage.Margin = new Thickness(0);
            ContentContainer.Children.Add(loginPage);
        }

        MainWindow main;
        User user;
        Operation operation;

        private void CreeFacture_Click(object sender, RoutedEventArgs e)
        {
            CreeFacture.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E90FF"));
            CreeFacture.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E90FF"));
            HistoriqueFacture.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
            HistoriqueFacture.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));

            ContentContainer.Children.Clear();

            // When switching to create facture, pass the stored operation if it exists
            CMainFa loginPage = new CMainFa(user, main, this, operation);
            Grid.SetRow(loginPage, 0);
            Grid.SetColumn(loginPage, 0);
            loginPage.HorizontalAlignment = HorizontalAlignment.Stretch;
            loginPage.VerticalAlignment = VerticalAlignment.Stretch;
            ContentContainer.Children.Add(loginPage);
        }

        private void HistoriqueFacture_Click(object sender, RoutedEventArgs e)
        {
            HistoriqueFacture.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E90FF"));
            HistoriqueFacture.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E90FF"));
            CreeFacture.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
            CreeFacture.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));

            ContentContainer.Children.Clear();

            CMainHf loginPage = new CMainHf(user, main);
            loginPage.HorizontalAlignment = HorizontalAlignment.Stretch;
            loginPage.VerticalAlignment = VerticalAlignment.Stretch;
            loginPage.Margin = new Thickness(0);
            ContentContainer.Children.Add(loginPage);
        }

        private void RetourButton_Click(object sender, RoutedEventArgs e)
        {
            main.load_main(user);
        }
    }
}