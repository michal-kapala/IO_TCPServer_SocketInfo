using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonProtocol
{
    public class SignInResponse : Response
    {
        public enum StatusId
        {
            SignedIn,
            WrongCredentials,
            AlreadyLoggedIn,
            NotFound
        }

        [JsonInclude]
        public StatusId Status { get; set; }
        [JsonInclude]
        public override string StatusMsg { get; set; }
        [JsonIgnore]
        public override byte[] Json { get; set; }

        public SignInResponse(StatusId status, string statusMsg,  ResponseId id = ResponseId.SignIn) : base(id)
        {
            Status = status;
            StatusMsg = statusMsg;
            try
            {
                Json = JsonSerializer.SerializeToUtf8Bytes(this);
            }
            catch(Exception ex)
            {
                string tmp = ex.ToString();
            }
        }
    }
}
