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
    /// Logique d'interaction pour CMainV.xaml
    /// </summary>
    public partial class CMainV : UserControl
    {
        private StringBuilder quantityBuilder = new StringBuilder("1");
        private StringBuilder barcodeBuilder = new StringBuilder();
        private DateTime lastKeystroke = DateTime.Now;

        public CMainV(User u, List<Famille> lf, List<User> lu, List<Role> lr, MainWindow main, List<Article> la, List<Fournisseur> lfo)
        {
            InitializeComponent();

            // Set focus to the UserControl
            this.Focusable = true;
            this.Loaded += (s, e) => { this.Focus(); };

            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
            CurrentUser.Text = u.UserName;
            this.u = u;
            this.lf = lf;
            this.la = la;
            this.lfo = lfo;
            this.main = main;
            FamillyContainer.Children.Clear();

            foreach (Famille famille in lf)
            {
                CSingleFamilly f = new CSingleFamilly(famille, this, lf);
                FamillyContainer.Children.Add(f);
            }
            LoadArticles(la);
            LoadPayments(main.lp);
            foreach (Role r in lr)
            {
                if (u.RoleID == r.RoleID)
                {
                    if (r.Ticket == false)
                    {
                        Ticket.IsChecked = false;
                        Ticket.IsEnabled = false;
                    }
                    if (r.SolderClient == false)
                    {
                        HalfButton.IsEnabled = false;
                        CreditButton.IsEnabled = false;
                    }
                    if (r.CashClient == false)
                    {
                        CashButton.IsEnabled = false;
                    }
                    break;
                }
            }

            // Initialize empty cart state
            UpdateCartEmptyState();
        }

        public User u;
        List<Famille> lf;
        public List<Article> la;
        public MainWindow main;
        public decimal TotalNett = 0;
        public int NbrA = 0;
        public List<Fournisseur> lfo;

        // Method to update the empty cart state visibility
        public void UpdateCartEmptyState()
        {
            if (EmptyStateCart != null)
            {
                // Count actual article items (not the empty state itself)
                int itemCount = 0;
                foreach (UIElement element in SelectedArticles.Children)
                {
                    if (element is CSingleArticle2)
                    {
                        itemCount++;
                    }
                }

                // Show empty state only when cart is empty
                EmptyStateCart.Visibility = itemCount == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - lastKeystroke;

            if (elapsed.TotalMilliseconds > 100)
            {
                barcodeBuilder.Clear();
            }

            lastKeystroke = DateTime.Now;

            if (e.Key == Key.Return)
            {
                string barcode = barcodeBuilder.ToString();
                OnBarcodeScanned(barcode);
                barcodeBuilder.Clear();
                e.Handled = true;
            }
            else
            {
                // Convert key to character
                string key = new KeyConverter().ConvertToString(e.Key);
                if (!string.IsNullOrEmpty(key) && key.Length == 1)
                {
                    barcodeBuilder.Append(key);
                }
            }
        }

        private void OnBarcodeScanned(string barcode)
        {
            // Search for the article with this barcode
            Article foundArticle = la.FirstOrDefault(a => a.Code.ToString() == barcode);

            if (foundArticle != null)
            {
                // Check if article is in stock
                if (foundArticle.Quantite <= 0)
                {
                    MessageBox.Show($"L'article '{foundArticle.ArticleName}' est en rupture de stock.");
                    return;
                }

                // Display the article in the SelectedArticle container (top preview)
                CSingleArticle1 previewArticle = new CSingleArticle1(foundArticle, this, lf, lfo, 0);
                SelectedArticle.Child = previewArticle;

                // Set the quantity to 1 for the scanned article
                ArticleQuantity.Text = "1";

                // Check if article already exists in cart
                foreach (UIElement element in SelectedArticles.Children)
                {
                    if (element is CSingleArticle2 item)
                    {
                        if (item.a.ArticleID == foundArticle.ArticleID)
                        {
                            if (item.qte + 1 > foundArticle.Quantite)
                            {
                                MessageBox.Show("La quantité en stock est insuffisante.");
                            }
                            else
                            {
                                item.Quantite.Text = (item.qte + 1).ToString();
                                item.qte += 1;
                                TotalNett += foundArticle.PrixVente;
                                TotalNet.Text = TotalNett.ToString("F2") + " DH";
                                NbrA += 1;
                                ArticleCount.Text = NbrA.ToString();
                            }
                            return;
                        }
                    }
                }

                // Add new article to cart
                TotalNett += foundArticle.PrixVente;
                TotalNet.Text = TotalNett.ToString("F2") + " DH";
                NbrA += 1;
                ArticleCount.Text = NbrA.ToString();
                CSingleArticle2 sa = new CSingleArticle2(foundArticle, 1, this);
                SelectedArticles.Children.Add(sa);
                UpdateCartEmptyState();
            }
            else
            {
                MessageBox.Show($"Aucun article trouvé avec le code: {barcode}");
            }
        }

        public void LoadArticles(List<Article> la)
        {
            ArticlesContainer.Children.Clear();
            foreach (Article a in la)
            {
                CSingleArticle1 ar = new CSingleArticle1(a, this, lf, lfo, 0);
                ArticlesContainer.Children.Add(ar);
            }
        }

        public void LoadPayments(List<PaymentMethod> lp)
        {
            PaymentMethodComboBox.Items.Clear();
            foreach (PaymentMethod a in lp)
            {
                PaymentMethodComboBox.Items.Add(a.PaymentMethodName);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            main.load_main(u);
        }

        private void MyBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WKeyPad wKeyPad = new WKeyPad(this);
            wKeyPad.ShowDialog();
        }

        private void KeyboardButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedArticle.Child is CSingleArticle1 sa1)
            {
                if (ArticleQuantity.Text == "")
                {
                    MessageBox.Show("Veuillez insérer une quantité");
                    return;
                }
                else if (Convert.ToInt32(ArticleQuantity.Text) == 0)
                {
                    MessageBox.Show("Veuillez insérer une quantité");
                    return;
                }
                else if (Convert.ToInt32(ArticleQuantity.Text) > sa1.a.Quantite)
                {
                    MessageBox.Show("La quantité insérée est plus grande que la quantité en stock");
                }
                else
                {
                    foreach (UIElement element in SelectedArticles.Children)
                    {
                        if (element is CSingleArticle2 item)
                        {
                            if (item.a.ArticleID == sa1.a.ArticleID)
                            {
                                if (Convert.ToInt32(ArticleQuantity.Text) + Convert.ToInt32(item.Quantite.Text) > sa1.a.Quantite)
                                {
                                    MessageBox.Show("La quantité dans le panier plus la quantité que vous voulez ajouter est plus grande que la quantité en stock");
                                }
                                else
                                {
                                    item.Quantite.Text = (Convert.ToInt32(ArticleQuantity.Text) + item.qte).ToString();
                                    item.qte += Convert.ToInt32(ArticleQuantity.Text);
                                    TotalNett += sa1.a.PrixVente * Convert.ToInt32(ArticleQuantity.Text);
                                    TotalNet.Text = TotalNett.ToString("F2") + " DH";
                                    NbrA += Convert.ToInt32(ArticleQuantity.Text);
                                    ArticleCount.Text = NbrA.ToString();
                                }
                                return;
                            }
                        }
                    }

                    TotalNett += sa1.a.PrixVente * Convert.ToInt32(ArticleQuantity.Text);
                    TotalNet.Text = TotalNett.ToString("F2") + " DH";
                    NbrA += Convert.ToInt32(ArticleQuantity.Text);
                    ArticleCount.Text = NbrA.ToString();
                    CSingleArticle2 sa = new CSingleArticle2(sa1.a, Convert.ToInt32(ArticleQuantity.Text), this);
                    SelectedArticles.Children.Add(sa);
                    UpdateCartEmptyState();
                    SelectedArticle.Child = new TextBlock
                    {
                        Name = "DesignationText",
                        Text = "Aucun produit sélectionné",
                        FontFamily = new FontFamily("Segoe UI"),
                        FontSize = 13,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#78350F")),
                        TextWrapping = TextWrapping.Wrap
                    };
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un article.");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedArticles.Children.Clear();
            TotalNett = 0;
            NbrA = 0;
            TotalNet.Text = "0.00 DH";
            ArticleCount.Text = "0";
            UpdateCartEmptyState();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ArticlesContainer.Children.Clear();
            foreach (Article a in la)
            {
                CSingleArticle1 ar = new CSingleArticle1(a, this, lf, lfo, 0);
                ArticlesContainer.Children.Add(ar);
            }
        }

        private void CodeBarreTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ArticlesContainer.Children.Clear();
            string code = sender is TextBox tb ? tb.Text : "";
            foreach (Article a in la)
            {
                if (a.Code.ToString().Contains(code) || a.ArticleName.ToLower().Contains(code.ToLower()))
                {
                    CSingleArticle1 ar = new CSingleArticle1(a, this, lf, lfo, 0);
                    ArticlesContainer.Children.Add(ar);
                }
            }
        }

        private async void CashButton_Click(object sender, RoutedEventArgs e)
        {
            int MethodID = 0;
            if (SelectedArticles.Children.Count == 0 || NbrA == 0)
            {
                MessageBox.Show("Aucun article sélectionné.");
                return;
            }
            else
            {
                if (PaymentMethodComboBox.Text == "")
                {
                    MessageBox.Show("Veuillez sélectionner un mode de paiement, si il n'y a aucune méthode de paiement, ajoutez-la depuis les paramètres");
                    return;
                }
                foreach (PaymentMethod p in main.lp)
                {
                    if (p.PaymentMethodName == PaymentMethodComboBox.SelectedValue.ToString())
                    {
                        MethodID = p.PaymentMethodID;
                    }
                }

                Client client = new Client();
                WSelectCient w = new WSelectCient(await client.GetClientsAsync(), this, 0, MethodID);
                w.ShowDialog();
            }
        }

        private async void HalfButton_Click(object sender, RoutedEventArgs e)
        {
            int MethodID = 0;
            if (SelectedArticles.Children.Count == 0 || NbrA == 0)
            {
                MessageBox.Show("Aucun article sélectionné.");
                return;
            }
            else
            {
                if (PaymentMethodComboBox.Text == "")
                {
                    MessageBox.Show("Veuillez sélectionner un mode de paiement, si il n'y a aucune méthode de paiement, ajoutez-la depuis les paramètres");
                    return;
                }
                foreach (PaymentMethod p in main.lp)
                {
                    if (p.PaymentMethodName == PaymentMethodComboBox.SelectedValue.ToString())
                    {
                        MethodID = p.PaymentMethodID;
                    }
                }
                Client client = new Client();
                WSelectCient w = new WSelectCient(await client.GetClientsAsync(), this, 1, MethodID);
                w.ShowDialog();
            }
        }

        private async void CreditButton_Click(object sender, RoutedEventArgs e)
        {
            int MethodID = 0;
            if (SelectedArticles.Children.Count == 0 || NbrA == 0)
            {
                MessageBox.Show("Aucun article sélectionné.");
                return;
            }
            else
            {
                if (PaymentMethodComboBox.Text == "")
                {
                    MessageBox.Show("Veuillez sélectionner un mode de paiement, si il n'y a aucune méthode de paiement, ajoutez-la depuis les paramètres");
                    return;
                }
                foreach (PaymentMethod p in main.lp)
                {
                    if (p.PaymentMethodName == PaymentMethodComboBox.SelectedValue.ToString())
                    {
                        MethodID = p.PaymentMethodID;
                    }
                }
                Client client = new Client();
                WSelectCient w = new WSelectCient(await client.GetClientsAsync(), this, 2, MethodID);
                w.ShowDialog();
            }
        }
    }
}