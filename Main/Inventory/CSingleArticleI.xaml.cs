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

namespace Superete.Main.Inventory
{
    /// <summary>
    /// Logique d'interaction pour CSingleArticleI.xaml
    /// </summary>
    public partial class CSingleArticleI : UserControl
    {
        public CSingleArticleI(Article a, List<Article> la, CMainI main,List<Famille> lf, List<Fournisseur> lfo)
        {
            InitializeComponent();
            ArticleID.Text = a.ArticleID.ToString();
            ArticleName.Text = a.ArticleName;
            Code.Text = a.Code.ToString();
            foreach (Famille f in lf)
                if(f.FamilleID==a.FamillyID)
                {
                    Famillee.Text = f.FamilleName;
                    break;
                }
            foreach (Fournisseur fo in lfo)
                if (fo.FournisseurID == a.FournisseurID)
                {
                    Fournisseur.Text = fo.Nom;
                    break;
                }
            PrixVente.Text = a.PrixVente.ToString("0.00") + " Dh";
            PrixAchat.Text=a.PrixAchat.ToString("0.00") + " Dh";
            PrixMP.Text=a.PrixMP.ToString("0.00") + " Dh";
            Quantite.Text = a.Quantite.ToString();
            this.la = la;
            this.main = main;
            this.lf = lf;
            this.a = a;
            this.lfo= lfo;
        }
        List<Article> la; List<Famille> lf; CMainI main; public Article a; List<Fournisseur> lfo;

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            WAddArticle af = new WAddArticle(a, la,lf,lfo,main,0,null);
            af.ShowDialog();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            WDeleteConfirmation df=new WDeleteConfirmation(a,la, null,main);
            df.ShowDialog();
        }
    }
}
