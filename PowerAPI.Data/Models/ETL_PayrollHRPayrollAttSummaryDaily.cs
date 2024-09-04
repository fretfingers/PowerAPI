using System;
using System.Collections.Generic;
using System.Text;

namespace PowerAPI.Data.Models
{
    public partial class ETL_PayrollHRPayrollAttSummaryDaily
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeId { get; set; }
        public int? ExpectedNoofWorkHrs { get; set; }
        public DateTime? ExpectedTimeIn { get; set; }
        public DateTime? ExpectedTimeOut { get; set; }
        public DateTime? TimeIn { get; set; }
        public DateTime? ClockedOut { get; set; }
        public int? WorkedHours { get; set; }
        public int? AbsentHours { get; set; }
        public int? Paidhours { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
