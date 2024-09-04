using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PayrollHrpayrollGroupLeave
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string GroupId { get; set; }
        public string LeaveTypeId { get; set; }
        public string Description { get; set; }
        public int? LeaveDays { get; set; }
        public bool? ActiveYn { get; set; }
        public DateTime EnteredDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
