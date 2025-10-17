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
            this.lr = lr;
            this.roles = roles;
        }

        List<Role> lr;
        WRoles roles;

        private void AnnulerButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Global Toggle Buttons
        private void ToutActiverBtn_Click(object sender, RoutedEventArgs e)
        {
            SetAllCheckboxes(Container, true);
        }

        private void ToutDesactiverBtn_Click(object sender, RoutedEventArgs e)
        {
            SetAllCheckboxes(Container, false);
        }

        // Section-specific Toggle Buttons
        private void ActiverPages_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(PagesSection, true);
        }

        private void DesactiverPages_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(PagesSection, false);
        }

        private void ActiverClient_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(ClientSection, true);
        }

        private void DesactiverClient_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(ClientSection, false);
        }

        private void ActiverFournisseur_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(FournisseurSection, true);
        }

        private void DesactiverFournisseur_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(FournisseurSection, false);
        }

        private void ActiverOperations_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(OperationsSection, true);
        }

        private void DesactiverOperations_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(OperationsSection, false);
        }

        private void ActiverManagement_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(ManagementSection, true);
        }

        private void DesactiverManagement_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(ManagementSection, false);
        }

        private void ActiverUsers_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(UsersSection, true);
        }

        private void DesactiverUsers_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(UsersSection, false);
        }

        private void ActiverFamilly_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(FamillySection, true);
        }

        private void DesactiverFamilly_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(FamillySection, false);
        }

        private void ActiverArticles_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(ArticlesSection, true);
        }

        private void DesactiverArticles_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(ArticlesSection, false);
        }

        private void ActiverReports_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(ReportsSection, true);
        }

        private void DesactiverReports_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(ReportsSection, false);
        }

        private void ActiverSettlements_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(SettlementsSection, true);
        }

        private void DesactiverSettlements_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(SettlementsSection, false);
        }

        private void ActiverInvoiceSettings_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(InvoiceSettingsSection, true);
        }

        private void DesactiverInvoiceSettings_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(InvoiceSettingsSection, false);
        }

        private void ActiverPaymentMethods_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(PaymentMethodsSection, true);
        }

        private void DesactiverPaymentMethods_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(PaymentMethodsSection, false);
        }

        private void ActiverSystemActions_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(SystemActionsSection, true);
        }

        private void DesactiverSystemActions_Click(object sender, RoutedEventArgs e)
        {
            SetSectionCheckboxes(SystemActionsSection, false);
        }

        // Helper method to set all checkboxes in a container
        private void SetAllCheckboxes(DependencyObject parent, bool isChecked)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is CheckBox checkBox)
                {
                    checkBox.IsChecked = isChecked;
                }
                else
                {
                    SetAllCheckboxes(child, isChecked);
                }
            }
        }

        // Helper method to set checkboxes in a specific section
        private void SetSectionCheckboxes(DependencyObject section, bool isChecked)
        {
            SetAllCheckboxes(section, isChecked);
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

                    // Pages Access
                    ViewClientsPage = check(ViewClientsPage),
                    ViewFournisseurPage = check(ViewFournisseurPage),
                    ViewInventrory = check(ViewInventory),
                    ViewVente = check(ViewVente),
                    ViewCreditClient = check(ViewCreditClient),
                    ViewCreditFournisseur = check(ViewCreditFournisseur),

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
                    //ModifyTicket = check(ModifyTicket),

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
                    CashClient = check(CashClient),
                    CashFournisseur = check(CashFournisseur),

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

                WCongratulations wCongratulations = new WCongratulations("Ajout avec succès", "l'ajout a ete effectue avec succes", 1);
                wCongratulations.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                WCongratulations wCongratulations = new WCongratulations("Ajout échoué", "l'ajout n'a pas ete effectue: " + ex.Message, 0);
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