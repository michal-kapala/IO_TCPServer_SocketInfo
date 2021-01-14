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
        public override byte[] Json { get; set; }

        public SignUpResponse(StatusId status, string msg, ResponseId id = ResponseId.SignIn) : base(id)
        {
            StatusMsg = msg;
            Status = status;
            Json = JsonSerializer.SerializeToUtf8Bytes(this);
        }
    }
}
