using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using GestionComerce.Main.Vente;

namespace GestionComerce.Vente
{
    /// <summary>
    /// Preview window showing thermal receipt/ticket format (80mm style)
    /// Shows how the configured settings will appear on actual receipt
    /// </summary>
    public partial class WFacturePreview : Window
    {
        private GestionComerce.FactureSettings settings;
        private bool isPreviewMode = false;

        // Constructor 1: Preview mode (from Settings)
        public WFacturePreview(GestionComerce.FactureSettings factureSettings)
        {
            InitializeComponent();
            this.settings = factureSettings;
            this.isPreviewMode = true;
            LoadTicketPreview();
        }

        // Constructor 2: Actual operation mode (from Sales)
        public WFacturePreview(
            GestionComerce.FactureSettings factureSettings,
            int operationId,
            DateTime operationDate,
            Client client,
            List<TicketArticleData> articles,
            decimal total,
            decimal remise,
            decimal credit,
            string paymentMethod,
            string operationType)
        {
            InitializeComponent();
            this.settings = factureSettings;
            this.isPreviewMode = false;
            LoadActualTicket(operationId, operationDate, client, articles, total, remise, credit, paymentMethod, operationType);
        }

        private void LoadTicketPreview()
        {
            try
            {
                // ===== COMPANY INFO =====
                txtCompanyName.Text = settings.CompanyName;
                txtCompanyAddress.Text = settings.CompanyAddress;
                txtCompanyPhone.Text = settings.CompanyPhone;
                txtCompanyEmail.Text = settings.CompanyEmail;

                // ===== LOGO =====
                if (!string.IsNullOrEmpty(settings.LogoPath) && File.Exists(settings.LogoPath))
                {
                    try
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(settings.LogoPath);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        imgLogo.Source = bitmap;
                        imgLogo.Visibility = Visibility.Visible;
                    }
                    catch
                    {
                        imgLogo.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    imgLogo.Visibility = Visibility.Collapsed;
                }

                // ===== INVOICE NUMBER & DATE =====
                txtInvoiceNumber.Text = $"{settings.InvoicePrefix}000001";
                txtInvoiceDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                // ===== CALCULATE SAMPLE TOTALS =====
                // Sample articles total: 100 + 75 + 75 = 250.00 DH
                decimal subtotal = 250.00m;
                decimal remise = 10.00m;
                decimal subtotalAfterRemise = subtotal - remise; // 240.00 DH

                // Calculate TAX based on configured percentage
                decimal taxAmount = subtotalAfterRemise * (settings.TaxPercentage / 100);
                decimal total = subtotalAfterRemise + taxAmount;

                // Display tax with percentage
                txtTaxLabel.Text = $"TVA ({settings.TaxPercentage:0.##}%):";
                txtTax.Text = $"{taxAmount:N2} DH";
                txtTotal.Text = $"{total:N2} DH";

                // ===== TERMS AND CONDITIONS =====
                if (!string.IsNullOrWhiteSpace(settings.TermsAndConditions))
                {
                    // For ticket format, show shortened version if too long
                    string terms = settings.TermsAndConditions;
                    if (terms.Length > 150)
                    {
                        terms = terms.Substring(0, 147) + "...";
                    }
                    txtTermsAndConditions.Text = terms;
                    txtTermsAndConditions.Visibility = Visibility.Visible;
                }
                else
                {
                    txtTermsAndConditions.Visibility = Visibility.Collapsed;
                }

                // ===== FOOTER MESSAGE =====
                if (!string.IsNullOrWhiteSpace(settings.FooterText))
                {
                    txtFooter.Text = settings.FooterText;
                    txtFooter.Visibility = Visibility.Visible;
                }
                else
                {
                    txtFooter.Text = "MERCI DE VOTRE VISITE";
                    txtFooter.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de l'aperçu du ticket: {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadActualTicket(
            int operationId,
            DateTime operationDate,
            Client client,
            List<TicketArticleData> articles,
            decimal total,
            decimal remise,
            decimal credit,
            string paymentMethod,
            string operationType)
        {
            try
            {
                // ===== COMPANY INFO =====
                txtCompanyName.Text = settings.CompanyName;
                txtCompanyAddress.Text = settings.CompanyAddress;
                txtCompanyPhone.Text = settings.CompanyPhone;
                txtCompanyEmail.Text = settings.CompanyEmail;

                // ===== LOGO =====
                if (!string.IsNullOrEmpty(settings.LogoPath) && File.Exists(settings.LogoPath))
                {
                    try
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(settings.LogoPath);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        imgLogo.Source = bitmap;
                        imgLogo.Visibility = Visibility.Visible;
                    }
                    catch
                    {
                        imgLogo.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    imgLogo.Visibility = Visibility.Collapsed;
                }

                // ===== INVOICE NUMBER & DATE =====
                string invoiceNumber = operationId.ToString().PadLeft(6, '0');
                txtInvoiceNumber.Text = $"{settings.InvoicePrefix}{invoiceNumber}";
                txtInvoiceDate.Text = operationDate.ToString("dd/MM/yyyy HH:mm");

                // ===== CLEAR EXISTING SAMPLE ARTICLES AND ADD REAL ONES =====
                // Find the StackPanel in the ticket area that contains articles
                StackPanel ticketStackPanel = FindTicketStackPanel(ticketArea);
                if (ticketStackPanel != null)
                {
                    // Remove sample articles (keep only header elements)
                    RemoveSampleArticles(ticketStackPanel);

                    // Add real articles dynamically
                    int articleInsertIndex = FindArticleInsertIndex(ticketStackPanel);
                    foreach (var article in articles)
                    {
                        Grid articleGrid = CreateArticleGrid(article);
                        ticketStackPanel.Children.Insert(articleInsertIndex++, articleGrid);
                    }
                }

                // ===== CALCULATE TOTALS =====
                decimal subtotal = total;
                decimal subtotalAfterRemise = subtotal - remise;
                decimal taxAmount = subtotalAfterRemise * (settings.TaxPercentage / 100);
                decimal finalTotal = subtotalAfterRemise + taxAmount;

                // Display tax with percentage
                txtTaxLabel.Text = $"TVA ({settings.TaxPercentage:0.##}%):";
                txtTax.Text = $"{taxAmount:N2} DH";
                txtTotal.Text = $"{finalTotal:N2} DH";

                // ===== UPDATE TOTALS IN THE TICKET =====
                UpdateTotalInTicket(ticketStackPanel, "SOUS-TOTAL:", $"{subtotal:N2} DH");
                if (remise > 0)
                {
                    UpdateTotalInTicket(ticketStackPanel, "REMISE:", $"-{remise:N2} DH");
                }
                else
                {
                    // Hide remise line if 0
                    HideRemiseLine(ticketStackPanel);
                }

                // ===== CLIENT INFO =====
                if (client != null)
                {
                    UpdateClientInfo(ticketStackPanel, $"Client: {client.Nom}");
                }
                else
                {
                    UpdateClientInfo(ticketStackPanel, "Client: Vente Sans Client");
                }

                // ===== PAYMENT METHOD =====
                UpdatePaymentMethod(ticketStackPanel, $"Mode de paiement: {paymentMethod}");

                // ===== CREDIT INFO (if applicable) =====
                if (credit > 0)
                {
                    AddCreditInfo(ticketStackPanel, credit, finalTotal);
                }

                // ===== OPERATION TYPE =====
                // You can add operation type indicator if needed

                // ===== TERMS AND CONDITIONS =====
                if (!string.IsNullOrWhiteSpace(settings.TermsAndConditions))
                {
                    string terms = settings.TermsAndConditions;
                    if (terms.Length > 150)
                    {
                        terms = terms.Substring(0, 147) + "...";
                    }
                    txtTermsAndConditions.Text = terms;
                    txtTermsAndConditions.Visibility = Visibility.Visible;
                }
                else
                {
                    txtTermsAndConditions.Visibility = Visibility.Collapsed;
                }

                // ===== FOOTER MESSAGE =====
                if (!string.IsNullOrWhiteSpace(settings.FooterText))
                {
                    txtFooter.Text = settings.FooterText;
                }
                else
                {
                    txtFooter.Text = "MERCI DE VOTRE VISITE";
                }

                // ===== UPDATE BARCODE =====
                UpdateBarcode(ticketStackPanel, invoiceNumber);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement du ticket: {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Helper methods to manipulate the ticket dynamically
        private StackPanel FindTicketStackPanel(Border ticketBorder)
        {
            if (ticketBorder.Child is StackPanel sp)
                return sp;
            return null;
        }

        private void RemoveSampleArticles(StackPanel stackPanel)
        {
            // Remove the 3 sample article grids (they come after the dotted line following the header)
            List<UIElement> toRemove = new List<UIElement>();
            bool foundFirstDottedLine = false;
            int articleCount = 0;

            foreach (UIElement element in stackPanel.Children)
            {
                if (element is TextBlock tb && tb.Text.Contains("- - - -") && !foundFirstDottedLine)
                {
                    foundFirstDottedLine = true;
                    continue;
                }

                if (foundFirstDottedLine && element is Grid && articleCount < 3)
                {
                    toRemove.Add(element);
                    articleCount++;
                }

                if (articleCount >= 3)
                    break;
            }

            foreach (var element in toRemove)
            {
                stackPanel.Children.Remove(element);
            }
        }

        private int FindArticleInsertIndex(StackPanel stackPanel)
        {
            // Find the first dotted line, articles should be inserted after it
            for (int i = 0; i < stackPanel.Children.Count; i++)
            {
                if (stackPanel.Children[i] is TextBlock tb && tb.Text.Contains("- - - -"))
                {
                    return i + 1;
                }
            }
            return stackPanel.Children.Count;
        }

        private Grid CreateArticleGrid(TicketArticleData article)
        {
            Grid articleGrid = new Grid { Margin = new Thickness(0, 0, 0, 8) };
            articleGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            articleGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Article name on first row
            TextBlock nameBlock = new TextBlock
            {
                Text = article.ArticleName,
                FontSize = 10,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 2)
            };
            Grid.SetRow(nameBlock, 0);
            articleGrid.Children.Add(nameBlock);

            // Quantity, price, total on second row
            Grid detailsGrid = new Grid();
            detailsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            detailsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
            detailsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
            detailsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(70) });

            TextBlock qtyBlock = new TextBlock { Text = article.Quantity.ToString(), FontSize = 10, TextAlignment = TextAlignment.Center };
            Grid.SetColumn(qtyBlock, 1);

            TextBlock priceBlock = new TextBlock { Text = article.UnitPrice.ToString("N2"), FontSize = 10, TextAlignment = TextAlignment.Right };
            Grid.SetColumn(priceBlock, 2);

            TextBlock totalBlock = new TextBlock { Text = article.Total.ToString("N2"), FontSize = 10, FontWeight = FontWeights.SemiBold, TextAlignment = TextAlignment.Right };
            Grid.SetColumn(totalBlock, 3);

            detailsGrid.Children.Add(qtyBlock);
            detailsGrid.Children.Add(priceBlock);
            detailsGrid.Children.Add(totalBlock);

            Grid.SetRow(detailsGrid, 1);
            articleGrid.Children.Add(detailsGrid);

            return articleGrid;
        }

        private void UpdateTotalInTicket(StackPanel stackPanel, string label, string value)
        {
            foreach (UIElement element in stackPanel.Children)
            {
                if (element is Grid grid && grid.ColumnDefinitions.Count == 2)
                {
                    if (grid.Children.Count >= 2 && grid.Children[0] is TextBlock tb && tb.Text == label)
                    {
                        if (grid.Children[1] is TextBlock valueBlock)
                        {
                            valueBlock.Text = value;
                            return;
                        }
                    }
                }
            }
        }

        private void HideRemiseLine(StackPanel stackPanel)
        {
            foreach (UIElement element in stackPanel.Children)
            {
                if (element is Grid grid && grid.ColumnDefinitions.Count == 2)
                {
                    if (grid.Children.Count >= 2 && grid.Children[0] is TextBlock tb && tb.Text == "REMISE:")
                    {
                        grid.Visibility = Visibility.Collapsed;
                        return;
                    }
                }
            }
        }

        private void UpdateClientInfo(StackPanel stackPanel, string clientText)
        {
            foreach (UIElement element in stackPanel.Children)
            {
                if (element is TextBlock tb && tb.Text.StartsWith("Client:"))
                {
                    tb.Text = clientText;
                    return;
                }
            }
        }

        private void UpdatePaymentMethod(StackPanel stackPanel, string paymentText)
        {
            foreach (UIElement element in stackPanel.Children)
            {
                if (element is TextBlock tb && tb.Text.StartsWith("Mode de paiement:"))
                {
                    tb.Text = paymentText;
                    return;
                }
            }
        }

        private void AddCreditInfo(StackPanel stackPanel, decimal credit, decimal total)
        {
            // Find the payment method line and add credit info after it
            int paymentIndex = -1;
            for (int i = 0; i < stackPanel.Children.Count; i++)
            {
                if (stackPanel.Children[i] is TextBlock tb && tb.Text.StartsWith("Mode de paiement:"))
                {
                    paymentIndex = i;
                    break;
                }
            }

            if (paymentIndex >= 0)
            {
                TextBlock creditBlock = new TextBlock
                {
                    Text = $"Crédit: {credit:N2} DH | Payé: {(total - credit):N2} DH",
                    FontSize = 10,
                    TextAlignment = TextAlignment.Center,
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.Red,
                    Margin = new Thickness(0, 5, 0, 10)
                };
                stackPanel.Children.Insert(paymentIndex + 1, creditBlock);
            }
        }

        private void UpdateBarcode(StackPanel stackPanel, string invoiceNumber)
        {
            foreach (UIElement element in stackPanel.Children)
            {
                if (element is TextBlock tb && tb.Text.StartsWith("*"))
                {
                    tb.Text = $"* {settings.InvoicePrefix}{invoiceNumber} *";
                    return;
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}