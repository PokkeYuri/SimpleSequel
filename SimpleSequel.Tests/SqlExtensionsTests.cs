namespace SimpleSequel.Tests
{
    [TestClass]
    public class SqlExtensionsTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            SimpleSequelManager.Initialize(SQLiteManager.Instance.Connection);
        }

        [TestMethod]
        public void T_ExecuteReader()
        {
            var reader = "SELECT * FROM Students WHERE Id = 1".ExecuteReader();
            reader.Read();
            Assert.AreEqual("Picard", (string)reader["Name"]);
        }

        [TestMethod]
        public void T_ExecuteScalar()
        {
            var result = "SELECT Name FROM Students WHERE Id = 1".ExecuteScalar();
            Assert.AreEqual("Picard", result?.ToString());
        }

        [TestMethod]
        public async Task T_ExecuteScalarAsync()
        {
            var result = await "SELECT Name FROM Students WHERE Id = 1".ExecuteScalarAsync();
            Assert.AreEqual("Picard", result?.ToString());
        }

        [TestMethod]
        public void T_ExecuteStatement()
        {
            string name = "Doc";
            string subject = "Time Travel";
            $"INSERT INTO Students ( Name, Subject ) VALUES ( '{name}', '{subject}' )".ExecuteStatement();
            var command = SimpleSequelManager.Instance.NewCommand();
            command.CommandText = $"SELECT * FROM Students WHERE Name = '{name}'";
            var reader = command.ExecuteReader();
            reader.Read();
            Assert.AreEqual(subject, (string)reader["Subject"]);
        }

        [TestMethod]
        public async Task T_ExecuteStatementAsync()
        {
            string name = "Gandalf";
            string subject = "Magic";
            await $"INSERT INTO Students ( Name, Subject ) VALUES ( '{name}', '{subject}' )".ExecuteStatementAsync();
            var command = SimpleSequelManager.Instance.NewCommand();
            command.CommandText = $"SELECT * FROM Students WHERE Name = '{name}'";
            var reader = command.ExecuteReader();
            reader.Read();
            Assert.AreEqual(subject, (string)reader["Subject"]);
        }

        [TestMethod]
        public void T_ExecuteToQuerySring()
        {
            string? nullStr = null;
            string queryNullStr = nullStr.ToQueryString();
            Assert.AreEqual("''", queryNullStr);

            string euroDecimal = "4'2";
            string queryDecimalStr = euroDecimal.ToQueryString();
            Assert.AreEqual("'4''2'", queryDecimalStr);

            string doubleQuoteStr = "te\"st";
            string queryDoubleQuoteStr = doubleQuoteStr.ToQueryString();
            Assert.AreEqual("'te\"\"st'", queryDoubleQuoteStr);

            string mixedStr = "te's.t\"";
            string queryMixedStr = mixedStr.ToQueryString();
            Assert.AreEqual("'te''s.t\"\"'", queryMixedStr);
        }

        [TestMethod]
        public void T_Get_DateTime()
        {
            var command = SimpleSequelManager.Instance.NewCommand();
            command.CommandText = "SELECT * FROM Students WHERE Id = 1";
            var reader = command.ExecuteReader();
            reader.Read();
            Assert.AreEqual(new DateTime(2013, 10, 07, 08, 23, 19, 120), reader.Get<DateTime>("RegisterDate"));
        }

        [TestMethod]
        public void T_Get_String()
        {
            var command = SimpleSequelManager.Instance.NewCommand();
            command.CommandText = "SELECT * FROM Students WHERE Id = 1";
            var reader = command.ExecuteReader();
            reader.Read();
            Assert.AreEqual("Picard", reader.Get<string>("Name"));
        }
    }
}