﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PayrollHrpayrollActivationHistory
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string EmployeeId { get; set; }
        public string Status { get; set; }
        public DateTime Period { get; set; }
        public DateTime? ActiveDate { get; set; }
        public DateTime? LastPayDate { get; set; }
        public bool? Porate { get; set; }
        public bool? Arrears { get; set; }
        public string Reason { get; set; }
        public string Notes { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public bool? Cleared { get; set; }
        public string ApprovedBy { get; set; }
        public bool? Approved { get; set; }
        public string PostedBy { get; set; }
        public DateTime? PostedDate { get; set; }
        public bool? Posted { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string PaymentMode { get; set; }
        public string BranchCode { get; set; }
    }
}
