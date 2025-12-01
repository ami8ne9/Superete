using GestionComerce;
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

namespace GestionComerce.Main.Facturation.CreateFacture
{
    public partial class WMouvments : Window
    {
        public CSingleOperation wso;
        Operation op;

        public WMouvments(CSingleOperation wso, Operation op)
        {
            InitializeComponent();
            this.wso = wso;
            this.op = op;

            // Get the main reference - check both paths
            MainWindow main = null;

            if (wso.mainfa != null && wso.mainfa.main != null)
            {
                // We're in the main view (CSingleOperation was added to CMainFa)
                main = wso.mainfa.main;
            }
            else if (wso.sc?.main?.main != null)
            {
                // We're in the selection view (WSelectOperation is open)
                main = wso.sc.main.main;
            }

            if (main == null || op == null)
            {
                MessageBox.Show("Unable to load operation articles", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LoadMouvments(main);

            // Show save button only for expedition invoices
            UpdateSaveButtonVisibility();
        }

        private void UpdateSaveButtonVisibility()
        {
            bool isExpedition = false;

            // Check both paths for invoice type
            if (wso.mainfa != null && wso.mainfa.InvoiceType == "Expedition")
            {
                isExpedition = true;
            }
            else if (wso.sc?.main != null && wso.sc.main.InvoiceType == "Expedition")
            {
                isExpedition = true;
            }

            // Show/hide save button
            btnSaveExpedition.Visibility = isExpedition ? Visibility.Visible : Visibility.Collapsed;

            // Update window title for expedition
            if (isExpedition)
            {
                this.Title = "Définir les quantités d'expédition";
            }
        }

        public void LoadMouvments(MainWindow main)
        {
            MouvmentsContainer.Children.Clear();

            // Check if main.loa exists
            if (main.loa == null)
            {
                MessageBox.Show("No articles found for this operation", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Filter articles for this operation
            var operationArticles = main.loa
                .Where(oa => oa.OperationID == op.OperationID)
                .ToList();

            if (operationArticles.Count == 0)
            {
                MessageBox.Show("No articles found for this operation", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            foreach (OperationArticle oa in operationArticles)
            {
                CSingleMouvment csm = new CSingleMouvment(this, oa);
                MouvmentsContainer.Children.Add(csm);
            }
        }

        private void btnClose_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSaveExpedition_Click(object sender, RoutedEventArgs e)
        {
            // Update all mouvments
            foreach (var child in MouvmentsContainer.Children)
            {
                if (child is CSingleMouvment mouvment)
                {
                    mouvment.UpdateInvoiceArticle();
                }
            }

            MessageBox.Show("Quantités d'expédition enregistrées", "Succès",
                MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}