using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceNoteGeneratedUnderwriter
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string InvoiceNumber { get; set; }
        public string NoteTypeId { get; set; }
        public string PolicyBrokerId { get; set; }
        public string PolicyUnderwriterId { get; set; }
        public string CustomerId { get; set; }
        public string VendorId { get; set; }
        public decimal? Apportionment { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
