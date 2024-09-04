using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceClaimLetter
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public int Letters { get; set; }
        public bool? Active { get; set; }
        public string ClaimAcknowledgement { get; set; }
        public string DocumentRequest { get; set; }
        public string InsurerNotification { get; set; }
        public string ReminderSettlement { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
