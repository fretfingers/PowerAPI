using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class CustomerCreditLimit
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string CreditLimitId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime CommencementDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime EnteredDate { get; set; }
        public string EnteredBy { get; set; }
        public bool? Cleared { get; set; }
        public string ClearedBy { get; set; }
        public DateTime? ClearedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public bool? Approved { get; set; }
        public double? Amount { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public bool? Liquidated { get; set; }
        public string LiquidatedBy { get; set; }
        public DateTime? LiquidationDate { get; set; }
        public bool? ActiveYn { get; set; }
    }
}
