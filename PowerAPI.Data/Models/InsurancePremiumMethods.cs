using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsurancePremiumMethods
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string InsurancePremiumMethodsId { get; set; }
        public string InsurancePremiumMethodsDescription { get; set; }
        public string PremiumFormula { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
