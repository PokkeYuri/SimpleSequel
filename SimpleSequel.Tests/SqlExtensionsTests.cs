using System.Xml.Linq;

namespace SimpleSequel.Tests
{
    [TestClass]
    public class SqlExtensionsTests
    {
        public SqlExtensionsTests()
        {
            SimpleSequelManager.Initialize(SQLiteManager.Instance.Connection);
        }

        [TestMethod]
        public void T_ExecuteReader()
        {
            using (var reader = "SELECT * FROM Students WHERE Id = 1".ExecuteReader())
            {
                reader.Read();
                Assert.AreEqual("Picard", (string)reader["Name"]);
            }
            SQLiteManager.Instance.Connection.Close();
        }

        [TestMethod]
        public async Task T_ExecuteReaderAsync()
        {
            using (var reader = await "SELECT * FROM Students WHERE Id = 1".ExecuteReaderAsync())
            {
                await reader.ReadAsync();
                Assert.AreEqual("Picard", (string)reader["Name"]);
            }
            await SQLiteManager.Instance.Connection.CloseAsync();
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
            Assert.AreEqual("Picard", result.ToString());
        }

        [TestMethod]
        public void T_ExecuteRow()
        {
            var result = "SELECT * FROM Students WHERE Id = 1".ExecuteRow();
            List<object> expected = [1, "Picard", "Archeology", 5, new DateTime(2013, 10, 07, 08, 23, 19, 120)];
            for(int i = 0; i < expected.Count; i++)
            {
                var type = expected[i].GetType();
                var expectedVal = Convert.ChangeType(expected[i], type);
                var resultVal = Convert.ChangeType(result[i], type);
                Assert.AreEqual(expectedVal, resultVal);
            }
        }

        [TestMethod]
        public async Task T_ExecuteRowAsync()
        {
            var result = await "SELECT * FROM Students WHERE Id = 1".ExecuteRowAsync();
            List<object> expected = [1, "Picard", "Archeology", 5, new DateTime(2013, 10, 07, 08, 23, 19, 120)];
            for (int i = 0; i < expected.Count; i++)
            {
                var type = expected[i].GetType();
                var expectedVal = Convert.ChangeType(expected[i], type);
                var resultVal = Convert.ChangeType(result[i], type);
                Assert.AreEqual(expectedVal, resultVal);
            }
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
        public void T_ExecuteStatement()
        {
            string name = "Doc";
            string subject = "Time Travel";
            $"INSERT INTO Students ( Name, Subject ) VALUES ( '{name}', '{subject}' )".ExecuteStatement();
            var command = SimpleSequelManager.Instance.NewCommand();
            command.CommandText = $"SELECT * FROM Students WHERE Name = '{name}'";
            SimpleSequelManager.Instance.Connection.Open();
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                Assert.AreEqual(subject, (string)reader["Subject"]);
            }
            SimpleSequelManager.Instance.Connection.Close();
        }


        [TestMethod]
        public async Task T_ExecuteStatementAsync()
        {
            string name = "Gandalf";
            string subject = "Magic";
            await $"INSERT INTO Students ( Name, Subject ) VALUES ( '{name}', '{subject}' )".ExecuteStatementAsync();
            var command = SimpleSequelManager.Instance.NewCommand();
            command.CommandText = $"SELECT * FROM Students WHERE Name = '{name}'";
            await SimpleSequelManager.Instance.Connection.OpenAsync();
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                Assert.AreEqual(subject, (string)reader["Subject"]);
            }
            await SimpleSequelManager.Instance.Connection.CloseAsync();
        }

        [TestMethod]
        public void T_Get_DateTime()
        {
            var command = SimpleSequelManager.Instance.NewCommand();
            command.CommandText = "SELECT * FROM Students WHERE Id = 1";
            SimpleSequelManager.Instance.Connection.Open();
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                Assert.AreEqual(new DateTime(2013, 10, 07, 08, 23, 19, 120), reader.Get<DateTime>("RegisterDate"));
            }         
            SimpleSequelManager.Instance.Connection.Close();
        }

        [TestMethod]
        public void T_Get_String()
        {
            var command = SimpleSequelManager.Instance.NewCommand();
            command.CommandText = "SELECT * FROM Students WHERE Id = 1";
            SimpleSequelManager.Instance.Connection.Open();
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                Assert.AreEqual("Picard", reader.Get<string>("Name"));
            }
            SimpleSequelManager.Instance.Connection.Close();
        }

        [TestMethod]
        public void T_ExecuteClass()
        {
            var picardResult = "SELECT * FROM Students WHERE Id = 1".ExecuteClass<SQLiteManager.Student>();
            var picard = new SQLiteManager.Student
            {
                Id = 1,
                Name = "Picard",
                Subject = "Archeology",
                Semester = 5,
                RegisterDate = new DateTime(2013, 10, 07, 08, 23, 19, 120)
            };

            Assert.AreEqual(picardResult, picard);


            string name = "Jones";
            string subject = "Artifacts";
            int id = 100;

            var command = SimpleSequelManager.Instance.NewCommand();
            SimpleSequelManager.Instance.Connection.Open();
            command.CommandText = $"INSERT INTO Students ( Id, Name, Subject ) VALUES ( {id}, '{name}', '{subject}' )";
            command.ExecuteNonQuery();
            SimpleSequelManager.Instance.Connection.Close();

            var jonesResult = $"SELECT * FROM Students WHERE Name = '{name}'".ExecuteClass<SQLiteManager.Student>();
            var jones = new SQLiteManager.Student
            {
                Id = id,
                Name = name,
                Subject = subject,
                Semester = null,
                RegisterDate = null
            };

            Assert.AreEqual(jonesResult, jones);
        }

    }
}