using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceBrokerInformation
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string BrokerId { get; set; }
        public string BrokerName { get; set; }
        public string BrokerAddress1 { get; set; }
        public string BrokerAddress2 { get; set; }
        public string BrokerAddress3 { get; set; }
        public string BrokerPhone { get; set; }
        public string BrokerFax { get; set; }
        public string Brokeremail { get; set; }
        public string Brokerweb { get; set; }
        public string CurrencyId { get; set; }
        public string AccountStatus { get; set; }
        public string GlsalesAccount { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public bool? NoPremiumNoCover { get; set; }
        public bool? EnforceDebitApprove { get; set; }
        public bool? EnforcePolicyApprove { get; set; }
        public int? PolicyRenewalDays { get; set; }
        public int? PolicyRenewalSmsdays { get; set; }
        public bool? PolicyPurchaseEmail { get; set; }
        public bool? PolicyPurchaseSms { get; set; }
        public bool? ClaimsEmail { get; set; }
        public bool? ClaimsSms { get; set; }
        public bool? CalBrokerageOnLegalcession { get; set; }
        public string BrokerMappedId { get; set; }
    }
}
