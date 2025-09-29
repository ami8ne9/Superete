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
    /// Logique d'interaction pour CMainC.xaml
    /// </summary>
    public partial class CMainC : UserControl
    {
        public CMainC(User u,MainWindow main,List<Client> lfo)
        {
            InitializeComponent();
            this.u = u;
            this.main = main;  
            this.lfo= lfo;
            LoadClients();
            
        }
        User u;MainWindow main;List<Client> lfo;
        public void LoadClients()
        {
            ClientContainer.Children.Clear();
            foreach (Client fo in lfo)
            {
                CSingleClient ar = new CSingleClient(fo, lfo, this);
                ClientContainer.Children.Add(ar);
            }
        }
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) { }
        private void AddNewClient_Click(object sender, RoutedEventArgs e) {
            WAddClient wAddClient = new WAddClient();
            wAddClient.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            main.load_main(u);
        }
    }
}
