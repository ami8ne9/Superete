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

namespace GestionComerce.Main.Vente
{
    /// <summary>
    /// Interaction logic for WAddCleint.xaml
    /// </summary>
    public partial class WAddCleint : Window
    {
        public WAddCleint(List<Client> lc,WSelectCient sc)
        {
            InitializeComponent();
            this.lc = lc;
            this.sc = sc;
        }
        List<Client> lc;
        WSelectCient sc;
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if(Name.Text == "" || Telephone.Text == "")
            {
                MessageBox.Show("Veuillez remplir tous les champs.");
                return;
            }
            foreach(Client nc in lc)
            {
                if(nc.Nom==Name.Text)
                {
                    MessageBox.Show("Ce nom de client existe déjà.");
                    return;
                }else if (nc.Telephone == Telephone.Text)
                {
                    MessageBox.Show("Ce numero de telephone est deja ascosie a un client.");
                    return;
                }
            }
            Client c = new Client();
            c.Nom = Name.Text;
            c.Telephone = Telephone.Text;
            c.ClientID=await c.InsertClientAsync();
            List<Client> lcc=lc;
            lcc.Add(c);
            sc.LoadClients();
            this.Close();
        }

        private void ClientNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Telephone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Only allow digits
            e.Handled = !e.Text.All(char.IsDigit);
        }
    }
}
