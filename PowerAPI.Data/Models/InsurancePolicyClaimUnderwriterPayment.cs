using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsurancePolicyClaimUnderwriterPayment
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string PolicyBrokerId { get; set; }
        public string VendorId { get; set; }
        public string CustomerId { get; set; }
        public string BrokerClaimId { get; set; }
        public int PaymentId { get; set; }
        public string UnderwriterClaimId { get; set; }
        public string BusinessClassId { get; set; }
        public string RiskTypeId { get; set; }
        public double? ReceiptAmount { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string BankName { get; set; }
        public string ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
