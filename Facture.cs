using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;

namespace Superete
{
    public class Facture
    {
        public int FactureID { get; set; }
        public string Name { get; set; } = "";
        public string ICE { get; set; } = "";
        public string VAT { get; set; } = "";
        public string Telephone { get; set; } = "";
        public string Adresse { get; set; } = "";
        public bool Etat { get; set; }

        // 🆕 New fields
        public string CompanyId { get; set; } = "";
        public string EtatJuridic { get; set; } = "";
        public string SiegeEntreprise { get; set; } = "";

        private static readonly string ConnectionString =
            "Server=localhost\\SQLEXPRESS;Database=GESTIONCOMERCE;Trusted_Connection=True;";

        // Get single active facture
        public async Task<Facture> GetFactureAsync()
        {
            string query = "SELECT TOP 1 * FROM Facture WHERE Etat = 1";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        Facture f = new Facture();
                        f.FactureID = Convert.ToInt32(reader["FactureID"]);
                        f.Name = reader["Name"]?.ToString() ?? "";
                        f.ICE = reader["ICE"]?.ToString() ?? "";
                        f.VAT = reader["VAT"]?.ToString() ?? "";
                        f.Telephone = reader["Telephone"]?.ToString() ?? "";
                        f.Adresse = reader["Adresse"]?.ToString() ?? "";
                        f.Etat = reader["Etat"] != DBNull.Value && Convert.ToBoolean(reader["Etat"]);

                        // 🆕 Load new fields
                        f.CompanyId = reader["CompanyId"]?.ToString() ?? "";
                        f.EtatJuridic = reader["EtatJuridic"]?.ToString() ?? "";
                        f.SiegeEntreprise = reader["SiegeEntreprise"]?.ToString() ?? "";

                        return f;
                    }
                }
            }

            return null;
        }

        // Insert or Update facture
        public async Task<int> InsertOrUpdateFactureAsync()
        {
            Facture existing = await GetFactureAsync();

            if (existing == null)
            {
                string insertQuery = @"
                    INSERT INTO Facture (Name, ICE, VAT, Telephone, Adresse, CompanyId, EtatJuridic, SiegeEntreprise, Etat)
                    VALUES (@Name, @ICE, @VAT, @Telephone, @Adresse, @CompanyId, @EtatJuridic, @SiegeEntreprise, 1)";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@Name", Name ?? "");
                            cmd.Parameters.AddWithValue("@ICE", ICE ?? "");
                            cmd.Parameters.AddWithValue("@VAT", VAT ?? "");
                            cmd.Parameters.AddWithValue("@Telephone", Telephone ?? "");
                            cmd.Parameters.AddWithValue("@Adresse", Adresse ?? "");
                            cmd.Parameters.AddWithValue("@CompanyId", CompanyId ?? "");
                            cmd.Parameters.AddWithValue("@EtatJuridic", EtatJuridic ?? "");
                            cmd.Parameters.AddWithValue("@SiegeEntreprise", SiegeEntreprise ?? "");

                            await cmd.ExecuteNonQueryAsync();
                            return 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Facture not inserted: " + ex.Message);
                        return 0;
                    }
                }
            }
            else
            {
                string updateQuery = @"
                    UPDATE Facture
                    SET Name=@Name, ICE=@ICE, VAT=@VAT, Telephone=@Telephone, Adresse=@Adresse,
                        CompanyId=@CompanyId, EtatJuridic=@EtatJuridic, SiegeEntreprise=@SiegeEntreprise
                    WHERE FactureID=@FactureID";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(updateQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@FactureID", existing.FactureID);
                            cmd.Parameters.AddWithValue("@Name", Name ?? "");
                            cmd.Parameters.AddWithValue("@ICE", ICE ?? "");
                            cmd.Parameters.AddWithValue("@VAT", VAT ?? "");
                            cmd.Parameters.AddWithValue("@Telephone", Telephone ?? "");
                            cmd.Parameters.AddWithValue("@Adresse", Adresse ?? "");
                            cmd.Parameters.AddWithValue("@CompanyId", CompanyId ?? "");
                            cmd.Parameters.AddWithValue("@EtatJuridic", EtatJuridic ?? "");
                            cmd.Parameters.AddWithValue("@SiegeEntreprise", SiegeEntreprise ?? "");

                            await cmd.ExecuteNonQueryAsync();
                            return 2;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Facture not updated: " + ex.Message);
                        return 0;
                    }
                }
            }
        }

        // Soft delete facture
        public async Task<int> DeleteFactureAsync()
        {
            string query = "UPDATE Facture SET Etat = 0 WHERE FactureID=@FactureID";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@FactureID", FactureID);
                        await cmd.ExecuteNonQueryAsync();
                        return 1;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Facture not deleted: " + ex.Message);
                    return 0;
                }
            }
        }
    }
}
