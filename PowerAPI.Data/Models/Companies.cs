﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class Companies
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress1 { get; set; }
        public string CompanyAddress2 { get; set; }
        public string CompanyAddress3 { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyState { get; set; }
        public string CompanyZip { get; set; }
        public string CompanyCountry { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyFax { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyWebAddress { get; set; }
        public string CompanyLogoUrl { get; set; }
        public string CompanyLogoFilename { get; set; }
        public string CompanyShoppingCartUrl { get; set; }
        public string CompanyReportsDirectoryUrl { get; set; }
        public string CompanySupportDirectoryUrl { get; set; }
        public string CompanyWebCrmdirectoryUrl { get; set; }
        public string CompanyNotes { get; set; }
        public string CurrencyId { get; set; }
        public string BankAccount { get; set; }
        public string Terms { get; set; }
        public string FedTaxId { get; set; }
        public string StateTaxId { get; set; }
        public string VatregistrationNumber { get; set; }
        public string VatsalesTaxId { get; set; }
        public string VatpurchaseTaxId { get; set; }
        public string VatotherRegistrationNumber { get; set; }
        public string DefaultGlpostingDate { get; set; }
        public string DefaultSalesGltracking { get; set; }
        public string DefaultGlpurchaseGltracking { get; set; }
        public string DefaultSalesTaxGroup { get; set; }
        public string DefaultPurchaseTaxGroup { get; set; }
        public string DefaultInventoryCostingMethod { get; set; }
        public string AgeInvoicesBy { get; set; }
        public string AgePurchaseOrdersBy { get; set; }
        public bool? FinanceCharge { get; set; }
        public double? FinanceChargePercent { get; set; }
        public string WarehouseId { get; set; }
        public string WarehouseBinId { get; set; }
        public string ShippingMethod { get; set; }
        public bool? ChargeHandling { get; set; }
        public bool? HandlingAsPercent { get; set; }
        public double? HandlingRate { get; set; }
        public bool? ChargeMinimumSurcharge { get; set; }
        public decimal? MinimumSurchargeThreshold { get; set; }
        public string Glapaccount { get; set; }
        public string GlapcashAccount { get; set; }
        public string GlapinventoryAccount { get; set; }
        public string GlapfreightAccount { get; set; }
        public string GlaphandlingAccount { get; set; }
        public string GlapmiscAccount { get; set; }
        public string GlapdiscountAccount { get; set; }
        public string GlapprePaidAccount { get; set; }
        public string GlapwriteOffAccount { get; set; }
        public string Glaraccount { get; set; }
        public string GlarcashAccount { get; set; }
        public string GlarsalesAccount { get; set; }
        public string Glarcogsaccount { get; set; }
        public string GlarinventoryAccount { get; set; }
        public string GlarfreightAccount { get; set; }
        public string GlarhandlingAccount { get; set; }
        public string GlardiscountAccount { get; set; }
        public string GlarmiscAccount { get; set; }
        public string GlarreturnAccount { get; set; }
        public string GlarwriteOffAccount { get; set; }
        public string GlfixedAccumDepreciationAccount { get; set; }
        public string GlfixedDepreciationAccount { get; set; }
        public string GlfixedAssetAccount { get; set; }
        public string GlfixedDisposalAccount { get; set; }
        public string GlbankInterestEarnedAccount { get; set; }
        public string GlbankServiceChargesAccount { get; set; }
        public string GlbankMiscWithdrawlAccount { get; set; }
        public string GlbankBankMisDepositAccount { get; set; }
        public string GlbankOtherChargesAccount { get; set; }
        public string GlretainedEarningsAccount { get; set; }
        public string GlprofitLossAccount { get; set; }
        public string GlunrealizedCurrencyProfitLossAccount { get; set; }
        public string GlrealizedCurrencyProfitLossAccount { get; set; }
        public string GlarfreightProfitLossAccount { get; set; }
        public string GlcurrencyGainLossAccount { get; set; }
        public string GlunrealizedCurrencyGainLossAccount { get; set; }
        public DateTime? FiscalStartDate { get; set; }
        public DateTime? FiscalEndDate { get; set; }
        public short? CurrentFiscalYear { get; set; }
        public DateTime? CurrentPeriod { get; set; }
        public DateTime? Period1Date { get; set; }
        public DateTime? Period2Date { get; set; }
        public DateTime? Period3Date { get; set; }
        public DateTime? Period4Date { get; set; }
        public DateTime? Period5Date { get; set; }
        public DateTime? Period6Date { get; set; }
        public DateTime? Period7Date { get; set; }
        public DateTime? Period8Date { get; set; }
        public DateTime? Period9Date { get; set; }
        public DateTime? Period10Date { get; set; }
        public DateTime? Period11Date { get; set; }
        public DateTime? Period12Date { get; set; }
        public DateTime? Period13Date { get; set; }
        public DateTime? Period14Date { get; set; }
        public bool? Period1Closed { get; set; }
        public bool? Period2Closed { get; set; }
        public bool? Period3Closed { get; set; }
        public bool? Period4Closed { get; set; }
        public bool? Period5Closed { get; set; }
        public bool? Period6Closed { get; set; }
        public bool? Period7Closed { get; set; }
        public bool? Period8Closed { get; set; }
        public bool? Period9Closed { get; set; }
        public bool? Period10Closed { get; set; }
        public bool? Period11Closed { get; set; }
        public bool? Period12Closed { get; set; }
        public bool? Period13Closed { get; set; }
        public bool? Period14Closed { get; set; }
        public bool? Gaapcompliant { get; set; }
        public bool? EditGltranssactions { get; set; }
        public bool? EditBankTransactions { get; set; }
        public bool? EditAptransactions { get; set; }
        public bool? EditArtransactions { get; set; }
        public bool? HardClose { get; set; }
        public bool? AuditTrail { get; set; }
        public bool? PeriodPosting { get; set; }
        public bool? SystemDates { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string AlternateCurrencyId { get; set; }
        public DateTime? LastEditDate { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? AllowTransactionDate { get; set; }
        public bool? PrinterCheck { get; set; }
        public bool? ReAuthorizePartialTransactions { get; set; }
        public string DividendAccount { get; set; }
        public string DividendPayableAccount { get; set; }
        public string RevaluationReserveAccount { get; set; }
        public string SurplusAssetRevaluationAccount { get; set; }
        public int? PasswordAttempt { get; set; }
        public int? PasswordDuration { get; set; }
        public bool? EnforceProjectId { get; set; }
        public bool? GeneratePaymentFromPurhase { get; set; }
        public string BranchCode { get; set; }
        public bool? CreateReceiptForDbnote { get; set; }
        public bool? Ifrs15 { get; set; }
        public bool? AuditByPass { get; set; }
        public bool? ApproveByPass { get; set; }
        public bool? ReleaseByPass { get; set; }
        public string CompanyBaseUrl { get; set; }
        public bool? EnforceQualityAssuranceOnPo { get; set; }
        public bool? BookDisbursedPayments { get; set; }
        public bool? GenerateInvoicePerOrderLine { get; set; }
        public string PackId { get; set; }
        public bool? AllowPack { get; set; }
        public bool? IsShift { get; set; }
        public bool? IsShowPaymentOnCashier { get; set; }
        public bool? PrintUpc { get; set; }
        public string DefaultPrinterName { get; set; }
        public bool? IsSwitchCategoryProduct { get; set; }
        public bool? IsFullScreen { get; set; }
        public bool? IsShowTable { get; set; }
        public double? MarkUpCost { get; set; }
        public bool? MarkUp { get; set; }
        public string MaterialUsageAccount { get; set; }
    }
}
