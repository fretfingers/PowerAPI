using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceSubAgencyTransaction
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string CustomerId { get; set; }
        public string VendorId { get; set; }
        public string BrokerId { get; set; }
        public string BusinessClassId { get; set; }
        public string InvoiceNumber { get; set; }
        public string RiskTypeId { get; set; }
        public string PolicyUnderwriterId { get; set; }
        public string PolicyBrokerId { get; set; }
        public string InvoiceType { get; set; }
        public string TransactionSourceId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? PostDate { get; set; }
        public double? PremiumAmount { get; set; }
        public double? BrokerCommPer { get; set; }
        public double? BrokerComm { get; set; }
        public double? SubAgentPer { get; set; }
        public double? SubAgentAmount { get; set; }
        public double? TaxDue { get; set; }
        public string ReceiptId { get; set; }
        public string TrackingNumber { get; set; }
        public string ProjectTypeId { get; set; }
        public string ProjectId { get; set; }
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
        public int? ConvertionRate { get; set; }
        public string CustomerName { get; set; }
        public decimal? SubAgentNetDue { get; set; }
    }
}
