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

        private const int ARTICLES_PER_PAGE = 10;
        private int currentlyLoadedCount = 0;
        private List<Article> filteredArticles;

        public CMainI(User u, List<Article> la, List<Famille> lf, List<Fournisseur> lfo, MainWindow main)
        {
            InitializeComponent();
            this.lf = lf;
            this.main = main;
            this.u = u;
            this.la = la;
            this.lfo = lfo;
            this.filteredArticles = new List<Article>();

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
                        RefreshArticlesList(la, true);
                    }
                    break;
                }
            }
        }

        // Main public method - called from other windows
        public void LoadArticles(List<Article> la)
        {
            RefreshArticlesList(la, false);
        }

        // Internal refresh method
        // Internal refresh method
        // Internal refresh method
        private void RefreshArticlesList(List<Article> la, bool resetPagination)
        {
            foreach (Role r in main.lr)
            {
                if (u.RoleID == r.RoleID)
                {
                    if (r.ViewArticle == true)
                    {
                        // Update the list reference
                        this.la = la;

                        // Filter articles with Etat == true
                        filteredArticles = new List<Article>();
                        foreach (Article a in la)
                        {
                            if (a.Etat)
                            {
                                filteredArticles.Add(a);
                            }
                        }

                        // Store the previous count BEFORE clearing
                        int previousCount = resetPagination ? 0 : currentlyLoadedCount;

                        // Clear the container
                        ArticlesContainer.Children.Clear();

                        // Update total stats
                        UpdateTotalStats();

                        // Determine how many articles to load
                        int articlesToLoad;
                        if (resetPagination || previousCount == 0)
                        {
                            // Initial load or reset - load first page only
                            articlesToLoad = Math.Min(ARTICLES_PER_PAGE, filteredArticles.Count);
                        }
                        else
                        {
                            // Keep showing the same number as before (or more if we added articles)
                            articlesToLoad = Math.Min(previousCount, filteredArticles.Count);
                        }

                        // Load the articles
                        for (int i = 0; i < articlesToLoad; i++)
                        {
                            Article a = filteredArticles[i];
                            CSingleArticleI ar = new CSingleArticleI(a, la, this, lf, lfo);
                            ArticlesContainer.Children.Add(ar);
                        }

                        currentlyLoadedCount = articlesToLoad;
                        UpdateViewMoreButtonVisibility();
                    }
                    break;
                }
            }
        }

        private void LoadMoreArticles()
        {
            int articlesToLoad = Math.Min(ARTICLES_PER_PAGE, filteredArticles.Count - currentlyLoadedCount);

            for (int i = currentlyLoadedCount; i < currentlyLoadedCount + articlesToLoad; i++)
            {
                Article a = filteredArticles[i];
                CSingleArticleI ar = new CSingleArticleI(a, la, this, lf, lfo);
                ArticlesContainer.Children.Add(ar);
            }

            currentlyLoadedCount += articlesToLoad;
            UpdateViewMoreButtonVisibility();
        }

        private void UpdateViewMoreButtonVisibility()
        {
            if (ViewMoreButton != null)
            {
                ViewMoreButton.Visibility = (currentlyLoadedCount < filteredArticles.Count)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private void UpdateTotalStats()
        {
            // Count all articles in the main list with Etat == true
            List<Article> allActiveArticles = new List<Article>();
            foreach (Article a in la)
            {
                if (a.Etat)
                {
                    allActiveArticles.Add(a);
                }
            }

            int count = allActiveArticles.Count;
            Decimal PrixATotall = 0;
            Decimal PrixMPTotall = 0;
            Decimal PrixVTotall = 0;
            int QuantiteTotall = 0;

            ArticlesTotal.Text = count.ToString();

            foreach (Article a in allActiveArticles)
            {
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

        // View More Button Click
        private void ViewMoreButton_Click(object sender, RoutedEventArgs e)
        {
            LoadMoreArticles();
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

            // If search is empty, show all articles with pagination (reset pagination on search clear)
            if (string.IsNullOrEmpty(searchText))
            {
                filteredArticles = new List<Article>();
                foreach (Article a in la)
                {
                    if (a.Etat)
                    {
                        filteredArticles.Add(a);
                    }
                }
                currentlyLoadedCount = 0;
                ArticlesContainer.Children.Clear();
                LoadMoreArticles();
                return;
            }

            // Get selected search criteria
            var selectedItem = SearchCriteriaComboBox.SelectedItem as ComboBoxItem;
            string criteria = selectedItem?.Content.ToString() ?? "Code";

            // Filter articles based on criteria
            filteredArticles = new List<Article>();

            foreach (Article a in la)
            {
                // Skip articles where Etat is false
                if (!a.Etat)
                    continue;

                bool matches = false;

                switch (criteria)
                {
                    case "Code":
                        matches = a.Code.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                        break;

                    case "Article":
                        matches = a.ArticleName != null &&
                                   a.ArticleName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                        break;

                    case "Fournisseur":
                        string fournisseurName = GetFournisseurName(a.FournisseurID);
                        matches = !string.IsNullOrEmpty(fournisseurName) &&
                                   fournisseurName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                        break;

                    case "Famille":
                        string familleName = GetFamilleName(a.FamillyID);
                        matches = !string.IsNullOrEmpty(familleName) &&
                                   familleName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                        break;

                    case "Numero de Lot":
                    case "Numéro de Lot":
                        matches = !string.IsNullOrEmpty(a.numeroLot) &&
                                   a.numeroLot.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                        break;

                    case "Bon de Livraison":
                        matches = !string.IsNullOrEmpty(a.bonlivraison) &&
                                   a.bonlivraison.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                        break;

                    case "Marque":
                        matches = !string.IsNullOrEmpty(a.marque) &&
                                   a.marque.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                        break;
                }

                if (matches)
                {
                    filteredArticles.Add(a);
                }
            }

            // Reset and load filtered results with pagination
            currentlyLoadedCount = 0;
            ArticlesContainer.Children.Clear();
            LoadMoreArticles();
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