using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;

namespace Superete
{
    public class Client
    {
        public int ClientID { get; set; }
        public string Nom { get; set; }
        public string Telephone { get; set; }
        public bool Etat { get; set; } // Added Etat property
        
        private static readonly string ConnectionString = "Server=localhost\\SQLEXPRESS;Database=SUPERETE;Trusted_Connection=True;";

        public async Task<List<Client>> GetClientsAsync()
        {
            var clients = new List<Client>();
            string query = "SELECT * FROM Client WHERE Etat=1";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Client client = new Client
                        {
                            ClientID = Convert.ToInt32(reader["ClientID"]),
                            Nom = reader["Nom"].ToString(),
                            Telephone = reader["Telephone"] == DBNull.Value ? string.Empty : reader["Telephone"].ToString()
                        };
                        clients.Add(client);
                    }
                }
            }
            return clients;
        }

        public async Task<int> InsertClientAsync()
        {
            string query = "INSERT INTO Client (Nom, Telephone) VALUES (@Nom, @Telephone); SELECT SCOPE_IDENTITY();";

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
                        int id = Convert.ToInt32(result);
                        return id;
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show($"Client not inserted, error: {err}");
                    return 0;
                }
            }
        }

        public async Task<int> UpdateClientAsync()
        {
            string query = "UPDATE Client SET Nom=@Nom, Telephone=@Telephone WHERE ClientID=@ClientID";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@Nom", this.Nom);
                        cmd.Parameters.AddWithValue("@Telephone", (object)this.Telephone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ClientID", this.ClientID);
                        await cmd.ExecuteNonQueryAsync();
                        return 1;
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show($"Client not updated: {err}");
                        return 0;
                    }
                }
            }
        }

        public async Task<int> DeleteClientAsync()
        {
            string query = "UPDATE Client SET Etat=0 WHERE ClientID=@ClientID";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@ClientID", this.ClientID);
                        await cmd.ExecuteNonQueryAsync();
                        return 1;
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show($"Client not deleted: {err}");
                        return 0;
                    }
                }
            }
        }
    }
}