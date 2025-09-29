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

namespace Superete.Main.Vente
{
    /// <summary>
    /// Interaction logic for CSingleArticle1.xaml
    /// </summary>
    public partial class CSingleArticle1 : UserControl
    {
        public CSingleArticle1(Article a, CMainV mainv,List<Famille> lf, List<Fournisseur> lfo)
        {
            InitializeComponent();
            ArticleID.Text = a.ArticleID.ToString();
            ArticleName.Text = a.ArticleName;
            PrixVente.Text = a.PrixVente.ToString("F2");
            Quantite.Text = a.Quantite.ToString();
            PrixAchat.Text = a.PrixAchat.ToString("F2");
            PrixMP.Text = a.PrixMP.ToString("F2");
            foreach (Famille f in lf)
                if (f.FamilleID == a.FamillyID)
                {
                    Famille.Text = f.FamilleName;
                    break;
                }
            foreach (Fournisseur fo in lfo)
                if (a.FournisseurID == fo.FournisseurID)
                {
                    FournisseurName.Text = fo.Nom;
                    break;
                }
            Code.Text = a.Code.ToString();
            this.mainv = mainv;
            this.a = a;
            this.lf = lf;
            this.lfo = lfo;


        }
        CMainV mainv;
        public Article a;
        List<Famille> lf;
        List<Fournisseur> lfo;
        private void ArticleClicked(object sender, RoutedEventArgs e)
        {
            
            mainv.SelectedArticle.Child=new CSingleArticle1(a,mainv, lf,lfo);
        }
    }
}
