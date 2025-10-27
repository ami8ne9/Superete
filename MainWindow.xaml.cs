using GestionComerce.Main;
using GestionComerce.Main.ClientPage;
using GestionComerce.Main.FournisseurPage;
using GestionComerce.Main.Inventory;
using GestionComerce.Main.ProjectManagment;
using GestionComerce.Main.Settings;
using GestionComerce.Main.Vente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace GestionComerce
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Login loginPage;

        public MainWindow()
        {
            InitializeComponent();
            MainGrid.Children.Clear();
            loginPage = new Login(this);
            loginPage.HorizontalAlignment = HorizontalAlignment.Stretch;
            loginPage.VerticalAlignment = VerticalAlignment.Stretch;
            loginPage.Margin = new Thickness(0);
            MainGrid.Children.Add(loginPage);
            this.Closed += MainWindow_Closed;
        }
        public List<User> lu;
        public List<Role> lr;
        public List<Famille> lf;
        public List<Article> la;
        public List<Article> laa;
        public List<Fournisseur> lfo;
        public List<Client> lc;
        public List<Operation> lo ;
        public List<OperationArticle> loa ;
        public List<Credit> credits ;
        public List<PaymentMethod> lp;
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (w != this) // skip the main window itself
                    w.Close();
            }
        }
        public async void load_main(User u)
        {

            List<User> lu = await u.GetUsersAsync();
            Role r = new Role();
            List<Role> lr = await r.GetRolesAsync();
            Famille f = new Famille();
            List<Famille> lf = await f.GetFamillesAsync();
            Article a = new Article();
            List<Article> la = await a.GetArticlesAsync();
            List<Article> laa = await a.GetAllArticlesAsync();
            Fournisseur fo = new Fournisseur();
            List<Fournisseur> lfo = await fo.GetFournisseursAsync();
            Client c = new Client();
            List<Client> lc = await c.GetClientsAsync();
            List<Operation> lo = await (new Operation()).GetOperationsAsync();
            List<OperationArticle> loa = await (new OperationArticle()).GetOperationArticlesAsync();
            List<PaymentMethod> lp = await (new PaymentMethod()).GetPaymentMethodsAsync();

            foreach (OperationArticle oa in loa)
            {
                foreach(Operation o in lo)
                {
                    if(o.OperationID == oa.OperationID)
                    {
                        oa.Date=o.DateOperation;
                    }
                }
            }
            loa = loa.OrderByDescending(oa => oa.Date).ToList();

            List<Credit> credits = await (new Credit()).GetCreditsAsync();
            this.lu = lu;
            this.lr = lr;
            this.lf = lf;
            this.la = la;
            this.laa = laa;
            this.lfo = lfo;
            this.lc = lc;
            this.lo = lo;
            this.loa = loa;
            this.lp = lp;
            this.credits = credits;
            MainGrid.Children.Clear();
            CMain loginPage = new CMain(this,u);
            loginPage.HorizontalAlignment = HorizontalAlignment.Stretch;
            loginPage.VerticalAlignment = VerticalAlignment.Stretch;
            loginPage.Margin = new Thickness(0);
            MainGrid.Children.Add(loginPage);
        }
        public void load_settings(User u)
        {
            MainGrid.Children.Clear();
            SettingsPage loginPage = new SettingsPage(u,lu,lr,lf,this);
            loginPage.HorizontalAlignment = HorizontalAlignment.Stretch;
            loginPage.VerticalAlignment = VerticalAlignment.Stretch;
            loginPage.Margin = new Thickness(0);
            MainGrid.Children.Add(loginPage);
        }
        public void load_vente(User u,List<Article> la)
        {
            MainGrid.Children.Clear();
            CMainV loginPage = new CMainV(u,lf, lu, lr, this, la,lfo);
            loginPage.HorizontalAlignment = HorizontalAlignment.Stretch;
            loginPage.VerticalAlignment = VerticalAlignment.Stretch;
            loginPage.Margin = new Thickness(0);
            MainGrid.Children.Add(loginPage);
        }
        public void load_inventory(User u)
        {
            MainGrid.Children.Clear();
            CMainI loginPage = new CMainI(u,la,lf,lfo,this);
            loginPage.HorizontalAlignment = HorizontalAlignment.Stretch;
            loginPage.VerticalAlignment = VerticalAlignment.Stretch;
            loginPage.Margin = new Thickness(0);
            MainGrid.Children.Add(loginPage);
        }
        public void load_fournisseur(User u)
        {
            MainGrid.Children.Clear();
            CMainF loginPage = new CMainF(u,this);
            loginPage.HorizontalAlignment = HorizontalAlignment.Stretch;
            loginPage.VerticalAlignment = VerticalAlignment.Stretch;
            loginPage.Margin = new Thickness(0);
            MainGrid.Children.Add(loginPage);
        }
        public void load_client(User u)
        {
            MainGrid.Children.Clear();
            CMainC loginPage = new CMainC(u, this);
            loginPage.HorizontalAlignment = HorizontalAlignment.Stretch;
            loginPage.VerticalAlignment = VerticalAlignment.Stretch;
            loginPage.Margin = new Thickness(0);
            MainGrid.Children.Add(loginPage);
        }
        public void load_ProjectManagement(User u)
        {
            MainGrid.Children.Clear();
            CMainP loginPage = new CMainP(u, this);
            loginPage.HorizontalAlignment = HorizontalAlignment.Stretch;
            loginPage.VerticalAlignment = VerticalAlignment.Stretch;
            loginPage.Margin = new Thickness(0);
            MainGrid.Children.Add(loginPage);
        }
        public void load_Login()
        {
            MainGrid.Children.Clear();
            Login loginPage = new Login(this);
            loginPage.HorizontalAlignment = HorizontalAlignment.Stretch;
            loginPage.VerticalAlignment = VerticalAlignment.Stretch;
            loginPage.Margin = new Thickness(0);
            MainGrid.Children.Add(loginPage);
        }
    }
}
