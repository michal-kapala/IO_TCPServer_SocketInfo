using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IO_TCPServer_API;
using System.Text.Json;
namespace Tests
{
    [TestClass]
    public class ServerTests
    {
        [TestMethod]
        public void JsonDisconnectResponse()
        {
            JsonMessage expected = new JsonMessage("disconnect", JsonMessageStatus.Ok, "abc");
            JsonMessage request = new JsonMessage("disconnect", username:"abc");
            JsonMessage response = JsonProtocol.disconnect(request);
            Assert.AreEqual(JsonSerializer.Serialize(expected), JsonSerializer.Serialize(response), "Unexpected response");
        }


    }
}
