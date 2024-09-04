using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PayrollHrpayrollAppraisalPeriod
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string PeriodId { get; set; }
        public string PeriodDescription { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? DeactivatedDate { get; set; }
        public bool? Deactivated { get; set; }
        public string DeactivatedBy { get; set; }
        public string AttestationStatement { get; set; }
        public bool? Active { get; set; }
    }
}
