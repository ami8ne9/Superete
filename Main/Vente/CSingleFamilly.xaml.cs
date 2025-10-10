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
    /// Interaction logic for CSingleFamilly.xaml
    /// </summary>
    public partial class CSingleFamilly : UserControl
    {
        public CSingleFamilly(Famille f,CMainV mainv,List<Famille> lf)
        {
            InitializeComponent();
            FamillyName.Content = f.FamilleName;
            this.f = f;
            this.mainv = mainv;
            this.lf = lf;
        }
        Famille f; CMainV mainv; List<Famille> lf;

        private async void FamillyName_Click(object sender, RoutedEventArgs e)
        {
            mainv.ArticlesContainer.Children.Clear();
            Article a = new Article();
            List<Article> Articles= await a.GetArticlesAsync();
            List<Article> articlesInFamily = Articles.Where(article => article.FamillyID == f.FamilleID).ToList();
            foreach (Article article in articlesInFamily)
            {
                mainv.ArticlesContainer.Children.Add(new CSingleArticle1(article, mainv,lf,mainv.lfo,0));
            }
        }
    }
}
