using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InventoryFamilies
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string ItemFamilyId { get; set; }
        public string FamilyName { get; set; }
        public string FamilyDescription { get; set; }
        public string FamilyLongDescription { get; set; }
        public string FamilyPictureUrl { get; set; }
        public string SalesControlAccount { get; set; }
        public string CoscontrolAccount { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public DateTime? LastEditDate { get; set; }
        public DateTime? CreationDate { get; set; }
        public string StockControlAccount { get; set; }
        public string BranchCode { get; set; }
        public bool? EnforceQualityAssuranceOnPo { get; set; }
    }
}
