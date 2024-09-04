using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceClaimsDocuments
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string ClaimsDocumentId { get; set; }
        public int AutoDocNo { get; set; }
        public string DocumentDescription { get; set; }
        public string DeleteStatus { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string ClaimsDocUpload { get; set; }
    }
}
