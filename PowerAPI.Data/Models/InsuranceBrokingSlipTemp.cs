using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceBrokingSlipTemp
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string BusinessClassId { get; set; }
        public string SummaryofCover { get; set; }
        public string Summarya { get; set; }
        public string Summaryb { get; set; }
        public string Summaryc { get; set; }
        public string Summaryd { get; set; }
        public string Summarye { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
