using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PayrollHrpayrollEmployeeLeaveHeaderHistory
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string EmployeeId { get; set; }
        public int Counter { get; set; }
        public int? NormalDays { get; set; }
        public int? AdditionalDays { get; set; }
        public int? UtilizedDays { get; set; }
        public int? Balance { get; set; }
        public DateTime? Period { get; set; }
        public DateTime? SystemDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
