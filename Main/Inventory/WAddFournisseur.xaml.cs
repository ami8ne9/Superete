using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace GestionComerce.Main.Inventory
{
    /// <summary>
    /// Logique d'interaction pour WAddFournisseur.xaml
    /// </summary>
    public partial class WAddFournisseur : Window
    {
        public WAddFournisseur(List<Fournisseur> lfo,WAddArticle main)
        {
            InitializeComponent();
            this.lfo = lfo;
            this.main = main;
        }
        List<Fournisseur> lfo; WAddArticle main;
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if(NameTextBox.Text.Length==0 || PhoneTextBox.Text.Length == 0)
            {
                MessageBox.Show("Remplisser tout les champs s'il vou plais.");
                return;
            }
            foreach(Fournisseur f in lfo)
            {
                if(f.Nom.ToLower()==NameTextBox.Text.ToLower())
                {
                    MessageBox.Show("Ce fournisseur existe déja.");
                    return;
                }
                if(f.Telephone==PhoneTextBox.Text)
                {
                    MessageBox.Show("Ce numéro de téléphone est déja utilisé.");
                    return;
                }
            }
            Fournisseur fo = new Fournisseur();
            fo.Nom = NameTextBox.Text;  
            fo.Telephone = PhoneTextBox.Text;
            int id=await fo.InsertFournisseurAsync();
            fo.FournisseurID = id;
            lfo.Add(fo);
            main.LoadFournisseurs(lfo,1);
            this.Close();

        }

        private static readonly Regex _regex = new Regex("^[0-9]+$"); // Only numbers

        private void Quantite_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !_regex.IsMatch(e.Text);
        }

        private void Quantite_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!_regex.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
