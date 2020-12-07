using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace JsonProtocol
{
    public enum RequestId
    {
        SignUp = 1,
        SignIn = 2
    }

    public abstract class Request
    {
        [JsonInclude]
        public RequestId Id { get; set; }
        [JsonIgnore]
        public virtual byte[] Json { get; set; }

        public Request(RequestId id)
        {
            Id = id;
        }
    }
}
