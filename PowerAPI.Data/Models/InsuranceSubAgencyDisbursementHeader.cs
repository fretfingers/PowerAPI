using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceSubAgencyDisbursementHeader
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string SubAgencyVoucherId { get; set; }
        public string BrokerId { get; set; }
        public string BrokerName { get; set; }
        public DateTime? DisburseDate { get; set; }
        public DateTime? PostDate { get; set; }
        public double? PremiumAmount { get; set; }
        public double? BrokerComm { get; set; }
        public double? SubAgentAmount { get; set; }
        public double? TaxDue { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string CurrencyId { get; set; }
        public string CurrencyExchangeRate { get; set; }
        public bool? SelectedForPayment { get; set; }
        public DateTime? SelectedForPaymentDate { get; set; }
        public bool? ApprovedForPayment { get; set; }
        public DateTime? ApprovedForPaymentDate { get; set; }
        public string ApprovedBy { get; set; }
        public string GlbankAccount { get; set; }
        public string BankId { get; set; }
        public string PaymentStatus { get; set; }
        public bool? Void { get; set; }
        public string TaxGroupId { get; set; }
    }
}
