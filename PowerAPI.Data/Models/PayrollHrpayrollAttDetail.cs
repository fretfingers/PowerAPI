using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PayrollHrpayrollAttDetail
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string EmployeeId { get; set; }
        public DateTime? TimeIn { get; set; }
        public bool? Absent { get; set; }
        public string Remarks { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public double? LatePeriod { get; set; }
        public DateTime? Period { get; set; }
        public string ShiftType { get; set; }
        public DateTime? ExpectedTimeIn { get; set; }
        public string BranchCode { get; set; }
        public DateTime? ClockOutTimeOut { get; set; }
        public string AttendanceStatus { get; set; }
        public bool? ClockedIn { get; set; }
        public bool? ClockedOut { get; set; }
        public string GeolocationAddress { get; set; }
        public double? GeoLongitude { get; set; }
        public double? GeoLatitude { get; set; }
        public DateTime? SystemDate { get; set; }
        public string GeolocationAddress2 { get; set; }
        public string EmployeeFirstName { get; set; }
        public DateTime? ExpectedTimeOut { get; set; }
        public string EmployeeName { get; set; }
        public int? Workedhours { get; set; }
        public int? Paidhours { get; set; }
        public int? PaidMin { get; set; }
        public string EmployeeLastName { get; set; }
    }
}
