using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InventoryFamiliesQualityAssurance
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string ItemFamilyId { get; set; }
        public string ItemFamilyQatypeId { get; set; }
        public string ItemFamilyQatypeDescription { get; set; }
        public string ItemFamilyQaunit { get; set; }
        public double? ItemFamilyQaspecFrom { get; set; }
        public double? ItemFamilyQaspecTo { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
    }
}
