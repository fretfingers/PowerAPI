using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsurancePolicyCoBroker
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string PolicyBrokerId { get; set; }
        public string CoBrokerId { get; set; }
        public double Apportionment { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public double? AdminCharge { get; set; }
        public bool? Flat { get; set; }
    }
}
