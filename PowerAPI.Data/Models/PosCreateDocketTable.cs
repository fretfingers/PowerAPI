﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class PosCreateDocketTable
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string TableId { get; set; }
        public string TableDescription { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public bool? ActiveYn { get; set; }
        public string BranchCode { get; set; }
    }
}
