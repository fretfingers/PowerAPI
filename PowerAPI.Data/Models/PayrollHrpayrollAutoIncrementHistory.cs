using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PayrollHrpayrollAutoIncrementHistory
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public int AutoId { get; set; }
        public string AutoIncrementId { get; set; }
        public string AutoIncrementTypeId { get; set; }
        public string AutoIncrementEntityId { get; set; }
        public DateTime? AutoIncrementPeriod { get; set; }
        public DateTime? SystemDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public double? Amount { get; set; }
        public bool? Flat { get; set; }
        public double? AmountOld { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
