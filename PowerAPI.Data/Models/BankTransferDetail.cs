﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class BankTransferDetail
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string BankTransactionId { get; set; }
        public decimal BankTransactionDetailId { get; set; }
        public string GlbankAccount2 { get; set; }
        public string BankIdto { get; set; }
        public string Notes { get; set; }
        public double? TransactionAmount { get; set; }
        public string ToCurrencyId { get; set; }
        public double? ToCurrencyExchangeRate { get; set; }
        public decimal? ExchangeAmount { get; set; }
        public string GlanalysisType1 { get; set; }
        public string GlanalysisType2 { get; set; }
        public string AssetId { get; set; }
        public string ProjectId { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
    }
}
