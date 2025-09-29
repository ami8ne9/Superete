using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;

namespace Superete
{
    public class Fournisseur
    {
        public int FournisseurID { get; set; }
        public string Nom { get; set; }
        public string Telephone { get; set; }
        public bool Etat { get; set; }

        private static readonly string ConnectionString = "Server=localhost\\SQLEXPRESS;Database=SUPERETE;Trusted_Connection=True;";

        public async Task<List<Fournisseur>> GetFournisseursAsync()
        {
            var fournisseurs = new List<Fournisseur>();
            string query = "SELECT * FROM Fournisseur WHERE Etat=1";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        fournisseurs.Add(new Fournisseur
                        {
                            FournisseurID = Convert.ToInt32(reader["FournisseurID"]),
                            Nom = reader["Nom"].ToString(),
                            Telephone = reader["Telephone"] == DBNull.Value ? string.Empty : reader["Telephone"].ToString(),
                            Etat = reader["Etat"] == DBNull.Value ? true : Convert.ToBoolean(reader["Etat"])
                        });
                    }
                }
            }
            return fournisseurs;
        }

        public async Task<int> InsertFournisseurAsync()
        {
            string query = "INSERT INTO Fournisseur (Nom, Telephone) VALUES (@Nom, @Telephone); SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Nom", this.Nom);
                        cmd.Parameters.AddWithValue("@Telephone", (object)this.Telephone ?? DBNull.Value);

                        object result = await cmd.ExecuteScalarAsync();
                        return Convert.ToInt32(result);
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show($"Fournisseur not inserted, error: {err}");
                    return 0;
                }
            }
        }

        public async Task<int> UpdateFournisseurAsync()
        {
            string query = "UPDATE Fournisseur SET Nom=@Nom, Telephone=@Telephone WHERE FournisseurID=@FournisseurID";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@Nom", this.Nom);
                        cmd.Parameters.AddWithValue("@Telephone", (object)this.Telephone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FournisseurID", this.FournisseurID);
                        await cmd.ExecuteNonQueryAsync();
                        return 1;
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show($"Fournisseur not updated: {err}");
                        return 0;
                    }
                }
            }
        }

        public async Task<int> DeleteFournisseurAsync()
        {
            string query = "UPDATE Fournisseur SET Etat=0 WHERE FournisseurID=@FournisseurID";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@FournisseurID", this.FournisseurID);
                        await cmd.ExecuteNonQueryAsync();
                        return 1;
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show($"Fournisseur not deleted: {err}");
                        return 0;
                    }
                }
            }
        }
    }
}