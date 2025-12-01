using Microsoft.Win32;
using Superete;
using GestionComerce.Main.Facturation.CreateFacture;
using GestionComerce.Main.Facturation;
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
    // Helper class to store article information for invoice calculations
    public class InvoiceArticle
    {
        public int OperationID { get; set; }
        public int ArticleID { get; set; }
        public string ArticleName { get; set; }
        public decimal Prix { get; set; }
        public decimal Quantite { get; set; }
        public decimal TVA { get; set; }
        public bool Reversed { get; set; }

        public decimal TotalHT => Prix * Quantite;
        public decimal MontantTVA => (TVA / 100) * TotalHT;
        public decimal TotalTTC => TotalHT + MontantTVA;
    }

    /// <summary>
    /// Interaction logic for CMainFa.xaml
    /// </summary>
    public partial class CMainFa : UserControl
    {
        private const int ETAT_FACTURE_NORMAL = 0;
        private const int ETAT_FACTURE_REVERSED = 1;

        public MainWindow main;
        User u;
        private decimal currentTotalHT = 0;
        public string InvoiceType = "Facture";

        // List to store all articles from selected operations
        public List<InvoiceArticle> InvoiceArticles = new List<InvoiceArticle>();

        // List to store selected operations
        public List<Operation> SelectedOperations = new List<Operation>();

        public CMainFa(User u, MainWindow main, CMainIn In, Operation op)
        {
            InitializeComponent();
            SelectedOperations = new List<Operation>();
            InvoiceArticles = new List<InvoiceArticle>();
            this.main = main;
            this.u = u;

            // Initialize after all UI elements are loaded
            this.Loaded += (s, e) =>
            {
                if (op != null)
                {
                    InitializeWithOperation(op);
                }
            };

            LoadFacture();
        }

        private void InitializeWithOperation(Operation op)
        {
            if (EtatFacture != null)
            {
                if (op.Reversed == true)
                {
                    EtatFacture.SelectedIndex = ETAT_FACTURE_REVERSED;
                    EtatFacture.IsEnabled = false;
                }
                else
                {
                    EtatFacture.SelectedIndex = ETAT_FACTURE_NORMAL;
                    EtatFacture.IsEnabled = false;

                    if (main?.loa != null)
                    {
                        foreach (OperationArticle oa in main.loa)
                        {
                            if (oa.OperationID == op.OperationID && oa.Reversed == true)
                            {
                                EtatFacture.IsEnabled = true;
                                break;
                            }
                        }
                    }
                }
            }

            // Add operation to selected operations and load its articles
            AddOperation(op);

            // Display operation
            if (OperationContainer != null)
            {
                CSingleOperation cSingleOperation = new CSingleOperation(this, null, op);
                OperationContainer.Children.Add(cSingleOperation);
            }

            if (Remise != null)
            {
                Remise.Text = op.Remise.ToString("0.00");
            }
        }

        public void AddOperation(Operation op)
        {
            // Check if operation already exists in main
            if (SelectedOperations.Any(o => o.OperationID == op.OperationID))
            {
                MessageBox.Show(
                    "Cette opération a déjà été ajoutée !",
                    "Opération dupliquée",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            SelectedOperations.Add(op);

            // Load articles from this operation
            LoadArticlesFromOperation(op);

            // Only recalculate if UI is ready
            if (txtTotalAmount != null && txtTVAAmount != null)
            {
                RecalculateTotals();
            }
        }

        public void RemoveOperation(Operation op)
        {
            SelectedOperations.RemoveAll(o => o.OperationID == op.OperationID);

            // Remove all articles from this operation
            InvoiceArticles.RemoveAll(ia => ia.OperationID == op.OperationID);

            RecalculateTotals();
        }

        private void LoadArticlesFromOperation(Operation op)
        {
            if (main?.loa == null || main?.la == null)
                return;

            foreach (OperationArticle oa in main.loa)
            {
                if (oa.OperationID == op.OperationID)
                {
                    var article = main.la.FirstOrDefault(a => a.ArticleID == oa.ArticleID);
                    if (article != null)
                    {
                        // Check if article already exists for this specific operation
                        var existingArticle = InvoiceArticles.FirstOrDefault(ia =>
                            ia.OperationID == op.OperationID &&
                            ia.ArticleID == article.ArticleID);

                        if (existingArticle != null)
                        {
                            // Article already exists for this operation, skip
                            continue;
                        }

                        // Determine initial quantity based on invoice type
                        decimal quantity = oa.QteArticle;

                        InvoiceArticle invoiceArticle = new InvoiceArticle
                        {
                            OperationID = op.OperationID,
                            ArticleID = article.ArticleID,
                            ArticleName = article.ArticleName,
                            Prix = article.PrixVente,
                            Quantite = quantity,
                            TVA = article.tva,
                            Reversed = oa.Reversed
                        };

                        // For regular invoices (not expedition), merge articles with same properties
                        if (InvoiceType != "Expedition")
                        {
                            AddOrMergeArticle(invoiceArticle, true);
                        }
                        else
                        {
                            // For expedition invoices, add article separately
                            InvoiceArticles.Add(invoiceArticle);
                        }
                    }
                }
            }
        }

        private void AddOrMergeArticle(InvoiceArticle newArticle, bool showMessage)
        {
            // For regular invoices, find if same article exists (same ID, name, price, TVA, reversed status)
            var existingArticle = InvoiceArticles.FirstOrDefault(ia =>
                ia.ArticleID == newArticle.ArticleID &&
                ia.ArticleName == newArticle.ArticleName &&
                ia.Prix == newArticle.Prix &&
                ia.TVA == newArticle.TVA &&
                ia.Reversed == newArticle.Reversed);

            if (existingArticle != null)
            {
                // Merge quantities
                existingArticle.Quantite += newArticle.Quantite;

                if (showMessage)
                {
                    MessageBox.Show(
                        $"Quantité mise à jour pour {newArticle.ArticleName} : {existingArticle.Quantite}",
                        "Quantité fusionnée",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            else
            {
                // Add new article
                InvoiceArticles.Add(newArticle);
            }
        }

        // Public method to add articles directly (for WMouvments window)
        public void AddArticlesToInvoice(List<InvoiceArticle> articles, bool showMessages = true)
        {
            if (InvoiceArticles == null)
            {
                InvoiceArticles = new List<InvoiceArticle>();
            }

            foreach (var article in articles)
            {
                // For expedition invoices, handle differently
                if (InvoiceType == "Expedition")
                {
                    // For expedition, find exact match (OperationID + ArticleID)
                    var existingArticle = InvoiceArticles.FirstOrDefault(a =>
                        a.OperationID == article.OperationID &&
                        a.ArticleID == article.ArticleID);

                    if (existingArticle != null)
                    {
                        // Update existing article
                        existingArticle.Quantite = article.Quantite;
                    }
                    else
                    {
                        // Add new article
                        InvoiceArticles.Add(article);
                    }
                }
                else
                {
                    // For regular invoices, merge articles
                    AddOrMergeArticle(article, showMessages);
                }
            }

            RecalculateTotals();
        }

        // Method to update article quantity for expedition
        public bool UpdateArticleQuantityForExpedition(int operationId, int articleId, decimal expeditionQuantity)
        {
            // For expedition invoices, find article by OperationID and ArticleID
            var invoiceArticle = InvoiceArticles.FirstOrDefault(ia =>
                ia.OperationID == operationId &&
                ia.ArticleID == articleId);

            if (invoiceArticle != null)
            {
                // Always save the quantity, even if 0
                invoiceArticle.Quantite = expeditionQuantity;
                RecalculateTotals();
                return true;
            }

            return false;
        }

        public void UpdateArticleQuantity(int operationId, int articleId, decimal newQuantity)
        {
            if (InvoiceType == "Expedition")
            {
                // For expedition invoices, update specific article
                UpdateArticleQuantityForExpedition(operationId, articleId, newQuantity);
            }
            else
            {
                // For regular invoices, this is more complex since articles are merged
                // We need to find all articles with this ArticleID and recalculate
                var similarArticles = InvoiceArticles
                    .Where(ia => ia.ArticleID == articleId)
                    .ToList();

                if (similarArticles.Count > 0)
                {
                    // This is a simplified approach - in reality you might need to track
                    // which operation contributed what quantity
                    // For now, we'll update the first matching article
                    var articleToUpdate = similarArticles.First();
                    articleToUpdate.Quantite = newQuantity;

                    // Remove other similar articles (they should have been merged)
                    for (int i = 1; i < similarArticles.Count; i++)
                    {
                        InvoiceArticles.Remove(similarArticles[i]);
                    }

                    RecalculateTotals();
                }
            }
        }

        public void RecalculateTotals()
        {
            // Check if UI elements are initialized
            if (txtTotalAmount == null || txtTVAAmount == null || txtApresTVAAmount == null ||
                txtApresRemiseAmount == null || txtTVARate == null)
                return;

            // For calculation, use only articles with quantity > 0
            var articlesForCalculation = InvoiceArticles.Where(ia => ia.Quantite > 0).ToList();

            decimal totalHT = 0;
            decimal totalTVA = 0;
            decimal totalHTReversed = 0;
            decimal totalTVAReversed = 0;
            bool hasReversedItems = false;
            bool hasNormalItems = false;

            // Filter articles based on EtatFacture selection
            foreach (var invoiceArticle in articlesForCalculation)
            {
                if (invoiceArticle.Reversed)
                {
                    totalHTReversed += invoiceArticle.TotalHT;
                    totalTVAReversed += invoiceArticle.MontantTVA;
                    hasReversedItems = true;
                }
                else
                {
                    totalHT += invoiceArticle.TotalHT;
                    totalTVA += invoiceArticle.MontantTVA;
                    hasNormalItems = true;
                }
            }

            // Enable Remise only when there are BOTH reversed AND normal items
            if (Remise != null)
            {
                if (hasReversedItems && hasNormalItems)
                {
                    Remise.IsEnabled = true;
                }
                else
                {
                    Remise.IsEnabled = false;
                    Remise.Text = "";
                }
            }

            // Get remise value
            decimal remiseValue = 0;
            if (Remise != null && !string.IsNullOrWhiteSpace(Remise.Text))
            {
                string cleanedRemise = CleanNumericInput(Remise.Text);
                decimal.TryParse(cleanedRemise, out remiseValue);
            }

            // Update displayed amounts based on selected state
            if (EtatFacture != null && EtatFacture.SelectedIndex == ETAT_FACTURE_REVERSED && hasReversedItems)
            {
                currentTotalHT = totalHTReversed;
                decimal totalAfterRemise = totalHTReversed - remiseValue;
                decimal tvaAfterRemise = (totalTVAReversed / totalHTReversed) * totalAfterRemise;

                txtTotalAmount.Text = totalHTReversed.ToString("0.00") + " DH";
                txtTVAAmount.Text = tvaAfterRemise.ToString("0.00") + " DH";
                txtApresTVAAmount.Text = (totalHTReversed + tvaAfterRemise).ToString("0.00") + " DH";
                txtApresRemiseAmount.Text = (totalAfterRemise + tvaAfterRemise).ToString("0.00") + " DH";

                decimal tvaPercentage = totalHTReversed > 0 ? (totalTVAReversed / totalHTReversed) * 100 : 0;
                txtTVARate.Text = tvaPercentage.ToString("0.00");
            }
            else
            {
                currentTotalHT = totalHT;
                decimal totalAfterRemise = totalHT - remiseValue;
                decimal tvaAfterRemise = totalHT > 0 ? (totalTVA / totalHT) * totalAfterRemise : 0;

                txtTotalAmount.Text = totalHT.ToString("0.00") + " DH";
                txtTVAAmount.Text = tvaAfterRemise.ToString("0.00") + " DH";
                txtApresTVAAmount.Text = (totalHT + tvaAfterRemise).ToString("0.00") + " DH";
                txtApresRemiseAmount.Text = (totalAfterRemise + tvaAfterRemise).ToString("0.00") + " DH";

                decimal tvaPercentage = totalHT > 0 ? (totalTVA / totalHT) * 100 : 0;
                txtTVARate.Text = tvaPercentage.ToString("0.00");
            }
        }

        // Get filtered articles for WFacturePage (exclude articles with quantity 0)
        public List<InvoiceArticle> GetFilteredInvoiceArticles()
        {
            // Return only articles with quantity > 0 for display
            return InvoiceArticles.Where(ia => ia.Quantite > 0).ToList();
        }

        // Get article by OperationID and ArticleID (for expedition)
        public InvoiceArticle GetArticleForExpedition(int operationId, int articleId)
        {
            return InvoiceArticles.FirstOrDefault(ia =>
                ia.OperationID == operationId &&
                ia.ArticleID == articleId);
        }

        public async Task LoadFacture()
        {
            try
            {
                Facture facturee = await new Facture().GetFactureAsync();
                txtUserName.Text = facturee.Name ?? "";
                txtUserICE.Text = facturee.ICE ?? "";
                txtUserVAT.Text = facturee.VAT ?? "";
                txtUserPhone.Text = facturee.Telephone ?? "";
                txtUserAddress.Text = facturee.Adresse ?? "";
                txtClientIdSociete.Text = facturee.CompanyId ?? "";
                txtClientEtatJuridique.Text = facturee.EtatJuridic ?? "";
                cmbClientSiegeEntreprise.Text = facturee.SiegeEntreprise ?? "";
                txtLogoPath.Text = facturee.LogoPath ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading facture: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSelectClient_Click(object sender, RoutedEventArgs e)
        {
            WSelectClient wSelectClient = new WSelectClient(this);
            wSelectClient.ShowDialog();
        }

        private void btnSelectOperation_Click(object sender, RoutedEventArgs e)
        {
            WSelectOperation wSelectOperation = new WSelectOperation(this);
            wSelectOperation.ShowDialog();
        }

        private void txtTotalAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtTotalAmount == null || txtTVARate == null || txtTVAAmount == null || txtApresTVAAmount == null)
                return;

            if (string.IsNullOrWhiteSpace(txtTotalAmount.Text) || string.IsNullOrWhiteSpace(txtTVARate.Text))
                return;

            try
            {
                string cleanedTotal = CleanNumericInput(txtTotalAmount.Text);
                string cleanedTVA = CleanNumericInput(txtTVARate.Text);

                if (!decimal.TryParse(cleanedTotal, out decimal total))
                    return;

                if (!decimal.TryParse(cleanedTVA, out decimal tvaRate))
                    return;

                currentTotalHT = total;

                decimal remiseValue = 0;
                if (Remise != null && !string.IsNullOrWhiteSpace(Remise.Text))
                {
                    string cleanedRemise = CleanNumericInput(Remise.Text);
                    decimal.TryParse(cleanedRemise, out remiseValue);
                }

                decimal totalAfterRemise = total - remiseValue;
                decimal tvaMultiplier = tvaRate * 0.01m;
                decimal tvaAmount = totalAfterRemise * tvaMultiplier;
                decimal totalWithTVA = totalAfterRemise + tvaAmount;

                txtTVAAmount.Text = tvaAmount.ToString("0.00") + " DH";
                txtApresTVAAmount.Text = (total + tvaAmount).ToString("0.00") + " DH";

                if (txtApresRemiseAmount != null)
                {
                    txtApresRemiseAmount.Text = totalWithTVA.ToString("0.00") + " DH";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in txtTotalAmount_TextChanged: {ex.Message}");
            }
        }

        private void txtTVARate_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox == null || string.IsNullOrWhiteSpace(textBox.Text))
                return;

            string cleanedText = CleanNumericInput(textBox.Text);

            if (!decimal.TryParse(cleanedText, out decimal tvaRate))
                return;

            if (tvaRate > 100)
            {
                if (textBox.Text.Length > 0)
                {
                    int caretPosition = textBox.CaretIndex;
                    textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1);
                    textBox.CaretIndex = Math.Min(caretPosition, textBox.Text.Length);
                }
                return;
            }

            if (txtTotalAmount == null || string.IsNullOrWhiteSpace(txtTotalAmount.Text))
                return;

            string cleanedTotal = CleanNumericInput(txtTotalAmount.Text);
            if (!decimal.TryParse(cleanedTotal, out decimal total))
                return;

            decimal remiseValue = 0;
            if (Remise != null && !string.IsNullOrWhiteSpace(Remise.Text))
            {
                string cleanedRemise = CleanNumericInput(Remise.Text);
                decimal.TryParse(cleanedRemise, out remiseValue);
            }

            decimal totalAfterRemise = total - remiseValue;
            decimal tvaMultiplier = tvaRate * 0.01m;
            decimal tvaAmount = totalAfterRemise * tvaMultiplier;
            decimal totalWithTVA = totalAfterRemise + tvaAmount;

            if (txtTVAAmount != null)
                txtTVAAmount.Text = tvaAmount.ToString("0.00") + " DH";

            if (txtApresTVAAmount != null)
                txtApresTVAAmount.Text = (total + tvaAmount).ToString("0.00") + " DH";

            if (txtApresRemiseAmount != null)
                txtApresRemiseAmount.Text = totalWithTVA.ToString("0.00") + " DH";
        }

        private void Remise_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox == null || string.IsNullOrWhiteSpace(textBox.Text))
            {
                // Only recalculate if UI is ready
                if (txtTotalAmount != null && txtTVAAmount != null)
                {
                    RecalculateTotals();
                }
                return;
            }

            string cleanedText = CleanNumericInput(textBox.Text);

            if (!decimal.TryParse(cleanedText, out decimal remiseValue))
                return;

            if (remiseValue > currentTotalHT)
            {
                if (textBox.Text.Length > 0)
                {
                    int caretPosition = textBox.CaretIndex;
                    textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1);
                    textBox.CaretIndex = Math.Min(caretPosition, textBox.Text.Length);
                }
                return;
            }

            // Only recalculate if UI is ready
            if (txtTotalAmount != null && txtTVAAmount != null)
            {
                RecalculateTotals();
            }
        }

        private string CleanNumericInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "0";

            string cleaned = new string(input.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());
            cleaned = cleaned.Replace(',', '.');

            int firstDecimalIndex = cleaned.IndexOf('.');
            if (firstDecimalIndex != -1)
            {
                cleaned = cleaned.Substring(0, firstDecimalIndex + 1) +
                          cleaned.Substring(firstDecimalIndex + 1).Replace(".", "");
            }

            return string.IsNullOrWhiteSpace(cleaned) ? "0" : cleaned;
        }

        private void IntegerTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void DecimalTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
            {
                e.Handled = true;
                return;
            }

            if (e.Text == "." || e.Text == ",")
            {
                e.Handled = textBox.Text.Contains(".") || textBox.Text.Contains(",");
            }
            else
            {
                e.Handled = !e.Text.All(char.IsDigit);
            }
        }

        private void IntegerTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!text.All(char.IsDigit))
                    e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void DecimalTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                var textBox = sender as TextBox;

                int dotCount = text.Count(c => c == '.' || c == ',');
                bool hasExistingDot = textBox?.Text.Contains(".") == true || textBox?.Text.Contains(",") == true;

                if ((dotCount > 1) || (dotCount == 1 && hasExistingDot) || text.Any(c => !char.IsDigit(c) && c != '.' && c != ','))
                    e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Facture facture = new Facture
                {
                    Name = txtUserName.Text,
                    ICE = txtUserICE.Text,
                    VAT = txtUserVAT.Text,
                    Telephone = txtUserPhone.Text,
                    Adresse = txtUserAddress.Text,
                    CompanyId = txtClientIdSociete.Text,
                    EtatJuridic = txtClientEtatJuridique.Text,
                    SiegeEntreprise = cmbClientSiegeEntreprise.Text,
                    LogoPath = txtLogoPath.Text
                };

                await facture.InsertOrUpdateFactureAsync();

                WCongratulations wCongratulations = new WCongratulations("Informations saved successfully!", "", 1);
                wCongratulations.ShowDialog();
            }
            catch (Exception ex)
            {
                WCongratulations wCongratulations = new WCongratulations($"Error: {ex.Message}\r\nInformations not saved!", "", 0);
                wCongratulations.ShowDialog();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            main.load_main(u);
        }

        private void BtnBrowseLogo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "Sélectionner le logo de l'entreprise"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                txtLogoPath.Text = openFileDialog.FileName;
            }
        }

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            if (OperationContainer.Children.Count == 0)
            {
                MessageBox.Show("There is no operation selected", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string dateValue = dpInvoiceDate?.SelectedDate?.ToString("dd/MM/yyyy") ?? DateTime.Now.ToString("dd/MM/yyyy");

                Dictionary<string, string> FactureInfo = new Dictionary<string, string>()
                {
                    { "NFacture", txtInvoiceNumber?.Text ?? "" },
                    { "Date", dateValue },
                    { "Type", cmbInvoiceType?.Text ?? "" },
                    { "NomU", txtUserName?.Text ?? "" },
                    { "ICEU", txtUserICE?.Text ?? "" },
                    { "VATU", txtUserVAT?.Text ?? "" },
                    { "TelephoneU", txtUserPhone?.Text ?? "" },
                    { "EtatJuridiqueU", txtUserEtatJuridique?.Text ?? "" },
                    { "IdSocieteU", txtUserIdSociete?.Text ?? "" },
                    { "SiegeEntrepriseU", cmbUserSiegeEntreprise?.Text ?? "" },
                    { "AdressU", txtUserAddress?.Text ?? "" },
                    { "NomC", txtClientName?.Text ?? "" },
                    { "ICEC", txtClientICE?.Text ?? "" },
                    { "VATC", txtClientVAT?.Text ?? "" },
                    { "TelephoneC", txtClientPhone?.Text ?? "" },
                    { "EtatJuridiqueC", txtClientEtatJuridique?.Text ?? "" },
                    { "IdSocieteC", txtClientIdSociete?.Text ?? "" },
                    { "SiegeEntrepriseC", cmbClientSiegeEntreprise?.Text ?? "" },
                    { "AdressC", txtClientAddress?.Text ?? "" },
                    { "EtatFature", EtatFacture?.Text ?? "" },
                    { "Device", txtCurrency?.Text ?? "" },
                    { "TVA", txtTVARate?.Text ?? "" },
                    { "MontantTotal", txtTotalAmount?.Text ?? "" },
                    { "MontantTVA", txtTVAAmount?.Text ?? "" },
                    { "MontantApresTVA", txtApresTVAAmount?.Text ?? "" },
                    { "MontantApresRemise", txtApresRemiseAmount?.Text ?? "" },
                    { "IndexDeFacture", IndexFacture?.Text ?? "" },
                    { "Description", txtDescription?.Text ?? "" },
                    { "Logo", txtLogoPath?.Text ?? "" },
                    { "Reversed", EtatFacture?.Text ?? "" },
                    { "Remise", Remise?.Text ?? "" }
                };

                // Pass FILTERED articles (excluding quantity 0) to WFacturePage for display
                WFacturePage wFacturePage = new WFacturePage(this, FactureInfo, GetFilteredInvoiceArticles());
                wFacturePage.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating preview: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtInvoiceNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void btnGenerateInvoiceNumber_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string year = DateTime.Now.Year.ToString();
                string month = DateTime.Now.Month.ToString("D2");
                string day = DateTime.Now.Day.ToString("D2");

                Random random = new Random();
                int randomNumber = random.Next(10000, 99999);

                string invoiceNumber = $"FAC-{year}{month}{day}-{randomNumber}";

                txtInvoiceNumber.Text = invoiceNumber;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating invoice number: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EtatFacture_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Only recalculate if UI is ready
            if (txtTotalAmount != null && txtTVAAmount != null)
            {
                RecalculateTotals();
            }
        }

        private void cmbInvoiceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            OperationContainer?.Children.Clear();
            SelectedOperations.Clear();
            InvoiceArticles.Clear();

            if (comboBox == null || comboBox.SelectedItem == null)
                return;

            ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                InvoiceType = selectedItem.Content.ToString();
            }
        }
    }
}