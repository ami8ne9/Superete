using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GestionComerce.Main.Inventory
{
    public partial class CSingleArticleI : UserControl
    {
        List<Article> la;
        List<Famille> lf;
        CMainI main;
        public Article a;
        List<Fournisseur> lfo;

        public CSingleArticleI(Article a, List<Article> la, CMainI main, List<Famille> lf, List<Fournisseur> lfo)
        {
            InitializeComponent();
            this.a = a;
            this.la = la;
            this.main = main;
            this.lf = lf;
            this.lfo = lfo;

            // Set article name
            ArticleName.Text = a.ArticleName;

            // Set famille name
            foreach (Famille f in lf)
                if (f.FamilleID == a.FamillyID)
                {
                    Famillee.Text = f.FamilleName;
                    break;
                }

            // Set fournisseur name
            foreach (Fournisseur fo in lfo)
                if (fo.FournisseurID == a.FournisseurID)
                {
                    Fournisseur.Text = fo.Nom;
                    break;
                }

            // Set prices
            PrixVente.Text = a.PrixVente.ToString("0.00") + " Dh";
            PrixAchat.Text = a.PrixAchat.ToString("0.00") + " Dh";

            // Set quantity
            Quantite.Text = a.Quantite.ToString();

            // Set expiration date - Handle nullable DateTime
            if (a.DateExpiration.HasValue)
            {
                DateExpiration.Text = a.DateExpiration.Value.ToString("dd/MM/yyyy");

                // Check if expired or near expiration
                TimeSpan timeUntilExpiration = a.DateExpiration.Value - DateTime.Now;

                if (timeUntilExpiration.TotalDays < 0)
                {
                    // Expired
                    DateExpiration.Foreground = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(220, 38, 38)); // Red
                    DateExpiration.FontWeight = FontWeights.Bold;
                }
                else if (timeUntilExpiration.TotalDays <= 30)
                {
                    // Expiring soon (within 30 days)
                    DateExpiration.Foreground = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(245, 158, 11)); // Orange
                    DateExpiration.FontWeight = FontWeights.SemiBold;
                }
            }
            else
            {
                DateExpiration.Text = "N/A";
            }

            // Set permissions based on role
            foreach (Role r in main.main.lr)
            {
                if (main.u.RoleID == r.RoleID)
                {
                    if (r.DeleteArticle == false)
                    {
                        Deletebtn.IsEnabled = false;
                    }
                    if (r.EditArticle == false)
                    {
                        Editbtn.IsEnabled = false;
                    }
                    break;
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            WAddArticle af = new WAddArticle(a, la, lf, lfo, main, 0, null, null);
            af.ShowDialog();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            WDeleteConfirmation df = new WDeleteConfirmation(a, la, null, main);
            df.ShowDialog();
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            WArticleDetails detailsWindow = new WArticleDetails(a, lf, lfo);
            detailsWindow.ShowDialog();
        }
    }
}