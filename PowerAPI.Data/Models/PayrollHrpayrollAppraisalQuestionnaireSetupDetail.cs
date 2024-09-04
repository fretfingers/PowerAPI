using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PayrollHrpayrollAppraisalQuestionnaireSetupDetail
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string PeriodId { get; set; }
        public string Department { get; set; }
        public int QuestionId { get; set; }
        public string QuestionTypeId { get; set; }
        public string AnsweredBy { get; set; }
        public string QuestionDescription { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public bool? IsSupervisor { get; set; }
        public bool? IsRecommendation { get; set; }
    }
}
