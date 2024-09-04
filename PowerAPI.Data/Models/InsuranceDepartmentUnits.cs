using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceDepartmentUnits
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string UnitId { get; set; }
        public string UnitName { get; set; }
        public string UnitPhone { get; set; }
        public string UnitFax { get; set; }
        public string UnitEmail { get; set; }
        public string UnitAttention { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
