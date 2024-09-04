using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PayrollHrpayrollLeaveType
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string LeaveTypeId { get; set; }
        public string Description { get; set; }
        public bool? Payable { get; set; }
        public bool? OnPayroll { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public bool? CalcOnLeaveDays { get; set; }
        public short? MaximumDays { get; set; }
        public string MaxWorkingdays { get; set; }
        public string UsagePeriod { get; set; }
        public string BranchCode { get; set; }
        public string LeaveGender { get; set; }
    }
}
