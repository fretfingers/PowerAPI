﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class DisbursementDetail
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string DisbursementId { get; set; }
        public decimal DisbursementDetailId { get; set; }
        public string PayedId { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string CurrencyId { get; set; }
        public double? CurrencyExchangeRate { get; set; }
        public decimal? DiscountTaken { get; set; }
        public decimal? WriteOffAmount { get; set; }
        public decimal? AppliedAmount { get; set; }
        public bool? Cleared { get; set; }
        public string GlexpenseAccount { get; set; }
        public string ProjectId { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
    }
}
