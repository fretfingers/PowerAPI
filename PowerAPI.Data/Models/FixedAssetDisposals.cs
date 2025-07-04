﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class FixedAssetDisposals
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string AssetId { get; set; }
        public string AssetName { get; set; }
        public string AssetSerialNumber { get; set; }
        public string AssetTypeId { get; set; }
        public string AssetStatusId { get; set; }
        public string VendorId { get; set; }
        public string AssetDescription { get; set; }
        public string AssetLocation { get; set; }
        public string AssetUsedBy { get; set; }
        public string AssetDepreciationMethodId { get; set; }
        public DateTime? AssetInServiceDate { get; set; }
        public string CurrencyId { get; set; }
        public double? CurrencyExchangeRate { get; set; }
        public decimal? AssetOriginalCost { get; set; }
        public short? AssetUsefulLife { get; set; }
        public decimal? AssetSalvageValue { get; set; }
        public decimal? AssetSalesPrice { get; set; }
        public DateTime? AssetPlanedDisposalDate { get; set; }
        public DateTime? AssetAcutalDisposalDate { get; set; }
        public decimal? AssetActualDisposalAmount { get; set; }
        public DateTime? LastDepreciationDate { get; set; }
        public decimal? LastDepreciationAmount { get; set; }
        public decimal? AccumulatedDepreciation { get; set; }
        public string DepreciationPeriod { get; set; }
        public decimal? AssetBookValue { get; set; }
        public string GlfixedAssetAccount { get; set; }
        public string GlfixedDepreciationAccount { get; set; }
        public string GlfixedAccumDepreciationAccount { get; set; }
        public string GlfixedDisposalAccount { get; set; }
        public bool? Approved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string EnteredBy { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public bool? Posted { get; set; }
        public bool? Disposed { get; set; }
        public string Narratives { get; set; }
        public string BankId { get; set; }
        public double? ProfitLoss { get; set; }
        public string ProjectId { get; set; }
        public string BranchCode { get; set; }
    }
}
