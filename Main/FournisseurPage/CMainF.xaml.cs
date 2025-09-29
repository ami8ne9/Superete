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

namespace Superete.Main.FournisseurPage
{
    /// <summary>
    /// Logique d'interaction pour CMainF.xaml
    /// </summary>
    public partial class CMainF : UserControl
    {
        public CMainF(User u,MainWindow main,List<Fournisseur> lfo)
        {
            InitializeComponent();
            this.u = u;
            this.main = main;  
            this.lfo= lfo;
            LoadFournisseurs();
            
        }
        User u;MainWindow main;List<Fournisseur> lfo;
        public void LoadFournisseurs()
        {
            FournisseurContainer.Children.Clear();
            foreach (Fournisseur fo in lfo)
            {
                CSingleFournisseur ar = new CSingleFournisseur(fo, lfo, this);
                FournisseurContainer.Children.Add(ar);
            }
        }
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) { }
        private void AddNewFournisseur_Click(object sender, RoutedEventArgs e) {
            WAddFournisseur wAddFournisseur = new WAddFournisseur();
            wAddFournisseur.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            main.load_main(u);
        }
    }
}
