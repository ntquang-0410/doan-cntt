using System;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace ConvenienceStoreApp
{
    public static class DatabaseHelper
    {
        private static readonly string ConnectionString = 
            ConfigurationManager.ConnectionStrings["MySQLConnection"] != null 
            ? ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString 
            : null;

        public static MySqlConnection GetConnection()
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new InvalidOperationException("Connection string 'MySQLConnection' is not configured in App.config.");
            }
            MySqlConnection conn = new MySqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        public static DataTable ExecuteQuery(string sql, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static int ExecuteNonQuery(string sql, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static object ExecuteScalar(string sql, params MySqlParameter[] parameters)
        {
            using (MySqlConnection conn = GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteScalar();
                }
            }
        }

        public static bool VerifyPassword(string inputPassword, string dbPasswordHash, string username)
        {
            // Fallback for placeholder bcrypt hash from seed_data.sql
            if (dbPasswordHash == "$2a$10$YourBCryptHashHere123456789012345678901234567890ABC")
            {
                // In seed data, we allow logging in with the username as password for local testing
                return inputPassword == username;
            }

            // Plain text comparison fallback
            if (inputPassword == dbPasswordHash)
            {
                return true;
            }

            // SHA256 fallback comparison
            try
            {
                using (var sha = System.Security.Cryptography.SHA256.Create())
                {
                    byte[] bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(inputPassword));
                    string hash = BitConverter.ToString(bytes).Replace("-", "").ToLower();
                    if (hash == dbPasswordHash.ToLower())
                    {
                        return true;
                    }
                }
            }
            catch { }

            return false;
        }

        public static string HashPassword(string password)
        {
            // Simple SHA256 helper for new passwords
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}
