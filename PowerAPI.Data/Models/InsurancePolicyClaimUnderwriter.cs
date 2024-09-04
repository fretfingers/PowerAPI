using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsurancePolicyClaimUnderwriter
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string BrokerClaimId { get; set; }
        public string VendorId { get; set; }
        public string PolicyBrokerId { get; set; }
        public string UnderwriterClaimId { get; set; }
        public int AutoClaimNo { get; set; }
        public string BusinessClassId { get; set; }
        public string RiskTypeId { get; set; }
        public string CustomerId { get; set; }
        public double? UnderWriterApportion { get; set; }
        public double? EstimateAmount { get; set; }
        public double? Dvamount { get; set; }
        public double? AdjusterAmount { get; set; }
        public double? ReceiptAmount { get; set; }
        public double? BalanceDue { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string VendorName { get; set; }
    }
}
