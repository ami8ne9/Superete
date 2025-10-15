using Superete;
using Superete.Main.FournisseurPage;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Superete.Main.ClientPage
{
    public partial class ClientOperationsWindow : Window
    {
        private Client _currentClient;
        private bool _isArticlesPanelVisible = false;
        private MainWindow _main;

        public ClientOperationsWindow(Client client)
        {
            InitializeComponent();
            _currentClient = client;

            // Get MainWindow reference
            _main = Application.Current.MainWindow as MainWindow;

            LoadOperations();
        }

        // Keep the parameterless constructor for design-time support
        public ClientOperationsWindow()
        {
            InitializeComponent();
            _currentClient = null;
            _main = Application.Current.MainWindow as MainWindow;
            LoadOperations();
        }

        private void LoadOperations()
        {
            try
            {
                // Load operations from MainWindow list instead of database
                List<Operation> operations = _main?.lo ?? new List<Operation>();

                // Filter operations by client if one is specified
                if (_currentClient != null)
                {
                    operations = operations.Where(op => op.ClientID == _currentClient.ClientID).ToList();
                }

                OperationsList.ItemsSource = operations;

                // Clear articles list and summary when operations are loaded
                ArticlesList.ItemsSource = null;
                ClearSummary();
                // Hide articles panel initially
                if (_isArticlesPanelVisible)
                {
                    HideArticlesPanel();
                }

                // Update info text based on context
                if (_currentClient != null)
                {
                    OperationInfoText.Text = $"{operations.Count} opération(s) pour {_currentClient.Nom}";
                    // Update window title to show client name
                    this.Title = $"Opérations Client - {_currentClient.Nom}";
                }
                else
                {
                    OperationInfoText.Text = $"{operations.Count} opération(s) trouvée(s)";
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des opérations: {ex.Message}",
                              "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowArticlesPanel()
        {
            if (!_isArticlesPanelVisible)
            {
                _isArticlesPanelVisible = true;

                // Calculate current center position
                double currentCenterX = this.Left + (this.Width / 2);
                double currentCenterY = this.Top + (this.Height / 2);

                // Set the new width and column width
                ArticlesColumn.Width = new GridLength(750);
                this.Width = 1500;

                // Recalculate position to keep window centered
                this.Left = currentCenterX - (this.Width / 2);
                this.Top = currentCenterY - (this.Height / 2);

                // Ensure window stays within screen bounds
                EnsureWindowOnScreen();
            }
        }

        private void HideArticlesPanel()
        {
            if (_isArticlesPanelVisible)
            {
                _isArticlesPanelVisible = false;

                // Calculate current center position
                double currentCenterX = this.Left + (this.Width / 2);
                double currentCenterY = this.Top + (this.Height / 2);

                // Hide the column and resize window
                ArticlesColumn.Width = new GridLength(0);
                this.Width = 750;

                // Recalculate position to keep window centered
                this.Left = currentCenterX - (this.Width / 2);
                this.Top = currentCenterY - (this.Height / 2);

                // Ensure window stays within screen bounds
                EnsureWindowOnScreen();
            }
        }

        private void EnsureWindowOnScreen()
        {
            // Get screen dimensions
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            // Ensure window doesn't go off the left or top
            if (this.Left < 0)
                this.Left = 0;
            if (this.Top < 0)
                this.Top = 0;

            // Ensure window doesn't go off the right or bottom
            if (this.Left + this.Width > screenWidth)
                this.Left = screenWidth - this.Width;
            if (this.Top + this.Height > screenHeight)
                this.Top = screenHeight - this.Height;
        }

        private void CloseArticlesButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear the selection which will trigger the hiding of the panel
            OperationsList.SelectedItem = null;
        }

        private void OperationsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OperationsList.SelectedItem is Operation selectedOperation)
            {
                try
                {
                    // Load from MainWindow lists instead of database
                    var allOperationArticles = _main?.loa ?? new List<OperationArticle>();
                    var operationArticles = allOperationArticles
                        .Where(oa => oa.OperationID == selectedOperation.OperationID)
                        .ToList();

                    // Get all articles from MainWindow list
                    var allArticles = _main?.la ?? new List<Article>();

                    // Get all families from MainWindow list
                    var allFamilies = _main?.lf ?? new List<Famille>();

                    // Create enriched operation articles with article details
                    var enrichedOperationArticles = operationArticles.Select(oa =>
                    {
                        var article = allArticles.FirstOrDefault(a => a.ArticleID == oa.ArticleID);
                        var famille = allFamilies?.FirstOrDefault(f => f.FamilleID == article?.FamillyID);

                        return new EnrichedOperationArticle
                        {
                            OperationID = oa.OperationID,
                            ArticleID = oa.ArticleID,
                            QteArticle = oa.QteArticle,
                            UnitPrice = article?.PrixVente ?? 0,
                            Total = oa.QteArticle * (article?.PrixVente ?? 0),
                            ArticleName = article?.ArticleName ?? "Article inconnu",
                            Famille = famille?.FamilleName ?? "N/A"
                        };
                    }).ToList();

                    ArticlesList.ItemsSource = enrichedOperationArticles;

                    // Update the summary and operation info
                    UpdateSummary(selectedOperation, operationArticles);
                    OperationInfoText.Text = $"Opération #{selectedOperation.OperationID} - {selectedOperation.DateOperation:dd/MM/yyyy} - {selectedOperation.OperationType ?? "N/A"}";
                    ShowArticlesPanel();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Erreur lors du chargement des articles: {ex.Message}",
                                  "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);

                    // Clear articles on error
                    ArticlesList.ItemsSource = null;
                    ClearSummary();
                    HideArticlesPanel();
                }
            }
            else
            {
                // Clear articles when no operation is selected
                ArticlesList.ItemsSource = null;
                ClearSummary();
                HideArticlesPanel();
                OperationInfoText.Text = "Sélectionnez une opération";
            }
        }

        private void UpdateSummary(Operation operation, List<OperationArticle> operationArticles)
        {
            if (operation == null || operationArticles == null)
            {
                ClearSummary();
                return;
            }

            // Calculate summary values
            int totalArticles = operationArticles.Count;
            int totalQuantity = operationArticles.Sum(a => a.QteArticle);

            // Update UI
            TotalArticlesText.Text = totalArticles.ToString();
            TotalQuantityText.Text = totalQuantity.ToString();
            TotalAmountText.Text = operation.PrixOperation.ToString("C2");

            // Show additional operation details if available
            if (operation.Remise > 0)
            {
                // You could add a remise display here if needed in the XAML
                // RemiseText.Text = operation.Remise.ToString("C2");
            }
        }

        private void ClearSummary()
        {
            TotalArticlesText.Text = "0";
            TotalQuantityText.Text = "0";
            TotalAmountText.Text = "0,00 €";
        }

        // Event handler for refresh button (if you have one in XAML)
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadOperations();
        }

        // Event handler for close button (if you have one in XAML)
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Optional: Method to export or print operations (if needed)
        private void ExportOperations_Click(object sender, RoutedEventArgs e)
        {
            // Implementation for exporting operations
            MessageBox.Show("Fonctionnalité d'export à implémenter",
                           "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
