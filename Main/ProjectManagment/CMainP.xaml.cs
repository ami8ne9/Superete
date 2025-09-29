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
using System.Xml.Linq;

namespace Superete.Main.ProjectManagment
{
    /// <summary>
    /// Interaction logic for CMainP.xaml
    /// </summary>
    public partial class CMainP : UserControl
    {
        public CMainP(User u,MainWindow main)
        {
            InitializeComponent();
            this.u = u;
            this.main = main;

            TypeOperationFilter.SelectedIndex = 0;
            LoadOperations(main.lo);
        }
        public User u;public MainWindow main;
        public void LoadOperations(List<Operation> lo)
        {
            OperationsContainer.Children.Clear();
            foreach(Operation operation in lo)
            {
                CSingleOperation wSingleOperation =new CSingleOperation(this,operation);
                OperationsContainer.Children.Add(wSingleOperation);
            }

        }
        private void RetourButton_Click(object sender, RoutedEventArgs e)
        {
            main.load_main(u);
        }


        private void ToutButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OperationsButton_Click(object sender, RoutedEventArgs e)
        {
            OperationsContent.Visibility = Visibility.Visible;
            MouvmentContent.Visibility = Visibility.Collapsed;
            OperationsButton.Foreground= new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E90FF") );
            OperationsButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E90FF"));
            StockButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
            StockButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
        }

        private void StockButton_Click(object sender, RoutedEventArgs e)
        {
            OperationsContent.Visibility = Visibility.Collapsed;
            MouvmentContent.Visibility = Visibility.Visible;
            OperationsButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
            OperationsButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
            StockButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E90FF"));
            StockButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E90FF"));
        }

        private void SearchInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            List<Operation> lo=new List<Operation>();
            foreach (Operation op in main.lo)
            {
                if ((SearchTypeFilter.SelectedItem as ComboBoxItem)?.Content.ToString() == "Operation ID")
                {
                        
                    if (op.OperationID.ToString().Contains(tb.Text))
                    {
                        lo.Add(op);
                    }
                }
                else if ((SearchTypeFilter.SelectedItem as ComboBoxItem)?.Content.ToString() == "Client")
                {
                    foreach (Client c in main.lc)
                    {
                        if (c.Nom.Contains(tb.Text))
                        {
                            if (op.ClientID == c.ClientID) {
                                lo.Add(op);
                            }
                            break;
                        }
                    }
                }
                else if ((SearchTypeFilter.SelectedItem as ComboBoxItem)?.Content.ToString() == "Fournisseur")
                {
                    foreach (Fournisseur f in main.lfo)
                    {
                        if (f.Nom.Contains(tb.Text))
                        {
                            if (op.FournisseurID == f.FournisseurID)
                            {
                                lo.Add(op);
                            }
                            break;
                        }
                    }
                }
                else if ((SearchTypeFilter.SelectedItem as ComboBoxItem)?.Content.ToString() == "Utilisateur")
                {
                    foreach (User u in main.lu)
                    {
                        if (u.UserName.Contains(tb.Text))
                        {
                            if (op.UserID == u.UserID)
                            {
                                lo.Add(op);
                            }
                            break;
                        }
                    }
                }
            }
            if (tb.Text == "")
            {
                LoadOperations(main.lo);
            }
            else
            {
                LoadOperations(lo);
            }


            }

        private void TypeOperationFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Operation> lo = new List<Operation>();
            foreach (Operation op in main.lo)
            {
                if ((TypeOperationFilter.SelectedItem as ComboBoxItem)?.Content.ToString() == "Vente")
                {

                    if (op.OperationType.StartsWith("V"))
                    {
                        lo.Add(op);
                    }
                }
                else if ((TypeOperationFilter.SelectedItem as ComboBoxItem)?.Content.ToString() == "Achat")
                {
                    foreach (Client c in main.lc)
                    {
                        if (op.OperationType.StartsWith("A"))
                        {
                            lo.Add(op);
                        }
                    }
                }
                else if ((TypeOperationFilter.SelectedItem as ComboBoxItem)?.Content.ToString() == "Modification")
                {
                    if (op.OperationType.StartsWith("M"))
                    {
                        lo.Add(op);
                    }
                }
                else if ((TypeOperationFilter.SelectedItem as ComboBoxItem)?.Content.ToString() == "Suppression")
                {
                    if (op.OperationType.StartsWith("D"))
                    {
                        lo.Add(op);
                    }
                }
            }
            if ((TypeOperationFilter.SelectedItem as ComboBoxItem)?.Content.ToString() == "Tout")
            {
                LoadOperations(main.lo);
            }
            else
            {
                LoadOperations(lo);
            }
        }
    }
}
