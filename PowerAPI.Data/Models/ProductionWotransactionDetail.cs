﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class ProductionWotransactionDetail
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string WotransId { get; set; }
        public string AssemblyId { get; set; }
        public string CurrencyId { get; set; }
        public double? CurrencyExchangeRate { get; set; }
        public double? ProductionUnit { get; set; }
        public string Notes { get; set; }
        public string EnteredBy { get; set; }
        public DateTime SystemDate { get; set; }
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
        public int? WotransReference { get; set; }
        public string WorkOrderDetailMemo8 { get; set; }
        public string WosourceId { get; set; }
    }
}
