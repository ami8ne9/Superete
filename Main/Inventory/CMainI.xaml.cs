using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Superete.Main.Inventory
{
    /// <summary>
    /// Logique d'interaction pour CMainI.xaml
    /// </summary>
    public partial class CMainI : UserControl
    {
        public CMainI(User u, List<Article> la, List<Famille> lf, List<Fournisseur> lfo,MainWindow main)
        {
            InitializeComponent();
            this.lf = lf;
            this.main = main;
            this.u = u;
            this.la = la;
            this.lfo = lfo;
            LoadArticles(la);
            foreach (Role r in main.lr)
            {
                if (u.RoleID == r.RoleID)
                {
                    if (r.ViewFamilly == false && r.AddFamilly==false)
                    {
                        ManageFamillies.IsEnabled = false;
                    }
                    break;
                }
            }

        }
        public List<Famille> lf; public MainWindow main;public User u; public List<Fournisseur> lfo; public List<Article> la;
        
        public void LoadArticles(List<Article> la)
        {
            int count = la.Count;
            Decimal PrixATotall = 0;
            Decimal PrixMPTotall = 0;
            Decimal PrixVTotall = 0;
            int QuantiteTotall = 0;
            ArticlesTotal.Text = count.ToString() ;
            ArticlesContainer.Children.Clear();
            foreach (Article a in la)
            {
                CSingleArticleI ar = new CSingleArticleI(a, la, this, lf,lfo);
                ArticlesContainer.Children.Add(ar);
                PrixATotall+=a.PrixAchat * a.Quantite;
                PrixMPTotall += a.PrixMP * a.Quantite;
                PrixVTotall += a.PrixVente * a.Quantite;
                QuantiteTotall+=a.Quantite;

            }
            PrixATotal.Text = PrixATotall.ToString("0.00") + " DH";
            PrixMPTotal.Text = PrixMPTotall.ToString("0.00") + " DH";
            PrixVTotal.Text = PrixVTotall.ToString("0.00") + " DH";
            QuantiteTotal.Text= QuantiteTotall.ToString();
        }
        // Back button in header
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            
            main.load_main(u);
        }

        // ComboBox for search type
        private void SearchTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // À implémenter : changement du type de recherche
        }

        // TextBox for search text
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if(WithCodeCheckBox.IsChecked==true)
            {
        
                foreach (var child in ArticlesContainer.Children)
                {
                    if (child is CSingleArticleI ar)
                    {
                        if (ar.a.Code.ToString().IndexOf(tb.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            ar.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            ar.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
            else
            {
                foreach (var child in ArticlesContainer.Children)
                {
                    if (child is CSingleArticleI ar)
                    {
                        if (ar.a.ArticleName.IndexOf(tb.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            ar.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            ar.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
                
        }

        // Search button (loupe)
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // À implémenter : lancer la recherche
        }

        // Nouveau Article button
        private void NewArticleButton_Click(object sender, RoutedEventArgs e)
        {
            WNouveauStock wNouveauStock = new WNouveauStock(lf, la, lfo, this,1,null,null);
            wNouveauStock.ShowDialog();
        }

        // Fournisseur management button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            main.load_fournisseur(u);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WManageFamillies wManageFamillies = new WManageFamillies(lf, la, this);
            wManageFamillies.ShowDialog();
        }

        private void AddMultipleArticlesButton_Click(object sender, RoutedEventArgs e)
        {
            WAddMultipleArticles wAddMultipleArticles = new WAddMultipleArticles(this);
            wAddMultipleArticles.ShowDialog();
        }
    }
}
