using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GestionComerce.Main.Inventory
{
    /// <summary>
    /// Logique d'interaction pour CMainI.xaml
    /// </summary>
    public partial class CMainI : UserControl
    {
        public List<Famille> lf;
        public MainWindow main;
        public User u;
        public List<Fournisseur> lfo;
        public List<Article> la;

        public CMainI(User u, List<Article> la, List<Famille> lf, List<Fournisseur> lfo, MainWindow main)
        {
            InitializeComponent();
            this.lf = lf;
            this.main = main;
            this.u = u;
            this.la = la;
            this.lfo = lfo;

            foreach (Role r in main.lr)
            {
                if (u.RoleID == r.RoleID)
                {
                    if (r.ViewFamilly == false && r.AddFamilly == false)
                    {
                        ManageFamillies.IsEnabled = false;
                    }
                    if (r.AddArticle == false)
                    {
                        NewArticleButton.IsEnabled = false;
                        AddMultipleArticlesButton.IsEnabled = false;
                    }
                    if (r.ViewFournisseur == false)
                    {
                        FournisseurManage.IsEnabled = false;
                    }
                    if (r.ViewArticle == true)
                    {
                        LoadArticles(la);
                    }
                    break;
                }
            }
        }

        public void LoadArticles(List<Article> la)
        {
            foreach (Role r in main.lr)
            {
                if (u.RoleID == r.RoleID)
                {
                    if (r.ViewArticle == true)
                    {
                        int count = la.Count;
                        Decimal PrixATotall = 0;
                        Decimal PrixMPTotall = 0;
                        Decimal PrixVTotall = 0;
                        int QuantiteTotall = 0;
                        ArticlesTotal.Text = count.ToString();
                        ArticlesContainer.Children.Clear();

                        foreach (Article a in la)
                        {
                            CSingleArticleI ar = new CSingleArticleI(a, la, this, lf, lfo);
                            ArticlesContainer.Children.Add(ar);
                            PrixATotall += a.PrixAchat * a.Quantite;
                            PrixMPTotall += a.PrixMP * a.Quantite;
                            PrixVTotall += a.PrixVente * a.Quantite;
                            QuantiteTotall += a.Quantite;
                        }

                        PrixATotal.Text = PrixATotall.ToString("0.00") + " DH";
                        PrixMPTotal.Text = PrixMPTotall.ToString("0.00") + " DH";
                        PrixVTotal.Text = PrixVTotall.ToString("0.00") + " DH";
                        QuantiteTotal.Text = QuantiteTotall.ToString();
                    }
                    break;
                }
            }
        }

        // Back button in header
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            main.load_main(u);
        }

        // ComboBox for search criteria selection
        private void SearchCriteriaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Reapply filter when criteria changes
            if (SearchTextBox != null && ArticlesContainer != null)
            {
                ApplySearchFilter();
            }
        }

        // TextBox for search text
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplySearchFilter();
        }

        // Search button (loupe)
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ApplySearchFilter();
        }

        // Clear button
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Clear();
            // Filter will be applied automatically through TextChanged event
        }

        // Main search filter method
        private void ApplySearchFilter()
        {
            string searchText = SearchTextBox.Text.Trim();

            // If search is empty, show all articles
            if (string.IsNullOrEmpty(searchText))
            {
                foreach (var child in ArticlesContainer.Children)
                {
                    if (child is CSingleArticleI ar)
                    {
                        ar.Visibility = Visibility.Visible;
                    }
                }
                return;
            }

            // Get selected search criteria
            var selectedItem = SearchCriteriaComboBox.SelectedItem as ComboBoxItem;
            string criteria = selectedItem?.Content.ToString() ?? "Code";

            // Apply filter based on selected criteria
            foreach (var child in ArticlesContainer.Children)
            {
                if (child is CSingleArticleI ar)
                {
                    bool isVisible = false;

                    switch (criteria)
                    {
                        case "Code":
                            isVisible = ar.a.Code.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                            break;

                        case "Article":
                            isVisible = ar.a.ArticleName != null &&
                                       ar.a.ArticleName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                            break;

                        case "Fournisseur":
                            string fournisseurName = GetFournisseurName(ar.a.FournisseurID);
                            isVisible = !string.IsNullOrEmpty(fournisseurName) &&
                                       fournisseurName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                            break;

                        case "Famille":
                            string familleName = GetFamilleName(ar.a.FamillyID);
                            isVisible = !string.IsNullOrEmpty(familleName) &&
                                       familleName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                            break;

                        case "Numero de Lot":
                        case "Numéro de Lot":
                            isVisible = !string.IsNullOrEmpty(ar.a.numeroLot) &&
                                       ar.a.numeroLot.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                            break;

                        case "Bon de Livraison":
                            isVisible = !string.IsNullOrEmpty(ar.a.bonlivraison) &&
                                       ar.a.bonlivraison.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                            break;

                        case "Marque":
                            isVisible = !string.IsNullOrEmpty(ar.a.marque) &&
                                       ar.a.marque.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                            break;
                    }

                    ar.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        // Helper method to get Fournisseur name by ID
        private string GetFournisseurName(int fournisseurId)
        {
            foreach (var fournisseur in lfo)
            {
                if (fournisseur.FournisseurID == fournisseurId)
                {
                    return fournisseur.Nom;
                }
            }
            return string.Empty;
        }

        // Helper method to get Famille name by ID
        private string GetFamilleName(int familleId)
        {
            foreach (var famille in lf)
            {
                if (famille.FamilleID == familleId)
                {
                    return famille.FamilleName;
                }
            }
            return string.Empty;
        }

        // Nouveau Article button
        private void NewArticleButton_Click(object sender, RoutedEventArgs e)
        {
            WNouveauStock wNouveauStock = new WNouveauStock(lf, la, lfo, this, 1, null, null);
            wNouveauStock.ShowDialog();
        }

        // Fournisseur management button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            main.load_fournisseur(u);
        }

        // Manage Famillies button
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WManageFamillies wManageFamillies = new WManageFamillies(lf, la, this);
            wManageFamillies.ShowDialog();
        }

        // Add Multiple Articles button
        private void AddMultipleArticlesButton_Click(object sender, RoutedEventArgs e)
        {
            WAddMultipleArticles wAddMultipleArticles = new WAddMultipleArticles(this);
            wAddMultipleArticles.ShowDialog();
        }
    }
}