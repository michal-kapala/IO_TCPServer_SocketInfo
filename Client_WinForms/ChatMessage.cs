using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForm
{
    public class ChatMessage
    {
        public ChatMessage(string username, string msg, string time)
        {
            this.username = username;
            this.msg = msg;
            this.time = time;
        }

        public string username { get; set; }
        public string msg { get; set; }
        public string time { get; set; }
    }
}
