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
        public MainWindow main;
        User u;
        public CMainFa(User u, MainWindow main)
        {
            InitializeComponent();
            this.main = main;
            this.u = u;
            LoadFacture();
        }
        public async Task LoadFacture()
        {
            Facture facturee = await new Facture().GetFactureAsync();
            txtUserName.Text = facturee.Name;
            txtUserICE.Text = facturee.ICE;
            txtUserVAT.Text = facturee.VAT;
            txtUserPhone.Text = facturee.Telephone;
            txtUserAddress.Text = facturee.Adresse;
            txtClientIdSociete.Text = facturee.CompanyId;
            txtClientEtatJuridique.Text = facturee.EtatJuridic;
            cmbClientSiegeEntreprise.Text = facturee.SiegeEntreprise;
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
            decimal total = Convert.ToDecimal(txtTotalAmount.Text.Replace("DH", ""));
            decimal tvaAmount = Convert.ToDecimal(txtTVARate.Text) * 0.01m;
            txtTVAAmount.Text = total * tvaAmount + " DH";
            txtApresTVAAmount.Text = (total + (total * tvaAmount)) + " DH";
        }

        private void txtTVARate_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "") return;
            if (Convert.ToDecimal(textBox.Text) > 100 )
            {
                textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1);
            }
            if (txtTotalAmount == null || txtTotalAmount.Text == "") return;


            decimal total = Convert.ToDecimal(txtTotalAmount.Text.Replace("DH", ""));
            decimal tvaAmount = Convert.ToDecimal(txtTVARate.Text) * 0.01m;
            txtTVAAmount.Text = total * tvaAmount + " DH";
            txtApresTVAAmount.Text = (total + (total * tvaAmount)) + " DH";

        }

        private void IntegerTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Autorise uniquement les chiffres
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void DecimalTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            // Autorise les chiffres et un seul point
            if (e.Text == ".")
            {
                // Refuse si déjà un point
                e.Handled = textBox.Text.Contains(".");
            }
            else
            {
                e.Handled = !e.Text.All(char.IsDigit);
            }
        }

        // Pour empêcher le collage de texte invalide
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
                // Autorise un seul point et le reste chiffres
                int dotCount = text.Count(c => c == '.');
                if (dotCount > 1 || text.Any(c => !char.IsDigit(c) && c != '.'))
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
                Facture facture = new Facture();
                facture.Name = txtUserName.Text;
                facture.ICE = txtUserICE.Text;
                facture.VAT = txtUserVAT.Text;
                facture.Telephone = txtUserPhone.Text;
                facture.Adresse = txtUserAddress.Text;
                facture.CompanyId = txtClientIdSociete.Text;
                facture.EtatJuridic = txtClientEtatJuridique.Text;
                facture.SiegeEntreprise = cmbClientSiegeEntreprise.Text;
                facture.InsertOrUpdateFactureAsync();
                WCongratulations wCongratulations = new WCongratulations("Facture saved successfully!", "", 1);
                wCongratulations.ShowDialog();
            }
            catch (Exception ex)
            {
                WCongratulations wCongratulations = new WCongratulations("Facture not saved!", "", 0);
                wCongratulations.ShowDialog();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            main.load_main(u);
        }

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            if (OperationContainer.Children.Count == 0)
            {
                MessageBox.Show("There is no operation selected");
                return;
            }
            WFacturePage wFacturePage = new WFacturePage(this);
            wFacturePage.ShowDialog();
        }
    }
}
