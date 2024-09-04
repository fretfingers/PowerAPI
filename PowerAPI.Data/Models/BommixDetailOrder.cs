using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class BommixDetailOrder
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string ProductionMixId { get; set; }
        public string OrderId { get; set; }
        public string BatchNumberId { get; set; }
        public string AssemblyId { get; set; }
        public string OrderDescription { get; set; }
        public double? EstimatedCost { get; set; }
        public string Notes { get; set; }
        public DateTime? SystemDate { get; set; }
        public bool? Cleared { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
        public string GlaccountNumber { get; set; }
    }
}
