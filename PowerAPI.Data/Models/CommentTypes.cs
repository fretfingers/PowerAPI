﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class CommentTypes
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string CommentType { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
    }
}
