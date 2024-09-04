using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PayrollHrpayrollAppraisalHeader
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string PeriodId { get; set; }
        public string AppraisalId { get; set; }
        public string AppraisalName { get; set; }
        public string Department { get; set; }
        public string GroupId { get; set; }
        public string AppraiseeId { get; set; }
        public double? TotalScore { get; set; }
        public double? MaxScore { get; set; }
        public double? Percentage { get; set; }
        public string Remark { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public bool? Cleared { get; set; }
        public bool? Approved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public bool? Posted { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
        public bool? ReviewStatus { get; set; }
        public bool? Reviewed { get; set; }
        public bool? Attest { get; set; }
        public bool? IsAttested { get; set; }
        public string AttestationComment { get; set; }
        public DateTime? DateAttested { get; set; }
        public string PostedBy { get; set; }
        public DateTime? PostedDate { get; set; }
        public bool? Confirmed { get; set; }
        public string ConfirmedBy { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public string ApprovedByName { get; set; }
        public string SecondLevelApprovedByName { get; set; }
    }
}
