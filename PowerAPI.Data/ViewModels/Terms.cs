using PowerAPI.Data.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.ViewModels
{
    public class Terms
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string TermsId { get; set; }
        public string TermsDescription { get; set; }
        public short? NetDays { get; set; }
        public double? DiscountPercent { get; set; }
        public short? DiscountDays { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
        public ICollection<SetupWorkflow> WorkFlowTrail { get; set; }
    }
}
