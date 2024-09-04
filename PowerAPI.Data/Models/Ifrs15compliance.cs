using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class Ifrs15compliance
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public int TransactionId { get; set; }
        public DateTime PostingTransactionDate { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string TransactionReference { get; set; }
        public string TransactionNotes { get; set; }
        public string UnEarnedAccount { get; set; }
        public string EarnedAccount { get; set; }
        public double? MonthlyCharge { get; set; }
        public double? DeferredRevenue { get; set; }
        public double? TotalRevenueGenerated { get; set; }
        public double? CarriedForwardRevenue { get; set; }
        public int? Period { get; set; }
        public DateTime? SystemDate { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? DateLastPosted { get; set; }
        public bool? Processed { get; set; }
        public string ProcessedBy { get; set; }
        public DateTime? ProcessDate { get; set; }
        public string MonthYear { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public int? InsurerCount { get; set; }
        public double? Netbrokerage { get; set; }
        public double? CoBrokerage { get; set; }
        public double? BrokerPercent { get; set; }
        public double? CoBrokerPercent { get; set; }
    }
}
