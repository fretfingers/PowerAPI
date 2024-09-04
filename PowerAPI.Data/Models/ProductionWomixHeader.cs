using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class ProductionWomixHeader
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string Woid { get; set; }
        public string MixId { get; set; }
        public string BatchNumberId { get; set; }
        public DateTime? ProductionDate { get; set; }
        public string Description { get; set; }
        public string CurrencyId { get; set; }
        public double? CurrencyExchangeRate { get; set; }
        public DateTime? ProductionMixDate { get; set; }
        public double? ProductionUnit { get; set; }
        public string MixDescription { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? SystemDate { get; set; }
        public bool? Cleared { get; set; }
        public string ClearedBy { get; set; }
        public DateTime? ClearedDate { get; set; }
        public bool? Approved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public bool? Posted { get; set; }
        public string PostedBy { get; set; }
        public DateTime? PostedDate { get; set; }
        public bool? Completed { get; set; }
        public string CompletedBy { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
        public double? OriginalMixWeight { get; set; }
        public double? OriginalBalanceMixWeight { get; set; }
        public string MixWarehouseId { get; set; }
        public string MixWarehouseBinId { get; set; }
        public string Reference { get; set; }
        public string Notes { get; set; }
        public double? ProductionWasteQty { get; set; }
        public double? ProductionWasteValue { get; set; }
    }
}
