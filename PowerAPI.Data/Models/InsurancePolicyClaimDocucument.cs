using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsurancePolicyClaimDocucument
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string PolicyBrokerId { get; set; }
        public string BrokerClaimId { get; set; }
        public string UnderwriterClaimId { get; set; }
        public string BusinessClassId { get; set; }
        public string RiskTypeId { get; set; }
        public string CustomerId { get; set; }
        public string ClaimsDocumentId { get; set; }
        public string DocumentName { get; set; }
        public bool? DocStatus { get; set; }
        public DateTime? DateSubmited { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string ClaimsDocument { get; set; }
        public string ClaimsDocUpload { get; set; }
    }
}
