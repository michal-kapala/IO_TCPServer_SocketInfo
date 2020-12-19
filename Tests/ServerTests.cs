using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IO_TCPServer_API;
using System.Text.Json;
using System.Data.SQLite;

namespace Tests
{
    [TestClass]
    public class ServerTests
    {
        [ClassInitialize]
        public static void Initialize(TestContext c)
        {
            DBManager.Connect();

        }

        [TestMethod]
        public void JsonDisconnectResponse()
        {

            JsonMessage expected = new JsonMessage("disconnect", JsonMessageStatus.Ok, "abc");
            JsonMessage request = new JsonMessage("disconnect", username: "abc");
            JsonMessage response = JsonProtocol.disconnect(request);
            Assert.AreEqual(JsonSerializer.Serialize(expected), JsonSerializer.Serialize(response), "Unexpected response");
        }

        [TestMethod]
        public void JsonRegisterResponse()
        {
            SQLiteCommand deleteUser = new SQLiteCommand("DELETE FROM users WHERE login='ab'", DBManager.connection);
            deleteUser.ExecuteNonQuery();
            deleteUser.Dispose();

            JsonMessage expected = new JsonMessage("register", JsonMessageStatus.Ok, "ab", "123");
            JsonMessage request = new JsonMessage("register", username: "ab", password:"123");
            JsonMessage response = JsonProtocol.register(request);
            Assert.AreEqual(JsonSerializer.Serialize(expected), JsonSerializer.Serialize(response), "Unexpected response");
        }

        [TestMethod]
        public void JsonRegisterResponseErr()
        {
            if (!DBManager.FindUser("abcd"))
            {
                DBManager.AddUser("abcd", "123");
            }
            JsonMessage expected = new JsonMessage("register", JsonMessageStatus.Err, "abcd", "123");
            JsonMessage request = new JsonMessage("register", username: "abcd", password: "123");
            JsonMessage response = JsonProtocol.register(request);
            Assert.AreEqual(JsonSerializer.Serialize(expected), JsonSerializer.Serialize(response), "Unexpected response");
        }



    }
}
