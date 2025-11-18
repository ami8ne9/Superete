using GestionComerce;
using GestionComerce.Main.Facturation;
using GestionComerce.Main.ProjectManagment;
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

namespace Superete.Main.Facturation
{
    /// <summary>
    /// Interaction logic for CSingleOperation.xaml
    /// </summary>
    public partial class CSingleOperation : UserControl
    {
        private const int ETAT_FACTURE_NORMAL = 0;
        private const int ETAT_FACTURE_REVERSED = 1;

        public WSelectOperation sc;
        public Operation op;

        public CSingleOperation(CMainFa mainfa, WSelectOperation sc, Operation op)
        {
            InitializeComponent();
            this.sc = sc;
            this.op = op;

            // Initialize UI elements
            OperationPrice.Text = op.PrixOperation.ToString("0.00") + " DH";
            OperationDate.Text = op.DateOperation.ToString();
            OperationType.Text = "Vente #" + op.OperationID.ToString();

            if (op.Reversed == true)
            {
                OperationType.Text += " (Reversed)";
                SideColor.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#828181"));
            }

            // Load client name
            LoadClientName(mainfa);
        }

        private void LoadClientName(CMainFa mainfa)
        {
            // Get the client list from the appropriate source
            var clientList = sc == null ? mainfa?.main?.lc : sc?.main?.main?.lc;

            if (clientList == null)
                return;

            // Find and display client name
            var client = clientList.FirstOrDefault(c => c.ClientID == op.ClientID);
            if (client != null)
            {
                OperationName.Text = "Client : " + client.Nom;
            }
        }

        private void btnArticle_Click(object sender, RoutedEventArgs e)
        {
            WMouvments wMouvments = new WMouvments(this, op);
            wMouvments.ShowDialog();
        }

        public void MyBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sc?.main?.main == null)
            {
                return;
            }

            decimal prixOp = 0;
            decimal prixOpR = 0;
            decimal tvaAmount = 0;
            decimal tvaAmountR = 0;

            if (op.Reversed == true)
            {
                // Handle fully reversed operation
                sc.main.EtatFacture.SelectedIndex = ETAT_FACTURE_REVERSED;
                sc.main.EtatFacture.IsEnabled = false;

                CalculateReversedOperationTotals(ref prixOpR, ref tvaAmountR);
            }
            else
            {
                // Handle normal or partially reversed operation
                sc.main.EtatFacture.SelectedIndex = ETAT_FACTURE_NORMAL;
                sc.main.EtatFacture.IsEnabled = false;

                CalculateNormalOperationTotals(ref prixOp, ref prixOpR, ref tvaAmount, ref tvaAmountR);
            }

            // Update UI containers
            UpdateOperationContainers();

            // Update displayed amounts
            UpdateDisplayedAmounts(prixOp, prixOpR, tvaAmount, tvaAmountR);

            // Display discount
            sc.main.Remise.Text = op.Remise.ToString("0.00") ;

            sc.Close();
        }

        private void CalculateReversedOperationTotals(ref decimal prixOpR, ref decimal tvaAmountR)
        {
            foreach (OperationArticle oa in sc.main.main.loa)
            {
                if (oa.OperationID == op.OperationID)
                {
                    var article = sc.main.main.la.FirstOrDefault(a => a.ArticleID == oa.ArticleID);
                    if (article != null)
                    {
                        prixOpR += article.PrixVente * oa.QteArticle;
                        tvaAmountR += (article.tva / 100) * (article.PrixVente * oa.QteArticle);
                    }
                }
            }
        }

        private void CalculateNormalOperationTotals(ref decimal prixOp, ref decimal prixOpR,
                                                     ref decimal tvaAmount, ref decimal tvaAmountR)
        {
            foreach (OperationArticle oa in sc.main.main.loa)
            {
                if (oa.OperationID == op.OperationID)
                {
                    var article = sc.main.main.la.FirstOrDefault(a => a.ArticleID == oa.ArticleID);
                    if (article == null)
                        continue;

                    if (oa.Reversed == true)
                    {
                        // Partially reversed items
                        sc.main.EtatFacture.IsEnabled = true;
                        prixOpR += article.PrixVente * oa.QteArticle;
                        tvaAmountR += (article.tva / 100) * (article.PrixVente * oa.QteArticle);
                    }
                    else
                    {
                        // Normal items
                        prixOp += article.PrixVente * oa.QteArticle;
                        tvaAmount += (article.tva / 100) * (article.PrixVente * oa.QteArticle);
                    }
                }
            }
        }

        private void UpdateOperationContainers()
        {
            sc.main.OperationContainer.Children.Clear();
            sc.OperationsContainer.Children.Clear();
            sc.main.OperationContainer.Children.Add(this);
        }

        private void UpdateDisplayedAmounts(decimal prixOp, decimal prixOpR, decimal tvaAmount, decimal tvaAmountR)
        {
            if (prixOpR == 0)
            {
                // No reversed items
                sc.main.txtTotalAmount.Text = prixOp.ToString("0.00") + " DH";
                sc.main.txtTVAAmount.Text = tvaAmount.ToString("0.00") + " DH";

                // Calculate TVA percentage: (tvaAmount / prixOp) * 100
                decimal tvaPercentage = prixOp > 0 ? (tvaAmount / prixOp) * 100 : 0;
                sc.main.txtTVARate.Text = tvaPercentage.ToString("0.00");
            }
            else if (prixOpR > 0)
            {
                // Has reversed items - display based on selected state
                if (sc.main.EtatFacture.SelectedIndex == ETAT_FACTURE_REVERSED)
                {
                    sc.main.txtTotalAmount.Text = prixOpR.ToString("0.00") + " DH";
                    sc.main.txtTVAAmount.Text = tvaAmountR.ToString("0.00") + " DH";

                    // Calculate TVA percentage for reversed items
                    decimal tvaPercentageR = prixOpR > 0 ? (tvaAmountR / prixOpR) * 100 : 0;
                    sc.main.txtTVARate.Text = tvaPercentageR.ToString("0.00");
                }
                else
                {
                    sc.main.txtTotalAmount.Text = prixOp.ToString("0.00") + " DH";
                    sc.main.txtTVAAmount.Text = tvaAmount.ToString("0.00") + " DH";

                    // Calculate TVA percentage for normal items
                    decimal tvaPercentage = prixOp > 0 ? (tvaAmount / prixOp) * 100 : 0;
                    sc.main.txtTVARate.Text = tvaPercentage.ToString("0.00");
                }
            }
            else
            {
                // Fallback to operation price
                sc.main.txtTotalAmount.Text = op.PrixOperation.ToString("0.00") + " DH";
                sc.main.txtTVAAmount.Text = tvaAmount.ToString("0.00") + " DH";

                // Calculate TVA percentage for fallback
                decimal tvaPercentage = op.PrixOperation > 0 ? (tvaAmount / op.PrixOperation) * 100 : 0;
                sc.main.txtTVARate.Text = tvaPercentage.ToString("0.00");
            }
        }
    }
}