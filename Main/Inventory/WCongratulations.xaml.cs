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

namespace Superete.Main.Inventory
{
    /// <summary>
    /// Interaction logic for WCongratulations.xaml
    /// </summary>
    public partial class WCongratulations : Window
    {
        public WCongratulations(int s)
        {
            InitializeComponent();
            if (s == 1)
            {
                Header.Text = "Modification Effectue";
                MessageText.Text = "Votre article a ete modifier";
            }
            else if(s == 2) {

                Header.Text = "Suppresion Effectue";
                MessageText.Text = "Votre article a ete supprimmer";
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window win in Application.Current.Windows.Cast<Window>().ToList())
            {
                if (win != Application.Current.MainWindow) // keep main window open
                {
                    win.Close();
                }
            }
        }
    }
}
