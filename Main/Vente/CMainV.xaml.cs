using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
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

namespace Superete.Main.Vente
{
    /// <summary>
    /// Logique d'interaction pour CMainV.xaml
    /// </summary>
    public partial class CMainV : UserControl
    {
        private StringBuilder quantityBuilder = new StringBuilder("1");

        public CMainV(User u, List<Famille> lf, List<User> lu, List<Role> lr, MainWindow main, List<Article> la, List<Fournisseur> lfo)
        {
            InitializeComponent();
            CurrentUser.Text = u.UserName;
            this.u = u;
            this.lf = lf;
            this.la = la;
            this.lfo = lfo;
            this.main = main;
            FamillyContainer.Children.Clear();
            foreach (Famille famille in lf)
            {
                CSingleFamilly f = new CSingleFamilly(famille, this,lf);
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
                    if (r.SolderClient == false) { 
                        HalfButton.IsEnabled = false;
                        CreditButton.IsEnabled = false;
                    }
                    break;
                }
            }
        }

        public User u;List<Famille> lf;public List<Article> la; public MainWindow main;public decimal TotalNett=0; public int NbrA = 0; public List<Fournisseur> lfo;
        public void LoadArticles(List<Article> la)
        {
            ArticlesContainer.Children.Clear();
            foreach (Article a in la)
            {
                CSingleArticle1 ar = new CSingleArticle1(a, this, lf, lfo,0);
                ArticlesContainer.Children.Add(ar);
            }
        }
        public void LoadPayments(List<PaymentMethod> lp)
        {
            PaymentMethodComboBox.Items.Clear();
            foreach(PaymentMethod a in lp)
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
                    MessageBox.Show("inserer une quantite");
                    return;
                } else if (Convert.ToInt32(ArticleQuantity.Text) == 0) {
                    MessageBox.Show("inserer une quantite");
                    return;
                } else if (Convert.ToInt32(ArticleQuantity.Text) > sa1.a.Quantite)
                {
                    MessageBox.Show("la quantite inserer est plus grande que la quantite en stock");
                }
                else
                {
                    MessageBox.Show(TotalNett.ToString());
                    foreach (CSingleArticle2 item in SelectedArticles.Children)
                    {
                        if (item.a.ArticleID == sa1.a.ArticleID)
                        {
                            if(Convert.ToInt32(ArticleQuantity.Text) + Convert.ToInt32(item.Quantite.Text)> sa1.a.Quantite)
                            {
                                MessageBox.Show("La quantite dans le panier plus la quantite vous voulez ajouter est plus grande que la quantite en stock");
                            }
                            else
                            {
                                item.Quantite.Text=(Convert.ToInt32(ArticleQuantity.Text) + item.qte).ToString();
                                item.qte += Convert.ToInt32(ArticleQuantity.Text);
                                TotalNett += sa1.a.PrixVente * Convert.ToInt32(ArticleQuantity.Text);
                                TotalNet.Text = TotalNett.ToString("F2") + " DH";
                                NbrA += Convert.ToInt32(ArticleQuantity.Text);
                                ArticleCount.Text = NbrA.ToString();
                            }
                            return;
                        }
                    }
                    
                    TotalNett += sa1.a.PrixVente * Convert.ToInt32(ArticleQuantity.Text);
                    TotalNet.Text = TotalNett.ToString("F2") + " DH";
                    NbrA += Convert.ToInt32(ArticleQuantity.Text);
                    ArticleCount.Text = NbrA.ToString();
                    CSingleArticle2 sa = new CSingleArticle2(sa1.a, Convert.ToInt32(ArticleQuantity.Text), this);
                    SelectedArticles.Children.Add(sa);
                    SelectedArticle.Child = new TextBlock
                    {
                        Name = "DesignationText",
                        Text = "Aucun article sélectionné",
                        FontFamily = new FontFamily("Segoe UI"),
                        FontSize = 13,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#92400E")),
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
            TotalNet.Text =" 0.00 DH";
            ArticleCount.Text = " 0";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ArticlesContainer.Children.Clear();
            foreach (Article a in la)
            {
                CSingleArticle1 ar = new CSingleArticle1(a, this,lf,lfo, 0);
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
            if (SelectedArticles.Children.Count == 0)
            {
                MessageBox.Show("Aucun article sélectionné .");
                return;
            }
            else
            {
                foreach (PaymentMethod p in main.lp)
                {
                    if (p.PaymentMethodName == PaymentMethodComboBox.SelectedValue)
                    {
                        MethodID=p.PaymentMethodID;
                    }
                }

                Client client = new Client();
                WSelectCient w = new WSelectCient(await client.GetClientsAsync(), this, 0,MethodID);
                w.ShowDialog();
            }
        }

        private async void HalfButton_Click(object sender, RoutedEventArgs e)
        {
            int MethodID = 0;
            if (SelectedArticles.Children.Count == 0)
            {
                MessageBox.Show("Aucun article sélectionné .");
                return;
            }
            else
            {
                foreach (PaymentMethod p in main.lp)
                {
                    if (p.PaymentMethodName == PaymentMethodComboBox.SelectedValue)
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
            if (SelectedArticles.Children.Count == 0)
            {
                MessageBox.Show("Aucun article sélectionné .");
                return;
            }
            else
            {
                foreach (PaymentMethod p in main.lp)
                {
                    if (p.PaymentMethodName == PaymentMethodComboBox.SelectedValue)
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
