using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsurancePolicyDetailExtension
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string PolicyBrokerId { get; set; }
        public string ExtensionId { get; set; }
        public int BrokingSlipAutoCount { get; set; }
        public string ExtensionDesc { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
