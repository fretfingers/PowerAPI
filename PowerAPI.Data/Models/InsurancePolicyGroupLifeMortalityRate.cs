using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsurancePolicyGroupLifeMortalityRate
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public int MortaliltyRateId { get; set; }
        public double? MinimumGrossPremium { get; set; }
        public double? MaximumGrossPremium { get; set; }
        public int? MinimumAge { get; set; }
        public int? MaximumAge { get; set; }
        public double? PremiumRate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
