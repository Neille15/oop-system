using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using oop_backend.Models;
namespace oop_backend
{

    public class Database
    {


        private readonly string connectionString;

        public Database()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            builder.DataSource = "MSSQLLocalDB";
            builder.InitialCatalog = "oop_system_db";
            builder.IntegratedSecurity = true;        // Use Windows Authentication

            // builder.UserID = "YourUsername";
            // builder.Password = "YourPassword";

            builder.ConnectTimeout = 30;

            this.connectionString = builder.ConnectionString;
        }
        public int RegisterUser(UserData userData)
        {
            string sql = "INSERT INTO users (firstName, lastName, email, birthDate) VALUES (@firstName, @lastName, @email, @birthDate)";
            var parameters = new Dictionary<string, object>
            {
                {"@firstName", userData.FirstName},
                {"@lastName", userData.LastName},
                {"@email", userData.Email},
                {"@birthDate", userData.BirthDate}
            };
            var status = this.ExecuteNonQuery(sql, parameters);
            return status;
        }
        public int ExecuteNonQuery(string query, Dictionary<string, object>? parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {

                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    try
                    {
                        connection.Open();
                        return command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine($"Database Error executing command: {ex.Message}");
                        return -1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                        return -1;
                    }
                }
            }
        }
        public object? ExecuteQueryScalar(string query, Dictionary<string, object>? parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {

                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    try
                    {
                        connection.Open();

                        object result = command.ExecuteScalar();

                        return result;
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine($"Database Error executing query '{query}': {ex.Message}");
                        return null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                        return null;
                    }
                }
            }
        }

        public DataTable? GetDataTable(string query, Dictionary<string, object>? parameters = null)
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {

                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    try
                    {
                        connection.Open();

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }

                        return dataTable;
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine($"Database Error executing query '{query}': {ex.Message}");
                        return null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                        return null;
                    }
                }
            }
        }
    }
}