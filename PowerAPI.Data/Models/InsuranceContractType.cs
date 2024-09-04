using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceContractType
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string ContractTypeId { get; set; }
        public string ContractTypeDescription { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string CommissionAccount { get; set; }
        public string Vataccount { get; set; }
        public string NetPremium { get; set; }
        public string DebtorsControl { get; set; }
        public string CreditorControl { get; set; }
        public string InvoiceTypeId { get; set; }
    }
}
