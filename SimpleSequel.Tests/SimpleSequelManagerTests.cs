namespace SimpleSequel.Tests
{
    [TestClass]
    public class SimpleSequelManagerTests
    {
        [TestMethod]
        public void A_InitializeManager()
        {
            SimpleSequelManager.Initialize(SQLiteManager.Instance.Connection);
        }

        [TestMethod]
        public void T_ExecuteSequel()
        {
            string statement = "SELECT Name FROM Students WHERE Id = 1";
            var result = SimpleSequelManager.Instance.ExecuteSequel(statement, command => command.ExecuteScalar());
            Assert.AreEqual("Picard", result?.ToString());
        }

        [TestMethod]
        public void T_ExecuteSequel_WithType()
        {
            string statement = "SELECT Name FROM Students WHERE Id = 1";
            var result = SimpleSequelManager.Instance.ExecuteSequel<string>(statement, command => command.ExecuteScalar());
            Assert.AreEqual("Picard", result);
        }

        [TestMethod]
        public async Task T_ExecuteSequelAsnyc()
        {
            string statement = "SELECT Name FROM Students WHERE Id = 1";
            var result = await SimpleSequelManager.Instance.ExecuteSequelAsnyc(statement, async (command) => await command.ExecuteScalarAsync());
            Assert.AreEqual("Picard", result.ToString());
        }

        [TestMethod]
        public async Task T_ExecuteSequelAsnyc_WithType()
        {
            string statement = "SELECT Name FROM Students WHERE Id = 1";
            var result = await SimpleSequelManager.Instance.ExecuteSequelAsnyc<string?>(statement, async (command) => await command.ExecuteScalarAsync());
            Assert.AreEqual("Picard", result);
        }
    }
}
