﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class WorkOrderTypes
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string WorkOrderTypes1 { get; set; }
        public string WorkOrderTypesDescription { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
    }
}
