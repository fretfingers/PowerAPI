﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceBrokingSlipGeneratedExclusion
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string BrokingSlipRefNo { get; set; }
        public int BrokingSlipExclusionCount { get; set; }
        public string ExclusionDescription { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
