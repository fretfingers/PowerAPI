using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceLocation
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string LocationId { get; set; }
        public string LocationName { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
