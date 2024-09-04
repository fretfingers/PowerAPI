using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PayrollHrpayrollYearPayAnalysis
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string PayAnalysisEntityId { get; set; }
        public DateTime PayAnalysisPeriod { get; set; }
        public string Description { get; set; }
        public double? GrossPay { get; set; }
        public double? Deduction { get; set; }
        public double? Netpay { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? Cleared { get; set; }
        public string ClearedBy { get; set; }
        public DateTime? ClearedDate { get; set; }
        public bool? Approved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public bool? Void { get; set; }
        public string VoidBy { get; set; }
        public DateTime? VoidDate { get; set; }
        public DateTime? SystemDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public bool? Converted { get; set; }
        public string ConvertedBy { get; set; }
        public DateTime? ConvertedDate { get; set; }
    }
}
