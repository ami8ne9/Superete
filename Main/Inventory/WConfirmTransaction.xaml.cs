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
        public WConfirmTransaction(WAddArticle ar, WAjoutQuantite aq,WAddMultipleArticles ama, Article a, int s,int methodID)
        {
            InitializeComponent();
            this.ar = ar;
            this.aq = aq;
            this.s = s;
            this.a = a;
            this.ama = ama;
            this.methodID = methodID;
            if (s != 1)
            {
                CreditColumn.Width = new GridLength(0);
                CreditStack.Visibility = Visibility.Collapsed;
            }
            if (aq != null)
            {
                NbrArticle.Text = aq.qte.ToString();
                Subtotal.Text = (a.PrixAchat * aq.qte).ToString("0.00") + " DH";
                FinalTotal.Text = (a.PrixAchat * aq.qte).ToString("0.00") + " DH";
            }
            if (ar != null)
            {
                NbrArticle.Text = a.Quantite.ToString();
                Subtotal.Text = (a.PrixAchat * a.Quantite).ToString("0.00") + " DH";
                FinalTotal.Text = (a.PrixAchat * a.Quantite).ToString("0.00") + " DH";
            }
            if(ama != null)
            {
                for (int i = ama.ArticlesContainer.Children.Count - 1; i >= 0; i--)
                {
                    if (ama.ArticlesContainer.Children[i] is CSingleRowArticle csra)
                    {
                        csra.Quantite.Text = csra.Quantite.Text.Replace("x", "");
                        NbrArticleTotal += Convert.ToInt32(csra.Quantite.Text);
                        Subtotall += csra.a.PrixAchat * Convert.ToInt32(csra.Quantite.Text);
                        FinalTotall+= csra.a.PrixAchat * Convert.ToInt32(csra.Quantite.Text);
                    }
                }
                NbrArticle.Text = NbrArticleTotal.ToString();
                Subtotal.Text = (Subtotall).ToString("0.00") + " DH";
                FinalTotal.Text = (FinalTotall).ToString("0.00") + " DH";
            }


        }
        WAddArticle ar;int s; WAjoutQuantite aq; Article a; WAddMultipleArticles ama; int NbrArticleTotal = 0;int methodID;
        decimal Subtotall = 0;
        decimal FinalTotall = 0;
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
            if (Remise.Text == "0.00") Remise.Text = "";
            FinalTotal.Text = FinalTotal.Text.Replace("DH", "").Trim();
            Remise.Text = Remise.Text.Replace("DH", "").Trim();
            Remise.Text = Remise.Text.Replace("-", "");
            if (Convert.ToDecimal(FinalTotal.Text) < 0)
            {
                MessageBox.Show("le total final ne peux pas etre negative");
                Remise.Text = "-" + Remise.Text + " DH";
                FinalTotal.Text= FinalTotal.Text+" DH";
                return;
            }
            //New Article
            if (ar != null)
            {
                
                if (s == 0)
                {
                    Operation Operation = new Operation();
                    Operation.PaymentMethodID = methodID;
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
                        if (Convert.ToDecimal(CreditInput.Text) > Convert.ToDecimal(a.PrixAchat * a.Quantite) - Convert.ToDecimal(Remise.Text))
                        {
                            MessageBox.Show("la valeur de credit est plus grande que le total mois la remise.");
                            return;
                        }
                    }
                    else
                    {
                        if (Convert.ToDecimal(CreditInput.Text) > Convert.ToDecimal(a.PrixAchat * a.Quantite))
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

                            ff.Total += Convert.ToDecimal(CreditInput.Text);
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
                        newCredit.Total = Convert.ToDecimal(CreditInput.Text);
                        creditId = await newCredit.InsertCreditAsync();

                    }

                    Operation Operation = new Operation();

                    Operation.PaymentMethodID = methodID;
                    Operation.OperationType = "Achat50";
                    Operation.PrixOperation = (a.PrixAchat * a.Quantite);
                    Operation.CreditValue = Convert.ToDecimal(CreditInput.Text);
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
                        if (Convert.ToDecimal(Remise.Text) > Convert.ToDecimal(a.PrixAchat * a.Quantite))
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
                    Operation.PaymentMethodID = methodID;
                    foreach (Credit cf in lcc)
                    {
                        if (cf.FournisseurID == a.FournisseurID)
                        {
                            if (Remise.Text != "")
                            {
                                cf.Total += Convert.ToDecimal(a.PrixAchat * a.Quantite) - Convert.ToDecimal(Remise.Text);
                                Operation.CreditValue = Convert.ToDecimal(a.PrixAchat * a.Quantite) - Convert.ToDecimal(Remise.Text);
                            }
                            else
                            {
                                cf.Total += Convert.ToDecimal(a.PrixAchat * a.Quantite);
                                Operation.CreditValue = Convert.ToDecimal(a.PrixAchat * a.Quantite);
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
                            newCredit.Total += Convert.ToDecimal(a.PrixAchat * a.Quantite) - Convert.ToDecimal(Remise.Text);
                            Operation.CreditValue = Convert.ToDecimal(a.PrixAchat * a.Quantite)- Convert.ToDecimal(Remise.Text);
                        }
                        else
                        {
                            newCredit.Total += Convert.ToDecimal(a.PrixAchat * a.Quantite);
                            Operation.CreditValue = Convert.ToDecimal(a.PrixAchat * a.Quantite);
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
                    Operation.PaymentMethodID = methodID;

                    Operation.OperationType = "AchatCa";
                    Operation.PrixOperation = a.PrixAchat * aq.qte;
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
                    a.Quantite += aq.qte;
                    a.UpdateArticleAsync();
                    ofa.ArticleID = a.ArticleID;
                    ofa.OperationID = idd;
                    ofa.QteArticle = Convert.ToInt32(aq.qte);
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
                        if (Convert.ToDecimal(CreditInput.Text) > Convert.ToDecimal(a.PrixAchat * a.Quantite) - Convert.ToDecimal(Remise.Text))
                        {
                            MessageBox.Show("la valeur de credit est plus grande que le total mois la remise.");
                            return;
                        }
                    }
                    else
                    {
                        if (Convert.ToDecimal(CreditInput.Text) > Convert.ToDecimal(a.PrixAchat * a.Quantite))
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

                            ff.Total += Convert.ToDecimal(CreditInput.Text);
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
                        newCredit.Total = Convert.ToDecimal(CreditInput.Text);
                        creditId = await newCredit.InsertCreditAsync();

                    }

                    Operation Operation = new Operation();
                    Operation.PaymentMethodID = methodID;

                    Operation.OperationType = "Achat50";
                    Operation.PrixOperation = (a.PrixAchat * aq.qte);
                    Operation.CreditValue = Convert.ToDecimal(CreditInput.Text);
                    Operation.CreditID = creditId;
                    if (Remise.Text != "")
                    {
                        Operation.Remise = Convert.ToDecimal(Remise.Text);
                    }

                    Operation.UserID = aq.sa.Main.u.UserID;
                    Operation.FournisseurID = a.FournisseurID;

                    int idd = await Operation.InsertOperationAsync();
                    OperationArticle ofa = new OperationArticle();

                    aq.sa.a.Quantite += aq.qte;
                    aq.sa.a.UpdateArticleAsync();
                    ofa.ArticleID = a.ArticleID;
                    ofa.OperationID = idd;
                    ofa.QteArticle = Convert.ToInt32(aq.qte);
                    await ofa.InsertOperationArticleAsync();


                    this.Close();
                }
                else
                {
                    if (Remise.Text != "")
                    {
                        if (Convert.ToDecimal(Remise.Text) > Convert.ToDecimal(a.PrixAchat * aq.qte))
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
                    Operation.PaymentMethodID = methodID;
                    //if there is no credit with Fournisseur id create a new one
                    foreach (Credit cf in lcc)
                    {
                        if (cf.FournisseurID == a.FournisseurID)
                        {
                            if (Remise.Text != "")
                            {
                                cf.Total += Convert.ToDecimal(a.PrixAchat * aq.qte) - Convert.ToDecimal(Remise.Text);
                                Operation.CreditValue = Convert.ToDecimal(a.PrixAchat * aq.qte) - Convert.ToDecimal(Remise.Text);
                            }
                            else
                            {
                                cf.Total += Convert.ToDecimal(a.PrixAchat * aq.qte);
                                Operation.CreditValue = Convert.ToDecimal(a.PrixAchat * aq.qte);
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
                            newCredit.Total += Convert.ToDecimal(a.PrixAchat * aq.qte) - Convert.ToDecimal(Remise.Text);
                            Operation.CreditValue = Convert.ToDecimal(a.PrixAchat * aq.qte) - Convert.ToDecimal(Remise.Text);
                        }
                        else
                        {
                            newCredit.Total += Convert.ToDecimal(a.PrixAchat * aq.qte);
                            Operation.CreditValue = Convert.ToDecimal(a.PrixAchat * aq.qte);
                        }
                        creditId = await newCredit.InsertCreditAsync();
                    }


                    

                    Operation.OperationType = "AchatCr";
                    Operation.PrixOperation = a.PrixAchat * aq.qte;
                    Operation.CreditID = creditId;
                    if (Remise.Text != "")
                    {
                        Operation.Remise = Convert.ToDecimal(Remise.Text);
                    }

                    Operation.UserID = aq.sa.Main.u.UserID;
                    Operation.FournisseurID = a.FournisseurID;

                    int idd = await Operation.InsertOperationAsync();
                    OperationArticle ofa = new OperationArticle();

                    aq.sa.a.Quantite += aq.qte;
                    aq.sa.a.UpdateArticleAsync();
                    
                    ofa.ArticleID = a.ArticleID;
                    ofa.OperationID = idd;
                    ofa.QteArticle = Convert.ToInt32(aq.qte);
                    await ofa.InsertOperationArticleAsync();

                    this.Close();
                }
                aq.sa.Main.LoadArticles(aq.ns.main.main.la);
                aq.sa.ea.LoadArticles(aq.ns.main.main.la);
                aq.Close();


            }
            //Add Multiple Articles
            if (ama != null)
            {
                if (s == 0)
                {
                    Operation Operation = new Operation();
                    Operation.PaymentMethodID = methodID;
                    Operation.OperationType = "AchatCa";
                    Operation.PrixOperation = Subtotall;
                    if (Remise.Text != "")
                    {
                        Operation.Remise = Convert.ToDecimal(Remise.Text);
                        if (Operation.Remise > Operation.PrixOperation)
                        {
                            MessageBox.Show("la remise est plus grande que le total.");
                            return;
                        }
                    }

                    Operation.UserID = ama.main.u.UserID;
                    Operation.FournisseurID = ama.fo.FournisseurID;
                    int idd = await Operation.InsertOperationAsync();
                    for (int i = ama.ArticlesContainer.Children.Count - 1; i >= 0; i--)
                    {
                        if (ama.ArticlesContainer.Children[i] is CSingleRowArticle csra)
                        {
                            if(csra.Fournisseur.Text== "Nouvelle Article")
                            {
                                OperationArticle ofa = new OperationArticle();

                                int id = await csra.a.InsertArticleAsync();
                                csra.a.ArticleID = id;

                                ofa.ArticleID = csra.a.ArticleID;
                                ofa.OperationID = idd;
                                ofa.QteArticle = Convert.ToInt32(csra.a.Quantite);
                                await ofa.InsertOperationArticleAsync();

                            }
                            else if (csra.Fournisseur.Text == "Ajout de quantite")
                            {
                                OperationArticle ofa = new OperationArticle();
                                csra.Quantite.Text = csra.Quantite.Text.Replace("x", "");
                                csra.a.Quantite += Convert.ToInt32(csra.Quantite.Text);
                                csra.a.UpdateArticleAsync();
                                ofa.ArticleID = csra.a.ArticleID;
                                ofa.OperationID = idd;
                                ofa.QteArticle = Convert.ToInt32(csra.Quantite.Text);
                                await ofa.InsertOperationArticleAsync();
                            }
                        }
                    }

                    
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
                        if (Convert.ToDecimal(CreditInput.Text) > Convert.ToDecimal(Subtotall) - Convert.ToDecimal(Remise.Text))
                        {
                            MessageBox.Show("la valeur de credit est plus grande que le total mois la remise.");
                            return;
                        }
                    }
                    else
                    {
                        if (Convert.ToDecimal(CreditInput.Text) > Convert.ToDecimal(Subtotall))
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
                        if (ff.FournisseurID == ama.fo.FournisseurID)
                        {

                            ff.Total += Convert.ToDecimal(CreditInput.Text);
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
                        newCredit.FournisseurID = ama.fo.FournisseurID;
                        newCredit.Total = Convert.ToDecimal(CreditInput.Text);
                        creditId = await newCredit.InsertCreditAsync();

                    }

                    Operation Operation = new Operation();
                    Operation.PaymentMethodID = methodID;
                    Operation.OperationType = "Achat50";
                    Operation.PrixOperation = Subtotall;
                    Operation.CreditValue = Convert.ToDecimal(CreditInput.Text);
                    Operation.CreditID = creditId;
                    if (Remise.Text != "")
                    {
                        Operation.Remise = Convert.ToDecimal(Remise.Text);
                    }

                    Operation.UserID = ama.main.u.UserID;
                    Operation.FournisseurID = ama.fo.FournisseurID;

                    int idd = await Operation.InsertOperationAsync();
                    //Start here
                    for (int i = ama.ArticlesContainer.Children.Count - 1; i >= 0; i--)
                    {
                        if (ama.ArticlesContainer.Children[i] is CSingleRowArticle csra)
                        {
                            if (csra.Fournisseur.Text == "Nouvelle Article")
                            {
                                OperationArticle ofa = new OperationArticle();

                                int id = await csra.a.InsertArticleAsync();
                                csra.a.ArticleID = id;

                                ofa.ArticleID = csra.a.ArticleID;
                                ofa.OperationID = idd;
                                ofa.QteArticle = Convert.ToInt32(csra.a.Quantite);
                                await ofa.InsertOperationArticleAsync();

                            }
                            else if (csra.Fournisseur.Text == "Ajout de quantite")
                            {
                                OperationArticle ofa = new OperationArticle();
                                csra.Quantite.Text = csra.Quantite.Text.Replace("x", "");
                                csra.a.Quantite += Convert.ToInt32(csra.Quantite.Text);
                                csra.a.UpdateArticleAsync();
                                ofa.ArticleID = csra.a.ArticleID;
                                ofa.OperationID = idd;
                                ofa.QteArticle = Convert.ToInt32(csra.Quantite.Text);
                                await ofa.InsertOperationArticleAsync();
                            }
                        }
                    }

                }
                else
                {
                    if (Remise.Text != "")
                    {
                        if (Convert.ToDecimal(Remise.Text) > Convert.ToDecimal(Subtotall))
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
                    Operation.PaymentMethodID = methodID;
                    //if there is no credit with Fournisseur id create a new one
                    foreach (Credit cf in lcc)
                    {
                        if (cf.FournisseurID == ama.fo.FournisseurID)
                        {
                            if (Remise.Text != "")
                            {
                                cf.Total += Convert.ToDecimal(Subtotall) - Convert.ToDecimal(Remise.Text);
                                Operation.CreditValue = Convert.ToDecimal(Subtotall) - Convert.ToDecimal(Remise.Text);
                            }
                            else
                            {
                                cf.Total += Convert.ToDecimal(Subtotall);
                                Operation.CreditValue = Convert.ToDecimal(Subtotall);
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
                        newCredit.FournisseurID = ama.fo.FournisseurID;
                        if (Remise.Text != "")
                        {
                            newCredit.Total += Convert.ToDecimal(Subtotall) - Convert.ToDecimal(Remise.Text);
                            Operation.CreditValue = Convert.ToDecimal(Subtotall) - Convert.ToDecimal(Remise.Text);
                        }
                        else
                        {
                            newCredit.Total += Convert.ToDecimal(Subtotall);
                            Operation.CreditValue = Convert.ToDecimal(Subtotall);
                        }
                        creditId = await newCredit.InsertCreditAsync();
                    }




                    Operation.OperationType = "AchatCr";
                    Operation.PrixOperation = Subtotall;
                    Operation.CreditID = creditId;
                    if (Remise.Text != "")
                    {
                        Operation.Remise = Convert.ToDecimal(Remise.Text);
                    }

                    Operation.UserID = ama.main.u.UserID;
                    Operation.FournisseurID = ama.fo.FournisseurID;

                    int idd = await Operation.InsertOperationAsync();


                    for (int i = ama.ArticlesContainer.Children.Count - 1; i >= 0; i--)
                    {
                        if (ama.ArticlesContainer.Children[i] is CSingleRowArticle csra)
                        {
                            if (csra.Fournisseur.Text == "Nouvelle Article")
                            {
                                OperationArticle ofa = new OperationArticle();

                                int id = await csra.a.InsertArticleAsync();
                                csra.a.ArticleID = id;

                                ofa.ArticleID = csra.a.ArticleID;
                                ofa.OperationID = idd;
                                ofa.QteArticle = Convert.ToInt32(csra.a.Quantite);
                                await ofa.InsertOperationArticleAsync();

                            }
                            else if (csra.Fournisseur.Text == "Ajout de quantite")
                            {
                                OperationArticle ofa = new OperationArticle();
                                csra.Quantite.Text = csra.Quantite.Text.Replace("x", "");
                                csra.a.Quantite += Convert.ToInt32(csra.Quantite.Text);
                                csra.a.UpdateArticleAsync();
                                ofa.ArticleID = csra.a.ArticleID;
                                ofa.OperationID = idd;
                                ofa.QteArticle = Convert.ToInt32(csra.Quantite.Text);
                                await ofa.InsertOperationArticleAsync();
                            }
                        }
                    }
                }
                ama.main.LoadArticles(ama.main.la);
            }
            WCongratulations wCongratulations = new WCongratulations(0);
            wCongratulations.ShowDialog();
        }

        private void RemiseInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            
            string currentText = (sender as TextBox).Text;

            Remise.Text= "-"+currentText+" DH";
            
            if (ama == null)
            {
                if (a == null) return;
                if (currentText.Length == 0)
                {
                    FinalTotal.Text = (a.PrixAchat * a.Quantite).ToString("0.00") + " DH";
                    Remise.Text = "-0.00 DH";
                    return;
                }
                FinalTotal.Text = ((a.PrixAchat * a.Quantite) - Convert.ToDecimal(currentText)).ToString("0.00") + " DH";
            }
            else
            {
                if (currentText.Length == 0)
                {
                    FinalTotal.Text = FinalTotall.ToString("0.00") + " DH";
                    Remise.Text = "-0.00 DH";
                    return;
                }
                FinalTotal.Text = (FinalTotall - Convert.ToDecimal(currentText)).ToString("0.00") + " DH";
            }
                
        }
    }
}
