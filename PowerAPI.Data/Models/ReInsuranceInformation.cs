using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class ReInsuranceInformation
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string ReInsuranceId { get; set; }
        public string ReInsuranceName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string WebAddress { get; set; }
        public string GlsalesAccount { get; set; }
        public bool? Active { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public double? LegalCession { get; set; }
        public string BranchCode { get; set; }
    }
}
