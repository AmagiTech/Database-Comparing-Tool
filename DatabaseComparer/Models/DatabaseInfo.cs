using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseComparer
{
    [Serializable]
    public class DatabaseInfo
    {
        private readonly string connectionString;
        public DatabaseInfo(string connectionString)
        {
            this.connectionString = connectionString;
            DatabaseName = GetDatabaseName();
            ServerCollation = GetServerCollation();
            DatabaseCollation = GetDatabaseCollation();
            Tables = GetTables();
            Indexes = GetIndexes();
            Procedures = GetSysObjects<Procedure>();
            Triggers = GetSysObjects<Trigger>();
            Functions = GetSysObjects<Function>();
            ForeignKeys = GetSysObjects<ForeignKey>();
        }


        public string DatabaseName { get; }
        public string ServerCollation { get; }
        public string DatabaseCollation { get; }
        public List<TableInfo> Tables { get; }
        public List<string> Indexes { get; }
        public List<Procedure> Procedures { get; }
        public List<Trigger> Triggers { get; }
        public List<Function> Functions { get; }
        public List<ForeignKey> ForeignKeys { get; }

        private string GetServerCollation()
        {
            return ExecuteScalar("SELECT CONVERT (VARCHAR, SERVERPROPERTY('collation'))").ToString();
        }
        private string GetDatabaseName()
        {
            using (var con = new SqlConnection(connectionString))
                return con.Database;
        }
        private string GetDatabaseCollation()
        {
            return ExecuteScalar("SELECT CONVERT (VARCHAR, DATABASEPROPERTYEX('pacsdb','collation'))").ToString();
        }
        private List<TableInfo> GetTables()
        {
            var result = new List<TableInfo>();
            var cmdText = "SELECT TABLE_CATALOG,TABLE_SCHEMA,TABLE_NAME,TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_NAME";
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(cmdText, con))
            {
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var ti = new TableInfo()
                    {
                        Catalog = reader.GetString(0),
                        Schema = reader.GetString(1),
                        Name = reader.GetString(2),
                        TableType = reader.GetString(3)
                    };
                    foreach (var ci in GetColumns(ti.Name))
                    {
                        ti.Columns.Add(ci);
                    }

                    result.Add(ti);
                }
            }
            return result;
        }

        private IEnumerable<ColumnInfo> GetColumns(string tableName = "")
        {
            var cmdText = "SELECT TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME, COLUMN_DEFAULT, IS_NULLABLE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION, NUMERIC_SCALE,DATETIME_PRECISION,COLLATION_NAME FROM INFORMATION_SCHEMA.COLUMNS"
                + (string.IsNullOrWhiteSpace(tableName) ? " " : " WHERE TABLE_NAME = @tableName ")
                + " ORDER BY TABLE_NAME,COLUMN_NAME ";
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(cmdText, con))
            {
                con.Open();
                if (!string.IsNullOrWhiteSpace(tableName))
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    yield return new ColumnInfo
                    {
                        TableCatalog = reader.GetString(0),
                        TableSchema = reader.GetString(1),
                        TableName = reader.GetString(2),
                        ColumnName = reader.GetString(3),
                        ColumnDefault = GetValue<string>("COLUMN_DEFAULT", reader),
                        IsNullable = GetValue<string>("IS_NULLABLE", reader),
                        DataType = GetValue<string>("DATA_TYPE", reader),
                        CharacterMaximumLength = GetValue<int?>("CHARACTER_MAXIMUM_LENGTH", reader),
                        NumericPrecision = GetValue<byte?>("NUMERIC_PRECISION", reader),
                        NumericScale = GetValue<int?>("NUMERIC_SCALE", reader),
                        DateTimePrecision = GetValue<short?>("DATETIME_PRECISION", reader),
                        CollationName = GetValue<string>("COLLATION_NAME", reader),
                    };
                }
            }
        }

        private List<string> GetIndexes()
        {

            var cmdText = "SELECT name FROM sys.indexes WHERE name is not null  AND object_id > (SELECT MIN(OBJECT_ID) - 1  FROM sys.indexes where name is not null and is_primary_key=1)";
            var result = new List<string>();
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(cmdText, con))
            {
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(reader.GetString(0));
                }
            }
            return result;
        }

        public List<T> GetSysObjects<T>() where T : SysObject, new()
        {
            var result = new List<T>();
            var cmdText = "SELECT name,OBJECT_DEFINITION (id) as Definition   FROM dbo.sysobjects WHERE type = @type";
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(cmdText, con))
            {
                var t = new T();
                cmd.Parameters.AddWithValue("@type", t.Type);
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var r = new T();
                    r.Name = reader.GetString(0);
                    r.Definition = GetValue<string>("Definition", reader);
                    result.Add(r);
                }
            }
            return result;
        }



        private object ExecuteScalar(string cmdText)
        {
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(cmdText, con))
            {
                con.Open();
                return cmd.ExecuteScalar().ToString();
            }
        }

        private T GetValue<T>(string columnName, SqlDataReader reader)
        {
            var ordinal = reader.GetOrdinal(columnName);
            var value = reader.GetValue(ordinal);
            return (value is DBNull) ? default(T) : (T)value;
        }
    }
}
