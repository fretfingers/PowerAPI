using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceRequest
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string RequestId { get; set; }
        public string CustomerId { get; set; }
        public string RequestDescription { get; set; }
        public DateTime? DateRequested { get; set; }
    }
}
