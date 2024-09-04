using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceNoteGeneratedDetail
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string InvoiceNumber { get; set; }
        public int InvoiceLineNumber { get; set; }
        public string NoteTypeId { get; set; }
        public string PolicyBrokerId { get; set; }
        public string ActualPolicyBrokerId { get; set; }
        public string PolicyUnderwriterId { get; set; }
        public string DetailDescription { get; set; }
        public decimal? SumInsured { get; set; }
        public string PremiumFomular { get; set; }
        public decimal? PremiumDue { get; set; }
        public decimal? Discount1 { get; set; }
        public decimal? BrokerCommissionRate { get; set; }
        public decimal? BrokerCommisson { get; set; }
        public decimal? Vatrate { get; set; }
        public decimal? Vatdue { get; set; }
        public decimal? NetDue { get; set; }
        public bool? FlatAmount { get; set; }
        public string ProjectTypeId { get; set; }
        public string ProjectId { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
