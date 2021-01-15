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
        [JsonInclude]
        public override string StatusMsg { get; set; }
        [JsonInclude]
        public ChatMessage ChatMsg { get; set; }
        [JsonIgnore]
        public override byte[] Json { get; set; }

        public ChatMessageResponse(StatusId Status, ChatMessage ChatMsg, ResponseId Id = ResponseId.ChatMessage) : base(Id)
        {
            this.ChatMsg = ChatMsg;
            this.Status = Status;
            this.StatusMsg = StatusMsg;
            this.Json = JsonSerializer.SerializeToUtf8Bytes(this);
        }
    }
}
