using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PayrollHrpayrollTrainingDetail
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string TrainingId { get; set; }
        public string EmployeeId { get; set; }
        public decimal? Amount { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int LineId { get; set; }
        public string CourseDescription { get; set; }
        public string FacilitatorDescription { get; set; }
        public string BranchCode { get; set; }
        public DateTime? PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }
        public string Feedback { get; set; }
        public string FeedbackBy { get; set; }
        public DateTime? FeedbackDate { get; set; }
        public bool? FeedbackDone { get; set; }
        public string PeriodId { get; set; }
        public string TrainingFocus { get; set; }
    }
}
