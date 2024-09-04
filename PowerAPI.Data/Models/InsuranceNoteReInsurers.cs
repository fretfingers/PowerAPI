using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceNoteReInsurers
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string ReInsurerId { get; set; }
        public string PolicyBrokerId { get; set; }
        public string InvoiceNumber { get; set; }
        public double Apportionment { get; set; }
        public decimal? GrossPremium { get; set; }
        public double? Brokerage { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public double? SubTotal { get; set; }
    }
}
