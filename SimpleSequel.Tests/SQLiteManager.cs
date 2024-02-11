using System.Data.Common;
using System.Data.SQLite;

namespace SimpleSequel.Tests
{
    internal class SQLiteManager
    {
        private static string _filepath = "testingDB.sqlite";
        private static string _connectionString = $"Data Source={_filepath};Version=3;";
        private static string _initStatement = "DROP TABLE IF EXISTS Students;" +
                                               "CREATE TABLE IF NOT EXISTS Students (" +
                                                "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                                                "Name TEXT NOT NULL," +
                                                "Subject TEXT NOT NULL," +
                                                "Semester INTEGER," +
                                                "RegisterDate DateTime" +
                                               ");" +
                                               "INSERT INTO Students VALUES ( NULL, 'Picard', 'Archeology', 5 , '2013-10-07 08:23:19.120' );";
            
        public static SQLiteManager Instance { get; } = new SQLiteManager();

        public DbConnection Connection { get; }
        private SQLiteManager()
        { 
            if(!File.Exists(_filepath))
                SQLiteConnection.CreateFile(_filepath);

            Connection = new SQLiteConnection(_connectionString);
            Connection.Open();

            using var command = Connection.CreateCommand();
            command.CommandText = _initStatement;
            command.ExecuteNonQuery();
        }
    }
}
