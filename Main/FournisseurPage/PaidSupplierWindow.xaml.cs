using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Superete.Main.FournisseurPage
{
    public partial class PaidSupplierWindow : Window
    {
        private User _currentUser;
        private readonly MainWindow _mainWindow;
        private readonly Fournisseur _supplier;
        private Credit[] _supplierCredits;

        public PaidSupplierWindow(User u,MainWindow mainWindow, Fournisseur supplier)
        {
            InitializeComponent();
            _currentUser = u;
            _mainWindow = mainWindow;
            _supplier = supplier;
            Loaded += PaidSupplierWindow_Loaded;
        }

        private void PaidSupplierWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SupplierNameLabel.Text = _supplier.Nom;
            LoadCredits();
        }

        private int GetCurrentUserId()
        {
           
            return _currentUser.UserID;

        }

        private void LoadCredits()
        {
            // Load from MainWindow credit list instead of database
            _supplierCredits = _mainWindow.credits
                .Where(c => c.FournisseurID == _supplier.FournisseurID && c.Etat)
                .ToArray();

            decimal total = _supplierCredits.Sum(c => c.Total);
            decimal paid = _supplierCredits.Sum(c => c.Paye);
            decimal diff = _supplierCredits.Sum(c => c.Difference);

            TotalCreditLabel.Text = $"{total:N2} DH";
            TotalPaidLabel.Text = $"{paid:N2} DH";
            DifferenceLabel.Text = $"{diff:N2} DH";
            RemainingBalanceLabel.Text = $"{diff:N2} DH";
        }

        private void PayMaxButton_Click(object sender, RoutedEventArgs e)
        {
            decimal diff = _supplierCredits.Sum(c => c.Difference);
            PaymentAmountTextBox.Text = diff.ToString("F2");
            RemainingBalanceLabel.Text = "0.00 DH";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();
        private async Task CreatePaymentOperationAsync(decimal paidAmount,int credidID)
        {
            try
            {
                var op = new Operation
                {
                    ClientID = null,
                    FournisseurID = _supplier.FournisseurID,
                    CreditID = credidID,
                    PrixOperation = paidAmount,
                    Remise = 0m,
                    CreditValue = -paidAmount,
                    UserID = GetCurrentUserId(),
                    DateOperation = DateTime.Now,
                    OperationType = "SUPPLIER_PAYMENT",
                    //Reversed = false,
                    Etat = true
                };

                int opId = await op.InsertOperationAsync();

                if (opId > 0)
                {
                    op.OperationID = opId;
                    _mainWindow.lo.Add(op);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Payment saved but failed to record operation: {ex.Message}",
                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private async void ProcessPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(PaymentAmountTextBox.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Enter a valid payment amount.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal remaining = amount;
            int creditId=0;

            // Apply to oldest credits first (by CreditID)
            foreach (var credit in _supplierCredits.OrderBy(c => c.CreditID))
            {
                if (remaining <= 0) break;
                if (credit.Difference <= 0) continue;
                
                decimal apply = Math.Min(credit.Difference, remaining);
                credit.Paye += apply;
                credit.Difference = credit.Total - credit.Paye;
                remaining -= apply;
                creditId = credit.CreditID;

                // Persist to database
                await credit.UpdateCreditAsync();

                // Update the credit in MainWindow list
                var creditInList = _mainWindow.credits.FirstOrDefault(c => c.CreditID == credit.CreditID);
                if (creditInList != null)
                {
                    creditInList.Paye = credit.Paye;
                    creditInList.Difference = credit.Difference;
                }
            }
            await CreatePaymentOperationAsync(amount, creditId);

            if (remaining > 0)
            {
                MessageBox.Show($"Payment processed, remaining amount {remaining:N2} DH was not applied (no outstanding credit).", "Note", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Payment processed.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            DialogResult = true;
            Close();

        }
    }
}