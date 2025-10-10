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
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace Superete.Main.Inventory
{
    /// <summary>
    /// Interaction logic for WAjoutQuantite.xaml
    /// </summary>
    public partial class WAjoutQuantite : Window
    {
        public WAjoutQuantite(CSingleRowArticle sa,int s, WNouveauStock ns)
        {
            InitializeComponent();
            this.a = sa.a;
            this.sa = sa;
            this.s = s;
            this.ns = ns;
            if (s == 5)
            {
                TicketCheckBox.Visibility = Visibility.Collapsed;
                ButtonsContainer.Visibility = Visibility.Collapsed;
                AjouterButton.Visibility = Visibility.Visible;
            }

            LoadPayments(ns.main.main.lp);
        }
        public Article a;public CSingleRowArticle sa;int s;public WNouveauStock ns;public int qte;
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void LoadPayments(List<PaymentMethod> lp)
        {
            PaymentMethodComboBox.Items.Clear();
            foreach (PaymentMethod a in lp)
            {
                PaymentMethodComboBox.Items.Add(a.PaymentMethodName);
            }
        }

        private void CashButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (Quantite.Text != "")
            {
                if (Convert.ToInt32(Quantite.Text) == 0)
                {
                    MessageBox.Show("s'il vous plais donner une quantite");
                    return;
                }
                
            }
            else
            {
                MessageBox.Show("s'il vous plais donner une quantite");
                return;
            }
            qte = Convert.ToInt32(Quantite.Text);
            int MethodID = 0;
            foreach (PaymentMethod p in ns.main.main.lp)
            {
                if (p.PaymentMethodName == PaymentMethodComboBox.SelectedValue)
                {
                    MethodID = p.PaymentMethodID;
                }
            }
            WConfirmTransaction wConfirmTransaction = new WConfirmTransaction(null, this,null, a, 0, MethodID);
            wConfirmTransaction.ShowDialog();
        }

        private void HalfButton_Click(object sender, RoutedEventArgs e)
        {
            if (Quantite.Text != "")
            {
                if (Convert.ToInt32(Quantite.Text) == 0)
                {
                    MessageBox.Show("s'il vous plais donner une quantite");
                    return;
                }

            }
            else
            {
                MessageBox.Show("s'il vous plais donner une quantite");
                return;
            }
            qte = Convert.ToInt32(Quantite.Text);
            int MethodID = 0;
            foreach (PaymentMethod p in ns.main.main.lp)
            {
                if (p.PaymentMethodName == PaymentMethodComboBox.SelectedValue)
                {
                    MethodID = p.PaymentMethodID;
                }
            }
            WConfirmTransaction wConfirmTransaction = new WConfirmTransaction(null, this, null, a, 1, MethodID);
            wConfirmTransaction.ShowDialog();
        }

        private void CreditButton_Click(object sender, RoutedEventArgs e)
        {
            if (Quantite.Text != "")
            {
                if (Convert.ToInt32(Quantite.Text) == 0)
                {
                    MessageBox.Show("s'il vous plais donner une quantite");
                    return;
                }

            }
            else
            {
                MessageBox.Show("s'il vous plais donner une quantite");
                return;
            }
            qte = Convert.ToInt32(Quantite.Text);
            int MethodID = 0;
            foreach (PaymentMethod p in ns.main.main.lp)
            {
                if (p.PaymentMethodName == PaymentMethodComboBox.SelectedValue)
                {
                    MethodID = p.PaymentMethodID;
                }
            }
            WConfirmTransaction wConfirmTransaction = new WConfirmTransaction(null, this, null, a, 2,MethodID);
            wConfirmTransaction.ShowDialog();
        }

        private void EnregistrerButton_Click(object sender, RoutedEventArgs e)
        {         
            
            CSingleRowArticle cSingleRowArticle = new CSingleRowArticle(a, ns.main.la, null, ns.main, 6, sa.ea, ns,Convert.ToInt32(Quantite.Text));
            if (s == 5)
            {
                foreach (CSingleRowArticle csra in ns.AMA.ArticlesContainer.Children)
                {

                    if (csra.a.ArticleID == a.ArticleID)
                    {
                        csra.Quantite.Text = "x" + (Convert.ToInt32(Quantite.Text) + Convert.ToInt32(csra.Quantite.Text.Substring(1))).ToString();
                        cSingleRowArticle.ea.Close();
                        ns.Close();
                        this.Close();
                        return;
                    }
                }
            }
            ns.AMA.ArticlesContainer.Children.Add(cSingleRowArticle);
            cSingleRowArticle.ea.Close();
            ns.Close();
            this.Close();
        }

        private static readonly Regex _regex = new Regex("^[0-9]+$"); // Only numbers

        private void Quantite_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !_regex.IsMatch(e.Text);
        }

        private void Quantite_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!_regex.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void Quantite_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
