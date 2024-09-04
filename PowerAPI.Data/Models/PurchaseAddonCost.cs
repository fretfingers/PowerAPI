using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PurchaseAddonCost
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string GlaccountNumber { get; set; }
        public string Notes { get; set; }
        public string ApportionmentBase { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
        public bool? IsActual { get; set; }
    }
}
