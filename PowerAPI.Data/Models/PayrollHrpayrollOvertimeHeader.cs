﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PayrollHrpayrollOvertimeHeader
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string EmployeeId { get; set; }
        public string PayTypeId { get; set; }
        public string CurrencyId { get; set; }
        public double? CurrencyExchangeRate { get; set; }
        public decimal? Total { get; set; }
        public string EnteredBy { get; set; }
        public bool? Cleared { get; set; }
        public bool? Approved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public double? TaxAmount { get; set; }
        public string BranchCode { get; set; }
    }
}
