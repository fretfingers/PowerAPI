﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PayrollEmployeeDepartment
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
    }
}
