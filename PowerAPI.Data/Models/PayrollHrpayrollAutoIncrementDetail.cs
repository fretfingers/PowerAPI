using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PayrollHrpayrollAutoIncrementDetail
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string AutoIncrementId { get; set; }
        public string AutoIncrementEntityId { get; set; }
        public DateTime? AutoIncrementPeriod { get; set; }
        public double? Amount { get; set; }
        public bool? Flat { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string EntityDescription { get; set; }
    }
}
