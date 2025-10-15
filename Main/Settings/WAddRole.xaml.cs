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
            try
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
                    RoleName = RoleName.Text,

                    // Client Permissions
                    CreateClient = check(CreateClient),
                    ModifyClient = check(ModifyClient),
                    DeleteClient = check(DeleteClient),
                    ViewClient = check(ViewClient),
                    ViewOperationClient = check(ViewOperationClient),
                    PayeClient = check(PayeClient),

                    // Fournisseur Permissions
                    CreateFournisseur = check(CreateFournisseur),
                    ModifyFournisseur = check(ModifyFournisseur),
                    DeleteFournisseur = check(DeleteFournisseur),
                    ViewFournisseur = check(ViewFournisseur),
                    ViewOperationFournisseur = check(ViewOperationFournisseur),
                    PayeFournisseur = check(PayeFournisseur),

                    // Operations / Movements
                    ReverseOperation = check(ReverseOperation),
                    ReverseMouvment = check(ReverseMouvment),
                    ViewOperation = check(ViewOperation),
                    ViewMouvment = check(ViewMouvment),

                    // Management & Settings
                    ViewProjectManagment = check(ViewProjectManagment),
                    ViewSettings = check(ViewSettings),

                    // Users & Roles
                    ViewUsers = check(ViewUsers),
                    AddUsers = check(AddUsers),
                    EditUsers = check(EditUsers),
                    DeleteUsers = check(DeleteUsers),
                    ViewRoles = check(ViewRoles),
                    AddRoles = check(AddRoles),
                    DeleteRoles = check(DeleteRoles),

                    // Familles
                    ViewFamilly = check(ViewFamilly),
                    AddFamilly = check(AddFamilly),
                    EditFamilly = check(EditFamilly),
                    DeleteFamilly = check(DeleteFamilly),

                    // Articles
                    AddArticle = check(AddArticle),
                    DeleteArticle = check(DeleteArticle),
                    EditArticle = check(EditArticle),
                    ViewArticle = check(ViewArticle),

                    // Reports & Tickets
                    Repport = check(Repport),
                    Ticket = check(Ticket),
                    ViewFacture = check(ViewFacture),

                    // Settlements
                    SolderFournisseur = check(SolderFournisseur),
                    SolderClient = check(SolderClient),

                    // Invoice Settings
                    ViewFactureSettings = check(ViewFactureSettings),
                    ModifyFactureSettings = check(ModifyFactureSettings),

                    // Payment Methods
                    ViewPaymentMethod = check(ViewPaymentMethod),
                    AddPaymentMethod = check(AddPaymentMethod),
                    ModifyPaymentMethod = check(ModifyPaymentMethod),
                    DeletePaymentMethod = check(DeletePaymentMethod),

                    // System Actions
                    ViewApropos = check(ViewApropos),
                    Logout = check(Logout),
                    ViewExit = check(Exit),
                    ViewShutDown = check(ShutDown)
                };


                int id = await newRole.InsertRoleAsync();
                newRole.RoleID = id;
                lr.Add(newRole);
                foreach (Role r in roles.CUM.sp.main.lr)
                {
                    if (roles.CUM.u.RoleID == r.RoleID)
                    {
                        if (r.ViewRoles == true)
                        {
                            roles.LoadRoles();
                        }
                        break;
                    }
                }

                //this.Close();

                WCongratulations wCongratulations = new WCongratulations("Ajout avec succès", "l'ajout a ete effectue avec succes", 1);
                wCongratulations.ShowDialog();
            }
            catch (Exception ex)
            {

                WCongratulations wCongratulations = new WCongratulations("Ajout échoué", "l'ajout n'a pas ete effectue ", 0);
                wCongratulations.ShowDialog();
            }
        }
        private bool check(CheckBox cb)
        {
            return cb != null && cb.IsChecked == true;
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
