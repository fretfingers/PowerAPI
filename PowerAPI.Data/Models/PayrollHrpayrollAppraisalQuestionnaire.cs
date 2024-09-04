using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PayrollHrpayrollAppraisalQuestionnaire
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string PeriodId { get; set; }
        public string Department { get; set; }
        public string AppraisalId { get; set; }
        public int QuestionId { get; set; }
        public string QuestionTypeId { get; set; }
        public string QuestionDescription { get; set; }
        public double? StaffRating { get; set; }
        public double? ManagerRating { get; set; }
        public string AnsweredBy { get; set; }
        public string Answer { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string StaffComment { get; set; }
        public string ManagerComment { get; set; }
        public bool? IsRecommendation { get; set; }
        public DateTime? DateAnswered { get; set; }
    }
}
