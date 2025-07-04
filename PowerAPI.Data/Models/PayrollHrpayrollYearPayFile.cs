﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PayrollHrpayrollYearPayFile
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string EmployeeId { get; set; }
        public string PayTypeId { get; set; }
        public string AttrId { get; set; }
        public DateTime TranDate { get; set; }
        public double? Amount { get; set; }
        public double? PayTypeBalance { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string PayTypeDescription { get; set; }
        public bool? Taxable { get; set; }
        public bool? ActiveYn { get; set; }
        public double? ExactAmount { get; set; }
        public bool? OnPayroll { get; set; }
        public double? Rate { get; set; }
        public double? Units { get; set; }
        public string Glaccount { get; set; }
        public string GlaccountEmployer { get; set; }
        public string GlaccountEmployerExp { get; set; }
        public string BranchCode { get; set; }
        public string CurrencyId { get; set; }
        public double? CurrencyExchangeRate { get; set; }
    }
}
