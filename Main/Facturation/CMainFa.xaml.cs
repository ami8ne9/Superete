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
        public MainWindow main;
        User u;
        public CMainFa(User u, MainWindow main,Operation op)
        {
            InitializeComponent();
            this.main = main;
            this.u = u;
            if(op != null)
            {
                if (op.Reversed == true)
                {
                    EtatFacture.SelectedIndex = 1;
                    EtatFacture.IsEnabled = false;
                }
                else
                {
                    EtatFacture.SelectedIndex = 0;
                    EtatFacture.IsEnabled = false;
                    foreach (OperationArticle oa in main.loa)
                    {
                        if (oa.OperationID == op.OperationID && oa.Reversed == true)
                        {
                            EtatFacture.IsEnabled = true;

                            break;
                        }
                    }
                }
                OperationContainer.Children.Clear();
                CSingleOperation cSingleOperation = new CSingleOperation(this,null, op);
                OperationContainer.Children.Add(cSingleOperation);
                txtTotalAmount.Text = op.PrixOperation.ToString("0.00") + " DH";
                Remise.Text = op.Remise.ToString("0.00") + " DH";
            }
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
            txtLogoPath.Text=facturee.LogoPath; 
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
                facture.LogoPath= txtLogoPath.Text;
                facture.InsertOrUpdateFactureAsync();
                WCongratulations wCongratulations = new WCongratulations("Informations saved successfully!", "", 1);
                wCongratulations.ShowDialog();
            }
            catch (Exception ex)
            {
                WCongratulations wCongratulations = new WCongratulations("\r\nInformations non enregistrées !", "", 0);
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
                MessageBox.Show("There is no operation selected");
                return;
            }
            Dictionary<string, string> FactureInfo = new Dictionary<string, string>()
            {
                { "NFacture", txtInvoiceNumber.Text},
                { "Date", Convert.ToString(dpInvoiceDate)},
                { "Type", cmbInvoiceType.Text},
                { "NomU", txtUserName.Text },
                { "ICEU", txtUserICE.Text},
                { "VATU", txtUserVAT.Text} ,
                { "TelephoneU", txtUserPhone.Text},
                { "EtatJuridiqueU", txtUserEtatJuridique.Text},
                { "IdSocieteU", txtUserIdSociete.Text},
                { "SiegeEntrepriseU", cmbUserSiegeEntreprise.Text},
                { "AdressU", txtUserAddress.Text},
                { "NomC", txtClientName.Text},
                { "ICEC", txtClientICE.Text},
                { "VATC", txtClientVAT.Text},
                { "TelephoneC",txtClientPhone.Text},
                { "EtatJuridiqueC",txtClientEtatJuridique.Text},
                { "IdSocieteC",txtClientIdSociete.Text},
                { "SiegeEntrepriseC",cmbClientSiegeEntreprise.Text},
                { "AdressC", txtClientAddress.Text},
                { "EtatFature", EtatFacture.Text},
                { "Device",txtCurrency.Text},
                { "TVA",txtTVARate.Text},
                { "MontantTotal",txtTotalAmount.Text},
                { "MontantTVA",txtTVAAmount.Text},
                { "MontantApresTVA",txtApresTVAAmount.Text},
                { "IndexDeFacture",IndexFacture.Text},
                { "Description", txtDescription.Text},
                { "Logo", txtLogoPath.Text},
                {"Reversed", EtatFacture.Text },
                {"Remise", Remise.Text  }
            };
            WFacturePage wFacturePage = new WFacturePage(this, FactureInfo);
            wFacturePage.ShowDialog();
        }

        private void txtInvoiceNumber_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnGenerateInvoiceNumber_Click(object sender, RoutedEventArgs e)
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
    }
}
