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
        public WAjoutQuantite(CSingleRowArticle sa)
        {
            InitializeComponent();
            this.a = sa.a;
            this.sa = sa;
        }
        public Article a;public CSingleRowArticle sa;
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CashButton_Click(object sender, RoutedEventArgs e)
        {
            a.Quantite += Convert.ToInt32(Quantite.Text);
            WConfirmTransaction wConfirmTransaction = new WConfirmTransaction(null,this, a, 0);
            wConfirmTransaction.ShowDialog();
        }

        private void HalfButton_Click(object sender, RoutedEventArgs e)
        {
            a.Quantite += Convert.ToInt32(Quantite.Text);
            WConfirmTransaction wConfirmTransaction = new WConfirmTransaction(null, this, a, 1);
            wConfirmTransaction.ShowDialog();
        }

        private void CreditButton_Click(object sender, RoutedEventArgs e)
        {
            a.Quantite += Convert.ToInt32(Quantite.Text);
            WConfirmTransaction wConfirmTransaction = new WConfirmTransaction(null, this, a, 2);
            wConfirmTransaction.ShowDialog();
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
    }
}
