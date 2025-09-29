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
using System.Windows.Shapes;

namespace Superete.Main.Inventory
{
    /// <summary>
    /// Interaction logic for WNouveauStock.xaml
    /// </summary>
    public partial class WNouveauStock : Window
    {
        public WNouveauStock(List<Famille> lf, List<Article> la, List<Fournisseur> lfo, CMainI main)
        {
            InitializeComponent();
            this.lf = lf;
            this.la = la;
            this.lfo = lfo;
            this.main = main;
        }
        List<Famille> lf; List<Article> la; List<Fournisseur> lfo; CMainI main;
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddNewArticleButton_Click(object sender, RoutedEventArgs e)
        {
            WAddArticle af = new WAddArticle(new Article(), la, lf, lfo, main, 1,null);
            af.ShowDialog();
        }

        private void AddExistingArticleButton_Click(object sender, RoutedEventArgs e)
        {
            WExistingArticles ea = new WExistingArticles(la,main,2);
            ea.ShowDialog();
        }


        private void AddStockNewSupplierButton_Click(object sender, RoutedEventArgs e)
        {
            WExistingArticles ea = new WExistingArticles(la, main,1); 
            ea.ShowDialog();
        }
    }
}
