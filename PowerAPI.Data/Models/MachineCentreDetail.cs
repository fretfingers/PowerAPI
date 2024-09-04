using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class MachineCentreDetail
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string MachineCenterId { get; set; }
        public string Machine { get; set; }
        public string ItemId { get; set; }
        public string ItemDescription { get; set; }
        public double? ProductionQty { get; set; }
        public string UnitOfMeasure { get; set; }
        public double? ProductionTimeInMins { get; set; }
        public double? ProductionRate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
