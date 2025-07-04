﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PayrollHrpayrollAbsenteeism
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string EmployeeId { get; set; }
        public DateTime AbsentDate { get; set; }
        public string Reason { get; set; }
        public double? Days { get; set; }
        public string Notes { get; set; }
        public string ApprovedBy { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public bool? Posted { get; set; }
        public string PostedBy { get; set; }
        public DateTime? PostedDate { get; set; }
        public bool? Cleared { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public bool? Approved { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string BranchCode { get; set; }
    }
}
