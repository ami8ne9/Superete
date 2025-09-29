using System;
using System.Collections;
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
    /// Logique d'interaction pour WAddArticle.xaml
    /// </summary>
    public partial class WAddArticle : Window
    {
        public WAddArticle(Article a, List<Article> la, List<Famille> lf, List<Fournisseur> lfo,CMainI main, int s, WExistingArticles ea)
        {
           
            InitializeComponent();
            this.a = a;
            this.la = la;   
            this.lf = lf;
            this.lfo = lfo;
            this.s = s;
            this.main = main;
            this.ea = ea;
            LoadFamillies(lf);
            LoadFournisseurs(lfo);
            FournisseurList.SelectedIndex = 0;
            FamilliesList.SelectedIndex = 0;
            List<Fournisseur> lfoo = new List<Fournisseur>();
            
            if (s == 0)
            {
                ButtonsContainer.Visibility = Visibility.Collapsed;
                EnregistrerButton.Visibility = Visibility.Visible;
                HeaderText.Text = "Modifier Article Existante";
                Code.Text= a.Code.ToString();
                ArticleName.Text = a.ArticleName;
                PrixV.Text = a.PrixVente.ToString("0.00");
                PrixA.Text = a.PrixAchat.ToString("0.00");
                PrixMP.Text = a.PrixMP.ToString("0.00");
                Quantite.Text = a.Quantite.ToString();
                foreach (Article ar in la)
                {

                    if (ar.ArticleName == a.ArticleName)
                    {
                        foreach (Fournisseur fo in lfo)
                        {
                            if (fo.FournisseurID == ar.FournisseurID && ar.FournisseurID != a.FournisseurID)
                            {
                                lfoo.Add(fo);
                                break;
                            }
                        }

                    }
                }
                LoadFournisseurs(lfo.Except(lfoo).ToList());
                foreach (Famille f in lf)
                    if (f.FamilleID == a.FamillyID)
                    {
                        FamilliesList.SelectedItem = f.FamilleName;
                        break;
                    }
                foreach (Fournisseur fo in lfo)
                {
                    if (fo.FournisseurID == a.FournisseurID)
                    {
                        FournisseurList.SelectedItem = fo.Nom;
                        break;
                    }
                }
            }

            if (s == 2)
            {
                AjouterFamille.Visibility = Visibility.Collapsed;
                Code.IsEnabled = false;
                ArticleName.IsEnabled = false;
                FamilliesList.IsEnabled = false;
                HeaderText.Text = "Ajouter Article avec Different Fournisseur";
                Code.Text = a.Code.ToString();
                ArticleName.Text = a.ArticleName;
                PrixV.Text = a.PrixVente.ToString("0.00");
                PrixA.Text = a.PrixAchat.ToString("0.00");
                PrixMP.Text = a.PrixMP.ToString("0.00");
                foreach (Article ar in la)
                {
                    
                    if (ar.ArticleName == a.ArticleName)
                    {
                        foreach (Fournisseur fo in lfo)
                        {
                            if (fo.FournisseurID == ar.FournisseurID)
                            {
                                lfoo.Add(fo);
                                break;
                            }
                        }

                    }
                }
                LoadFournisseurs(lfo.Except(lfoo).ToList());
                foreach (Famille f in lf)
                    if (f.FamilleID == a.FamillyID)
                    {
                        FamilliesList.SelectedItem = f.FamilleName;
                        break;
                    }
                FournisseurList.SelectedIndex = 0;
            }
        }
        public Article a; public List<Article> la; List<Famille> lf; List<Fournisseur> lfo; int s; public CMainI main; public WExistingArticles ea;

        private void AddSupplierButton_Click(object sender, RoutedEventArgs e)
        {
            WAddFournisseur wAddFournisseur = new WAddFournisseur(lfo, this);
            wAddFournisseur.ShowDialog();

        }
        public void LoadFournisseurs(List<Fournisseur> lfo)
        {
            FournisseurList.Items.Clear();
            foreach (Fournisseur fo in lfo)
            {
                FournisseurList.Items.Add(fo.Nom);
            }
        }
        public void LoadFamillies(List<Famille> lf)
        {
            FamilliesList.Items.Clear();
            foreach (Famille f in lf)
            {
                FamilliesList.Items.Add(f.FamilleName);
            }
        }

        private async void EnregistrerButton_Click(object sender, RoutedEventArgs e)
        {
            if (ArticleName.Text=="" || Code.Text=="" || PrixV.Text=="" || PrixA.Text=="" || PrixMP.Text=="" || Quantite.Text=="")
            {
                MessageBox.Show("Veuillez remplir tous les champs.");
                return;
            }
            a.Code = Convert.ToInt32(Code.Text);
            a.ArticleName = ArticleName.Text;
            a.PrixVente = Convert.ToDecimal(PrixV.Text);
            a.PrixAchat = Convert.ToDecimal(PrixA.Text);
            a.PrixMP = Convert.ToDecimal(PrixMP.Text);
            if (a.PrixVente < a.PrixAchat)
            {
                MessageBox.Show("Le prix de vente doit être Superieure ou égal au prix d'achat.");
                return;
            }
            if (a.PrixVente < a.PrixMP)
            {
                MessageBox.Show("Le prix de vente doit être Superieure ou égal au prix mp.");
                return;
            }
            if (a.PrixAchat > a.PrixMP)
            {
                MessageBox.Show("Le prix mp doit être Superieure ou égal au prix d'achat.");
                return;
            }
            if (a.Quantite != Convert.ToInt32(Quantite.Text))
            {
                Operation Operation = new Operation();
                Operation.OperationType = "ModificationQu";
                Operation.PrixOperation = (a.Quantite - Convert.ToInt32(Quantite.Text))*a.PrixAchat;

                Operation.UserID = main.u.UserID;

                int idd = await Operation.InsertOperationAsync();
                OperationArticle ofa = new OperationArticle();
                ofa.ArticleID = a.ArticleID;
                ofa.OperationID = idd;
                ofa.QteArticle = Convert.ToInt32(a.Quantite);
                await ofa.InsertOperationArticleAsync();
                a.Quantite = Convert.ToInt32(Quantite.Text);

            }
            
            
            foreach (Famille f in lf)
                if (f.FamilleName == FamilliesList.SelectedItem)
                {
                    a.FamillyID = f.FamilleID;
                    break;
                }
            foreach (Fournisseur fo in lfo)
            {
                
                if (fo.Nom == FournisseurList.SelectedItem)
                {
                    a.FournisseurID = fo.FournisseurID;
                    
                    break;
                }
            }
            var filtered = la.Where(x => x != a).ToList();
            foreach (Article ar in filtered)
            {
                if (ar.ArticleName == a.ArticleName)
                {
                    MessageBox.Show("Ce Nom d'Article Exist deja, si vous voules enregistrer le meme article sous un different fournisseur clickez sur le boutton ajouter 'article existant sous nouveau fournisseur' .");
                    return;
                }
                if (ar.Code == a.Code)
                {
                    MessageBox.Show("Ce Code d'Article Exist deja, si vous voules enregistrer le meme article sous un different fournisseur clickez sur le boutton ajouter 'article existant sous nouveau fournisseur' ..");
                    return;
                }
            }
            a.UpdateArticleAsync();
            for (int i = 0; i < la.Count; i++)
            {
                if (la[i].ArticleID == a.ArticleID)
                {
                    la[i] = a;
                    break;
                }
            }
            main.LoadArticles(la);
            WCongratulations wCongratulations = new WCongratulations(1);
            wCongratulations.Show();

            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddFamillyButton_Click(object sender, RoutedEventArgs e)
        {
            WAddFamille wAddFamille= new WAddFamille(lf,this,new Famille(),null,0);
            wAddFamille.ShowDialog();
        }

        private void IntegerTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Autorise uniquement les chiffres
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void DecimalTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            // Autorise les chiffres et un seul point
            if (e.Text == ".")
            {
                // Refuse si déjà un point
                e.Handled = textBox.Text.Contains(".");
            }
            else
            {
                e.Handled = !e.Text.All(char.IsDigit);
            }
        }

        // Pour empêcher le collage de texte invalide
        private void IntegerTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!text.All(char.IsDigit))
                    e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void DecimalTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                // Autorise un seul point et le reste chiffres
                int dotCount = text.Count(c => c == '.');
                if (dotCount > 1 || text.Any(c => !char.IsDigit(c) && c != '.'))
                    e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void CashButton_Click(object sender, RoutedEventArgs e)
        {
            if (ArticleName.Text == "" || Code.Text == "" || PrixV.Text == "" || PrixA.Text == "" || PrixMP.Text == "" || Quantite.Text == "")
            {
                MessageBox.Show("Veuillez remplir tous les champs.");
                return;
            }
            a.Code = Convert.ToInt32(Code.Text);
            a.ArticleName = ArticleName.Text;
            a.PrixVente = Convert.ToDecimal(PrixV.Text);
            a.PrixAchat = Convert.ToDecimal(PrixA.Text);
            a.PrixMP = Convert.ToDecimal(PrixMP.Text);
            a.Quantite = Convert.ToInt32(Quantite.Text);
            if (a.PrixVente < a.PrixAchat)
            {
                MessageBox.Show("Le prix de vente doit être Superieure ou égal au prix d'achat.");
                return;
            }
            if (a.PrixVente < a.PrixMP)
            {
                MessageBox.Show("Le prix de vente doit être Superieure ou égal au prix mp.");
                return;
            }
            if (a.PrixAchat > a.PrixMP)
            {
                MessageBox.Show("Le prix mp doit être Superieure ou égal au prix d'achat.");
                return;
            }

            foreach (Famille f in lf)
                if (f.FamilleName == FamilliesList.SelectedItem)
                {
                    a.FamillyID = f.FamilleID;
                    break;
                }
            foreach (Fournisseur fo in lfo)
            {

                if (fo.Nom == FournisseurList.SelectedItem)
                {
                    a.FournisseurID = fo.FournisseurID;

                    break;
                }
            }
            if (s != 2)
            {
                foreach (Article ar in la)
                {
                    if (ar.ArticleName == a.ArticleName)
                    {
                        MessageBox.Show("Ce Nom d'Article Exist deja, si vous voules enregistrer le meme article sous un different fournisseur clickez sur le boutton ajouter 'article existant sous nouveau fournisseur' .");
                        return;
                    }
                    if (ar.Code == a.Code)
                    {
                        MessageBox.Show("Ce Code d'Article Exist deja, si vous voules enregistrer le meme article sous un different fournisseur clickez sur le boutton ajouter 'article existant sous nouveau fournisseur' ..");
                        return;
                    }
                }
            }
            
            WConfirmTransaction w = new WConfirmTransaction(this,null,a,0);
            w.ShowDialog();
        }

        private void HalfButton_Click(object sender, RoutedEventArgs e)
        {
            if (ArticleName.Text == "" || Code.Text == "" || PrixV.Text == "" || PrixA.Text == "" || PrixMP.Text == "" || Quantite.Text == "")
            {
                MessageBox.Show("Veuillez remplir tous les champs.");
                return;
            }
            a.Code = Convert.ToInt32(Code.Text);
            a.ArticleName = ArticleName.Text;
            a.PrixVente = Convert.ToDecimal(PrixV.Text);
            a.PrixAchat = Convert.ToDecimal(PrixA.Text);
            a.PrixMP = Convert.ToDecimal(PrixMP.Text);
            a.Quantite = Convert.ToInt32(Quantite.Text);
            if (a.PrixVente < a.PrixAchat)
            {
                MessageBox.Show("Le prix de vente doit être Superieure ou égal au prix d'achat.");
                return;
            }
            if (a.PrixVente < a.PrixMP)
            {
                MessageBox.Show("Le prix de vente doit être Superieure ou égal au prix mp.");
                return;
            }
            if (a.PrixAchat > a.PrixMP)
            {
                MessageBox.Show("Le prix mp doit être Superieure ou égal au prix d'achat.");
                return;
            }

            foreach (Famille f in lf)
                if (f.FamilleName == FamilliesList.SelectedItem)
                {
                    a.FamillyID = f.FamilleID;
                    break;
                }
            foreach (Fournisseur fo in lfo)
            {

                if (fo.Nom == FournisseurList.SelectedItem)
                {
                    a.FournisseurID = fo.FournisseurID;

                    break;
                }
            }
            if (s != 2)
            {
                foreach (Article ar in la)
                {
                    if (ar.ArticleName == a.ArticleName)
                    {
                        MessageBox.Show("Ce Nom d'Article Exist deja, si vous voules enregistrer le meme article sous un different fournisseur clickez sur le boutton ajouter 'article existant sous nouveau fournisseur' .");
                        return;
                    }
                    if (ar.Code == a.Code)
                    {
                        MessageBox.Show("Ce Code d'Article Exist deja, si vous voules enregistrer le meme article sous un different fournisseur clickez sur le boutton ajouter 'article existant sous nouveau fournisseur' ..");
                        return;
                    }
                }
            }
            WConfirmTransaction w = new WConfirmTransaction(this, null, a, 1);
            w.ShowDialog();
        }

        private void CreditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ArticleName.Text == "" || Code.Text == "" || PrixV.Text == "" || PrixA.Text == "" || PrixMP.Text == "" || Quantite.Text == "")
            {
                MessageBox.Show("Veuillez remplir tous les champs.");
                return;
            }
            a.Code = Convert.ToInt32(Code.Text);
            a.ArticleName = ArticleName.Text;
            a.PrixVente = Convert.ToDecimal(PrixV.Text);
            a.PrixAchat = Convert.ToDecimal(PrixA.Text);
            a.PrixMP = Convert.ToDecimal(PrixMP.Text);
            a.Quantite = Convert.ToInt32(Quantite.Text);
            if (a.PrixVente < a.PrixAchat)
            {
                MessageBox.Show("Le prix de vente doit être Superieure ou égal au prix d'achat.");
                return;
            }
            if (a.PrixVente < a.PrixMP)
            {
                MessageBox.Show("Le prix de vente doit être Superieure ou égal au prix mp.");
                return;
            }
            if (a.PrixAchat > a.PrixMP)
            {
                MessageBox.Show("Le prix mp doit être Superieure ou égal au prix d'achat.");
                return;
            }

            foreach (Famille f in lf)
                if (f.FamilleName == FamilliesList.SelectedItem)
                {
                    a.FamillyID = f.FamilleID;
                    break;
                }
            foreach (Fournisseur fo in lfo)
            {

                if (fo.Nom == FournisseurList.SelectedItem)
                {
                    a.FournisseurID = fo.FournisseurID;

                    break;
                }
            }
            if (s != 2)
            {
                foreach (Article ar in la)
                {
                    if (ar.ArticleName == a.ArticleName)
                    {
                        MessageBox.Show("Ce Nom d'Article Exist deja, si vous voules enregistrer le meme article sous un different fournisseur clickez sur le boutton ajouter 'article existant sous nouveau fournisseur' .");
                        return;
                    }
                    if (ar.Code == a.Code)
                    {
                        MessageBox.Show("Ce Code d'Article Exist deja, si vous voules enregistrer le meme article sous un different fournisseur clickez sur le boutton ajouter 'article existant sous nouveau fournisseur' ..");
                        return;
                    }
                }
            }
            WConfirmTransaction w = new WConfirmTransaction(this, null,a, 2);
            w.ShowDialog();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
