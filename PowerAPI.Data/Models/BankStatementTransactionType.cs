using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class BankStatementTransactionType
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string BankStatementTransactionTypeId { get; set; }
        public string Description { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
