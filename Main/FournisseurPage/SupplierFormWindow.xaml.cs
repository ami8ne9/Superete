using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GestionComerce.Main.FournisseurPage
{
    public partial class SupplierFormWindow : Window
    {
        public event EventHandler SupplierSaved;
        private readonly MainWindow _mainWindow;
        private readonly Fournisseur _editingSupplier;
        private readonly bool _isEdit;

        // Constructor for Add mode
        public SupplierFormWindow(MainWindow mainWindow) : this(mainWindow, null)
        {
        }

        // if supplier == null -> Add mode; else Edit mode.
        public SupplierFormWindow(MainWindow mainWindow, Fournisseur supplier = null)
        {
            InitializeComponent();
            _mainWindow = mainWindow;

            if (supplier == null)
            {
                _isEdit = false;
                _editingSupplier = new Fournisseur();
                UpdateButton.Content = "Add";
                this.Title = "Add Supplier";
            }
            else
            {
                _isEdit = true;
                _editingSupplier = supplier;
                UpdateButton.Content = "Update";
                this.Title = "Update Supplier";

                // populate fields
                NameTextBox.Text = _editingSupplier.Nom;
                PhoneTextBox.Text = _editingSupplier.Telephone;

                // Load balance from MainWindow credit list
                var myCredits = _mainWindow.credits
                    .Where(c => c.FournisseurID == _editingSupplier.FournisseurID && c.Etat)
                    .ToList();
                decimal diff = myCredits.Count > 0 ? myCredits.Sum(x => x.Difference) : 0m;
                BalanceTextBox.Text = diff.ToString("F2");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            // basic validation
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Please enter a name.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal parsedBalance = 0m;
            if (!string.IsNullOrWhiteSpace(BalanceTextBox.Text) &&
                !decimal.TryParse(BalanceTextBox.Text, out parsedBalance))
            {
                MessageBox.Show("Balance must be a number.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check for duplicates (only when adding new supplier or when name/phone changed)
            string newName = NameTextBox.Text.Trim();
            string newPhone = PhoneTextBox.Text.Trim();

            if (_mainWindow.lfo != null)
            {
                // Check for duplicate name
                var existingName = _mainWindow.lfo.FirstOrDefault(f =>
                    f.Nom.Equals(newName, StringComparison.OrdinalIgnoreCase) &&
                    f.Etat &&
                    (!_isEdit || f.FournisseurID != _editingSupplier.FournisseurID));

                if (existingName != null)
                {
                    MessageBox.Show($"A supplier with the name '{newName}' already exists.",
                        "Duplicate Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check for duplicate phone number (only if phone is provided)
                if (!string.IsNullOrWhiteSpace(newPhone))
                {
                    var existingPhone = _mainWindow.lfo.FirstOrDefault(f =>
                        !string.IsNullOrWhiteSpace(f.Telephone) &&
                        f.Telephone.Equals(newPhone, StringComparison.OrdinalIgnoreCase) &&
                        f.Etat &&
                        (!_isEdit || f.FournisseurID != _editingSupplier.FournisseurID));

                    if (existingPhone != null)
                    {
                        MessageBox.Show($"A supplier with the phone number '{newPhone}' already exists.",
                            "Duplicate Phone", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }

            if (_isEdit)
            {
                // update fournisseur (name, phone)
                _editingSupplier.Nom = newName;
                _editingSupplier.Telephone = newPhone;
                int result = await _editingSupplier.UpdateFournisseurAsync();

                if (result > 0)
                {
                    // Update in MainWindow list
                    var supplierInList = _mainWindow.lfo.FirstOrDefault(f => f.FournisseurID == _editingSupplier.FournisseurID);
                    if (supplierInList != null)
                    {
                        supplierInList.Nom = _editingSupplier.Nom;
                        supplierInList.Telephone = _editingSupplier.Telephone;
                    }

                    // Optionally: if user filled Balance field and it's >0 we create a new credit record
                    if (parsedBalance > 0)
                    {
                        var credit = new Credit
                        {
                            FournisseurID = _editingSupplier.FournisseurID,
                            Total = parsedBalance,
                            Paye = 0,
                            Difference = parsedBalance,
                            Etat = true
                        };
                        int creditId = await credit.InsertCreditAsync();

                        // Add to MainWindow credit list
                        if (creditId > 0)
                        {
                            credit.CreditID = creditId;
                            _mainWindow.credits.Add(credit);
                        }
                    }

                    //MessageBox.Show("Supplier updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    SupplierSaved?.Invoke(this, EventArgs.Empty);
                    //DialogResult = true;
                    //Close();
                    WCongratulations wCongratulations = new WCongratulations("Modification Succes", "Fournisseur Modifier avec succes", 1);
                    wCongratulations.ShowDialog();
                }
                else
                {
                    //MessageBox.Show("Update failed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    WCongratulations wCongratulations = new WCongratulations("Modification Echoue", "Fournisseur n'a pas ete Modifier", 0);
                    wCongratulations.ShowDialog();
                }
            }
            else
            {
                // Insert new fournisseur
                _editingSupplier.Nom = newName;
                _editingSupplier.Telephone = newPhone;
                _editingSupplier.Etat = true;

                int newId = await _editingSupplier.InsertFournisseurAsync();

                if (newId > 0)
                {
                    // Add to MainWindow list
                    _editingSupplier.FournisseurID = newId;
                    _mainWindow.lfo.Add(_editingSupplier);

                    // if initial balance > 0 create a credit record
                    if (parsedBalance > 0)
                    {
                        var credit = new Credit
                        {
                            FournisseurID = newId,
                            Total = parsedBalance,
                            Paye = 0,
                            Difference = parsedBalance,
                            Etat = true
                        };
                        int creditId = await credit.InsertCreditAsync();

                        // Add to MainWindow credit list
                        if (creditId > 0)
                        {
                            credit.CreditID = creditId;
                            _mainWindow.credits.Add(credit);
                        }
                    }

                    //MessageBox.Show("Supplier added.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    SupplierSaved?.Invoke(this, EventArgs.Empty);
                    //DialogResult = true;
                    //Close();
                    WCongratulations wCongratulations = new WCongratulations("Ajout Succes", "Fournisseur Ajouter avec succes", 1);
                    wCongratulations.ShowDialog();
                }
                else
                {
                    //MessageBox.Show("Insert failed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    WCongratulations wCongratulations = new WCongratulations("Ajout Echoue", "Fournisseur n'a pas ete Ajouter", 0);
                    wCongratulations.ShowDialog();
                }
            }
        }
    }
}