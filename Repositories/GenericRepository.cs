using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Repositories
{
    public class GenericRepository<T> where T : class
    {
        private readonly string _connectionString = "Server=K5\\SQLEXPRESS;Database=TaskDb;Trusted_Connection=true;Encrypt=false";

        public T? GetById(int id)
        {
            using IDbConnection connection = new SqlConnection(_connectionString);
            string tableName = GetTableName();
            string keyColumn = GetKeyColumnName()!;
            string query = $"SELECT * FROM {tableName} WHERE {keyColumn} = @Id";
            return connection.QueryFirstOrDefault<T>(query, new { Id = id });
        }

        public IEnumerable<T> GetAll()
        {
            using IDbConnection connection = new SqlConnection(_connectionString);
            string tableName = GetTableName();
            string query = $"SELECT * FROM {tableName}";
            return connection.Query<T>(query);
        }

        public T Add(T entity)
        {
            using IDbConnection connection = new SqlConnection(_connectionString);
            string tableName = GetTableName();
            string columns = GetColumnNames(true);
            string values = GetColumnValues(true);
            string keyColumn = GetKeyColumnName()!;

            string query = $"INSERT INTO {tableName} ({columns}) VALUES ({values}); " +
                           $"SELECT * FROM {tableName} WHERE {keyColumn} = SCOPE_IDENTITY()";

            return connection.QueryFirstOrDefault<T>(query, entity)!;
        }

        public bool Update(T entity)
        {
            using IDbConnection connection = new SqlConnection(_connectionString);
            string tableName = GetTableName();
            string keyColumn = GetKeyColumnName()!;
            string keyProperty = GetKeyPropertyName()!;
        
            StringBuilder query = new StringBuilder($"UPDATE {tableName} SET ");
            var parameters = new DynamicParameters();
        
            foreach (var prop in GetProperties(true))
            {
                if (prop.Name == keyProperty || prop.Name == "DateCreated") continue;
                string columnName = prop.GetCustomAttribute<ColumnAttribute>()?.Name ?? prop.Name;
                var value = prop.GetValue(entity);
        
        
                if (value is DateTime dt && dt < (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue.Value)
                {
                    value = System.Data.SqlTypes.SqlDateTime.MinValue.Value;  
                }
        
                query.Append($"{columnName} = @{prop.Name}, ");
                parameters.Add($"@{prop.Name}", value);
            }
        
            query.Length -= 2;  // Remove last comma
            query.Append($" WHERE {keyColumn} = @{keyProperty}");
        
            var keyValue = typeof(T).GetProperty(keyProperty)?.GetValue(entity);
            parameters.Add($"@{keyProperty}", keyValue);
        
            int rowsAffected = connection.Execute(query.ToString(), parameters);
            return rowsAffected == 1;
        }
        public bool Delete(T entity)
        {
            using IDbConnection connection = new SqlConnection(_connectionString);
            string tableName = GetTableName();
            string keyColumn = GetKeyColumnName()!;
            string keyProperty = GetKeyPropertyName()!;

            string query = $"DELETE FROM {tableName} WHERE {keyColumn} = @{keyProperty}";
            int rows = connection.Execute(query, entity);
            return rows == 1;
        }

        // Helpers
        private IEnumerable<PropertyInfo> GetProperties(bool excludeKey = false) =>
            typeof(T).GetProperties().Where(p => !excludeKey || p.GetCustomAttribute<KeyAttribute>() == null);

        private string GetTableName() => typeof(T).GetCustomAttribute<TableAttribute>()?.Name ?? typeof(T).Name;

        private string GetColumnNames(bool excludeKey = false) =>
            string.Join(", ", GetProperties(excludeKey).Select(p => p.GetCustomAttribute<ColumnAttribute>()?.Name ?? p.Name));

        private string GetColumnValues(bool excludeKey = false) =>
            string.Join(", ", GetProperties(excludeKey).Select(p => $"@{p.Name}"));

        private string? GetKeyPropertyName() =>
            typeof(T).GetProperties().FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null)?.Name;

        private string? GetKeyColumnName()
        {
            var prop = typeof(T).GetProperties().FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null);
            return prop?.GetCustomAttribute<ColumnAttribute>()?.Name ?? prop?.Name;
        }
    }
}
