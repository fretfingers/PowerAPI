using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceNoteBrokers
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string BrokerId { get; set; }
        public string PolicyBrokerId { get; set; }
        public string InvoiceNumber { get; set; }
        public double Apportionment { get; set; }
        public double? GrossBrokerage { get; set; }
        public double? AdminCharge { get; set; }
        public double? NetBrokerage { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
