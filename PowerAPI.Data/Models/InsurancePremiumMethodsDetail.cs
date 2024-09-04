using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsurancePremiumMethodsDetail
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string InsurancePremiumMethodsId { get; set; }
        public int InsurancePremiumDetailLine { get; set; }
        public bool? BegingBracket { get; set; }
        public bool? EndBracket { get; set; }
        public string FieldName1 { get; set; }
        public string FieldOperators1 { get; set; }
        public string FieldName2 { get; set; }
        public string FieldOperators2 { get; set; }
        public string FieldName3 { get; set; }
        public string FieldOperators3 { get; set; }
        public string FieldName4 { get; set; }
        public string FieldOperators4 { get; set; }
        public string FieldName5 { get; set; }
        public string FieldOperators5 { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
