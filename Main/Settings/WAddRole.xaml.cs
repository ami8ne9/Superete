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

namespace Superete.Main.Settings
{
    /// <summary>
    /// Logique d'interaction pour WAddRole.xaml
    /// </summary>
    public partial class WAddRole : Window
    {
        public WAddRole(WRoles roles, List<Role> lr)
        {
            InitializeComponent();
            this.lr = lr; this.roles = roles;
        }
        List<Role> lr;WRoles roles;

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDeselectAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AnnulerButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ToutActiverBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in Container.Children)
            {
                if (child is CheckBox cb)
                {
                    cb.IsChecked = true;
                }
            }
        }

        private void ToutDesactiverBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in Container.Children)
            {
                if (child is CheckBox cb)
                {
                    cb.IsChecked = false;
                }
            }
        }

        private async void AppliquerButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RoleName.Text))
            {
                MessageBox.Show("Le nom du rôle ne peut pas être vide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (lr.Any(r => r.RoleName.Equals(RoleName.Text, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Un rôle avec ce nom existe déjà.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Role newRole = new Role
            {
                RoleName = RoleName.Text
                ,
                Operation = check(Operation)
                ,
                Remise= check(Remise)
                ,
                ModifPrix= check(ModifPrix)
                ,
                ModifQuantite= check(ModifQuantite)
                ,
                Prix=check(Prix)
                ,
                Divers= check(Divers)
                ,
                Duplica = check(Duplica)
                ,
                Flash= check(Flash)
                ,
                Tiroir = check(Tiroir)
                ,
                Anulation = check(Anulation)
                ,
                Rapport= check(Rapport)
                ,
                Depences = check(Depences)
                ,
                Reseption= check(Reception)
                ,
                Sorties= check(Sorties)
                ,
                Clients = check(Clients)
                ,
                ClientRegles = check(ClientRegles)
                ,
                Articles = check(Articles)
                ,
                Solder = check(Solder)
                ,
                Cloture = check(Cloture)
            };
            int id=await newRole.InsertRoleAsync();
            newRole.RoleID = id;
            lr.Add(newRole);
            roles.LoadRoles();
            this.Close();
        }
        private int check(CheckBox cb)
        {
            if (cb.IsChecked == true) return 1;
            return 0;
        }
    }
}
