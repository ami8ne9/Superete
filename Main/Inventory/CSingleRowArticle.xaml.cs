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

namespace Superete.Main.Inventory
{
    /// <summary>
    /// Interaction logic for CSingleRowArticle.xaml
    /// </summary>
    public partial class CSingleRowArticle : UserControl
    {
        public CSingleRowArticle(Article a, List<Article> la, CSingleRowFamilly sf, CMainI Main, int s,WExistingArticles ea)
        {
            InitializeComponent();
            this.a = a;
            this.la = la;
            this.Main = Main;
            this.sf = sf;
            this.s = s;
            this.ea = ea;
            foreach (Fournisseur fo in Main.lfo)
                if (fo.FournisseurID == a.FournisseurID)
                {
                    Fournisseur.Text = fo.Nom;
                    break;
                }
            ArticleName.Text = a.ArticleName;
            Quantite.Text = "x"+a.Quantite.ToString();
            EditButton.Visibility = Visibility.Collapsed;
            if (s == 2)
            {

                ButtonsContainerPanel.Width = 67;
                EditButton.Visibility = Visibility.Visible;
                MainContainer.MouseLeftButtonDown -= MyGrid_MouseLeftButtonDown;
                DeleteButton.Visibility = Visibility.Collapsed;
            }
            else if (s == 1)
            {
                DeleteButton.Visibility = Visibility.Collapsed;
                ButtonsContainer.Width = new GridLength(0);
            }
        }
        public Article a; List<Article> la;public CMainI Main; CSingleRowFamilly sf; int s;public WExistingArticles ea;

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            WAjoutQuantite w = new WAjoutQuantite(this);
            w.ShowDialog();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            WDeleteConfirmation df = new WDeleteConfirmation(a, la, sf, Main);
            df.ShowDialog();
        }
        
        private void MyGrid_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            WAddArticle af = new WAddArticle(a, la, Main.lf, Main.lfo, Main,2,ea);
            af.ShowDialog();
        }
    }
}
