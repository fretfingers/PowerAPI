using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceSubAgencyTransDate
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public DateTime? Transdate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
