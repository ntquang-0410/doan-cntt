using System;
using System.Collections.Generic;
using System.Data;
using ConvenienceStoreApp.DTOs;
using ConvenienceStoreApp.Models;
using MySql.Data.MySqlClient;

namespace ConvenienceStoreApp.DAL
{
    public class CustomerRepository
    {
        public List<CustomerListItemDto> Search(string keyword)
        {
            string sql = @"
                SELECT id, name, phone, email, loyalty_points, total_spent, is_active
                FROM customers
                WHERE 1=1";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql += " AND (name LIKE @kw OR phone = @kwExact)";
                parameters.Add(new MySqlParameter("@kw", "%" + keyword.Trim() + "%"));
                parameters.Add(new MySqlParameter("@kwExact", keyword.Trim()));
            }

            sql += " ORDER BY id DESC";

            DataTable table = DatabaseHelper.ExecuteQuery(sql, parameters.ToArray());
            List<CustomerListItemDto> result = new List<CustomerListItemDto>();

            foreach (DataRow row in table.Rows)
            {
                result.Add(new CustomerListItemDto
                {
                    Id = Convert.ToInt32(row["id"]),
                    HoTen = row["name"].ToString(),
                    SoDienThoai = row["phone"].ToString(),
                    Email = row["email"] == DBNull.Value ? "" : row["email"].ToString(),
                    DiemTichLuy = Convert.ToInt32(row["loyalty_points"]),
                    TongChiTieu = Convert.ToDecimal(row["total_spent"]),
                    TrangThai = Convert.ToBoolean(row["is_active"]) ? "Hoạt động" : "Ngưng"
                });
            }

            return result;
        }

        public Customer GetById(int id)
        {
            DataTable table = DatabaseHelper.ExecuteQuery(
                "SELECT * FROM customers WHERE id = @id",
                new MySqlParameter("@id", id));

            if (table.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = table.Rows[0];
            return new Customer
            {
                Id = Convert.ToInt32(row["id"]),
                Name = row["name"].ToString(),
                Phone = row["phone"].ToString(),
                Email = row["email"] == DBNull.Value ? "" : row["email"].ToString(),
                LoyaltyPoints = Convert.ToInt32(row["loyalty_points"]),
                TotalSpent = Convert.ToDecimal(row["total_spent"]),
                IsActive = Convert.ToBoolean(row["is_active"]),
                CreatedAt = Convert.ToDateTime(row["created_at"]),
                UpdatedAt = Convert.ToDateTime(row["updated_at"])
            };
        }

        public List<LoyaltyHistoryDto> GetLoyaltyHistory(int customerId)
        {
            string sql = @"
                SELECT lt.id, lt.order_id, lt.points,
                       CASE lt.transaction_type
                           WHEN 'earn' THEN 'Cộng điểm (+)'
                           WHEN 'redeem' THEN 'Trừ điểm (-)'
                           WHEN 'expire' THEN 'Hết hạn'
                           ELSE 'Điều chỉnh'
                       END AS transaction_type_display,
                       lt.description,
                       lt.created_at
                FROM loyalty_transactions lt
                WHERE lt.customer_id = @cid
                ORDER BY lt.created_at DESC";

            DataTable table = DatabaseHelper.ExecuteQuery(sql, new MySqlParameter("@cid", customerId));
            List<LoyaltyHistoryDto> result = new List<LoyaltyHistoryDto>();

            foreach (DataRow row in table.Rows)
            {
                result.Add(new LoyaltyHistoryDto
                {
                    LogId = Convert.ToInt32(row["id"]),
                    DonHang = row["order_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["order_id"]),
                    SoDiem = Convert.ToInt32(row["points"]),
                    LoaiGiaoDich = row["transaction_type_display"].ToString(),
                    MoTa = row["description"] == DBNull.Value ? "" : row["description"].ToString(),
                    ThoiGian = Convert.ToDateTime(row["created_at"])
                });
            }

            return result;
        }

        public void Create(Customer customer)
        {
            string sql = "INSERT INTO customers (name, phone, email, is_active) VALUES (@name, @phone, @email, @active)";
            DatabaseHelper.ExecuteNonQuery(
                sql,
                new MySqlParameter("@name", customer.Name),
                new MySqlParameter("@phone", customer.Phone),
                new MySqlParameter("@email", string.IsNullOrWhiteSpace(customer.Email) ? DBNull.Value : (object)customer.Email),
                new MySqlParameter("@active", customer.IsActive ? 1 : 0));
        }

        public void Update(Customer customer)
        {
            string sql = "UPDATE customers SET name = @name, phone = @phone, email = @email, is_active = @active WHERE id = @id";
            DatabaseHelper.ExecuteNonQuery(
                sql,
                new MySqlParameter("@name", customer.Name),
                new MySqlParameter("@phone", customer.Phone),
                new MySqlParameter("@email", string.IsNullOrWhiteSpace(customer.Email) ? DBNull.Value : (object)customer.Email),
                new MySqlParameter("@active", customer.IsActive ? 1 : 0),
                new MySqlParameter("@id", customer.Id));
        }
    }
}
