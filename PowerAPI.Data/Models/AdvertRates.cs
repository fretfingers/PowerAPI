using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class AdvertRates
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string ProductTypeId { get; set; }
        public string Attribute { get; set; }
        public string UnitOfMeasure { get; set; }
        public string AdvertType { get; set; }
        public double Rate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
    }
}
