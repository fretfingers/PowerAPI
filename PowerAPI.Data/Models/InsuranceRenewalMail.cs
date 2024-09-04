using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceRenewalMail
    {
        public decimal MailNumber { get; set; }
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public DateTime MailDate { get; set; }
        public DateTime? LockTs { get; set; }
        public string LockedBy { get; set; }
    }
}
