using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Superete.Main.FournisseurPage
{
    public partial class DeleteSupplierWindow : Window
    {
        private readonly MainWindow _mainWindow;
        private readonly Fournisseur _supplier;

        public DeleteSupplierWindow(MainWindow mainWindow, Fournisseur supplier)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _supplier = supplier;
            SupplierNameLabel.Text = supplier.Nom;
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            // Soft delete: set Etat=0 in DB
            var f = new Fournisseur { FournisseurID = _supplier.FournisseurID };
            int res = await f.DeleteFournisseurAsync();

            if (res > 0)
            {
                // Update the list in MainWindow
                var supplierInList = _mainWindow.lfo.FirstOrDefault(x => x.FournisseurID == _supplier.FournisseurID);
                if (supplierInList != null)
                {
                    supplierInList.Etat = false;
                }
                WCongratulations wCongratulations = new WCongratulations("Suppression Succes", "Fournisseur Supprimer avec succes", 1);
                wCongratulations.ShowDialog();
                //MessageBox.Show("Operation failed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                
                //MessageBox.Show("Supplier hidden (soft deleted).", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
                //DialogResult = true;
                //Close();
            }
            else
            {
                //MessageBox.Show("Operation failed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                WCongratulations wCongratulations = new WCongratulations("Suppression Echoue", "Fournisseur n'a pas ete Supprimer", 0);
                wCongratulations.ShowDialog();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}