using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class OrderTrackingHeader
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderDescription { get; set; }
        public string OrderLongDescription { get; set; }
        public string SpecialInstructions { get; set; }
        public string SpecialNeeds { get; set; }
        public string EnteredBy { get; set; }
        public bool? Approved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
    }
}
