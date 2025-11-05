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
        public WSelectOperation sc; public Operation op;
        public CSingleOperation(WSelectOperation sc, Operation op)
        {
            InitializeComponent();
            this.sc = sc;
            this.op = op;
            OperationPrice.Text = op.PrixOperation.ToString("0.00") + " DH";
            OperationDate.Text = op.DateOperation.ToString();
            OperationType.Text = "Vente #" + op.OperationID.ToString();
            if(op.Reversed == true)
            {
                OperationType.Text += " (Reversed)";
                SideColor.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#828181"));

            }
            foreach (Client c in sc.main.main.lc)
            {
                if (op.ClientID == c.ClientID)
                {
                    OperationName.Text = "Client : " + c.Nom;
                    break;
                }
            }
        }

        private void btnArticle_Click(object sender, RoutedEventArgs e)
        {
            WMouvments wMouvments = new WMouvments(this, op);
            wMouvments.ShowDialog();
        }

        private void MyBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(op.Reversed == true)
            {
                sc.main.EtatFacture.SelectedIndex = 1;
                sc.main.EtatFacture.IsEnabled = false;
            }
            else
            {
                sc.main.EtatFacture.SelectedIndex = 0;
                sc.main.EtatFacture.IsEnabled = false;
                foreach (OperationArticle oa in sc.main.main.loa)
                {
                    if(oa.OperationID == op.OperationID && oa.Reversed == true)
                    {
                        sc.main.EtatFacture.IsEnabled = true;

                        break;
                    }
                }
            }
                sc.main.OperationContainer.Children.Clear();
            sc.OperationsContainer.Children.Clear();
            sc.main.OperationContainer.Children.Add(this);
            sc.main.txtTotalAmount.Text = op.PrixOperation.ToString("0.00") + " DH";
            sc.main.Remise.Text = op.Remise.ToString("0.00") + " DH";
            sc.Close();
        }
    }
}
