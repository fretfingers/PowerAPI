using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsurancePolicyUnderwritersHistory
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string PolicyBrokerId { get; set; }
        public string VendorId { get; set; }
        public string PolicyUnderwriterId { get; set; }
        public string PackagePololicyId { get; set; }
        public double? Apportionment { get; set; }
        public double? TotalUnderApp { get; set; }
        public double? BalanceApp { get; set; }
        public DateTime? DateAdded { get; set; }
        public string EmployeeId { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public int TransactionId { get; set; }
    }
}
