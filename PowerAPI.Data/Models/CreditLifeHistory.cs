using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class CreditLifeHistory
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string CertificateNo { get; set; }
        public int CreditId { get; set; }
        public int CreditHistoryId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public double? SumInsured { get; set; }
        public double? Rate { get; set; }
        public double? Premium { get; set; }
        public double? Commission { get; set; }
        public double? Vat { get; set; }
        public double? Netpremium { get; set; }
        public string EnteredBy { get; set; }
        public string Tenor { get; set; }
        public string Scope { get; set; }
        public string BankId { get; set; }
        public string Insurer { get; set; }
        public string Employer { get; set; }
        public string InsuredName { get; set; }
        public string TransactionNo { get; set; }
        public DateTime? InceptionDate { get; set; }
        public string EmployeeId { get; set; }
        public string BusinessClassId { get; set; }
        public string RiskTypeId { get; set; }
        public string BranchCode { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string InsuredAccountNo { get; set; }
        public string InsuredType { get; set; }
    }
}
