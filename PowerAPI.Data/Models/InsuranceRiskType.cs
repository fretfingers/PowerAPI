using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceRiskType
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string BusinessClassId { get; set; }
        public string RiskTypeId { get; set; }
        public string RiskName { get; set; }
        public double? BrokerComm { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string RiskTypeName { get; set; }
        public string InsuranceCategoryId { get; set; }
    }
}
