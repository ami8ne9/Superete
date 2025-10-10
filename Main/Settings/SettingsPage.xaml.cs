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
        public SettingsPage(User u, List<User> lu, List<Role> lr, List<Famille> lf, MainWindow main)
        {
            InitializeComponent();
            this.u = u;
            this.lu = lu;
            this.lr = lr;
            this.lf = lf;
            this.main = main;
            ContentGrid.Children.Clear();
            CUserManagment UserM = new CUserManagment(u,lu,lr,this);
            UserM.HorizontalAlignment = HorizontalAlignment.Stretch;
            UserM.VerticalAlignment = VerticalAlignment.Stretch;
            UserM.Margin = new Thickness(0);
            ContentGrid.Children.Add(UserM);

        }
        User u; List<User> lu; public MainWindow main;List<Role> lr; List<Famille> lf;

        private void NavigationItem_Click(object sender, RoutedEventArgs e)
        {

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            main.load_main(u);
        }
    }
}
