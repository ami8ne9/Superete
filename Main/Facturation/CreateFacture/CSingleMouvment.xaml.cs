using GestionComerce;
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

namespace GestionComerce.Main.Facturation.CreateFacture
{
    /// <summary>
    /// Interaction logic for CSingleMouvment.xaml
    /// </summary>
    public partial class CSingleMouvment : UserControl
    {
        public WMouvments ms;
        OperationArticle oa;
        Article article;
        CSingleOperation singleOp;
        private decimal expeditionQuantity = 0;
        private bool isOperationSelected = false;

        public CSingleMouvment(WMouvments ms, OperationArticle oa)
        {
            InitializeComponent();
            this.ms = ms;
            this.oa = oa;
            this.singleOp = ms?.wso;

            // Check if operation is selected (in CMainFa view, not selection view)
            isOperationSelected = singleOp?.mainfa != null && singleOp.mainfa.SelectedOperations.Any(o => o.OperationID == oa.OperationID);

            // Check if oa is null
            if (oa == null)
            {
                MessageBox.Show("OperationArticle is null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ArticleQuantity.Text = oa.QteArticle.ToString();

            // Load article information
            Article foundArticle = null;
            MainWindow main = null;

            // Get the main window reference
            if (singleOp?.mainfa?.main != null)
            {
                main = singleOp.mainfa.main;
            }
            else if (ms?.wso?.sc?.main?.main != null)
            {
                main = ms.wso.sc.main.main;
            }

            if (main?.la != null)
            {
                foundArticle = main.la.FirstOrDefault(a => a.ArticleID == oa.ArticleID);
            }

            if (foundArticle != null)
            {
                article = foundArticle;
                ArticleName.Text = article.ArticleName;
                if (oa.Reversed == true)
                {
                    ArticleName.Text += " (Reversed)";
                }
            }
            else
            {
                ArticleName.Text = "Article not found (ID: " + oa.ArticleID + ")";
            }

            // Handle Expedition type invoice
            string invoiceType = null;
            CMainFa mainfa = null;

            // Get the CMainFa reference
            if (singleOp?.mainfa != null)
            {
                mainfa = singleOp.mainfa;
                invoiceType = singleOp.mainfa.InvoiceType;
            }
            else if (ms?.wso?.sc?.main != null)
            {
                mainfa = ms.wso.sc.main;
                invoiceType = ms.wso.sc.main.InvoiceType;
            }

            // Only show expedition controls if:
            // 1. Invoice type is Expedition
            // 2. Operation is selected (in CMainFa, not just in selection window)
            if (invoiceType == "Expedition" && isOperationSelected)
            {
                NbrArticleExpide.Visibility = Visibility.Visible;
                NbrArticleExpide.IsEnabled = true;

                // Try to get saved quantity from InvoiceArticles
                decimal savedQuantity = GetSavedQuantityFromInvoiceArticles(mainfa);

                // Always use saved quantity if it exists (even if 0)
                NbrArticleExpide.Text = savedQuantity.ToString();
                expeditionQuantity = savedQuantity;

                // Add event handlers
                NbrArticleExpide.TextChanged += NbrArticleExpide_TextChanged;
                NbrArticleExpide.PreviewTextInput += NbrArticleExpide_PreviewTextInput;
                NbrArticleExpide.LostFocus += NbrArticleExpide_LostFocus;
            }
            else
            {
                NbrArticleExpide.Visibility = Visibility.Collapsed;
                NbrArticleExpide.IsEnabled = false;
            }
        }

        // Get saved quantity from InvoiceArticles
        private decimal GetSavedQuantityFromInvoiceArticles(CMainFa mainfa)
        {
            if (mainfa?.InvoiceArticles == null || article == null || oa == null)
                return oa?.QteArticle ?? 0;

            // For expedition invoices, look for exact match (OperationID + ArticleID)
            if (mainfa.InvoiceType == "Expedition")
            {
                var invoiceArticle = mainfa.InvoiceArticles.FirstOrDefault(ia =>
                    ia.OperationID == oa.OperationID &&
                    ia.ArticleID == oa.ArticleID);

                if (invoiceArticle != null)
                {
                    return invoiceArticle.Quantite;
                }
            }
            else
            {
                // For regular invoices, look for article with same properties
                var invoiceArticle = mainfa.InvoiceArticles.FirstOrDefault(ia =>
                    ia.ArticleID == oa.ArticleID &&
                    ia.ArticleName == article.ArticleName &&
                    ia.Prix == article.PrixVente &&
                    ia.TVA == article.tva);

                if (invoiceArticle != null)
                {
                    return invoiceArticle.Quantite;
                }
            }

            // If not found, return default quantity
            return oa.QteArticle;
        }

        // Prevent non-numeric input
        private void NbrArticleExpide_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0) && e.Text != "." && e.Text != ",";
        }

        private void NbrArticleExpide_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (oa == null)
                return;

            TextBox textBox = sender as TextBox;
            if (textBox == null || string.IsNullOrWhiteSpace(textBox.Text))
                return;

            string text = textBox.Text.Replace(',', '.');

            if (decimal.TryParse(text, out decimal enteredValue))
            {
                if (enteredValue > oa.QteArticle)
                {
                    textBox.Text = oa.QteArticle.ToString();
                    textBox.CaretIndex = textBox.Text.Length;
                    expeditionQuantity = oa.QteArticle;

                    MessageBox.Show(
                        $"La quantité ne peut pas dépasser {oa.QteArticle}",
                        "Limite dépassée",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
                else if (enteredValue < 0)
                {
                    textBox.Text = "0";
                    textBox.CaretIndex = textBox.Text.Length;
                    expeditionQuantity = 0;
                }
                else
                {
                    expeditionQuantity = enteredValue;

                    // Auto-update for expedition invoices
                    if (singleOp?.mainfa?.InvoiceType == "Expedition" && isOperationSelected)
                    {
                        UpdateInvoiceArticle();
                    }
                }
            }
            else
            {
                textBox.Text = "0";
                textBox.CaretIndex = textBox.Text.Length;
                expeditionQuantity = 0;

                if (singleOp?.mainfa?.InvoiceType == "Expedition" && isOperationSelected)
                {
                    UpdateInvoiceArticle();
                }
            }
        }

        private void NbrArticleExpide_LostFocus(object sender, RoutedEventArgs e)
        {
            if (isOperationSelected)
            {
                UpdateInvoiceArticle();
            }
        }

        public void UpdateInvoiceArticle()
        {
            if (oa == null || article == null || !isOperationSelected)
                return;

            CMainFa mainfa = null;
            if (singleOp?.mainfa != null)
            {
                mainfa = singleOp.mainfa;
            }
            else if (ms?.wso?.sc?.main != null)
            {
                mainfa = ms.wso.sc.main;
            }

            if (mainfa == null)
                return;

            // Get quantity from textbox
            decimal quantityToUse = expeditionQuantity;

            if (NbrArticleExpide.Visibility == Visibility.Visible)
            {
                if (decimal.TryParse(NbrArticleExpide.Text.Replace(',', '.'), out decimal expQuantity))
                {
                    quantityToUse = expQuantity;
                    expeditionQuantity = expQuantity;
                }
            }

            // Update in InvoiceArticles
            if (mainfa.InvoiceType == "Expedition")
            {
                // For expedition, update specific article
                mainfa.UpdateArticleQuantityForExpedition(oa.OperationID, oa.ArticleID, quantityToUse);
            }
            else
            {
                // For regular invoices, update merged article
                mainfa.UpdateArticleQuantity(oa.OperationID, oa.ArticleID, quantityToUse);
            }
        }
    }
}