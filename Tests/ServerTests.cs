using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IO_TCPServer_API;
using System.Text.Json;
using System.Data.SQLite;
using JsonProtocol;

namespace Tests
{
    [TestClass]
    public class ServerTests
    {
        string login = "abc";
        string password = "123";

        [ClassInitialize]
        public static void Initialize(TestContext c)
        {
            DBManager.Connect();
        }

        [TestMethod]
        public void JsonDisconnectResponse()
        {
            JsonMessage expected = new JsonMessage("disconnect", JsonMessageStatus.Ok, login);
            JsonMessage request = new JsonMessage("disconnect", username: login);
            //TODO
            //JsonMessage response = JsonProtocol.disconnect(request);
            //Assert.AreEqual(JsonSerializer.Serialize(expected), JsonSerializer.Serialize(response), "Unexpected response");
        }

        [TestMethod]
        public void JsonRegisterResponse()
        {
            SQLiteCommand deleteUser = new SQLiteCommand("DELETE FROM users WHERE login='" + login + "'", DBManager.connection);
            if (DBManager.FindUser(login))
            {
                deleteUser.ExecuteNonQuery();
                deleteUser.Dispose();
            }
            JsonMessage expected = new JsonMessage("register", JsonMessageStatus.Ok, login, password);
            JsonMessage request = new JsonMessage("register", username: login, password:password);
            //JsonMessage response = JsonProtocol.register(request);
            //.AreEqual(JsonSerializer.Serialize(expected), JsonSerializer.Serialize(response), "Unexpected response");

            deleteUser.ExecuteNonQuery();
            deleteUser.Dispose();
        }

        [TestMethod]
        public void JsonRegisterResponseErr()
        {
            if (!DBManager.FindUser(login))
            {
                DBManager.AddUser(login, password);
            }
            JsonMessage expected = new JsonMessage("register", JsonMessageStatus.Err, login, password);
            JsonMessage request = new JsonMessage("register", username: login, password: password);
            //JsonMessage response = JsonProtocol.register(request);
            //Assert.AreEqual(JsonSerializer.Serialize(expected), JsonSerializer.Serialize(response), "Unexpected response");

            SQLiteCommand deleteUser = new SQLiteCommand("DELETE FROM users WHERE login='" + login + "'", DBManager.connection);
            deleteUser.ExecuteNonQuery();
            deleteUser.Dispose();
        }

        [TestMethod]
        public void JsonSignInOk() //right login and password
        {
            if (!DBManager.FindUser(login)) { 
                DBManager.AddUser(login, password);
            }
            JsonMessage expected = new JsonMessage("signin", JsonMessageStatus.Ok, login, password);
            JsonMessage request = new JsonMessage("signin", username: login, password: password);
            //JsonMessage response = JsonProtocol.signin(request);
            //Assert.AreEqual(JsonSerializer.Serialize(expected), JsonSerializer.Serialize(response), "Unexpected response");

            SQLiteCommand deleteUser = new SQLiteCommand("DELETE FROM users WHERE login='" + login + "'", DBManager.connection);
            deleteUser.ExecuteNonQuery();
            deleteUser.Dispose();
        }


        [TestMethod]
        public void JsonSignInErrPassword() //wrong password
        {
            if (!DBManager.FindUser(login))
            {
                DBManager.AddUser(login, "1234");
            }
            JsonMessage expected = new JsonMessage("signin", JsonMessageStatus.Err, login, password);
            JsonMessage request = new JsonMessage("signin", username: login, password: password);
            //JsonMessage response = JsonProtocol.signin(request);
            //Assert.AreEqual(JsonSerializer.Serialize(expected), JsonSerializer.Serialize(response), "Unexpected response");

            SQLiteCommand deleteUser = new SQLiteCommand("DELETE FROM users WHERE login='" + login + "'", DBManager.connection);
            deleteUser.ExecuteNonQuery();
            deleteUser.Dispose();
        }

        [TestMethod]
        public void JsonSignInErrLogin() //login does not exist
        {
            SQLiteCommand deleteUser = new SQLiteCommand("DELETE FROM users WHERE login='" + login + "'", DBManager.connection);
            if (DBManager.FindUser(login))
            {
                deleteUser.ExecuteNonQuery();
                deleteUser.Dispose();
            }
            JsonMessage expected = new JsonMessage("signin", JsonMessageStatus.Err, login, password);
            JsonMessage request = new JsonMessage("signin", username: login, password: password);
            //JsonMessage response = JsonProtocol.signin(request);
            //Assert.AreEqual(JsonSerializer.Serialize(expected), JsonSerializer.Serialize(response), "Unexpected response");
        }

        [TestMethod]
        public void JsonMessage()
        {
            if (!DBManager.FindUser(login))
            {
                DBManager.AddUser(login, password);
            }
            ChatMessage chat = new ChatMessage(login, "new message", "05/29/2015 5:50");
            JsonMessage expected = new JsonMessage("message", JsonMessageStatus.Ok, username: login, chatMsg:chat);
            JsonMessage request = new JsonMessage("message", username: login, chatMsg:chat);
            //JsonMessage response = JsonProtocol.message(request);
            //Assert.AreEqual(JsonSerializer.Serialize(expected), JsonSerializer.Serialize(response), "Unexpected response");
        }
    }
}
