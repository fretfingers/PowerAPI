using PowerAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerAPI.Data.ViewModels
{
    public class EmployeeDept
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string EmployeeDepartmentId { get; set; }
        public string EmployeeDepartmentDescription { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string GlaccountNumber { get; set; }
        public string BranchCode { get; set; }

        public List<PayrollEmployeeDepartmentDetails> employeeDepartmentDetails { get; set; }
    }
}
