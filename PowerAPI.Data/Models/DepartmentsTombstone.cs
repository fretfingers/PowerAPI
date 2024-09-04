using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class DepartmentsTombstone
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public DateTime? DeletionDate { get; set; }
        public string BranchCode { get; set; }
    }
}
