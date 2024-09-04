using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class CustomerCreditLimitLiquidate
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string CreditLimitId { get; set; }
        public string CustomerId { get; set; }
        public bool? Liquidated { get; set; }
        public string LiquidatedBy { get; set; }
        public DateTime? LiquidationDate { get; set; }
        public decimal? Arbalance { get; set; }
        public decimal? CreditLimitAmount { get; set; }
        public DateTime? CommencementDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string BranchCode { get; set; }
        public DateTime? SystemDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public decimal CreditLimitLiquidateId { get; set; }
        public string CustomerName { get; set; }
    }
}
