using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceIncomeType
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string IncomeTypeId { get; set; }
        public string IncomeTypeDescription { get; set; }
        public string GlsalesAccount { get; set; }
        public string GlcommisionAccount { get; set; }
        public string Glvataccount { get; set; }
        public string Glcogaccount { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string NaicomdrAccount { get; set; }
        public string NaicomcrAccount { get; set; }
        public string GlunEarnedAccount { get; set; }
    }
}
