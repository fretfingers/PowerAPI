using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceBusinessClassSubagencyByClient
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string CustomerId { get; set; }
        public string VendorId { get; set; }
        public string BusinessClassId { get; set; }
        public string CommissionPer { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public int SubAgentLineId { get; set; }
    }
}
