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
    /// Logique d'interaction pour CSingleFournisseur.xaml
    /// </summary>
    public partial class CSingleFournisseur : UserControl
    {
        public CSingleFournisseur(Fournisseur fo,List<Fournisseur> lfo,CMainF main)
        {
            InitializeComponent();
            this.fo = fo; this.lfo = lfo; this.main = main;
            FournisseurID.Text= fo.FournisseurID.ToString();
            Nom.Text= fo.Nom;
            Telephone.Text= fo.Telephone;
        }
        Fournisseur fo; List<Fournisseur> lfo;CMainF main;
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            WAddFournisseur wAddFournisseur = new WAddFournisseur();
            wAddFournisseur.ShowDialog();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            WDeleteFournisseur wDeleteFournisseur = new WDeleteFournisseur();
            wDeleteFournisseur.ShowDialog();
        }

        private void Paid_Click(object sender, RoutedEventArgs e)
        {
            WPayeFournisseur wPayeFournisseur = new WPayeFournisseur();
            wPayeFournisseur.ShowDialog();
        }

        private void Operations_Click(object sender, RoutedEventArgs e)
        {
            WHistoryOperation wHistoryOperation = new WHistoryOperation();
            wHistoryOperation.ShowDialog();
        }
    }
}
