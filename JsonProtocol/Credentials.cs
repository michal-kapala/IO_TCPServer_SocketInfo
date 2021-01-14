using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonProtocol
{
    public class Credentials
    {
        [JsonInclude]
        public string Login { get; private set; }
        [JsonInclude]
        public string Password { get; private set; }

        public Credentials(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }
}
