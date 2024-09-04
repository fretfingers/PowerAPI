using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class OrderReferenceLog
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string RefNumber { get; set; }
        public string OrderId { get; set; }
        public bool? Used { get; set; }
        public DateTime? DateUsed { get; set; }
        public DateTime? SystemDate { get; set; }
        public string TransSource { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
