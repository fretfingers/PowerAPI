using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PayrollHrpayrollLeaveDetail
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string LeaveType { get; set; }
        public string EmployeeId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string PayTypeId { get; set; }
        public int? LeaveDays { get; set; }
        public int? OutLeaveDays { get; set; }
        public double? ActualAmount { get; set; }
        public decimal? Amount { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public bool? Cleared { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public bool? Approved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public bool? Posted { get; set; }
        public string PostedBy { get; set; }
        public DateTime? PostedDate { get; set; }
        public int? PriorLeaveDays { get; set; }
        public int LeaveId { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public string EmployeeName { get; set; }
        public string SsapprovedBy { get; set; }
        public bool? ExecutivePermision { get; set; }
        public string ExecutivePermisionBy { get; set; }
        public DateTime? ExecutiveApproveDate { get; set; }
        public string BranchCode { get; set; }
        public string ExecutivePermisionName { get; set; }
        public string Transtype { get; set; }
        public string NotesB { get; set; }
        public bool? Audited { get; set; }
        public string AuditedBy { get; set; }
        public DateTime? AuditedDate { get; set; }
        public string ApprovedByName { get; set; }
        public string PostedByName { get; set; }
    }
}
