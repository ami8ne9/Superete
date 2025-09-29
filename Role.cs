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
        public int Operation { get; set; }
        public int Remise { get; set; }
        public int ModifPrix { get; set; }
        public int ModifQuantite { get; set; }
        public int Prix { get; set; }
        public int Divers { get; set; }
        public int Duplica { get; set; }
        public int Flash { get; set; }
        public int Tiroir { get; set; }
        public int Anulation { get; set; }
        public int Rapport { get; set; }
        public int Depences { get; set; }
        public int Reseption { get; set; }
        public int Sorties { get; set; }
        public int Clients { get; set; }
        public int ClientRegles { get; set; }
        public int Articles { get; set; }
        public int Solder { get; set; }
        public int Cloture { get; set; }


        private static readonly string ConnectionString =
            "Server=localhost\\SQLEXPRESS;Database=SUPERETE;Trusted_Connection=True;";

        // Get all roles
        public async Task<List<Role>> GetRolesAsync()
        {
            var roles = new List<Role>();
            string query = "SELECT * FROM Role where Etat=1";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Role role = new Role
                        {
                            RoleID = Convert.ToInt32(reader["RoleID"]),
                            RoleName = reader["RoleName"].ToString(),
                            Operation = Convert.ToInt32(reader["Operation"]),
                            Remise = Convert.ToInt32(reader["Remise"]),
                            ModifPrix = Convert.ToInt32(reader["ModifPrix"]),
                            ModifQuantite = Convert.ToInt32(reader["ModifQuantite"]),
                            Prix = Convert.ToInt32(reader["Prix"]),
                            Divers = Convert.ToInt32(reader["Divers"]),
                            Duplica = Convert.ToInt32(reader["Duplica"]),
                            Flash = Convert.ToInt32(reader["Flash"]),
                            Tiroir = Convert.ToInt32(reader["Tiroir"]),
                            Anulation = Convert.ToInt32(reader["Anulation"]),
                            Rapport = Convert.ToInt32(reader["Rapport"]),
                            Depences = Convert.ToInt32(reader["Depences"]),
                            Reseption = Convert.ToInt32(reader["Reseption"]),
                            Sorties = Convert.ToInt32(reader["Sorties"]),
                            Clients = Convert.ToInt32(reader["Clients"]),
                            ClientRegles = Convert.ToInt32(reader["ClientRegles"]),
                            Articles = Convert.ToInt32(reader["Articles"]),
                            Solder = Convert.ToInt32(reader["Solder"]),
                            Cloture = Convert.ToInt32(reader["Cloture"])
                        };
                        roles.Add(role);
                    }
                }
            }
            return roles;
        }

        // Insert a new role
        public async Task<int> InsertRoleAsync()
        {
            string query = @"INSERT INTO Role 
                (RoleName, Operation, Remise, ModifPrix, ModifQuantite, Prix, Divers, Duplica, Flash, Tiroir, Anulation, Rapport, Depences, Reseption, Sorties, Clients, ClientRegles, Articles, Solder, Cloture)
                VALUES (@RoleName, @Operation, @Remise, @ModifPrix, @ModifQuantite, @Prix, @Divers, @Duplica, @Flash, @Tiroir, @Anulation, @Rapport, @Depences, @Reseption, @Sorties, @Clients, @ClientRegles, @Articles, @Solder, @Cloture);
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleName", this.RoleName);
                        cmd.Parameters.AddWithValue("@Operation", this.Operation);
                        cmd.Parameters.AddWithValue("@Remise", this.Remise);
                        cmd.Parameters.AddWithValue("@ModifPrix", this.ModifPrix);
                        cmd.Parameters.AddWithValue("@ModifQuantite", this.ModifQuantite);
                        cmd.Parameters.AddWithValue("@Prix", this.Prix);
                        cmd.Parameters.AddWithValue("@Divers", this.Divers);
                        cmd.Parameters.AddWithValue("@Duplica", this.Duplica);
                        cmd.Parameters.AddWithValue("@Flash", this.Flash);
                        cmd.Parameters.AddWithValue("@Tiroir", this.Tiroir);
                        cmd.Parameters.AddWithValue("@Anulation", this.Anulation);
                        cmd.Parameters.AddWithValue("@Rapport", this.Rapport);
                        cmd.Parameters.AddWithValue("@Depences", this.Depences);
                        cmd.Parameters.AddWithValue("@Reseption", this.Reseption);
                        cmd.Parameters.AddWithValue("@Sorties", this.Sorties);
                        cmd.Parameters.AddWithValue("@Clients", this.Clients);
                        cmd.Parameters.AddWithValue("@ClientRegles", this.ClientRegles);
                        cmd.Parameters.AddWithValue("@Articles", this.Articles);
                        cmd.Parameters.AddWithValue("@Solder", this.Solder);
                        cmd.Parameters.AddWithValue("@Cloture", this.Cloture);

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

        // Update a role
        public async Task<int> UpdateRoleAsync()
        {
            string query = @"UPDATE Role SET 
                RoleName=@RoleName, Operation=@Operation, Remise=@Remise, ModifPrix=@ModifPrix, ModifQuantite=@ModifQuantite, 
                Prix=@Prix, Divers=@Divers, Duplica=@Duplica, Flash=@Flash, Tiroir=@Tiroir, Anulation=@Anulation, 
                Rapport=@Rapport, Depences=@Depences, Reseption=@Reseption, Sorties=@Sorties, Clients=@Clients, 
                ClientRegles=@ClientRegles, Articles=@Articles, Solder=@Solder, Cloture=@Cloture
                WHERE RoleID=@RoleID";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@RoleName", this.RoleName);
                        cmd.Parameters.AddWithValue("@Operation", this.Operation);
                        cmd.Parameters.AddWithValue("@Remise", this.Remise);
                        cmd.Parameters.AddWithValue("@ModifPrix", this.ModifPrix);
                        cmd.Parameters.AddWithValue("@ModifQuantite", this.ModifQuantite);
                        cmd.Parameters.AddWithValue("@Prix", this.Prix);
                        cmd.Parameters.AddWithValue("@Divers", this.Divers);
                        cmd.Parameters.AddWithValue("@Duplica", this.Duplica);
                        cmd.Parameters.AddWithValue("@Flash", this.Flash);
                        cmd.Parameters.AddWithValue("@Tiroir", this.Tiroir);
                        cmd.Parameters.AddWithValue("@Anulation", this.Anulation);
                        cmd.Parameters.AddWithValue("@Rapport", this.Rapport);
                        cmd.Parameters.AddWithValue("@Depences", this.Depences);
                        cmd.Parameters.AddWithValue("@Reseption", this.Reseption);
                        cmd.Parameters.AddWithValue("@Sorties", this.Sorties);
                        cmd.Parameters.AddWithValue("@Clients", this.Clients);
                        cmd.Parameters.AddWithValue("@ClientRegles", this.ClientRegles);
                        cmd.Parameters.AddWithValue("@Articles", this.Articles);
                        cmd.Parameters.AddWithValue("@Solder", this.Solder);
                        cmd.Parameters.AddWithValue("@Cloture", this.Cloture);
                        cmd.Parameters.AddWithValue("@RoleID", this.RoleID);

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

        // Delete role (hard delete, or could do soft delete if you add Etat)
        public async Task<int> DeleteRoleAsync()
        {
            string query = "UPDATE Client SET Etat=0 WHERE RoleID=@RoleID";

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
