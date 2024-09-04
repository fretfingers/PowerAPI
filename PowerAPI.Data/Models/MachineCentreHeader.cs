using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class MachineCentreHeader
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string MachineCenterId { get; set; }
        public string Machine { get; set; }
        public string MachineCentreDescription { get; set; }
        public double Capacity { get; set; }
        public double RunPeriodinMins { get; set; }
        public double ProductionRate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool? Cleared { get; set; }
        public string ClearedBy { get; set; }
        public DateTime? ClearedDate { get; set; }
        public bool? Approved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public double? MachineCostPerHour { get; set; }
        public bool? Void { get; set; }
        public string VoidBy { get; set; }
        public DateTime? VoidDate { get; set; }
        public bool? IsMixer { get; set; }
    }
}
