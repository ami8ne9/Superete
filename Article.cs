using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;

namespace Superete
{
    public class Article
    {
        public int ArticleID { get; set; }
        public int Quantite { get; set; }
        public decimal PrixAchat { get; set; }
        public decimal PrixVente { get; set; }
        public decimal PrixMP { get; set; }
        public int FamillyID { get; set; }
        public long Code { get; set; }
        public string ArticleName { get; set; }
        public bool Etat { get; set; }

        public int FournisseurID { get; set; }

        private static readonly string ConnectionString = "Server=localhost\\SQLEXPRESS;Database=SUPERETE;Trusted_Connection=True;";

        public async Task<List<Article>> GetArticlesAsync()
        {
            var articles = new List<Article>();
            string query = "SELECT * FROM Article where Etat=1";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Article article = new Article
                        {
                            ArticleID = Convert.ToInt32(reader["ArticleID"]),
                            Quantite = Convert.ToInt32(reader["Quantite"]),
                            PrixAchat = Convert.ToDecimal(reader["PrixAchat"]),
                            PrixVente = Convert.ToDecimal(reader["PrixVente"]),
                            PrixMP = Convert.ToDecimal(reader["PrixMP"]),
                            FamillyID = Convert.ToInt32(reader["FamillyID"]),
                            FournisseurID = Convert.ToInt32(reader["FournisseurID"]),
                            Code = reader["Code"] == DBNull.Value ? 0 : Convert.ToInt64(reader["Code"]),
                            ArticleName = reader["ArticleName"] == DBNull.Value ? string.Empty : reader["ArticleName"].ToString()
                        };
                        articles.Add(article);
                    }
                }
            }
            return articles;
        }
        public async Task<List<Article>> GetAllArticlesAsync()
        {
            var articles = new List<Article>();
            string query = "SELECT * FROM Article";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Article article = new Article
                        {
                            ArticleID = Convert.ToInt32(reader["ArticleID"]),
                            Quantite = Convert.ToInt32(reader["Quantite"]),
                            PrixAchat = Convert.ToDecimal(reader["PrixAchat"]),
                            PrixVente = Convert.ToDecimal(reader["PrixVente"]),
                            PrixMP = Convert.ToDecimal(reader["PrixMP"]),
                            FamillyID = Convert.ToInt32(reader["FamillyID"]),
                            FournisseurID = Convert.ToInt32(reader["FournisseurID"]),
                            Etat= Convert.ToBoolean(reader["Etat"]),
							Code = reader["Code"] == DBNull.Value ? 0L : Convert.ToInt64(reader["Code"]),
							ArticleName = reader["ArticleName"] == DBNull.Value ? string.Empty : reader["ArticleName"].ToString()
                        };
                        articles.Add(article);
                    }
                }
            }
            return articles;
        }

        public async Task<int> InsertArticleAsync()
        {
            string query = "INSERT INTO Article (Quantite, PrixAchat, PrixVente, PrixMP, FamillyID, Code,FournisseurID, ArticleName) " +
                           "VALUES (@Quantite, @PrixAchat, @PrixVente, @PrixMP, @FamillyID, @Code,@FournisseurID, @ArticleName); SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Quantite", this.Quantite);
                        cmd.Parameters.AddWithValue("@PrixAchat", this.PrixAchat);
                        cmd.Parameters.AddWithValue("@PrixVente", this.PrixVente);
                        cmd.Parameters.AddWithValue("@PrixMP", this.PrixMP);
                        cmd.Parameters.AddWithValue("@FamillyID", this.FamillyID);
                        cmd.Parameters.AddWithValue("@Code", this.Code);
                        cmd.Parameters.AddWithValue("@FournisseurID", this.FournisseurID);
                        cmd.Parameters.AddWithValue("@ArticleName", this.ArticleName ?? string.Empty);

                        object result = await cmd.ExecuteScalarAsync();
                        int id = Convert.ToInt32(result);
                        return id;
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show($"Article not inserted, error: {err}");
                    return 0;
                }
            }
        }

        public async Task<int> DeleteArticleAsync()
        {
            string query = "UPDATE Article SET Etat=0 WHERE ArticleID=@ArticleID";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@ArticleID", this.ArticleID);
                        await cmd.ExecuteNonQueryAsync();
                        return 1;
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show($"Article not deleted: {err}");
                        return 0;
                    }
                }
            }
        }
        public async Task<int> BringBackArticleAsync()
        {
            string query = "UPDATE Article SET Etat=1 WHERE ArticleID=@ArticleID";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@ArticleID", this.ArticleID);
                        await cmd.ExecuteNonQueryAsync();
                        return 1;
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show($"Article not deleted: {err}");
                        return 0;
                    }
                }
            }
        }

        public async Task<int> UpdateArticleAsync()
        {
            string query = "UPDATE Article SET " +
                           "Quantite=@Quantite, " +
                           "PrixAchat=@PrixAchat, " +
                           "PrixVente=@PrixVente, " +
                           "PrixMP=@PrixMP, " +
                           "FamillyID=@FamillyID, " +
                           "Code=@Code, " +
                           "ArticleName=@ArticleName " +
                           "WHERE ArticleID=@ArticleID";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@Quantite", this.Quantite);
                        cmd.Parameters.AddWithValue("@PrixAchat", this.PrixAchat);
                        cmd.Parameters.AddWithValue("@PrixVente", this.PrixVente);
                        cmd.Parameters.AddWithValue("@PrixMP", this.PrixMP);
                        cmd.Parameters.AddWithValue("@FamillyID", this.FamillyID);
                        cmd.Parameters.AddWithValue("@Code", this.Code);
                        cmd.Parameters.AddWithValue("@ArticleName", this.ArticleName ?? string.Empty);
                        cmd.Parameters.AddWithValue("@ArticleID", this.ArticleID);
                        await cmd.ExecuteNonQueryAsync();
                        return 1;
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show($"Article not updated: {err}");
                        return 0;
                    }
                }
            }
        }
    }
}