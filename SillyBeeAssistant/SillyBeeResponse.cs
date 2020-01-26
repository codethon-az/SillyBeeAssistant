using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SillyBeeAssistant
{
    public class SillyBeeResponse
    {
        public string commandType { get; set; }
        public string command { get; set; }
    }

    public class SillyBeeRequest
    {
        public string query { get; set; }
        public string response { get; set; }
    }
}
