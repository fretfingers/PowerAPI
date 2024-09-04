using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceBusinessClassCover
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string BusinessClassId { get; set; }
        public int SummaryAutoId { get; set; }
        public string SummaryofCover { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
    }
}
