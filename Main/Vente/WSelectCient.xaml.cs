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
using System.Text.RegularExpressions;
using Superete.Main.Inventory;

namespace Superete.Main.Vente
{
    /// <summary>
    /// Interaction logic for WSelectCient.xaml
    /// </summary>
    public partial class WSelectCient : Window
    {
        public WSelectCient(List<Client> lc,CMainV main,int Credit,int MethodID)
        {
            InitializeComponent();
            this.lc = lc;
            LoadClients();
            this.main = main;
            this.Credit = Credit;
            this.MethodID = MethodID;
            if (Credit == 0)
            {
                Creditt.Visibility = Visibility.Collapsed;
            }
            else if (Credit == 1)
            {
                NoClient.Visibility = Visibility.Collapsed;
            }
            else
            {
                Creditt.Visibility = Visibility.Collapsed;
                NoClient.Visibility = Visibility.Collapsed;
            }
            foreach(Role r in main.main.lr)
            {
                if (main.u.RoleID == r.RoleID){
                    if (r.CreateClient == false)
                    {
                        AddClient.IsEnabled = false;
                    }
                    break;
                }
            }
        }
        List<Client> lc;public int selected = 0; CMainV main;int Credit;public int MethodID;

        public void LoadClients()
        {
            ClientContainer.Children.Clear();
            foreach (Client c in lc)
            {
                CSingleClient ar = new CSingleClient(c, this);
                ClientContainer.Children.Add(ar);
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NoClientButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedClient.Text = "Aucun client sélectionné";
            selected = 0;
        }

        private void NewClientButton_Click(object sender, RoutedEventArgs e)
        {
            WAddCleint wAddCleint = new WAddCleint(lc,this);
            wAddCleint.ShowDialog();
        }

        private async void ValidateButton_Click(object sender, RoutedEventArgs e)
        {

            if (Credit == 0)
            {
                Operation Operation = new Operation();
                Operation.OperationType = "VenteCa";
                Operation.PrixOperation = main.TotalNett;
                Operation.PaymentMethodID = MethodID;
                if (Remise.Text != "")
                {
                    Operation.Remise = Convert.ToDecimal(Remise.Text);
                    if (Operation.Remise > Operation.PrixOperation)
                    {
                        MessageBox.Show("la remise est plus grande que le total.");
                        return;
                    }
					
                }
                Operation.UserID = main.u.UserID;
                if (selected == 0)
                {
                    Operation.ClientID = null;
                    int id = await Operation.InsertOperationAsync();
                    foreach (CSingleArticle2 sa2 in main.SelectedArticles.Children)
                    {
                        OperationArticle oca = new OperationArticle();
                        oca.ArticleID = sa2.a.ArticleID;
                        oca.OperationID = id;
                        oca.QteArticle = Convert.ToInt32(sa2.qte);

                        await oca.InsertOperationArticleAsync();
                        sa2.a.Quantite -= Convert.ToInt32(sa2.qte);
                        await sa2.a.UpdateArticleAsync();
                        foreach (Article ar in main.la)
                        {
                            if (ar.ArticleID == sa2.a.ArticleID)
                            {
                                main.la[main.la.IndexOf(ar)].Quantite = sa2.a.Quantite;
                                if (ar.Quantite == 0)
                                {
                                    ar.DeleteArticleAsync();
                                    main.la.Remove(ar);
                                }
                                main.LoadArticles(main.la);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Operation.ClientID = selected;
                    int id = await Operation.InsertOperationAsync();
                    foreach (CSingleArticle2 sa2 in main.SelectedArticles.Children)
                    {
                        OperationArticle oca = new OperationArticle();
                        oca.ArticleID = sa2.a.ArticleID;
                        oca.OperationID = id;
                        oca.QteArticle = Convert.ToInt32(sa2.qte);
                        await oca.InsertOperationArticleAsync();
                        sa2.a.Quantite -= Convert.ToInt32(sa2.qte);
                        await sa2.a.UpdateArticleAsync();
                        foreach (Article ar in main.la)
                        {
                            if (ar.ArticleID == sa2.a.ArticleID)
                            {
                                main.la[main.la.IndexOf(ar)].Quantite = sa2.a.Quantite;
                                main.LoadArticles(main.la);
                                break;
                            }
                        }
                    }
                }
                
            }
            else if (Credit == 1)
            {
                if (selected == 0)
                {
                    MessageBox.Show("Veuillez sélectionner un client pour le crédit.");
                    return;
                }
                if (Credittext.Text == "")
                {
                    MessageBox.Show("Doneer un valeur de credit.");
                    return;
                }

                if (Remise.Text != "")
                {
                    if (Convert.ToDecimal(Credittext.Text) > Convert.ToDecimal(main.TotalNett) - Convert.ToDecimal(Remise.Text))
                    {
                        MessageBox.Show("la valeur de credit est plus grande que le total mois la remise.");
                        return;
                    }
                }
                else
                {
                    if (Convert.ToDecimal(Credittext.Text) > Convert.ToDecimal(main.TotalNett))
                    {
                        MessageBox.Show("la valeur de credit est plus grande que le total.");
                        return;
                    }
                }
                int creditId = 0;
                bool creditExists = false;
                //import Credit
                Credit Credit = new Credit();
                List<Credit> lcc = await Credit.GetCreditsAsync();
                //if there is no credit with client id create a new one
                foreach (Credit cc in lcc)
                {
                    if (cc.ClientID == selected)
                    {

                        cc.Total += Convert.ToDecimal(Credittext.Text);
                        await cc.UpdateCreditAsync();
                        creditExists = true;
                        creditId = cc.CreditID;
                        break;
                    }
                }
                //else update the credit value
                if (!creditExists)
                {
                    Credit newCredit = new Credit();
                    newCredit.ClientID = selected;
                    newCredit.Total = Convert.ToInt32(Credittext.Text);
                    creditId = await newCredit.InsertCreditAsync();

                }

                Operation Operation = new Operation();

                Operation.OperationType = "Vente50";

                Operation.PaymentMethodID = MethodID;
                Operation.PrixOperation = main.TotalNett;
				Operation.CreditValue = Convert.ToDecimal(Credittext.Text);
                
                Operation.CreditID = creditId;
                if (Remise.Text != "")
                {
                    Operation.Remise = Convert.ToDecimal(Remise.Text);
                }

                Operation.UserID = main.u.UserID;
                Operation.ClientID = selected;

                int id = await Operation.InsertOperationAsync();
                foreach (CSingleArticle2 sa2 in main.SelectedArticles.Children)
                {
                    OperationArticle oca = new OperationArticle();
                    oca.ArticleID = sa2.a.ArticleID;
                    oca.OperationID = id;
                    oca.QteArticle = Convert.ToInt32(sa2.qte);
                    await oca.InsertOperationArticleAsync();
                    sa2.a.Quantite -= Convert.ToInt32(sa2.qte);
                    await sa2.a.UpdateArticleAsync();
                    foreach (Article ar in main.la)
                    {
                        if (ar.ArticleID == sa2.a.ArticleID)
                        {
                            main.la[main.la.IndexOf(ar)].Quantite = sa2.a.Quantite;
                            if (ar.Quantite == 0)
                            {
                                ar.DeleteArticleAsync();
                                main.la.Remove(ar);
                            }
                            main.LoadArticles(main.la);
                            break;
                        }
                    }
                }


            }
            else
            {
                if (selected == 0)
                {
                    MessageBox.Show("Veuillez sélectionner un client pour le crédit.");
                    return;
                }
                if (Remise.Text != "")
                {
                    if (Convert.ToDecimal(Remise.Text) > Convert.ToDecimal(main.TotalNett))
                    {
                        MessageBox.Show("la remise est plus grande que le total.");
                        return;
                    }
                }
                int creditId = 0;
                bool creditExists = false;
                //import Credit
                Credit Credit = new Credit();
                List<Credit> lcc = await Credit.GetCreditsAsync();
                Operation Operation = new Operation();

                Operation.PaymentMethodID = MethodID;
                // update the credit value
                foreach (Credit cc in lcc)
                {
                    if (cc.ClientID == selected)
                    {
                        if (Remise.Text != "")
                        {
							cc.Total += Convert.ToDecimal(main.TotalNett) - Convert.ToDecimal(Remise.Text);
                            Operation.CreditValue = main.TotalNett - Convert.ToDecimal(Remise.Text);
                        }
                        else
                        {
							cc.Total += Convert.ToDecimal(main.TotalNett);
                            Operation.CreditValue = main.TotalNett ;
                        }
                        await cc.UpdateCreditAsync();
                        creditExists = true;
                        creditId = cc.CreditID;
                        break;
                    }
                }
				
				//if there is no credit with client id create a new one
				if (!creditExists)
                {
                    Credit newCredit = new Credit();
                    newCredit.ClientID = selected;
                    if (Remise.Text != "")
                    {
						newCredit.Total += Convert.ToDecimal(main.TotalNett) - Convert.ToDecimal(Remise.Text);
                        Operation.CreditValue = main.TotalNett - Convert.ToDecimal(Remise.Text);
                    }
                    else
                    {
                        
                        newCredit.Total += Convert.ToDecimal(main.TotalNett);
                        Operation.CreditValue = main.TotalNett ;
                    }
                    creditId = await newCredit.InsertCreditAsync();
                }


                

                Operation.OperationType = "VenteCr";
				Operation.PrixOperation = main.TotalNett;
                
                Operation.CreditID = creditId;
                if (Remise.Text != "")
                {
                    Operation.Remise = Convert.ToDecimal(Remise.Text);
                }

                Operation.UserID = main.u.UserID;
                Operation.ClientID = selected;

                int id = await Operation.InsertOperationAsync();
                foreach (CSingleArticle2 sa2 in main.SelectedArticles.Children)
                {
                    OperationArticle oca = new OperationArticle();
                    oca.ArticleID = sa2.a.ArticleID;
                    oca.OperationID = id;
                    oca.QteArticle = Convert.ToInt32(sa2.qte);
                    await oca.InsertOperationArticleAsync();
                    sa2.a.Quantite -= Convert.ToInt32(sa2.qte);
                    await sa2.a.UpdateArticleAsync();
                    foreach (Article ar in main.la)
                    {
                        if (ar.ArticleID == sa2.a.ArticleID)
                        {
                            main.la[main.la.IndexOf(ar)].Quantite = sa2.a.Quantite;
                            if (ar.Quantite == 0)
                            {
                                ar.DeleteArticleAsync();
                                main.la.Remove(ar);
                            }
                            main.LoadArticles(main.la);
                            break;
                        }
                    }

                }
            }

            main.SelectedArticles.Children.Clear();
            main.TotalNet.Text = " 0.00 DH";
            main.ArticleCount.Text = " 0";
            main.TotalNett = 0;
            main.NbrA = 0;

            WCongratulations wCongratulations = new WCongratulations(0);
            wCongratulations.ShowDialog();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ClientContainer.Children.Clear();
            string code = sender is TextBox tb ? tb.Text : "";
            foreach (Client c in lc)
            {
                if (c.Nom.Contains(code))
                {
                    CSingleClient ar = new CSingleClient(c, this);
                    ClientContainer.Children.Add(ar);
                }

            }
        }

        private void ClientsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only digits and optionally one decimal separator
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]*\\.?[0-9]*$");
        }
    }
}
