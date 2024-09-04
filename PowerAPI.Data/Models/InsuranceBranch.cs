using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceBranch
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string BranchManager { get; set; }
        public string BranchAddress { get; set; }
        public string ContactPerson { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
