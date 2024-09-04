using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InventoryAssemblies
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string AssemblyId { get; set; }
        public string ItemId { get; set; }
        public double? NumberOfItemsInAssembly { get; set; }
        public string CurrencyId { get; set; }
        public double? CurrencyExchangeRate { get; set; }
        public decimal? LaborCost { get; set; }
        public string WarehouseId { get; set; }
        public string WarehouseBinId { get; set; }
        public bool? Approved { get; set; }
        public string ApprivedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string EnteredBy { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
    }
}
