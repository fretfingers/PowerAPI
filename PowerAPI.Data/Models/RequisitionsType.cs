using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class RequisitionsType
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string RequisitionTypeId { get; set; }
        public string Description { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
