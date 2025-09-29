using Superete.Main.Vente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Superete.Main.Inventory
{
    /// <summary>
    /// Interaction logic for WConfirmTransaction.xaml
    /// </summary>
    public partial class WConfirmTransaction : Window
    {
        public WConfirmTransaction(WAddArticle ar, WAjoutQuantite aq, Article a, int s)
        {
            InitializeComponent();
            this.ar = ar;
            this.aq = aq;
            this.s = s;
            this.a = a;
            if (s != 1)
            {
                CreditColumn.Width = new GridLength(0);
                CreditStack.Visibility = Visibility.Collapsed;
            }
            NbrArticle.Text=a.Quantite.ToString();
            Subtotal.Text=(a.PrixAchat*a.Quantite).ToString("0.00")+" DH";
            FinalTotal.Text=(a.PrixAchat*a.Quantite).ToString("0.00")+" DH";

        }
        WAddArticle ar;int s; WAjoutQuantite aq; Article a;
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (Remise.Text == "-0.00 DH") Remise.Text = "";
            //New Article
            if (ar != null)
            {
                
                if (s == 0)
                {
                    Operation Operation = new Operation();
                    Operation.OperationType = "AchatCa";
                    Operation.PrixOperation = a.PrixAchat * a.Quantite;
                    if (Remise.Text != "")
                    {
                        Operation.Remise = Convert.ToDecimal(Remise.Text);
                        if (Operation.Remise > Operation.PrixOperation)
                        {
                            MessageBox.Show("la remise est plus grande que le total.");
                            return;
                        }
                    }
                    
                    Operation.UserID = ar.main.u.UserID;
                    Operation.FournisseurID = a.FournisseurID;
                    int idd = await Operation.InsertOperationAsync();
                    OperationArticle ofa = new OperationArticle();

                    int id = await a.InsertArticleAsync();
                    a.ArticleID = id;

                    ofa.ArticleID = a.ArticleID;
                    ofa.OperationID = idd;
                    ofa.QteArticle = Convert.ToInt32(a.Quantite);
                    await ofa.InsertOperationArticleAsync();
                    
                    this.Close();
                }
                else if (s == 1)
                {
                    if (Convert.ToDecimal(CreditInput.Text)==0)
                    {
                        MessageBox.Show("Doneer un valeur de credit.");
                        return;
                    }

                    if (Remise.Text != "")
                    {
                        if (Convert.ToInt32(CreditInput.Text) > Convert.ToInt32(a.PrixAchat * a.Quantite) - Convert.ToDecimal(Remise.Text))
                        {
                            MessageBox.Show("la valeur de credit est plus grande que le total mois la remise.");
                            return;
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(CreditInput.Text) > Convert.ToInt32(a.PrixAchat * a.Quantite))
                        {
                            MessageBox.Show("la valeur de credit est plus grande que le total.");
                            return;
                        }
                    }
                    int creditId = 0;
                    bool creditExists = false;
                    //import Credit
                    Credit Credit = new Credit();
                    List<Credit> lff = await Credit.GetCreditsAsync();
                    //if there is no credit with Fournisseur id create a new one
                    foreach (Credit ff in lff)
                    {
                        if (ff.FournisseurID == a.FournisseurID)
                        {

                            ff.Total += Convert.ToInt32(CreditInput.Text);
                            await ff.UpdateCreditAsync();
                            creditExists = true;
                            creditId = ff.CreditID;
                            break;
                        }
                    }
                    //else update the credit value
                    if (!creditExists)
                    {
                        MessageBox.Show("Creating new credit");
                        Credit newCredit = new Credit();
                        newCredit.FournisseurID = a.FournisseurID;
                        newCredit.Total = Convert.ToInt32(CreditInput.Text);
                        creditId = await newCredit.InsertCreditAsync();

                    }

                    Operation Operation = new Operation();
                    Operation.OperationType = "Achat50";
                    Operation.PrixOperation = (a.PrixAchat * a.Quantite);
                    Operation.CreditValue = Convert.ToInt32(CreditInput.Text);
                    Operation.CreditID = creditId;
                    if (Remise.Text != "")
                    {
                        Operation.Remise = Convert.ToDecimal(Remise.Text);
                    }

                    Operation.UserID = ar.main.u.UserID;
                    Operation.FournisseurID = a.FournisseurID;

                    int idd = await Operation.InsertOperationAsync();
                    OperationArticle ofa = new OperationArticle();
                    int id = await a.InsertArticleAsync();
                    a.ArticleID = id;
                    ofa.ArticleID = a.ArticleID;
                    ofa.OperationID = idd;
                    ofa.QteArticle = Convert.ToInt32(a.Quantite);
                    await ofa.InsertOperationArticleAsync();
                    

                    this.Close();
                }
                else
                {
                    
                    if (Remise.Text != "")
                    {
                        if (Convert.ToDecimal(Remise.Text) > Convert.ToInt32(a.PrixAchat * a.Quantite))
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
                    //if there is no credit with Fournisseur id create a new one
                    Operation Operation = new Operation();
                    foreach (Credit cf in lcc)
                    {
                        if (cf.FournisseurID == a.FournisseurID)
                        {
                            if (Remise.Text != "")
                            {
                                cf.Total += Convert.ToInt32(a.PrixAchat * a.Quantite) - Convert.ToDecimal(Remise.Text);
                                Operation.CreditValue = Convert.ToInt32(a.PrixAchat * a.Quantite) - Convert.ToDecimal(Remise.Text);
                            }
                            else
                            {
                                cf.Total += Convert.ToInt32(a.PrixAchat * a.Quantite);
                                Operation.CreditValue = Convert.ToInt32(a.PrixAchat * a.Quantite);
                            }
                            await cf.UpdateCreditAsync();
                            creditExists = true;
                            creditId = cf.CreditID;
                            break;
                        }
                    }
                    //else update the credit value
                    if (!creditExists)
                    {
                        Credit newCredit = new Credit();
                        newCredit.FournisseurID = a.FournisseurID;
                        if (Remise.Text != "")
                        {
                            newCredit.Total += Convert.ToInt32(a.PrixAchat * a.Quantite) - Convert.ToDecimal(Remise.Text);
                            Operation.CreditValue = Convert.ToInt32(a.PrixAchat * a.Quantite)- Convert.ToDecimal(Remise.Text);
                        }
                        else
                        {
                            newCredit.Total += Convert.ToInt32(a.PrixAchat * a.Quantite);
                            Operation.CreditValue = Convert.ToInt32(a.PrixAchat * a.Quantite);
                        }
                        creditId = await newCredit.InsertCreditAsync();
                    }


                    
                    Operation.OperationType = "AchatCr";
                    Operation.PrixOperation = a.PrixAchat * a.Quantite;
                    Operation.CreditID = creditId;
                    if (Remise.Text != "")
                    {
                        Operation.Remise = Convert.ToDecimal(Remise.Text);
                    }

                    Operation.UserID = ar.main.u.UserID;
                    Operation.FournisseurID = a.FournisseurID;

                    int idd = await Operation.InsertOperationAsync();
                    OperationArticle ofa = new OperationArticle();
                    int id = await a.InsertArticleAsync();
                    a.ArticleID = id;
                    ofa.ArticleID = a.ArticleID;
                    ofa.OperationID = idd;
                    ofa.QteArticle = Convert.ToInt32(a.Quantite);
                    await ofa.InsertOperationArticleAsync();

                    this.Close();
                }
                ar.la.Add(a);
                List<Article> la1 = ar.la;
                ar.main.LoadArticles(la1);
                ar.ea?.LoadArticles(la1);
                ar.Close();
            }
            //Add Quantity
            if (aq != null)
            {

                if (s == 0)
                {
                    Operation Operation = new Operation();

                    Operation.OperationType = "AchatCa";
                    Operation.PrixOperation = a.PrixAchat * a.Quantite;
                    if (Remise.Text != "")
                    {
                        Operation.Remise = Convert.ToDecimal(Remise.Text);
                        if (Operation.Remise > Operation.PrixOperation)
                        {
                            MessageBox.Show("la remise est plus grande que le total.");
                            return;
                        }
                    }

                    Operation.UserID = aq.sa.Main.u.UserID;
                    Operation.FournisseurID = a.FournisseurID;
                    int idd = await Operation.InsertOperationAsync();
                    OperationArticle ofa = new OperationArticle();

                    await a.UpdateArticleAsync();

                    ofa.ArticleID = a.ArticleID;
                    ofa.OperationID = idd;
                    ofa.QteArticle = Convert.ToInt32(a.Quantite);
                    await ofa.InsertOperationArticleAsync();

                    this.Close();
                }
                else if (s == 1)
                {
                    if (Convert.ToDecimal(CreditInput.Text) == 0)
                    {
                        MessageBox.Show("Doneer un valeur de credit.");
                        return;
                    }

                    if (Remise.Text != "")
                    {
                        if (Convert.ToInt32(CreditInput.Text) > Convert.ToInt32(a.PrixAchat * a.Quantite) - Convert.ToDecimal(Remise.Text))
                        {
                            MessageBox.Show("la valeur de credit est plus grande que le total mois la remise.");
                            return;
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(CreditInput.Text) > Convert.ToInt32(a.PrixAchat * a.Quantite))
                        {
                            MessageBox.Show("la valeur de credit est plus grande que le total.");
                            return;
                        }
                    }
                    int creditId = 0;
                    bool creditExists = false;
                    //import Credit
                    Credit Credit = new Credit();
                    List<Credit> lff = await Credit.GetCreditsAsync();
                    //if there is no credit with Fournisseur id create a new one
                    foreach (Credit ff in lff)
                    {
                        if (ff.FournisseurID == a.FournisseurID)
                        {

                            ff.Total += Convert.ToInt32(CreditInput.Text);
                            await ff.UpdateCreditAsync();
                            creditExists = true;
                            creditId = ff.CreditID;
                            break;
                        }
                    }
                    //else update the credit value
                    if (!creditExists)
                    {
                        Credit newCredit = new Credit();
                        newCredit.FournisseurID = a.FournisseurID;
                        newCredit.Total = Convert.ToInt32(CreditInput.Text);
                        creditId = await newCredit.InsertCreditAsync();

                    }

                    Operation Operation = new Operation();

                    Operation.OperationType = "Achat50";
                    Operation.PrixOperation = (a.PrixAchat * a.Quantite);
                    Operation.CreditValue = Convert.ToInt32(CreditInput.Text);
                    Operation.CreditID = creditId;
                    if (Remise.Text != "")
                    {
                        Operation.Remise = Convert.ToDecimal(Remise.Text);
                    }

                    Operation.UserID = aq.sa.Main.u.UserID;
                    Operation.FournisseurID = a.FournisseurID;

                    int idd = await Operation.InsertOperationAsync();
                    OperationArticle ofa = new OperationArticle();
                    a.UpdateArticleAsync();
                    ofa.ArticleID = a.ArticleID;
                    ofa.OperationID = idd;
                    ofa.QteArticle = Convert.ToInt32(a.Quantite);
                    await ofa.InsertOperationArticleAsync();


                    this.Close();
                }
                else
                {
                    if (Remise.Text != "")
                    {
                        if (Convert.ToDecimal(Remise.Text) > Convert.ToInt32(a.PrixAchat * a.Quantite))
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
                    //if there is no credit with Fournisseur id create a new one
                    foreach (Credit cf in lcc)
                    {
                        if (cf.FournisseurID == a.FournisseurID)
                        {
                            if (Remise.Text != "")
                            {
                                cf.Total += Convert.ToInt32(a.PrixAchat * a.Quantite) - Convert.ToDecimal(Remise.Text);
                                Operation.CreditValue = Convert.ToInt32(a.PrixAchat * a.Quantite) - Convert.ToDecimal(Remise.Text);
                            }
                            else
                            {
                                cf.Total += Convert.ToInt32(a.PrixAchat * a.Quantite);
                                Operation.CreditValue = Convert.ToInt32(a.PrixAchat * a.Quantite);
                            }
                            await cf.UpdateCreditAsync();
                            creditExists = true;
                            creditId = cf.CreditID;
                            break;
                        }
                    }
                    //else update the credit value
                    if (!creditExists)
                    {
                        Credit newCredit = new Credit();
                        newCredit.FournisseurID = a.FournisseurID;
                        if (Remise.Text != "")
                        {
                            newCredit.Total += Convert.ToInt32(a.PrixAchat * a.Quantite) - Convert.ToDecimal(Remise.Text);
                            Operation.CreditValue = Convert.ToInt32(a.PrixAchat * a.Quantite) - Convert.ToDecimal(Remise.Text);
                        }
                        else
                        {
                            newCredit.Total += Convert.ToInt32(a.PrixAchat * a.Quantite);
                            Operation.CreditValue = Convert.ToInt32(a.PrixAchat * a.Quantite);
                        }
                        creditId = await newCredit.InsertCreditAsync();
                    }


                    

                    Operation.OperationType = "AchatCr";
                    Operation.PrixOperation = a.PrixAchat * a.Quantite;
                    Operation.CreditID = creditId;
                    if (Remise.Text != "")
                    {
                        Operation.Remise = Convert.ToDecimal(Remise.Text);
                    }

                    Operation.UserID = aq.sa.Main.u.UserID;
                    Operation.FournisseurID = a.FournisseurID;

                    int idd = await Operation.InsertOperationAsync();
                    OperationArticle ofa = new OperationArticle();
                    a.UpdateArticleAsync();
                    
                    ofa.ArticleID = a.ArticleID;
                    ofa.OperationID = idd;
                    ofa.QteArticle = Convert.ToInt32(a.Quantite);
                    await ofa.InsertOperationArticleAsync();

                    this.Close();
                }
                List<Article> la1 = aq.sa.Main.la;
                foreach(Article art in la1)
                {
                    if(art.ArticleID == a.ArticleID)
                    {
                        art.Quantite = a.Quantite;
                        break;
                    }
                }
                aq.sa.Main.LoadArticles(la1);
                aq.sa.ea.LoadArticles(la1);
                aq.Close();


            }

            WCongratulations wCongratulations = new WCongratulations(0);
            wCongratulations.ShowDialog();
        }

        private void RemiseInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (a == null) return;
            string currentText = (sender as TextBox).Text;

            Remise.Text= currentText;
            if(currentText.Length == 0)
            {
                FinalTotal.Text = (a.PrixAchat * a.Quantite).ToString("0.00") + " DH";
                return;
            }
            FinalTotal.Text = ((a.PrixAchat * a.Quantite) - Convert.ToDecimal(currentText)).ToString("0.00") + " DH";
        }
    }
}
