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

namespace Superete.Main
{
    /// <summary>
    /// Logique d'interaction pour CMain.xaml
    /// </summary>
    public partial class CMain : UserControl
    {
        public CMain(MainWindow main,User u)
        {
            InitializeComponent();
            this.main = main;
            this.u = u;
            Name.Text = u.UserName;
        }
        MainWindow main; User u;


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            main.load_settings(u);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Article a = new Article();
            List<Article> la = await a.GetArticlesAsync();
            main.load_vente(u,la);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            main.load_inventory(u);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            main.load_fournisseur(u);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            main.load_client(u);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            main.load_ProjectManagement(u);
        }
    }
}
