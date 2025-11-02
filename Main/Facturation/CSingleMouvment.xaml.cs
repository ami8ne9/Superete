using GestionComerce;
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

namespace Superete.Main.Facturation
{
    /// <summary>
    /// Interaction logic for CSingleMouvment.xaml
    /// </summary>
    public partial class CSingleMouvment : UserControl
    {

        public WMouvments ms;
        OperationArticle oa;
        public CSingleMouvment(WMouvments ms,OperationArticle oa)
        {
            InitializeComponent();
            this.ms = ms;
            this.oa = oa;
            ArticleQuantity.Text = oa.QteArticle.ToString();
            foreach (Article a in ms.wso.sc.main.main.la)
            {
                if(oa.ArticleID == a.ArticleID)
                {
                    ArticleName.Text = a.ArticleName;
                    if (oa.Reversed == true)
                    {
                        ArticleName.Text +=" (Reversed)";
                    }
                }
            }

        }
    }
}
