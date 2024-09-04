using PowerAPI.Data.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.ViewModels
{
    public class TaxGroupDetail
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string TaxGroupDetailId { get; set; }
        public string TaxId { get; set; }
        public string Description { get; set; }
        public string GltaxAccount { get; set; }
        public double? TaxPercent { get; set; }
        public int? TaxOrder { get; set; }
        public bool? TaxOnTax { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
        public ICollection<SetupWorkflow> WorkFlowTrail { get; set; }
    }
}
