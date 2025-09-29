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

namespace Superete.Main.ProjectManagment
{
    /// <summary>
    /// Interaction logic for CSingleOperation.xaml
    /// </summary>
    public partial class CSingleOperation : UserControl
    {
        public CSingleOperation(CMainP main,Operation op)
        {
            InitializeComponent();
            this.main = main;
            this.op = op;
            OperationPrice.Text=op.PrixOperation.ToString("0.00") + " DH";
            OperationDate.Text=op.DateOperation.ToString(); 
            if (op.OperationType.StartsWith("V"))
            {
                SideColor.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#10B981"));
                OperationType.Text="Vente #"+ op.OperationID.ToString();

                foreach(Client c in main.main.lc)
                {
                    if (op.ClientID == c.ClientID)
                    {
                        OperationName.Text = "Client : "+c.Nom;
                        break;
                    }
                }
                
            } 
            else if (op.OperationType.StartsWith("A")) {
                SideColor.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff7614"));
                OperationType.Text = "Achat #" + op.OperationID.ToString();
                foreach (Fournisseur f in main.main.lfo)
                {
                    if (op.FournisseurID == f.FournisseurID)
                    {
                        OperationName.Text = "Fournisseur : " + f.Nom;
                        break;
                    }
                }
            } else if (op.OperationType.StartsWith("M"))
            {
                SideColor.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#2d42fc"));
                OperationType.Text = "Modification #" + op.OperationID.ToString();
                foreach (User u in main.main.lu)
                {
                    if (op.UserID == u.UserID)
                    {
                        OperationName.Text = "User : " + u.UserName;
                        break;
                    }
                }
            } 
            else if (op.OperationType.StartsWith("D"))
            {
                SideColor.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff3224"));
                OperationType.Text = "Suppression #" + op.OperationID.ToString();
                foreach (User u in main.main.lu)
                {
                    if (op.UserID == u.UserID)
                    {
                        OperationName.Text = "User : " + u.UserName;
                        break;
                    }
                }
            }
            if (op.Reversed == true)
            {
                SideColor.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#828181"));
                OperationType.Text += " (Reversed)";
            }
        }
        public CMainP main;public Operation op;public bool entered;
        private void MyBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WPlus wPlus = new WPlus(this);
            wPlus.ShowDialog();
        }
    }
}
