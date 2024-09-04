using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InventoryItemsQualityAssurance
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string ItemId { get; set; }
        public string ItemQatypeId { get; set; }
        public string ItemQatypeDescription { get; set; }
        public string ItemQaunit { get; set; }
        public double? ItemQaspecFrom { get; set; }
        public double? ItemQaspecTo { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
    }
}
