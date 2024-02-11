using System.Globalization;
using System.Data;
using System.Data.Common;

namespace SimpleSequel
{
    public static class SqlExtensions
    {
        public static DbDataReader ExecuteReader(this string statement)
        {
            DbDataReader? dbReader= (DbDataReader?)SimpleSequelManager.Instance.ExecuteSequel(statement, command => command.ExecuteReader());
            return dbReader ?? throw new SimpleSequelException("Null Reference on creating DBDataReader.");
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
