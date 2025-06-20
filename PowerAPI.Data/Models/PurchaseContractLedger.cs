﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PurchaseContractLedger
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string PurchaseContractNumber { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string PurchaseOrderLineNumber { get; set; }
        public int NumberUsed { get; set; }
        public DateTime DateUsed { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
    }
}
