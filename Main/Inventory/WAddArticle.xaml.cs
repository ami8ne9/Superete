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
        public WAddArticle(Article a, List<Article> la, List<Famille> lf, List<Fournisseur> lfo,CMainI main, int s, WExistingArticles ea, WNouveauStock ns)
        {
           
            InitializeComponent();
            this.a = a;
            this.la = la;   
            this.lf = lf;
            this.lfo = lfo;
            this.s = s;
            this.main = main;
            this.ea = ea;
            this.ns = ns;
            LoadFamillies(lf);
            LoadFournisseurs(lfo);

            LoadPayments(main.main.lp);
            foreach (Role r in main.main.lr)
            {
                if (main.u.RoleID == r.RoleID)
                {
                    if (r.AddFamilly == false)
                    {
                        AjouterFamille.IsEnabled = false;
                    }
                    if (r.CreateFournisseur == false)
                    {
                        AjouterFournisseur.IsEnabled = false;
                    }
                    break;
                }
            }
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

            if (s == 5)
            {
                ButtonsContainer.Visibility = Visibility.Collapsed;
                EnregistrerButton.Visibility = Visibility.Collapsed;
                AjouterButton.Visibility = Visibility.Visible;
                foreach (Fournisseur fo in lfo)
                {
                    if (fo.FournisseurID == ns.AMA.fo.FournisseurID)
                    {
                        FournisseurList.SelectedItem = fo.Nom;
                        break;
                    }
                }
                FournisseurList.IsEnabled = false;
                AjouterFournisseur.Visibility = Visibility.Collapsed;
            }

            if (s == 2)
            {
                //AjouterFamille.Visibility = Visibility.Collapsed;
                //Code.IsEnabled = false;
                //ArticleName.IsEnabled = false;
                //FamilliesList.IsEnabled = false;
                //HeaderText.Text = "Ajouter Article avec Different Fournisseur";
                //Code.Text = a.Code.ToString();
                //ArticleName.Text = a.ArticleName;
                //PrixV.Text = a.PrixVente.ToString("0.00");
                //PrixA.Text = a.PrixAchat.ToString("0.00");
                //PrixMP.Text = a.PrixMP.ToString("0.00");
                //foreach (Article ar in la)
                //{
                    
                //    if (ar.ArticleName == a.ArticleName)
                //    {
                //        foreach (Fournisseur fo in lfo)
                //        {
                //            if (fo.FournisseurID == ar.FournisseurID)
                //            {
                //                lfoo.Add(fo);
                //                break;
                //            }
                //        }

                //    }
                //}
                //LoadFournisseurs(lfo.Except(lfoo).ToList());
                //foreach (Famille f in lf)
                //    if (f.FamilleID == a.FamillyID)
                //    {
                //        FamilliesList.SelectedItem = f.FamilleName;
                //        break;
                //    }
                //FournisseurList.SelectedIndex = 0;
            }
        }
        public Article a; public List<Article> la; List<Famille> lf; List<Fournisseur> lfo; int s; public CMainI main; public WExistingArticles ea; WNouveauStock ns;

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
        public void LoadPayments(List<PaymentMethod> lp)
        {
            PaymentMethodComboBox.Items.Clear();
            foreach (PaymentMethod a in lp)
            {
                PaymentMethodComboBox.Items.Add(a.PaymentMethodName);
            }
        }

        private async void EnregistrerButton_Click(object sender, RoutedEventArgs e)
        {
            bool modifier= false;
            if (ArticleName.Text=="" || Code.Text=="" || PrixV.Text=="" || PrixA.Text=="" || PrixMP.Text=="" || Quantite.Text == "")
            {
                MessageBox.Show("Veuillez remplir tous les champs.");
                return;
            }
            if(ArticleName.Text != a.ArticleName || Code.Text != a.Code.ToString() || PrixV.Text != a.PrixVente.ToString() || PrixA.Text != a.PrixAchat.ToString() || PrixMP.Text != a.PrixMP.ToString() || Quantite.Text != a.Quantite.ToString())
            {
                modifier = true;
            }
            if (Convert.ToDecimal(PrixV.Text) < Convert.ToDecimal(PrixA.Text))
            {
                MessageBox.Show("Le prix de vente doit être Superieure ou égal au prix d'achat.");
                return;
            }
            if (Convert.ToDecimal(PrixV.Text)< Convert.ToDecimal(PrixMP.Text))
            {
                MessageBox.Show("Le prix de vente doit être Superieure ou égal au prix mp.");
                return;
            }
            if (Convert.ToDecimal(PrixA.Text) > Convert.ToDecimal(PrixMP.Text))
            {
                MessageBox.Show("Le prix mp doit être Superieure ou égal au prix d'achat.");
                return;
            }
            if (Convert.ToInt32(Quantite.Text) == 0)
            {
                MessageBox.Show("Donner une quntite.");
                return;
            }
            if (ArticleName.Text != a.ArticleName && Code.Text == a.Code.ToString())
            {
                MessageBoxResult result = MessageBox.Show(
                    "Changer le nom de cet article changera le nom de tous les articles avec le même code, voulez-vous continuer?",  
                    "Confirmation",  
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question 
                );

                if (result == MessageBoxResult.Yes)
                {
                    foreach(Article ar in la)
                    {
                        if (ar.Code == a.Code)
                        {
                            ar.ArticleName = ArticleName.Text;
                            ar.UpdateArticleAsync();
                        }
                    }
                }
                else if (result == MessageBoxResult.No)
                {
                    return;
                }

            }
            if (Code.Text != a.Code.ToString() && ArticleName.Text == a.ArticleName)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Changer le Code de cet article changera le Code de tous les articles avec le même nom, voulez-vous continuer?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    foreach (Article ar in la)
                    {
                        if (ar.ArticleName == a.ArticleName)
                        {
                            ar.Code = Convert.ToInt32(Code.Text);
                            ar.UpdateArticleAsync();
                        }
                    }
                }
                else if (result == MessageBoxResult.No)
                {
                    return;
                }

            }
            a.Code = Convert.ToInt32(Code.Text);
            a.ArticleName = ArticleName.Text;
            a.PrixVente = Convert.ToDecimal(PrixV.Text);
            a.PrixAchat = Convert.ToDecimal(PrixA.Text);
            a.PrixMP = Convert.ToDecimal(PrixMP.Text);
            
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
                    if (a.FamillyID != f.FamilleID)
                    {
                        modifier = true;
                        a.FamillyID = f.FamilleID;
                    }

                    break;
                }
            foreach (Fournisseur fo in lfo)
            {
                
                if (fo.Nom == FournisseurList.SelectedItem)
                {
                    if (a.FournisseurID != fo.FournisseurID)
                    {
                        modifier = true;
                        a.FournisseurID = fo.FournisseurID;
                    }
                        
                    
                    break;
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
            if (modifier == true)
            {
                main.LoadArticles(la);
                WCongratulations wCongratulations = new WCongratulations(1);
                wCongratulations.Show();
            }
                

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
            if (a.Quantite == 0)
            {
                MessageBox.Show("Donner une quntite.");
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
            foreach (Article aa in la)
            {
                if (aa.ArticleName.ToLower() == a.ArticleName.ToLower() && aa.Code != a.Code)
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Le Nom de ce Article exist avec un different Code Vous voulez pozer le code de cette Article?",     
                        "Confirmation",                       
                        MessageBoxButton.YesNo,          
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        Code.Text=aa.Code.ToString();
                        a.Code = aa.Code;
                    }
                    return;
                }
                if (aa.ArticleName.ToLower() != a.ArticleName.ToLower() && aa.Code == a.Code)
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Le Code de ce Article exist avec un different Nom Vous voulez pozer le Nom de cette Article?",
                        "Confirmation",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        ArticleName.Text = aa.ArticleName.ToString();
                        a.ArticleName = aa.ArticleName;
                    }
                    return;
                }

                if (aa.ArticleName.ToLower() == a.ArticleName.ToLower() && aa.Code == a.Code)
                {
                    if(aa.FournisseurID == a.FournisseurID)
                    {
                        MessageBox.Show("Ce Article deja exist sous ce fournisseur.");
                        return;
                    }
                }
            }
            int MethodID = 0;
            foreach (PaymentMethod p in main.main.lp)
            {
                if (p.PaymentMethodName == PaymentMethodComboBox.SelectedValue)
                {
                    MethodID = p.PaymentMethodID;
                }
            }

            WConfirmTransaction w = new WConfirmTransaction(this,null, null, a,0, MethodID);
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
            if (a.Quantite == 0)
            {
                MessageBox.Show("Donner une quntite.");
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
            foreach (Article aa in la)
            {
                if (aa.ArticleName.ToLower() == a.ArticleName.ToLower() && aa.Code != a.Code)
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Le Nom de ce Article exist avec un different Code Vous voulez pozer le code de cette Article?",
                        "Confirmation",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        Code.Text = aa.Code.ToString();
                        a.Code = aa.Code;
                    }
                    return;
                }
                if (aa.ArticleName.ToLower() != a.ArticleName.ToLower() && aa.Code == a.Code)
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Le Code de ce Article exist avec un different Nom Vous voulez pozer le Nom de cette Article?",
                        "Confirmation",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        ArticleName.Text = aa.ArticleName.ToString();
                        a.ArticleName = aa.ArticleName;
                    }
                    return;
                }

                if (aa.ArticleName.ToLower() == a.ArticleName.ToLower() && aa.Code == a.Code)
                {
                    if (aa.FournisseurID == a.FournisseurID)
                    {
                        MessageBox.Show("Ce Article deja exist sous ce fournisseur.");
                        return;
                    }
                }
            }
            int MethodID = 0;
            foreach (PaymentMethod p in ns.main.main.lp)
            {
                if (p.PaymentMethodName == PaymentMethodComboBox.SelectedValue)
                {
                    MethodID = p.PaymentMethodID;
                }
            }
            WConfirmTransaction w = new WConfirmTransaction(this, null, null, a, 1, MethodID);
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
            if (a.Quantite == 0)
            {
                MessageBox.Show("Donner une quntite.");
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
            foreach (Article aa in la)
            {
                if (aa.ArticleName.ToLower() == a.ArticleName.ToLower() && aa.Code != a.Code)
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Le Nom de ce Article exist avec un different Code Vous voulez pozer le code de cette Article?",
                        "Confirmation",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        Code.Text = aa.Code.ToString();
                        a.Code = aa.Code;
                    }
                    return;
                }
                if (aa.ArticleName.ToLower() != a.ArticleName.ToLower() && aa.Code == a.Code)
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Le Code de ce Article exist avec un different Nom Vous voulez pozer le Nom de cette Article?",
                        "Confirmation",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        ArticleName.Text = aa.ArticleName.ToString();
                        a.ArticleName = aa.ArticleName;
                    }
                    return;
                }

                if (aa.ArticleName.ToLower() == a.ArticleName.ToLower() && aa.Code == a.Code)
                {
                    if (aa.FournisseurID == a.FournisseurID)
                    {
                        MessageBox.Show("Ce Article deja exist sous ce fournisseur.");
                        return;
                    }
                }
            }
            int MethodID = 0;
            foreach (PaymentMethod p in main.main.lp)
            {
                if (p.PaymentMethodName == PaymentMethodComboBox.SelectedValue)
                {
                    MethodID = p.PaymentMethodID;
                }
            }
            WConfirmTransaction w = new WConfirmTransaction(this, null, null, a, 2, MethodID);
            w.ShowDialog();
        }
        private void AjouterButton_Click(object sender, RoutedEventArgs e)
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
            if (a.Quantite == 0)
            {
                MessageBox.Show("Donner une quntite.");
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
            foreach (Article aa in la)
            {
                if (aa.ArticleName.ToLower() == a.ArticleName.ToLower() && aa.Code != a.Code)
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Le Nom de ce Article exist avec un different Code Vous voulez pozer le code de cette Article?",
                        "Confirmation",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        Code.Text = aa.Code.ToString();
                        a.Code = aa.Code;
                    }
                    return;
                }
                if (aa.ArticleName.ToLower() != a.ArticleName.ToLower() && aa.Code == a.Code)
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Le Code de ce Article exist avec un different Nom Vous voulez pozer le Nom de cette Article?",
                        "Confirmation",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        ArticleName.Text = aa.ArticleName.ToString();
                        a.ArticleName = aa.ArticleName;
                    }
                    return;
                }

                if (aa.ArticleName.ToLower() == a.ArticleName.ToLower() && aa.Code == a.Code)
                {
                    if (aa.FournisseurID == a.FournisseurID)
                    {
                        MessageBox.Show("Ce Article deja exist sous ce fournisseur.");
                        return;
                    }
                }
            }
            foreach (CSingleRowArticle csar in ns.AMA.ArticlesContainer.Children)
            {
                if (csar.a.ArticleName == a.ArticleName)
                {
                    MessageBox.Show("Vous avez deja ajouter un article avec ce nom");
                    return;
                }
                else if (csar.a.Code == a.Code)
                {
                    MessageBox.Show("Vous avez deja ajouter un article avec ce code");
                    return;
                }
             }
            CSingleRowArticle cSingleRowArticle = new CSingleRowArticle(a, main.la, null, main, 7, ea, ns,0);
            ns.AMA.ArticlesContainer.Children.Add(cSingleRowArticle);
            ns.Close();
            this.Close();

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
