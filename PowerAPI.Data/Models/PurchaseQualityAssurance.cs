using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PurchaseQualityAssurance
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string PurchaseNumber { get; set; }
        public string ItemId { get; set; }
        public string PurchaseQatypeId { get; set; }
        public string PurchaseQadescription { get; set; }
        public string PurchaseQaunit { get; set; }
        public double? PurchaseQaspecFrom { get; set; }
        public double? PurchaseQaspecTo { get; set; }
        public string PurchaseQamethod { get; set; }
        public double? PurchaseQaresult { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
    }
}
