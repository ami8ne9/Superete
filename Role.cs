using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;

namespace Superete
{
    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }

        // ✅ New Permission Columns (BIT -> bool in C#)
        public bool CreateClient { get; set; }
        public bool ModifyClient { get; set; }
        public bool DeleteClient { get; set; }
        public bool ViewOperationClient { get; set; }
        public bool PayeClient { get; set; }
        public bool ViewClient { get; set; }

        public bool CreateFournisseur { get; set; }
        public bool ModifyFournisseur { get; set; }
        public bool DeleteFournisseur { get; set; }
        public bool ViewOperationFournisseur { get; set; }
        public bool PayeFournisseur { get; set; }
        public bool ViewFournisseur { get; set; }

        public bool ReverseOperation { get; set; }
        public bool ReverseMouvment { get; set; }
        public bool ViewOperation { get; set; }
        public bool ViewMouvment { get; set; }

        public bool ViewProjectManagment { get; set; }
        public bool ViewSettings { get; set; }

        public bool ViewUsers { get; set; }
        public bool EditUsers { get; set; }
        public bool DeleteUsers { get; set; }
        public bool AddUsers { get; set; }

        public bool ViewRoles { get; set; }
        public bool AddRoles { get; set; }
        public bool DeleteRoles { get; set; }

        public bool ModifyTicket { get; set; }

        public bool ViewFamilly { get; set; }
        public bool EditFamilly { get; set; }
        public bool DeleteFamilly { get; set; }
        public bool AddFamilly { get; set; }

        private static readonly string ConnectionString =
            "Server=localhost\\SQLEXPRESS;Database=SUPERETE;Trusted_Connection=True;";

        // ✅ Get all roles
        public async Task<List<Role>> GetRolesAsync()
        {
            var roles = new List<Role>();
            string query = "SELECT * FROM Role WHERE Etat=1";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var role = new Role
                        {
                            RoleID = Convert.ToInt32(reader["RoleID"]),
                            RoleName = reader["RoleName"].ToString(),

                            CreateClient = Convert.ToBoolean(reader["CreateClient"]),
                            ModifyClient = Convert.ToBoolean(reader["ModifyClient"]),
                            DeleteClient = Convert.ToBoolean(reader["DeleteClient"]),
                            ViewOperationClient = Convert.ToBoolean(reader["ViewOperationClient"]),
                            PayeClient = Convert.ToBoolean(reader["PayeClient"]),
                            ViewClient = Convert.ToBoolean(reader["ViewClient"]),

                            CreateFournisseur = Convert.ToBoolean(reader["CreateFournisseur"]),
                            ModifyFournisseur = Convert.ToBoolean(reader["ModifyFournisseur"]),
                            DeleteFournisseur = Convert.ToBoolean(reader["DeleteFournisseur"]),
                            ViewOperationFournisseur = Convert.ToBoolean(reader["ViewOperationFournisseur"]),
                            PayeFournisseur = Convert.ToBoolean(reader["PayeFournisseur"]),
                            ViewFournisseur = Convert.ToBoolean(reader["ViewFournisseur"]),

                            ReverseOperation = Convert.ToBoolean(reader["ReverseOperation"]),
                            ReverseMouvment = Convert.ToBoolean(reader["ReverseMouvment"]),
                            ViewOperation = Convert.ToBoolean(reader["ViewOperation"]),
                            ViewMouvment = Convert.ToBoolean(reader["ViewMouvment"]),

                            ViewProjectManagment = Convert.ToBoolean(reader["ViewProjectManagment"]),
                            ViewSettings = Convert.ToBoolean(reader["ViewSettings"]),

                            ViewUsers = Convert.ToBoolean(reader["ViewUsers"]),
                            EditUsers = Convert.ToBoolean(reader["EditUsers"]),
                            DeleteUsers = Convert.ToBoolean(reader["DeleteUsers"]),
                            AddUsers = Convert.ToBoolean(reader["AddUsers"]),

                            ViewRoles = Convert.ToBoolean(reader["ViewRoles"]),
                            AddRoles = Convert.ToBoolean(reader["AddRoles"]),
                            DeleteRoles = Convert.ToBoolean(reader["DeleteRoles"]),

                            ModifyTicket = Convert.ToBoolean(reader["ModifyTicket"]),

                            ViewFamilly = Convert.ToBoolean(reader["ViewFamilly"]),
                            EditFamilly = Convert.ToBoolean(reader["EditFamilly"]),
                            DeleteFamilly = Convert.ToBoolean(reader["DeleteFamilly"]),
                            AddFamilly = Convert.ToBoolean(reader["AddFamilly"])
                        };
                        roles.Add(role);
                    }
                }
            }
            return roles;
        }

        // ✅ Insert new role
        public async Task<int> InsertRoleAsync()
        {
            string query = @"INSERT INTO Role 
                (RoleName, CreateClient, ModifyClient, DeleteClient, ViewOperationClient, PayeClient, ViewClient,
                 CreateFournisseur, ModifyFournisseur, DeleteFournisseur, ViewOperationFournisseur, PayeFournisseur, ViewFournisseur,
                 ReverseOperation, ReverseMouvment, ViewOperation, ViewMouvment,
                 ViewProjectManagment, ViewSettings,
                 ViewUsers, EditUsers, DeleteUsers, AddUsers,
                 ViewRoles, AddRoles, DeleteRoles,
                 ModifyTicket,
                 ViewFamilly, EditFamilly, DeleteFamilly, AddFamilly)
                VALUES 
                (@RoleName, @CreateClient, @ModifyClient, @DeleteClient, @ViewOperationClient, @PayeClient, @ViewClient,
                 @CreateFournisseur, @ModifyFournisseur, @DeleteFournisseur, @ViewOperationFournisseur, @PayeFournisseur, @ViewFournisseur,
                 @ReverseOperation, @ReverseMouvment, @ViewOperation, @ViewMouvment,
                 @ViewProjectManagment, @ViewSettings,
                 @ViewUsers, @EditUsers, @DeleteUsers, @AddUsers,
                 @ViewRoles, @AddRoles, @DeleteRoles,
                 @ModifyTicket,
                 @ViewFamilly, @EditFamilly, @DeleteFamilly, @AddFamilly);
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleName", this.RoleName);

                        cmd.Parameters.AddWithValue("@CreateClient", this.CreateClient);
                        cmd.Parameters.AddWithValue("@ModifyClient", this.ModifyClient);
                        cmd.Parameters.AddWithValue("@DeleteClient", this.DeleteClient);
                        cmd.Parameters.AddWithValue("@ViewOperationClient", this.ViewOperationClient);
                        cmd.Parameters.AddWithValue("@PayeClient", this.PayeClient);
                        cmd.Parameters.AddWithValue("@ViewClient", this.ViewClient);

                        cmd.Parameters.AddWithValue("@CreateFournisseur", this.CreateFournisseur);
                        cmd.Parameters.AddWithValue("@ModifyFournisseur", this.ModifyFournisseur);
                        cmd.Parameters.AddWithValue("@DeleteFournisseur", this.DeleteFournisseur);
                        cmd.Parameters.AddWithValue("@ViewOperationFournisseur", this.ViewOperationFournisseur);
                        cmd.Parameters.AddWithValue("@PayeFournisseur", this.PayeFournisseur);
                        cmd.Parameters.AddWithValue("@ViewFournisseur", this.ViewFournisseur);

                        cmd.Parameters.AddWithValue("@ReverseOperation", this.ReverseOperation);
                        cmd.Parameters.AddWithValue("@ReverseMouvment", this.ReverseMouvment);
                        cmd.Parameters.AddWithValue("@ViewOperation", this.ViewOperation);
                        cmd.Parameters.AddWithValue("@ViewMouvment", this.ViewMouvment);

                        cmd.Parameters.AddWithValue("@ViewProjectManagment", this.ViewProjectManagment);
                        cmd.Parameters.AddWithValue("@ViewSettings", this.ViewSettings);

                        cmd.Parameters.AddWithValue("@ViewUsers", this.ViewUsers);
                        cmd.Parameters.AddWithValue("@EditUsers", this.EditUsers);
                        cmd.Parameters.AddWithValue("@DeleteUsers", this.DeleteUsers);
                        cmd.Parameters.AddWithValue("@AddUsers", this.AddUsers);

                        cmd.Parameters.AddWithValue("@ViewRoles", this.ViewRoles);
                        cmd.Parameters.AddWithValue("@AddRoles", this.AddRoles);
                        cmd.Parameters.AddWithValue("@DeleteRoles", this.DeleteRoles);

                        cmd.Parameters.AddWithValue("@ModifyTicket", this.ModifyTicket);

                        cmd.Parameters.AddWithValue("@ViewFamilly", this.ViewFamilly);
                        cmd.Parameters.AddWithValue("@EditFamilly", this.EditFamilly);
                        cmd.Parameters.AddWithValue("@DeleteFamilly", this.DeleteFamilly);
                        cmd.Parameters.AddWithValue("@AddFamilly", this.AddFamilly);

                        object result = await cmd.ExecuteScalarAsync();
                        return Convert.ToInt32(result);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Role not inserted, error: {ex.Message}");
                    return 0;
                }
            }
        }

        // ✅ Update role
        public async Task<int> UpdateRoleAsync()
        {
            string query = @"UPDATE Role SET 
                RoleName=@RoleName,
                CreateClient=@CreateClient, ModifyClient=@ModifyClient, DeleteClient=@DeleteClient, ViewOperationClient=@ViewOperationClient, PayeClient=@PayeClient, ViewClient=@ViewClient,
                CreateFournisseur=@CreateFournisseur, ModifyFournisseur=@ModifyFournisseur, DeleteFournisseur=@DeleteFournisseur, ViewOperationFournisseur=@ViewOperationFournisseur, PayeFournisseur=@PayeFournisseur, ViewFournisseur=@ViewFournisseur,
                ReverseOperation=@ReverseOperation, ReverseMouvment=@ReverseMouvment, ViewOperation=@ViewOperation, ViewMouvment=@ViewMouvment,
                ViewProjectManagment=@ViewProjectManagment, ViewSettings=@ViewSettings,
                ViewUsers=@ViewUsers, EditUsers=@EditUsers, DeleteUsers=@DeleteUsers, AddUsers=@AddUsers,
                ViewRoles=@ViewRoles, AddRoles=@AddRoles, DeleteRoles=@DeleteRoles,
                ModifyTicket=@ModifyTicket,
                ViewFamilly=@ViewFamilly, EditFamilly=@EditFamilly, DeleteFamilly=@DeleteFamilly, AddFamilly=@AddFamilly
                WHERE RoleID=@RoleID";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleName", this.RoleName);
                        cmd.Parameters.AddWithValue("@RoleID", this.RoleID);

                        cmd.Parameters.AddWithValue("@CreateClient", this.CreateClient);
                        cmd.Parameters.AddWithValue("@ModifyClient", this.ModifyClient);
                        cmd.Parameters.AddWithValue("@DeleteClient", this.DeleteClient);
                        cmd.Parameters.AddWithValue("@ViewOperationClient", this.ViewOperationClient);
                        cmd.Parameters.AddWithValue("@PayeClient", this.PayeClient);
                        cmd.Parameters.AddWithValue("@ViewClient", this.ViewClient);

                        cmd.Parameters.AddWithValue("@CreateFournisseur", this.CreateFournisseur);
                        cmd.Parameters.AddWithValue("@ModifyFournisseur", this.ModifyFournisseur);
                        cmd.Parameters.AddWithValue("@DeleteFournisseur", this.DeleteFournisseur);
                        cmd.Parameters.AddWithValue("@ViewOperationFournisseur", this.ViewOperationFournisseur);
                        cmd.Parameters.AddWithValue("@PayeFournisseur", this.PayeFournisseur);
                        cmd.Parameters.AddWithValue("@ViewFournisseur", this.ViewFournisseur);

                        cmd.Parameters.AddWithValue("@ReverseOperation", this.ReverseOperation);
                        cmd.Parameters.AddWithValue("@ReverseMouvment", this.ReverseMouvment);
                        cmd.Parameters.AddWithValue("@ViewOperation", this.ViewOperation);
                        cmd.Parameters.AddWithValue("@ViewMouvment", this.ViewMouvment);

                        cmd.Parameters.AddWithValue("@ViewProjectManagment", this.ViewProjectManagment);
                        cmd.Parameters.AddWithValue("@ViewSettings", this.ViewSettings);

                        cmd.Parameters.AddWithValue("@ViewUsers", this.ViewUsers);
                        cmd.Parameters.AddWithValue("@EditUsers", this.EditUsers);
                        cmd.Parameters.AddWithValue("@DeleteUsers", this.DeleteUsers);
                        cmd.Parameters.AddWithValue("@AddUsers", this.AddUsers);

                        cmd.Parameters.AddWithValue("@ViewRoles", this.ViewRoles);
                        cmd.Parameters.AddWithValue("@AddRoles", this.AddRoles);
                        cmd.Parameters.AddWithValue("@DeleteRoles", this.DeleteRoles);

                        cmd.Parameters.AddWithValue("@ModifyTicket", this.ModifyTicket);

                        cmd.Parameters.AddWithValue("@ViewFamilly", this.ViewFamilly);
                        cmd.Parameters.AddWithValue("@EditFamilly", this.EditFamilly);
                        cmd.Parameters.AddWithValue("@DeleteFamilly", this.DeleteFamilly);
                        cmd.Parameters.AddWithValue("@AddFamilly", this.AddFamilly);

                        await cmd.ExecuteNonQueryAsync();
                        return 1;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Role not updated: {ex.Message}");
                    return 0;
                }
            }
        }

        // ✅ Soft delete role (Etat=0)
        public async Task<int> DeleteRoleAsync()
        {
            string query = "UPDATE Role SET Etat=0 WHERE RoleID=@RoleID";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleID", this.RoleID);
                        await cmd.ExecuteNonQueryAsync();
                        return 1;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Role not deleted: {ex.Message}");
                    return 0;
                }
            }
        }
    }
}
