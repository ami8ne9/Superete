using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Superete.Main.ClientPage
{
    public partial class ClientFormWindow : Window
    {
        private readonly Client _editingClient;
        private readonly bool _isEdit;
        private MainWindow _main;

        // keep track if we inserted a credit during this session
        private Credit _insertedCredit = null;

        public ClientFormWindow(MainWindow main, Client client = null)
        {
            InitializeComponent();
            _main = main;

            if (client == null)
            {
                _isEdit = false;
                _editingClient = new Client();
                UpdateButton.Content = "Add";
                this.Title = "Add Client";
            }
            else
            {
                _isEdit = true;
                _editingClient = client;
                UpdateButton.Content = "Update";
                this.Title = "Update Client";

                // populate fields
                NameTextBox.Text = _editingClient.Nom;
                PhoneTextBox.Text = _editingClient.Telephone;

                // read balance from MainWindow in-memory credits
                try
                {
                    var credits = _main?.credits ?? new System.Collections.Generic.List<Credit>();
                    var my = credits.FindAll(c => c.ClientID == _editingClient.ClientID && c.Etat);
                    decimal diff = my.Count > 0 ? my.Sum(x => x.Difference) : 0m;
                    BalanceTextBox.Text = diff.ToString("F2");
                }
                catch
                {
                    // keep silent — balance is optional for display
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
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

            // Check for duplicates (only when adding new client or when name/phone changed)
            string newName = NameTextBox.Text.Trim();
            string newPhone = PhoneTextBox.Text.Trim();

            if (_main.lc != null)
            {
                // Check for duplicate name
                var existingName = _main.lc.FirstOrDefault(c =>
                    c.Nom.Equals(newName, StringComparison.OrdinalIgnoreCase) &&
                    c.Etat &&
                    (!_isEdit || c.ClientID != _editingClient.ClientID));

                if (existingName != null)
                {
                    MessageBox.Show($"A client with the name '{newName}' already exists.",
                        "Duplicate Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check for duplicate phone number (only if phone is provided)
                if (!string.IsNullOrWhiteSpace(newPhone))
                {
                    var existingPhone = _main.lc.FirstOrDefault(c =>
                        !string.IsNullOrWhiteSpace(c.Telephone) &&
                        c.Telephone.Equals(newPhone, StringComparison.OrdinalIgnoreCase) &&
                        c.Etat &&
                        (!_isEdit || c.ClientID != _editingClient.ClientID));

                    if (existingPhone != null)
                    {
                        MessageBox.Show($"A client with the phone number '{newPhone}' already exists.",
                            "Duplicate Phone", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }

            if (_isEdit)
            {
                _editingClient.Nom = newName;
                _editingClient.Telephone = newPhone;

                // Save to database
                int result = await _editingClient.UpdateClientAsync();

                if (result > 0)
                {
                    // Update MainWindow list
                    var existing = _main.lc.FirstOrDefault(c => c.ClientID == _editingClient.ClientID);
                    if (existing != null)
                    {
                        existing.Nom = _editingClient.Nom;
                        existing.Telephone = _editingClient.Telephone;
                    }

                    if (parsedBalance > 0)
                    {
                        var credit = new Credit
                        {
                            ClientID = _editingClient.ClientID,
                            Total = parsedBalance,
                            Paye = 0,
                            Difference = parsedBalance,
                            Etat = true
                        };

                        try
                        {
                            var insertRes = await credit.InsertCreditAsync();
                            _insertedCredit = credit;

                            // Add to MainWindow list
                            if (_main.credits == null)
                                _main.credits = new System.Collections.Generic.List<Credit>();
                            _main.credits.Add(credit);
                        }
                        catch
                        {
                            // ignore insert-credit failure
                        }
                    }
                    WCongratulations wCongratulations=new WCongratulations("Modification Succes", "Client Modifier avec succes",1);
                    wCongratulations.ShowDialog();
                    DialogResult = true;
                    Close();
                }
                else
                {
                    WCongratulations wCongratulations = new WCongratulations("Modification Echoue", "Client n'a pas ete Modifier", 0);
                    wCongratulations.ShowDialog();
                }
            }
            else
            {
                _editingClient.Nom = newName;
                _editingClient.Telephone = newPhone;

                // Save to database
                int newId = await _editingClient.InsertClientAsync();
                if (newId > 0)
                {
                    // Update the client object with the new ID
                    _editingClient.ClientID = newId;
                    _editingClient.Etat = true;

                    // Add to MainWindow list
                    if (_main.lc == null)
                        _main.lc = new System.Collections.Generic.List<Client>();
                    _main.lc.Add(_editingClient);

                    if (parsedBalance > 0)
                    {
                        var credit = new Credit
                        {
                            ClientID = newId,
                            Total = parsedBalance,
                            Paye = 0,
                            Difference = parsedBalance,
                            Etat = true
                        };

                        try
                        {
                            var insertRes = await credit.InsertCreditAsync();
                            _insertedCredit = credit;

                            // Add to MainWindow list
                            if (_main.credits == null)
                                _main.credits = new System.Collections.Generic.List<Credit>();
                            _main.credits.Add(credit);
                        }
                        catch
                        {
                            // ignore - we still added the client
                        }
                    }

                    //MessageBox.Show("Client added.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    //DialogResult = true;
                    //Close();

                    WCongratulations wCongratulations = new WCongratulations("Ajout Succes", "Client Ajouter avec succes", 1);
                    wCongratulations.ShowDialog();
                }
                else
                {
                    //MessageBox.Show("Insert failed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    WCongratulations wCongratulations = new WCongratulations("Ajout Echoue", "Client n'a pas ete Ajouter", 0);
                    wCongratulations.ShowDialog();
                }
            }
        }

        /// <summary>
        /// This method is kept for backward compatibility but is no longer needed
        /// since we update lists directly in UpdateButton_Click
        /// </summary>
        public async Task SaveClientToDatabaseAndList()
        {
            // No longer needed - kept for compatibility
            await Task.CompletedTask;
        }

    }
}