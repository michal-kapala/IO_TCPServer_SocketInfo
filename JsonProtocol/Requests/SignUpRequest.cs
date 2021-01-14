using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonProtocol
{
    public class SignUpRequest : Request
    {
        [JsonInclude]
        public Credentials Credentials { get; set; }
        [JsonIgnore]
        public override byte[] Json { get; set; }

        public SignUpRequest(Credentials credentials, RequestId id = RequestId.SignUp) : base(id)
        {
            Credentials = credentials;
            Json = JsonSerializer.SerializeToUtf8Bytes(this);
        }
    }
}