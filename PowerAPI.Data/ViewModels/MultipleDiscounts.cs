using PowerAPI.Data.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.ViewModels
{
    public class MultipleDiscounts
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string MultipleDiscountId { get; set; }
        public string MultipleDiscountDescription { get; set; }
        public double? MultipleDiscountPercent { get; set; }
        public string GlmultipleDiscountAccount { get; set; }
        public int? MultipleDiscountOrder { get; set; }
        public bool? Approved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string EnteredBy { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
        public ICollection<SetupWorkflow> WorkFlowTrail { get; set; }
    }
}
