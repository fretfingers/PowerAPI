using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.Models
{
    public partial class OTPGeneratorCode
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string CustomerId { get; set; }
        public string OTPCode { get; set; }
        public bool OTPUsed { get; set; }
        public bool TransactionCompleted { get; set; }
        public DateTime? TimeSent { get; set;}
        public DateTime? TimeExpired { get; set;}    
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set;}

    }
}
