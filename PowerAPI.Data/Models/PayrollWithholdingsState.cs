﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PayrollWithholdingsState
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string State { get; set; }
        public string WithholdingStatus { get; set; }
        public float TaxBracket { get; set; }
        public string Period { get; set; }
        public decimal? OverAmnt { get; set; }
        public decimal? NotOver { get; set; }
        public decimal? Cumulative { get; set; }
        public decimal? Allowance { get; set; }
        public int? PayrollYear { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
    }
}
