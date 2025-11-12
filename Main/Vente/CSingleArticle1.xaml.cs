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

namespace GestionComerce.Main.Vente
{
    /// <summary>
    /// Interaction logic for CSingleArticle1.xaml
    /// </summary>
    public partial class CSingleArticle1 : UserControl
    {
        public CSingleArticle1(Article a, CMainV mainv, List<Famille> lf, List<Fournisseur> lfo, int s)
        {
            InitializeComponent();
            ArticleID.Text = a.ArticleID.ToString();
            ArticleName.Text = a.ArticleName;
            PrixVente.Text = a.PrixVente.ToString("F2");
            Quantite.Text = a.Quantite.ToString();
            PrixAchat.Text = a.PrixAchat.ToString("F2");
            PrixMP.Text = a.PrixMP.ToString("F2");

            foreach (Famille f in lf)
            {
                if (f.FamilleID == a.FamillyID)
                {
                    Famille.Text = f.FamilleName;
                    break;
                }
            }

            foreach (Fournisseur fo in lfo)
            {
                if (a.FournisseurID == fo.FournisseurID)
                {
                    FournisseurName.Text = fo.Nom;
                    break;
                }
            }

            Code.Text = a.Code.ToString();
            this.mainv = mainv;
            this.a = a;
            this.lf = lf;
            this.lfo = lfo;

            if (s == 1)
            {
                ArticleID.Visibility = Visibility.Collapsed;
                ArticleIDC.Width = new GridLength(0);
                PrixAchat.Visibility = Visibility.Collapsed;
                PrixAchatC.Width = new GridLength(0);
                PrixVente.Visibility = Visibility.Collapsed;
                PrixVenteC.Width = new GridLength(0);
                PrixMP.Visibility = Visibility.Collapsed;
                PrixMPC.Width = new GridLength(0);
                Famille.Visibility = Visibility.Collapsed;
                FamilleC.Width = new GridLength(0);

                ArticleIDC.MinWidth = 0;
                PrixAchatC.MinWidth = 0;
                PrixVenteC.MinWidth = 0;
                PrixMPC.MinWidth = 0;
                FamilleC.MinWidth = 0;

                ArticleNameC.MinWidth = 30;
                QuantiteC.MinWidth = 30;
                FournisseurNameC.MinWidth = 30;
                CodeC.MinWidth = 30;

                ArticleNameC.Width = new GridLength(1, GridUnitType.Star);
                QuantiteC.Width = new GridLength(1, GridUnitType.Star);
                FournisseurNameC.Width = new GridLength(1, GridUnitType.Star);
                CodeC.Width = new GridLength(1, GridUnitType.Star);
            }
        }

        CMainV mainv;
        public Article a;
        List<Famille> lf;
        List<Fournisseur> lfo;

        private void ArticleClicked(object sender, RoutedEventArgs e)
        {
            // Check if article already exists in cart
            foreach (UIElement element in mainv.SelectedArticles.Children)
            {
                if (element is CSingleArticle2 item)
                {
                    if (item.a.ArticleID == a.ArticleID)
                    {
                        if (a.Quantite <= Convert.ToInt32(item.Quantite.Text))
                        {
                            MessageBox.Show("La quantité dans le panier est la même que celle que vous avez en stock");
                            return;
                        }
                        item.Quantite.Text = (Convert.ToInt32(item.Quantite.Text) + 1).ToString();
                        item.qte++;
                        mainv.TotalNett += a.PrixVente;
                        mainv.TotalNet.Text = mainv.TotalNett.ToString("F2") + " DH";
                        mainv.NbrA += 1;
                        mainv.ArticleCount.Text = mainv.NbrA.ToString();
                        return;
                    }
                }
            }

            // Add new article to cart
            mainv.TotalNett += a.PrixVente;
            mainv.TotalNet.Text = mainv.TotalNett.ToString("F2") + " DH";
            mainv.NbrA += 1;
            mainv.ArticleCount.Text = mainv.NbrA.ToString();
            CSingleArticle2 sa = new CSingleArticle2(a, 1, mainv);
            mainv.SelectedArticles.Children.Add(sa);
            mainv.UpdateCartEmptyState();
            mainv.SelectedArticle.Child = new CSingleArticle1(a, mainv, lf, lfo, 1);
        }
    }
}