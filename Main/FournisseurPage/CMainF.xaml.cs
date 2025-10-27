using GestionComerce.Main.ClientPage;
using GestionComerce.Main.FournisseurPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GestionComerce.Main.FournisseurPage
{
    public partial class CMainF : UserControl
    {
        private MainWindow _mainWindow;
        private User _currentUser;
        private List<Fournisseur> _allFournisseurs = new List<Fournisseur>();
        private List<Credit> _credits = new List<Credit>();

        public CMainF(User u, MainWindow mainWindow)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
            _currentUser = u;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach(Role r in _mainWindow.lr)
            {
                if (_currentUser.RoleID == r.RoleID)
                {
                    if (r.ViewFournisseur==true)
                    {
                        LoadAllData();
                    }
                }
            }
            
        }

        public void ReloadSuppliers()
        {
            foreach (Role r in _mainWindow.lr)
            {
                if (_currentUser.RoleID == r.RoleID)
                {
                    if (r.ViewFournisseur==true)
                    {
                        LoadAllData();
                    }
                }
            }
        }

        public void LoadAllData()
        {
            try
            {
                // Load from MainWindow lists (already loaded in memory)
                _allFournisseurs = _mainWindow.lfo ?? new List<Fournisseur>();
                _credits = _mainWindow.credits ?? new List<Credit>();

                System.Diagnostics.Debug.WriteLine($"Suppliers from list: {_allFournisseurs.Count}, Credits from list: {_credits.Count}");

                RefreshSupplierDisplay();
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine($"Error in LoadAllData: {ex}");
            }
        }

        private void RefreshSupplierDisplay()
        {
            try
            {
                SuppliersContainer.Children.Clear();

                if (_allFournisseurs == null || _allFournisseurs.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("No suppliers to display");
                    return;
                }

                var activeSuppliers = _allFournisseurs.Where(f => f.Etat).ToList();
                System.Diagnostics.Debug.WriteLine($"Active suppliers: {activeSuppliers.Count}");

                foreach (var supplier in activeSuppliers)
                {
                    var supplierRow = CreateSupplierRow(supplier);
                    SuppliersContainer.Children.Add(supplierRow);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing supplier display: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine($"Error in RefreshSupplierDisplay: {ex}");
            }
        }

        private UserControl CreateSupplierRow(Fournisseur supplier)
        {
            var supplierRow = new SingleRowSupplier(_mainWindow, _currentUser);
            supplierRow.DataContext = supplier;
            return supplierRow;
        }

        private void UpdateStatistics()
        {
            try
            {
                var activeSuppliers = _allFournisseurs?.Where(f => f.Etat).ToList() ?? new List<Fournisseur>();

                // Filter only SUPPLIER credits (where FournisseurID is not null)
                var supplierCredits = _credits?.Where(c => c.Etat && c.FournisseurID.HasValue).ToList() ?? new List<Credit>();

                int totalSuppliers = activeSuppliers.Count;
                decimal totalCredit = supplierCredits.Sum(c => c.Total);
                decimal totalPaid = supplierCredits.Sum(c => c.Paye);
                decimal pending = supplierCredits.Sum(c => c.Difference);

                if (TotalSuppliersText != null)
                    TotalSuppliersText.Text = totalSuppliers.ToString();

                if (TotalCreditText != null)
                    TotalCreditText.Text = $"{totalCredit:N2} DH";

                if (PaidThisMonthText != null)
                    PaidThisMonthText.Text = $"{totalPaid:N2} DH";

                if (PendingText != null)
                    PendingText.Text = $"{pending:N2} DH";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating statistics: {ex.Message}");
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (_allFournisseurs == null) return;

                string query = (SearchTextBox?.Text ?? "").Trim().ToLowerInvariant();

                SuppliersContainer.Children.Clear();

                List<Fournisseur> filteredSuppliers;

                if (string.IsNullOrEmpty(query))
                {
                    filteredSuppliers = _allFournisseurs.Where(f => f.Etat).ToList();
                }
                else
                {
                    filteredSuppliers = _allFournisseurs.Where(f => f.Etat &&
                        ((!string.IsNullOrEmpty(f.Nom) && f.Nom.ToLowerInvariant().Contains(query)) ||
                         (!string.IsNullOrEmpty(f.Telephone) && f.Telephone.ToLowerInvariant().Contains(query)))
                    ).ToList();
                }

                foreach (var supplier in filteredSuppliers)
                {
                    var supplierRow = CreateSupplierRow(supplier);
                    SuppliersContainer.Children.Add(supplierRow);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in search: {ex.Message}");
            }
        }

        private async void AddNewSupplier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var wnd = new SupplierFormWindow(_mainWindow);
                wnd.SupplierSaved += (s, ev) => LoadAllData();
                bool? result = wnd.ShowDialog();

                if (result == true)
                {
                    LoadAllData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening supplier form: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.load_main(_currentUser);
        }
    }
}