﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PaymentsDetail
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string PaymentId { get; set; }
        public decimal PaymentDetailId { get; set; }
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
        public string TrackingNumber { get; set; }
        public string TaxGroupId { get; set; }
        public decimal? TaxAmount { get; set; }
        public double? TaxRate { get; set; }
        public string Notes { get; set; }
        public string GlanalysisType1 { get; set; }
        public string GlanalysisType2 { get; set; }
        public string AssetId { get; set; }
        public string VendorId { get; set; }
        public string BranchCode { get; set; }
        public string ReceiptNo { get; set; }
        public string CustomerId { get; set; }
        public double? CommsionDue { get; set; }
        public double? GrossPremiumDue { get; set; }
        public double? Vatdue { get; set; }
        public bool? RemittedClosed { get; set; }
        public DateTime? Closedate { get; set; }
    }
}
