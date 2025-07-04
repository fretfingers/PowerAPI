﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InventoryReceivedSerialNumberDetail
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string InvoiceNumber { get; set; }
        public string ItemId { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? IssuedDate { get; set; }
        public string EnteredBy { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string DocumentType { get; set; }
        public string BranchCode { get; set; }
    }
}
