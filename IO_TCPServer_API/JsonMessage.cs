using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IO_TCPServer_API
{
    public class JsonMessageStatus
    {
        public const string Ok = "ok";
        public const string Err = "err";
        public const string Null = "null";
    }

    public class JsonMessage
    {

        public string type { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string status { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string username { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string password { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ChatMessage chatMsg { get; set; }

        public JsonMessage()
        {
        }

        public JsonMessage(JsonMessage msg, string status)
        {
            this.type = msg.type;
            this.status = status;
            this.username = msg.username;
            this.password = msg.password;
            this.chatMsg = msg.chatMsg;
        }

        public JsonMessage(string type, string status = JsonMessageStatus.Null, string username = null, string password = null, ChatMessage chatMsg = null)
        {
            this.type = type;
            this.status = status;
            this.username = username;
            this.password = password;
            this.chatMsg = chatMsg;
        }
    }

}
