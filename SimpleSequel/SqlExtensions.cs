using System.Globalization;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace SimpleSequel
{
    public static class SqlExtensions
    {
        public static DbDataReader ExecuteReader(this string statement)
        {
            DbDataReader? dbReader= (DbDataReader?)SimpleSequelManager.Instance.ExecuteSequel(statement, command => command.ExecuteReader(), false);
            return dbReader ?? throw new SimpleSequelException("Null Reference on creating DBDataReader.");
        }

        public static async Task<DbDataReader> ExecuteReaderAsync(this string statement)
        {
            DbDataReader? dbReader = (DbDataReader?) await SimpleSequelManager.Instance.ExecuteSequelAsnyc(statement, async command => await command.ExecuteReaderAsync(), false);
            return dbReader ?? throw new SimpleSequelException("Null Reference on creating DBDataReader.");
        }

        public static T ExecuteClass<T>(this string statement)
        {
            T result = Activator.CreateInstance<T>();
            using var reader = statement.ExecuteReader();

            bool isConnectionOnInputOpen = SimpleSequelManager.Instance.Connection.State == ConnectionState.Open;
            if (!isConnectionOnInputOpen)
                SimpleSequelManager.Instance.Connection.Open();

            int propertieCounter = 0;
            if (reader.Read())
            {
                PropertyInfo[] properties = typeof(T).GetProperties();
                foreach (PropertyInfo propertyInfo in properties)
                {
                    try
                    {
                        var value = reader.GetValue(propertyInfo.Name.ToLower());
                        var type = propertyInfo.PropertyType switch
                        {
                            _ when propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) => propertyInfo.PropertyType.GenericTypeArguments[0],
                            _ => propertyInfo.PropertyType
                        };
                        var convertedValue = Convert.ChangeType(value, type);
                        propertyInfo.SetValue(result, convertedValue);
                        //Only increment when propertie successfully set value
                        propertieCounter++;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            if (!isConnectionOnInputOpen)
                SimpleSequelManager.Instance.Connection.Close();

            if (propertieCounter == 0)
                throw new SimpleSequelException($"Could not create Class '{typeof(T).Name}' from SQL statement!", statement);

            return result;
        }

        public static object? ExecuteScalar(this string statement) => 
            SimpleSequelManager.Instance.ExecuteSequel(statement, command => command.ExecuteScalar());

        public static T? ExecuteScalar<T>(this string statement) => 
            SimpleSequelManager.Instance.ExecuteSequel<T>(statement, command => command.ExecuteScalar());

        public static async Task<object?> ExecuteScalarAsync(this string statement) => 
            await SimpleSequelManager.Instance.ExecuteSequelAsnyc(statement, async command => await command.ExecuteScalarAsync());

        public static async Task<T?> ExecuteScalarAsync<T>(this string statement) =>
            await SimpleSequelManager.Instance.ExecuteSequelAsnyc<T>(statement, async command => await command.ExecuteScalarAsync());

        public static void ExecuteStatement(this string statement) => 
            SimpleSequelManager.Instance.ExecuteSequel(statement, command => command.ExecuteNonQuery());

        public static async Task ExecuteStatementAsync(this string statement) => 
            await SimpleSequelManager.Instance.ExecuteSequelAsnyc(statement, async command => await command.ExecuteNonQueryAsync());

        public static async Task<List<dynamic>> ExecuteRowAsync(this string statement)
        {
            var result = new List<dynamic>();
            using DbDataReader reader = await statement.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                for(int i = 0; i < reader.FieldCount; i++)
                {
                    result.Add(reader.GetValue(i));
                }
            }
            return result;
        }

        public static List<object> ExecuteRow(this string statement)
        {
            var result = new List<object>();
            using DbDataReader reader = statement.ExecuteReader();
            if (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    result.Add(reader.GetValue(i));
                }
            }
            return result;
        }

        public static T? Get<T>(this DbDataReader reader, string columnName, IFormatProvider? formatProvider = null)
        {
            object result = reader.GetValue(columnName);
            return ConvertToType<T>(result, formatProvider);
        }

        public static string ToQueryString(this string? value)
        {
            if (value == null)
                return "''";

            value = value.Replace("\"", "\"\"");
            value = value.Replace("'", "''");
            value = value.Insert(0, "'").Insert(value.Length + 1, "'");
            return value;
        }

        internal static T? ConvertToType<T>(object? value, IFormatProvider? formatProvider = null) =>
            value == null || value == DBNull.Value ? default : (T)Convert.ChangeType(value, typeof(T), formatProvider ?? CultureInfo.InvariantCulture);
    }
}
