﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class AircraftHeader
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string CustomerId { get; set; }
        public string AircraftId { get; set; }
        public string AircraftName { get; set; }
        public string Description { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
    }
}
