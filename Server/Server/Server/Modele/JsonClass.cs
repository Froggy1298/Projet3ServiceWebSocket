using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Modele
{
    class JsonClass
    {
        public class Settings
        {
            public string IP { get; set; }
            public int Port { get; set; }
        }
        public class SqlConnection
        {
            public string Server { get; set; }
            public string Port { get; set; }
            public string Database { get; set; }
            public string User { get; set; }
            public string Password { get; set; }
        }
    }
}
