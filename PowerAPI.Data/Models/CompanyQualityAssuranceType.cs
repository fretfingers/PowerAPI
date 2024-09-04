using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class CompanyQualityAssuranceType
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string QatypeId { get; set; }
        public string QatypeDescription { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
    }
}
