﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class WarehouseBinTypes
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string WarehouseBinTypeId { get; set; }
        public string WarehouseBinTypeDescription { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
    }
}
