using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.ViewModels
{
    public class MessageResponse
    {
        public int batchID { get; set; }
        public string messageID { get; set; }
        public string senderID { get; set; }
        public string messageText { get; set; }
        public string mobileNumber { get; set; }
        public DateTime submitDate { get; set; }
        public double Charged { get; set; }
        public Report reports { get; set; }
    }
    public partial class Report
    {
        public string status { get; set; }
        public string smscID { get; set; }
        public string reportDate { get; set; }
    }


    
}
