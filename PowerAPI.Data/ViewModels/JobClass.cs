using PowerAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerAPI.Data.ViewModels
{
    public class JobClass
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string JobClassId { get; set; }
        public string JobClassDescription { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }

        public List<PayrollHrpayrollJobClassDetail> payrollJobClassDetails { get; set; }
    }
}
