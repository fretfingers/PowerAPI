using System;
using System.Collections.Generic;
using System.Text;

namespace PowerAPI.Data.POCO
{
    public class PayrollJournalBasis
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string JournalBasis { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTS { get; set; }
    }
}
