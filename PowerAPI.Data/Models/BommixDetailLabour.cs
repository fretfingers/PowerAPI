using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class BommixDetailLabour
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string ProductionMixId { get; set; }
        public string LabourTypeId { get; set; }
        public string BatchNumberId { get; set; }
        public string AssemblyId { get; set; }
        public string LabourTypeDescription { get; set; }
        public double? NumberOfWorkers { get; set; }
        public double? EstimatedHours { get; set; }
        public double? EstimatedCost { get; set; }
        public string UnitOfMeasure { get; set; }
        public string WarehouseId { get; set; }
        public string WarehouseBinId { get; set; }
        public string CurrencyId { get; set; }
        public double? CurrencyExchangeRate { get; set; }
        public double? ProductionUnit { get; set; }
        public string Notes { get; set; }
        public DateTime? SystemDate { get; set; }
        public bool? Cleared { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
        public string GlaccountNumber { get; set; }
    }
}
