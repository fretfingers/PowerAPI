﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class ProductionWodetail
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string Woid { get; set; }
        public int WorkOrderDetailLineId { get; set; }
        public string WorkOrderNumber { get; set; }
        public string WorkOrderBomnumber { get; set; }
        public string WorkOrderItemId { get; set; }
        public DateTime? WorkOrderIssuedDate { get; set; }
        public string WorkOrderReference { get; set; }
        public double? WorkOrderRequiredQty { get; set; }
        public double? WorkOrderIssueQty { get; set; }
        public double? WorkOrderIssuedQtyToDate { get; set; }
        public string ProjectId { get; set; }
        public double? WorkOrderItemCost { get; set; }
        public double? WorkOrderItemValue { get; set; }
        public double? WorkOrderRequiredValue { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string GlanalysisType1 { get; set; }
        public string GlanalysisType2 { get; set; }
        public string AssetId { get; set; }
        public string BranchCode { get; set; }
        public string WotransId { get; set; }
    }
}
