using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonProtocol
{

    public class SignUpResponse : Response
    {
        public enum StatusId
        {
            SignedUp,
            AlreadyExists
        }
    
        [JsonInclude]
        public StatusId Status { get; set; }
        [JsonInclude]
        public override string StatusMsg { get; set; }
        [JsonIgnore]
        public override byte[] Json { get; set; }

        public SignUpResponse(StatusId Status, string StatusMsg, ResponseId Id = ResponseId.SignIn) : base(Id)
        {
            StatusMsg = StatusMsg;
            Status = Status;
            Json = JsonSerializer.SerializeToUtf8Bytes(this);
        }
    }
}
