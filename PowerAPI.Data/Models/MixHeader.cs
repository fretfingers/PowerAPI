using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class MixHeader
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string ProductionMixId { get; set; }
        public string BatchNumberId { get; set; }
        public string AssemblyId { get; set; }
        public string AssemblyDescription { get; set; }
        public string CurrencyId { get; set; }
        public double? CurrencyExchangeRate { get; set; }
        public DateTime? ProductionMixDate { get; set; }
        public double? ProductionUnit { get; set; }
        public string MixDescription { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? SystemDate { get; set; }
        public bool? Cleared { get; set; }
        public bool? Approved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
        public string WarehouseId { get; set; }
        public string WarehouseBinId { get; set; }
        public string AssemblyName { get; set; }
        public double? AssemblyWeight { get; set; }
        public DateTime? IssueDate { get; set; }
        public double? MixWeight { get; set; }
        public double? MaterialValue { get; set; }
        public double? MachineValue { get; set; }
        public double? TotalMixValue { get; set; }
        public bool? Issued { get; set; }
        public string IssuedBy { get; set; }
        public bool? IssueCompleted { get; set; }
        public DateTime? IssueCompletedDate { get; set; }
        public string IssuedCompletedBy { get; set; }
        public double? LabourValue { get; set; }
        public string MaterialIssueVarianceAccount { get; set; }
        public double? OrderValue { get; set; }
        public int? IssueCount { get; set; }
        public double? MixBalanceWeight { get; set; }
    }
}
