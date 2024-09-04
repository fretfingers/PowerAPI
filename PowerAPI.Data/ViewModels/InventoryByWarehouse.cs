using PowerAPI.Data.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.ViewModels
{
    public class InventoryByWarehouse
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string ItemId { get; set; }
        public string WarehouseId { get; set; }
        public string WarehouseBinId { get; set; }
        public double? QtyOnHand { get; set; }
        public double? QtyCommitted { get; set; }
        public double? QtyOnOrder { get; set; }
        public double? QtyOnBackorder { get; set; }
        public string CycleCode { get; set; }
        public DateTime? LastCountDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public DateTime? LastEditDate { get; set; }
        public DateTime? CreationDate { get; set; }
        public string BranchCode { get; set; }
        public ICollection<SetupWorkflow> WorkFlowTrail { get; set; }
    }
}
