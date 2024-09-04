using PowerAPI.Data.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.ViewModels
{
    public class OrderTypes
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string OrderTypeId { get; set; }
        public string OrderTypeDescription { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
        public ICollection<SetupWorkflow> WorkFlowTrail { get; set; }
    }
}
