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
    public class ChatMessageResponse : Response
    {
        public enum StatusId
        {
            Ok,
            InternalError
        }

        [JsonInclude]
        public StatusId Status { get; set; }
        [JsonIgnore]
        public override string StatusMsg { get; set; }
        [JsonInclude]
        public ChatMessage ChatMsg { get; set; }

        public override byte[] Json { get; set; }

        public ChatMessageResponse(StatusId status, ChatMessage msg, ResponseId id = ResponseId.ChatMessage) : base(id)
        {
            ChatMsg = msg;
            Status = status;
            Json = JsonSerializer.SerializeToUtf8Bytes(this);
        }
    }
}
