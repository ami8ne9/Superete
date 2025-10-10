using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Superete.Main.FournisseurPage
{
    public partial class SingleRowSupplier : UserControl
    {
        private readonly MainWindow _main;
        private readonly User _currentUser;

     

        public SingleRowSupplier(MainWindow main, User currentUser)
        {
            InitializeComponent();
            _main = main;
            _currentUser = currentUser;
            DataContextChanged += SingleRowSupplier_DataContextChanged;
        }



        private void SingleRowSupplier_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Fournisseur fournisseur)
                PopulateRow(fournisseur);
        }

        private void PopulateRow(Fournisseur fournisseur)
        {
            IdText.Text = fournisseur.FournisseurID.ToString();
            NameText.Text = fournisseur.Nom ?? "N/A";
            PhoneText.Text = string.IsNullOrEmpty(fournisseur.Telephone) ? "-" : fournisseur.Telephone;

            var supplierCredits = _main.credits
                .Where(c => c.FournisseurID == fournisseur.FournisseurID && c.Etat)
                .ToList();

            decimal balance = supplierCredits.Sum(c => c.Difference);
            BalanceText.Text = $"{balance:F2} DH";
            BalanceText.Foreground = balance > 0 ? Brushes.Red : Brushes.Green;
        }

        private void Paid_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is Fournisseur fournisseur)) return;

            var wnd = new PaidSupplierWindow(_currentUser, _main, fournisseur);
            if (wnd.ShowDialog() == true)
            {
                // Refresh after payment
                var parent = this.Parent;
                while (parent != null && !(parent is CMainF))
                    parent = LogicalTreeHelper.GetParent(parent);

                if (parent is CMainF supplierPage)
                    supplierPage.LoadAllData();
            }
        }
    

// Buttons: open the corresponding windows
    private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is Fournisseur f)) return;
            if (_main == null) return;

            var wnd = new SupplierFormWindow(_main, f); // edit mode
            bool? res = wnd.ShowDialog();
            if (res == true)
            {
                // Refresh the parent CMainF
                var parent = FindParentCMainF();
                parent?.ReloadSuppliers();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is Fournisseur f)) return;
            if (_main == null) return;

            var wnd = new DeleteSupplierWindow(_main, f);
            bool? res = wnd.ShowDialog();
            if (res == true)
            {
                var parent = FindParentCMainF();
                parent?.ReloadSuppliers();
            }
        }

        private void Operations_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is Fournisseur f)) return;
            if (_main == null) return;

            var window = new SupplierOperationsWindow(_main, f);
            window.ShowDialog();
        }

        // Helper method to find parent CMainF
        private CMainF FindParentCMainF()
        {
            DependencyObject current = this;
            while (current != null)
            {
                current = System.Windows.Media.VisualTreeHelper.GetParent(current);
                if (current is CMainF cmainf)
                    return cmainf;
            }
            return null;
        }
    }
}