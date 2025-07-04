﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class OrderTrackingDetail
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string OrderNumber { get; set; }
        public decimal CommentNumber { get; set; }
        public DateTime? CommentDate { get; set; }
        public string Comment { get; set; }
        public string CommentDetails { get; set; }
        public bool? Approved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
    }
}
