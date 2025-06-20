﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PurchaseDetail
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string PurchaseNumber { get; set; }
        public decimal PurchaseLineNumber { get; set; }
        public string ItemId { get; set; }
        public string VendorItemId { get; set; }
        public string Description { get; set; }
        public string WarehouseId { get; set; }
        public string WarehouseBinId { get; set; }
        public string SerialNumber { get; set; }
        public double? OrderQty { get; set; }
        public string ItemUom { get; set; }
        public double? ItemWeight { get; set; }
        public double? DiscountPerc { get; set; }
        public bool? Taxable { get; set; }
        public decimal? ItemCost { get; set; }
        public string CurrencyId { get; set; }
        public double? CurrencyExchangeRate { get; set; }
        public double? ItemUnitPrice { get; set; }
        public string TaxGroupId { get; set; }
        public decimal? TaxAmount { get; set; }
        public double? TaxPercent { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Total { get; set; }
        public double? TotalWeight { get; set; }
        public string GlpurchaseAccount { get; set; }
        public string Glcogaccount { get; set; }
        public string ProjectId { get; set; }
        public bool? Received { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public double? ReceivedQty { get; set; }
        public string RecivingNumber { get; set; }
        public string TrackingNumber { get; set; }
        public string DetailMemo1 { get; set; }
        public string DetailMemo2 { get; set; }
        public string DetailMemo3 { get; set; }
        public string DetailMemo4 { get; set; }
        public string DetailMemo5 { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public double? ItemAddOnCost { get; set; }
        public double? ItemTotalAddOnCost { get; set; }
        public double? MarkUp { get; set; }
        public double? Margin { get; set; }
        public string GlanalysisType1 { get; set; }
        public string GlanalysisType2 { get; set; }
        public string AssetId { get; set; }
        public string BudgetId { get; set; }
        public string TaxGroupIdvat { get; set; }
        public decimal? TaxAmountVat { get; set; }
        public string TaxGroupIdwht { get; set; }
        public decimal? TaxAmountWht { get; set; }
        public string MultipleDiscountGroupId { get; set; }
        public decimal? MultipleDiscountAmount { get; set; }
        public double? MultipleDiscountPercent { get; set; }
        public string ItemUpccode { get; set; }
        public string BranchCode { get; set; }
        public DateTime? DocumentDate { get; set; }
        public double? AddOnCostSpecificPerItem { get; set; }
        public string GladdOnCostExpenseAccount { get; set; }
        public string VendorId { get; set; }
        public double? ItemUnitSellingPrice { get; set; }
        public double? MarkupCost { get; set; }
    }
}
