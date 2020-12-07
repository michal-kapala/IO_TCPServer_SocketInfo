using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace JsonProtocol
{
    public enum ResponseId
    {
        SignUp = 1,
        SignIn = 2
    }

    public abstract class Response
    {
        [JsonInclude]
        public ResponseId Id { get; set; }
        [JsonInclude]
        public abstract string StatusMsg { get; set; }
        [JsonIgnore]
        public virtual byte[] Json { get; set; }

        public Response(ResponseId id)
        {
            Id = id;
        }
    }
}
