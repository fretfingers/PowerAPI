﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PayrollHrpayrollStatus
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string StatusId { get; set; }
        public string StatusDescription { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
    }
}
