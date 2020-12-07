using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonProtocol
{
    public class SignInRequest : Request
    {
        [JsonInclude]
        public Credentials Credentials { get; set; }
        [JsonIgnore]
        public override byte[] Json { get; set; }

        public SignInRequest(Credentials credentials, RequestId id = RequestId.SignIn) : base(id)
        {
            Credentials = credentials;
            Json = JsonSerializer.SerializeToUtf8Bytes(this);
        }
    }
}
