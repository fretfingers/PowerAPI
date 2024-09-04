using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class ProductionWomixDetail
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string Woid { get; set; }
        public string MixId { get; set; }
        public string AssemblyId { get; set; }
        public string CurrencyId { get; set; }
        public double? CurrencyExchangeRate { get; set; }
        public double? ProductionUnit { get; set; }
        public string Notes { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? SystemDate { get; set; }
        public bool? Cleared { get; set; }
        public bool? Approved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? WorkOrderStartDate { get; set; }
        public DateTime? WorkOrderExpectedDate { get; set; }
        public bool? WorkOrderCompleted { get; set; }
        public DateTime? WorkOrderCompletedDate { get; set; }
        public string MaterialIssueVarianceAccount { get; set; }
        public string AdditionalCostAccount { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
        public string AssemblyDescription { get; set; }
        public string WotransId { get; set; }
        public DateTime? WorkOrderIssuedDate { get; set; }
        public string WorkOrderReference { get; set; }
        public string WarehouseId { get; set; }
        public string WarehouseBinId { get; set; }
        public string FgwarehouseId { get; set; }
        public string FgwarehouseBinId { get; set; }
        public double? QuantityProduced { get; set; }
        public double? QuantityProducedToDate { get; set; }
        public double? YieldVariance { get; set; }
        public string ProductionReference { get; set; }
        public double? WorkOrderBomunitCost { get; set; }
        public double? WorkOrderTotalCost { get; set; }
        public string ProjectId { get; set; }
        public double? QuantityToProduced { get; set; }
        public double? WorkOrderBomunitLabor { get; set; }
        public double? WorkOrderBomotherCost { get; set; }
        public double? WorkOrderBomunitMaterialCost { get; set; }
        public double? NewBomitemCost { get; set; }
        public string WorkOrderCompletedBy { get; set; }
        public double? WorkOrderBomunitMachineCost { get; set; }
        public double? WorkOrderItemValue { get; set; }
        public string WorkOrderDetailMemo8 { get; set; }
        public double? WasteQty { get; set; }
    }
}
