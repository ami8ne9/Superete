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
    /// <summary>
    /// Interaction logic for WSelectOperation.xaml
    /// </summary>
    public partial class WSelectOperation : Window
    {
        public CMainFa main;
        public WSelectOperation(CMainFa main)
        {
            InitializeComponent();
            this.main = main;
            LoadOperations();
        }
        public void LoadOperations()
        {
            OperationsContainer.Children.Clear();
            foreach (Operation op in main.main.lo)
            {
                if(!op.OperationType.StartsWith("V"))
                {
                    continue;
                }
                CSingleOperation cSingleOperation = new CSingleOperation(main,this, op);
                OperationsContainer.Children.Add(cSingleOperation);
            }
        }

        private void btnClose_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        { 
            string ClientName = "";
            TextBox textBox = sender as TextBox;
            foreach (Operation op in main.main.lo)
            {
                if (!op.OperationType.StartsWith("V"))
                {
                    continue;
                }
                foreach (Client c in main.main.lc)
                {
                    if (op.ClientID == c.ClientID)
                    {
                        ClientName = c.Nom;
                        break;
                    }
                }
                if (op.OperationID.ToString().IndexOf(textBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    ClientName.IndexOf(textBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    // Match found, ensure the operation is visible
                    foreach (CSingleOperation so in OperationsContainer.Children)
                    {
                        if (so.op.OperationID == op.OperationID)
                        {
                            so.Visibility = Visibility.Visible;
                            break;
                        }
                    }
                }
                else
                {
                    // No match, hide the operation
                    foreach (CSingleOperation so in OperationsContainer.Children)
                    {
                        if (so.op.OperationID == op.OperationID)
                        {
                            so.Visibility = Visibility.Collapsed;
                            break;
                        }
                    }
                }
            }
        }
    }
}
