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
using System.Windows.Shapes;

namespace Superete.Main.Facturation
{
    public partial class WMouvments : Window
    {
        public CSingleOperation wso;
        Operation op;
        public WMouvments(CSingleOperation wso, Operation op)
        {
            InitializeComponent();
            this.wso = wso;
            this.op = op;
            LoadMouvments();
        }
        public void LoadMouvments()
        {
            foreach (OperationArticle oa in wso.sc.main.main.loa)
            {
                if (oa.OperationID == op.OperationID)
                {
                    CSingleMouvment csm = new CSingleMouvment(this, oa);
                    MouvmentsContainer.Children.Add(csm);
                }
            }
        }

        private void btnClose_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
