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

namespace Superete.Main.Inventory
{
    /// <summary>
    /// Interaction logic for WExistingArticles.xaml
    /// </summary>
    public partial class WExistingArticles : Window
    {
        public WExistingArticles(List<Article> la,CMainI main,int s)
        {
            InitializeComponent();
            this.la = la;
            this.main = main;
            this.s = s;
            LoadArticles(la);   
        }
        List<Article> la; CMainI main;int s;
        public void LoadArticles(List<Article> la)
        {
            ArticlesContainer.Children.Clear();
            foreach (Article a in la) 
            {
                CSingleRowArticle ar = new CSingleRowArticle(a, la, null, main, s,this);
                ArticlesContainer.Children.Add(ar);
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void ArticleInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            foreach (var child in ArticlesContainer.Children)
            {
                if (child is CSingleRowArticle ar)
                {
                    if (ar.a.ArticleName.IndexOf(ArticleInput.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        ar.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        ar.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }
    }
}
