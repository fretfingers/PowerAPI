﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InvoiceDetailBatches
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string BillNumber { get; set; }
        public DateTime BillDate { get; set; }
        public string BillLineNumber { get; set; }
        public decimal InvoiceLineNumber { get; set; }
        public string ItemId { get; set; }
        public string ItemUpccode { get; set; }
        public string WarehouseId { get; set; }
        public string WarehouseBinId { get; set; }
        public string SerialNumber { get; set; }
        public double? OrderQty { get; set; }
        public bool? BackOrdered { get; set; }
        public double? BackOrderQty { get; set; }
        public string ItemUom { get; set; }
        public double? ItemWeight { get; set; }
        public string Description { get; set; }
        public double? DiscountPerc { get; set; }
        public bool? Taxable { get; set; }
        public string CurrencyId { get; set; }
        public double? CurrencyExchangeRate { get; set; }
        public decimal? ItemCost { get; set; }
        public decimal? ItemUnitPrice { get; set; }
        public string TaxGroupId { get; set; }
        public decimal? TaxAmount { get; set; }
        public double? TaxPercent { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Total { get; set; }
        public double? TotalWeight { get; set; }
        public string GlsalesAccount { get; set; }
        public string Glcogaccount { get; set; }
        public string ProjectId { get; set; }
        public string WarehouseBinZone { get; set; }
        public string PalletLevel { get; set; }
        public string CartonLevel { get; set; }
        public string PackLevelA { get; set; }
        public string PackLevelB { get; set; }
        public string PackLevelC { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime? ScheduledStartDate { get; set; }
        public DateTime? ScheduledEndDate { get; set; }
        public DateTime? ServiceStartDate { get; set; }
        public DateTime? ServiceEndDate { get; set; }
        public string PerformedBy { get; set; }
        public string DetailMemo1 { get; set; }
        public string DetailMemo2 { get; set; }
        public string DetailMemo3 { get; set; }
        public string DetailMemo4 { get; set; }
        public string DetailMemo5 { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string ItemPricingCode { get; set; }
        public string DeliveryNumber { get; set; }
        public string GlanalysisType1 { get; set; }
        public string GlanalysisType2 { get; set; }
        public string AssetId { get; set; }
        public string BudgetId { get; set; }
        public string BranchCode { get; set; }
    }
}
