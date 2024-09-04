using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.ViewModels
{
    public class MessageRequest
    {
        public string senderID { get; set; }
        public string messageText { get; set; }
        public string mobileNumber { get; set; }
        public string route { get; set; }

    }
}
