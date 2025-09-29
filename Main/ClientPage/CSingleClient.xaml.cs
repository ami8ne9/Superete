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

namespace Superete.Main.ClientPage
{
    /// <summary>
    /// Logique d'interaction pour CSingleClient.xaml
    /// </summary>
    public partial class CSingleClient : UserControl
    {
        public CSingleClient(Client fo,List<Client> lfo,CMainC main)
        {
            InitializeComponent();
            this.fo = fo; this.lfo = lfo; this.main = main;
            ClientID.Text= fo.ClientID.ToString();
            Nom.Text= fo.Nom;
            Telephone.Text= fo.Telephone;
        }
        Client fo; List<Client> lfo;CMainC main;
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            WAddClient wAddClient = new WAddClient();
            wAddClient.ShowDialog();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            WDeleteClient wDeleteClient = new WDeleteClient();
            wDeleteClient.ShowDialog();
        }

        private void Paid_Click(object sender, RoutedEventArgs e)
        {
            WPayeClient wPayeClient = new WPayeClient();
            wPayeClient.ShowDialog();
        }

        private void Operations_Click(object sender, RoutedEventArgs e)
        {
            WHistoryOperationC WHistoryOperationC = new WHistoryOperationC();
            WHistoryOperationC.ShowDialog();
        }
    }
}
