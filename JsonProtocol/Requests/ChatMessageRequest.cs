using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using IO_TCPServer_API;

namespace JsonProtocol
{
    public class ChatMessageRequest : Request
    {
        [JsonInclude]
        public ChatMessage ChatMsg { get; set; }
        [JsonIgnore]
        public override byte[] Json { get; set; }

        /*public ChatMessageRequest(string username, string msg, RequestId id = RequestId.ChatMessage) : base(id)
        {
            ChatMsg = new ChatMessage(username, msg, DateTime.Now.ToString());
            Json = JsonSerializer.SerializeToUtf8Bytes(this);
        }*/

        public ChatMessageRequest(ChatMessage chatMsg, RequestId id = RequestId.ChatMessage) : base(id)
        {
            ChatMsg = chatMsg;
            Json = JsonSerializer.SerializeToUtf8Bytes(this);
        }
    }
}
