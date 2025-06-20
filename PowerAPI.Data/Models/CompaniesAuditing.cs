﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class CompaniesAuditing
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public bool? ApproveCustomer { get; set; }
        public bool? ApproveVendor { get; set; }
        public bool? ApprovePayment { get; set; }
        public bool? ApprovePurchase { get; set; }
        public bool? ApproveTransfer { get; set; }
        public bool? ApproveAdjustment { get; set; }
        public bool? ApproveReceipt { get; set; }
        public bool? ApprovePayroll { get; set; }
        public bool? ApproveApchedks { get; set; }
        public bool? ApproveEmployees { get; set; }
        public bool? ApproveItems { get; set; }
        public bool? ApproveLowMargins { get; set; }
        public bool? ApproveOrders { get; set; }
        public bool? ApproveContract { get; set; }
        public bool? ApproveReturns { get; set; }
        public bool? ApproveRma { get; set; }
        public bool? RequireSignatures { get; set; }
        public bool? RequireSignaturesPayables { get; set; }
        public bool? RequireSignaturesReceivables { get; set; }
        public bool? RequireSignaturesOrders { get; set; }
        public bool? RequireSignaturesPurchases { get; set; }
        public bool? RequireSignaturesContracts { get; set; }
        public bool? RequireSignaturesInv { get; set; }
        public bool? RequireSignaturesInvAdj { get; set; }
        public bool? RequireSignaturesLedger { get; set; }
        public bool? RequireSignaturesPayroll { get; set; }
        public bool? RequireSignaturesPeriodEnd { get; set; }
        public bool? RequireSignaturesYearEnd { get; set; }
        public bool? RequireSignaturesOther { get; set; }
        public bool? RequireBatchControls { get; set; }
        public bool? RequireBatchControlsPayables { get; set; }
        public bool? RequireBatchControlsReceivables { get; set; }
        public bool? RequireBatchControlsInv { get; set; }
        public bool? RequireBatchControlsInvAdj { get; set; }
        public bool? RequireBatchControlsLedger { get; set; }
        public bool? RequireBatchControlsPayroll { get; set; }
        public bool? RequireBatchControlsOther { get; set; }
        public bool? RequireSignaturesOnBatches { get; set; }
        public bool? Challange { get; set; }
        public bool? ChallangePayments { get; set; }
        public decimal? ChallangePaymentsLower { get; set; }
        public decimal? ChallangePaymentsAbove { get; set; }
        public double? ChallangePaymentsPercent { get; set; }
        public double? ChallangePaymentsAverage { get; set; }
        public double? ChallangePaymentsDeivations { get; set; }
        public string ChallangePaymentsThreshold { get; set; }
        public bool? ChallangeReceipts { get; set; }
        public decimal? ChallangeReceiptsLower { get; set; }
        public decimal? ChallangeReceiptsAbove { get; set; }
        public double? ChallangeReceiptsPercent { get; set; }
        public double? ChallangeReceiptsAverage { get; set; }
        public double? ChallangeReceiptsDeivations { get; set; }
        public string ChallangeReceiptsThreshold { get; set; }
        public bool? ChallangeInv { get; set; }
        public decimal? ChallangeInvLower { get; set; }
        public decimal? ChallangeInvAbove { get; set; }
        public double? ChallangeInvPercent { get; set; }
        public double? ChallangeInvAverage { get; set; }
        public double? ChallangeInvDeivations { get; set; }
        public string ChallangeInvThreshold { get; set; }
        public bool? ChallangeInvAdj { get; set; }
        public decimal? ChallangeInvAdjLower { get; set; }
        public decimal? ChallangeInvAdjAbove { get; set; }
        public double? ChallangeInvAdjPercent { get; set; }
        public double? ChallangeInvAdjAverage { get; set; }
        public double? ChallangeInvAdjDeivations { get; set; }
        public string ChallangeInvAdjThreshold { get; set; }
        public bool? ChallangeLedger { get; set; }
        public decimal? ChallangeLedgerLower { get; set; }
        public decimal? ChallangeLedgerAbove { get; set; }
        public double? ChallangeLedgerPercent { get; set; }
        public double? ChallangeLedgerAverage { get; set; }
        public double? ChallangeLedgerDeivations { get; set; }
        public string ChallangeLedgerThreshold { get; set; }
        public bool? ChallangePayroll { get; set; }
        public decimal? ChallangePayrollLower { get; set; }
        public decimal? ChallangePayrollAbove { get; set; }
        public double? ChallangePayrollPercent { get; set; }
        public double? ChallangePayrollAverage { get; set; }
        public double? ChallangePayrollDeivations { get; set; }
        public string ChallangePayrollThreshold { get; set; }
        public bool? ChallangeOther { get; set; }
        public decimal? ChallangeOtherLower { get; set; }
        public decimal? ChallangeOtherAbove { get; set; }
        public double? ChallangeOtherPercent { get; set; }
        public double? ChallangeOtherAverage { get; set; }
        public double? ChallangeOtherDeivations { get; set; }
        public string ChallangeOtherThreshold { get; set; }
        public bool? ChallangeExpense { get; set; }
        public decimal? ChallangeExpenseLower { get; set; }
        public decimal? ChallangeExpenseAbove { get; set; }
        public double? ChallangeExpensePercent { get; set; }
        public double? ChallangeExpenseAverage { get; set; }
        public double? ChallangeExpenseDeivations { get; set; }
        public string ChallangeExpenseThreshold { get; set; }
        public DateTime? LockTs { get; set; }
        public string LockedBy { get; set; }
        public string BranchCode { get; set; }
    }
}
