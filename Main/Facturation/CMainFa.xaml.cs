using Microsoft.Win32;
using Superete;
using Superete.Main.Facturation;
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

namespace GestionComerce.Main.Facturation
{
    /// <summary>
    /// Interaction logic for CMainFa.xaml
    /// </summary>
    public partial class CMainFa : UserControl
    {
        private const int ETAT_FACTURE_NORMAL = 0;
        private const int ETAT_FACTURE_REVERSED = 1;

        public MainWindow main;
        User u;
        private decimal currentTotalHT = 0; // Store current HT amount for validation

        public CMainFa(User u, MainWindow main, Operation op)
        {
            InitializeComponent();
            this.main = main;
            this.u = u;

            if (op != null)
            {
                InitializeWithOperation(op);
            }

            LoadFacture();
        }

        private void InitializeWithOperation(Operation op)
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

                // Check if any operation articles are reversed
                foreach (OperationArticle oa in main.loa)
                {
                    if (oa.OperationID == op.OperationID && oa.Reversed == true)
                    {
                        EtatFacture.IsEnabled = true;
                        break;
                    }
                }
            }

            // Display operation details
            OperationContainer.Children.Clear();
            CSingleOperation cSingleOperation = new CSingleOperation(this, null, op);
            OperationContainer.Children.Add(cSingleOperation);
            txtTotalAmount.Text = op.PrixOperation.ToString("0.00") + " DH";
            Remise.Text = op.Remise.ToString("0.00");
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

                // Store current HT amount for Remise validation
                currentTotalHT = total;

                // Get remise value
                decimal remiseValue = 0;
                if (Remise != null && !string.IsNullOrWhiteSpace(Remise.Text))
                {
                    string cleanedRemise = CleanNumericInput(Remise.Text);
                    decimal.TryParse(cleanedRemise, out remiseValue);
                }

                // Calculate total after remise
                decimal totalAfterRemise = total - remiseValue;

                decimal tvaMultiplier = tvaRate * 0.01m;
                decimal tvaAmount = totalAfterRemise * tvaMultiplier;
                decimal totalWithTVA = totalAfterRemise + tvaAmount;

                txtTVAAmount.Text = tvaAmount.ToString("0.00") + " DH";
                txtApresTVAAmount.Text = (total+tvaAmount).ToString("0.00") + " DH";

                // Update Montant Total TTC apres Remise if exists
                if (txtApresRemiseAmount != null)
                {
                    txtApresRemiseAmount.Text = totalWithTVA.ToString("0.00") + " DH";
                }
            }
            catch (Exception ex)
            {
                // Silently handle conversion errors to avoid UI disruption
                System.Diagnostics.Debug.WriteLine($"Error in txtTotalAmount_TextChanged: {ex.Message}");
            }
        }

        private void txtTVARate_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Early exit if textbox is null or empty
            if (textBox == null || string.IsNullOrWhiteSpace(textBox.Text))
                return;

            // Remove any non-numeric characters except decimal point
            string cleanedText = CleanNumericInput(textBox.Text);

            // Try to parse the TVA rate
            if (!decimal.TryParse(cleanedText, out decimal tvaRate))
                return;

            // Validate TVA rate is not greater than 100
            if (tvaRate > 100)
            {
                // Remove last character and update text
                if (textBox.Text.Length > 0)
                {
                    int caretPosition = textBox.CaretIndex;
                    textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1);
                    textBox.CaretIndex = Math.Min(caretPosition, textBox.Text.Length);
                }
                return;
            }

            // Check if total amount is available
            if (txtTotalAmount == null || string.IsNullOrWhiteSpace(txtTotalAmount.Text))
                return;

            // Clean and parse total amount
            string cleanedTotal = CleanNumericInput(txtTotalAmount.Text);
            if (!decimal.TryParse(cleanedTotal, out decimal total))
                return;

            // Get remise value
            decimal remiseValue = 0;
            if (Remise != null && !string.IsNullOrWhiteSpace(Remise.Text))
            {
                string cleanedRemise = CleanNumericInput(Remise.Text);
                decimal.TryParse(cleanedRemise, out remiseValue);
            }

            // Calculate total after remise
            decimal totalAfterRemise = total - remiseValue;

            // Calculate TVA amounts
            decimal tvaMultiplier = tvaRate * 0.01m;
            decimal tvaAmount = totalAfterRemise * tvaMultiplier;
            decimal totalWithTVA = totalAfterRemise + tvaAmount;

            // Update display fields
            if (txtTVAAmount != null)
                txtTVAAmount.Text = tvaAmount.ToString("0.00") + " DH";

            if (txtApresTVAAmount != null)
                txtApresTVAAmount.Text = (total+tvaAmount).ToString("0.00") + " DH";

            // Update Montant Total TTC apres Remise if exists
            if (txtApresRemiseAmount != null)
                txtApresRemiseAmount.Text = totalWithTVA.ToString("0.00") + " DH";
        }

        private void Remise_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Early exit if textbox is null or empty
            if (textBox == null || string.IsNullOrWhiteSpace(textBox.Text))
            {
                // Recalculate without remise when empty
                RecalculateWithRemise(0);
                return;
            }

            // Remove any non-numeric characters except decimal point
            string cleanedText = CleanNumericInput(textBox.Text);

            // Try to parse the remise value
            if (!decimal.TryParse(cleanedText, out decimal remiseValue))
                return;

            // Validate remise is not greater than total HT
            if (remiseValue > currentTotalHT)
            {
                // Remove last character and update text
                if (textBox.Text.Length > 0)
                {
                    int caretPosition = textBox.CaretIndex;
                    textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1);
                    textBox.CaretIndex = Math.Min(caretPosition, textBox.Text.Length);
                }
                return;
            }

            // Recalculate with the new remise value
            RecalculateWithRemise(remiseValue);
        }

        private void RecalculateWithRemise(decimal remiseValue)
        {
            // Check if total amount and TVA rate are available
            if (txtTotalAmount == null || string.IsNullOrWhiteSpace(txtTotalAmount.Text))
                return;

            if (txtTVARate == null || string.IsNullOrWhiteSpace(txtTVARate.Text))
                return;

            try
            {
                // Clean and parse total amount
                string cleanedTotal = CleanNumericInput(txtTotalAmount.Text);
                if (!decimal.TryParse(cleanedTotal, out decimal total))
                    return;

                // Clean and parse TVA rate
                string cleanedTVA = CleanNumericInput(txtTVARate.Text);
                if (!decimal.TryParse(cleanedTVA, out decimal tvaRate))
                    return;

                // Calculate total after remise
                decimal totalAfterRemise = total - remiseValue;

                // Calculate TVA amounts
                decimal tvaMultiplier = tvaRate * 0.01m;
                decimal tvaAmount = totalAfterRemise * tvaMultiplier;
                decimal totalWithTVA = totalAfterRemise + tvaAmount;

                // Update display fields
                if (txtTVAAmount != null)
                    txtTVAAmount.Text = tvaAmount.ToString("0.00") + " DH";

                if (txtApresTVAAmount != null)
                    txtApresTVAAmount.Text = (total + tvaAmount).ToString("0.00") + " DH";

                // Update Montant Total TTC apres Remise if exists
                if (txtApresRemiseAmount != null)
                    txtApresRemiseAmount.Text = totalWithTVA.ToString("0.00") + " DH";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in RecalculateWithRemise: {ex.Message}");
            }
        }

        /// <summary>
        /// Removes non-numeric characters from input, keeping only digits and decimal point
        /// </summary>
        private string CleanNumericInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "0";

            // Remove "DH" and other non-numeric characters, keep digits and decimal point
            string cleaned = new string(input.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());

            // Replace comma with period for decimal parsing
            cleaned = cleaned.Replace(',', '.');

            // Handle multiple decimal points - keep only the first one
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
            // Allow only digits
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

            // Allow digits and a single decimal point
            if (e.Text == "." || e.Text == ",")
            {
                // Reject if already has a decimal point
                e.Handled = textBox.Text.Contains(".") || textBox.Text.Contains(",");
            }
            else
            {
                e.Handled = !e.Text.All(char.IsDigit);
            }
        }

        // Prevent pasting of invalid text
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

                // Allow a single decimal point and the rest digits
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
                // Format date to only include the date part (no time)
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

                WFacturePage wFacturePage = new WFacturePage(this, FactureInfo);
                wFacturePage.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating preview: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtInvoiceNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Placeholder for future functionality
        }

        private void btnGenerateInvoiceNumber_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Generate a random invoice number with format: FAC-YYYYMMDD-XXXXX
                // Example: FAC-20241109-12345

                string year = DateTime.Now.Year.ToString();
                string month = DateTime.Now.Month.ToString("D2");
                string day = DateTime.Now.Day.ToString("D2");

                // Generate a random 5-digit number
                Random random = new Random();
                int randomNumber = random.Next(10000, 99999);

                // Combine to create invoice number
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
            // Get the CSingleOperation control from the container
            var singleOperation = OperationContainer?.Children.OfType<CSingleOperation>().FirstOrDefault();
            if (singleOperation == null)
                return;

            // Recalculate and update amounts based on the new selection
            RecalculateAmounts(singleOperation.op);
        }

        private void RecalculateAmounts(Operation op)
        {
            if (op == null || main?.loa == null || main?.la == null)
                return;

            decimal prixOp = 0;
            decimal prixOpR = 0;
            decimal tvaAmount = 0;
            decimal tvaAmountR = 0;
            bool hasReversedItems = false;
            bool hasNormalItems = false;

            if (op.Reversed == true)
            {
                // Calculate reversed operation totals
                foreach (OperationArticle oa in main.loa)
                {
                    if (oa.OperationID == op.OperationID)
                    {
                        var article = main.la.FirstOrDefault(a => a.ArticleID == oa.ArticleID);
                        if (article != null)
                        {
                            prixOpR += article.PrixVente * oa.QteArticle;
                            tvaAmountR += (article.tva / 100) * (article.PrixVente * oa.QteArticle);
                            hasReversedItems = true;
                        }
                    }
                }
            }
            else
            {
                // Calculate normal operation totals
                foreach (OperationArticle oa in main.loa)
                {
                    if (oa.OperationID == op.OperationID)
                    {
                        var article = main.la.FirstOrDefault(a => a.ArticleID == oa.ArticleID);
                        if (article == null)
                            continue;

                        if (oa.Reversed == true)
                        {
                            prixOpR += article.PrixVente * oa.QteArticle;
                            tvaAmountR += (article.tva / 100) * (article.PrixVente * oa.QteArticle);
                            hasReversedItems = true;
                        }
                        else
                        {
                            prixOp += article.PrixVente * oa.QteArticle;
                            tvaAmount += (article.tva / 100) * (article.PrixVente * oa.QteArticle);
                            hasNormalItems = true;
                        }
                    }
                }
            }

            // Enable Remise only when there are BOTH reversed AND normal items
            if (hasReversedItems && hasNormalItems)
            {
                Remise.IsEnabled = true;
            }
            else
            {
                Remise.IsEnabled = false;
                Remise.Text = "";
            }

            // Update the displayed amounts based on the selected state
            if (!hasReversedItems)
            {
                // No reversed items
                txtTotalAmount.Text = prixOp.ToString("0.00") + " DH";
                txtTVAAmount.Text = tvaAmount.ToString("0.00") + " DH";

                decimal tvaPercentage = prixOp > 0 ? (tvaAmount / prixOp) * 100 : 0;
                txtTVARate.Text = tvaPercentage.ToString("0.00");
                currentTotalHT = prixOp;
            }
            else if (!hasNormalItems)
            {
                // Only reversed items
                txtTotalAmount.Text = prixOpR.ToString("0.00") + " DH";
                txtTVAAmount.Text = tvaAmountR.ToString("0.00") + " DH";

                decimal tvaPercentageR = prixOpR > 0 ? (tvaAmountR / prixOpR) * 100 : 0;
                txtTVARate.Text = tvaPercentageR.ToString("0.00");
                currentTotalHT = prixOpR;
            }
            else
            {
                // Has BOTH reversed AND normal items - display based on selected state
                if (EtatFacture.SelectedIndex == ETAT_FACTURE_REVERSED)
                {
                    currentTotalHT = prixOpR;
                    txtTotalAmount.Text = prixOpR.ToString("0.00") + " DH";
                    txtTVAAmount.Text = tvaAmountR.ToString("0.00") + " DH";

                    decimal tvaPercentageR = prixOpR > 0 ? (tvaAmountR / prixOpR) * 100 : 0;
                    txtTVARate.Text = tvaPercentageR.ToString("0.00");
                }
                else
                {
                    currentTotalHT = prixOp;
                    txtTotalAmount.Text = prixOp.ToString("0.00") + " DH";
                    txtTVAAmount.Text = tvaAmount.ToString("0.00") + " DH";

                    decimal tvaPercentage = prixOp > 0 ? (tvaAmount / prixOp) * 100 : 0;
                    txtTVARate.Text = tvaPercentage.ToString("0.00");
                }
            }
        }
    }
}