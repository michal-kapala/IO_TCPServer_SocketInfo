using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IO_TCPServer_API
{
    public class JsonMessage
    {
        public string type { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string status { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string username { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string password { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ChatMessage chatMsg;

        public JsonMessage()
        {
        }

        public JsonMessage(string type, string status = null, string username = null, string password = null, ChatMessage chatMsg = null)
        {
            this.type = type;
            this.status = status;
            this.username = username;
            this.password = password;
            this.chatMsg = chatMsg;
        }
    }
}
