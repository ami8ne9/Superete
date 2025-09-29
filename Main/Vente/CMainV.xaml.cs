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
            Key0.Click += Keypad_Click;
            Key1.Click += Keypad_Click;
            Key2.Click += Keypad_Click;
            Key3.Click += Keypad_Click;
            Key4.Click += Keypad_Click;
            Key5.Click += Keypad_Click;
            Key6.Click += Keypad_Click;
            Key7.Click += Keypad_Click;
            Key8.Click += Keypad_Click;
            Key9.Click += Keypad_Click;
            KeyDot.Click += Keypad_Click;
            KeyBackspace.Click += KeyBackspace_Click;
            CurrentUser.Text = u.UserName;
            UpdateQuantityDisplay();
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

        }
        public User u;List<Famille> lf;public List<Article> la; MainWindow main;public decimal TotalNett=0; public int NbrA = 0; public List<Fournisseur> lfo;
        public void LoadArticles(List<Article> la)
        {
            ArticlesContainer.Children.Clear();
            foreach (Article a in la)
            {
                CSingleArticle1 ar = new CSingleArticle1(a, this, lf, lfo);
                ArticlesContainer.Children.Add(ar);
            }
        }

        private void Keypad_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string value = btn.Content.ToString();
                if (value == ".")
                {
                    if (!quantityBuilder.ToString().Contains("."))
                        quantityBuilder.Append(".");
                }
                else
                {
                    if (quantityBuilder.ToString() == "0")
                        quantityBuilder.Clear();
                    quantityBuilder.Append(value);
                }
                UpdateQuantityDisplay();
            }
        }

        private void KeyBackspace_Click(object sender, RoutedEventArgs e)
        {
            if (quantityBuilder.Length > 0)
            {
                quantityBuilder.Remove(quantityBuilder.Length - 1, 1);
                if (quantityBuilder.Length == 0)
                    quantityBuilder.Append("0");
                UpdateQuantityDisplay();
            }
        }

        private void UpdateQuantityDisplay()
        {
            ArticleQuantity.Text = quantityBuilder.ToString();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            main.load_main(u);
        }

        private void KeyboardButton_Click(object sender, RoutedEventArgs e)
        {
            if(SelectedArticle.Child is CSingleArticle1 sa1)
            {
                if (Convert.ToInt32(ArticleQuantity.Text)==0)
                {
                    MessageBox.Show("inserer une quantite");
                    return;
                } else if (Convert.ToInt32(ArticleQuantity.Text)>sa1.a.Quantite)
                {
                    MessageBox.Show("la quantite inserer est plus grande que la quantite en stock");
                }
                else
                {
                    foreach (CSingleArticle2 item in SelectedArticles.Children)
                    {
                        if(item.a.ArticleID==sa1.a.ArticleID)
                        {
                            MessageBox.Show("Article déjà ajouté. Veuillez modifier la quantité dans le panier.");
                            return;
                        }
                    }
                    TotalNett += sa1.a.PrixVente * Convert.ToInt32(ArticleQuantity.Text);
                    TotalNet.Text = TotalNett.ToString("F2") + " DH";
                    NbrA += Convert.ToInt32(ArticleQuantity.Text);
                    ArticleCount.Text = NbrA.ToString();
                    CSingleArticle2 sa = new CSingleArticle2(sa1, ArticleQuantity.Text,this);
                    SelectedArticles.Children.Add(sa);
                    SelectedArticle.Child = new TextBlock
                    {
                        Name="DesignationText",
                        Text="Aucun article sélectionné",
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
            TotalNet.Text =" 0.00 DH";
            ArticleCount.Text = " 0";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ArticlesContainer.Children.Clear();
            foreach (Article a in la)
            {
                CSingleArticle1 ar = new CSingleArticle1(a, this,lf,lfo);
                ArticlesContainer.Children.Add(ar);
            }
        }

        private void CodeBarreTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CodeArticlesContainer.Children.Clear();
            string code=sender is TextBox tb ? tb.Text : "";
            foreach (Article a in la)
            {
                if (a.Code.ToString().Contains(code) && code!="")
                {
                    CSingleArticle1 ar = new CSingleArticle1(a, this, lf,lfo);
                    CodeArticlesContainer.Children.Add(ar);
                }
                
            }
        }


        private async void CashButton_Click(object sender, RoutedEventArgs e)

        {
            if (SelectedArticles.Children.Count == 0)
            {
                MessageBox.Show("Aucun article sélectionné .");
                return;
            }
            else
            {
                Client client = new Client();
                WSelectCient w = new WSelectCient(await client.GetClientsAsync(), this, 0);
                w.ShowDialog();
            }
        }

        private async void HalfButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedArticles.Children.Count == 0)
            {
                MessageBox.Show("Aucun article sélectionné .");
                return;
            }
            else
            {
                Client client = new Client();
                WSelectCient w = new WSelectCient(await client.GetClientsAsync(), this, 1);
                w.ShowDialog();
            }
        }

        private async void CreditButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedArticles.Children.Count == 0)
            {
                MessageBox.Show("Aucun article sélectionné .");
                return;
            }
            else
            {
                Client client = new Client();
                WSelectCient w = new WSelectCient(await client.GetClientsAsync(), this, 2);
                w.ShowDialog();
            }
        }
    }
}
