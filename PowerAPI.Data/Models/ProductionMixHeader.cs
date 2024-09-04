using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class ProductionMixHeader
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string ProductionMixId { get; set; }
        public string BatchNumberId { get; set; }
        public string AssemblyId { get; set; }
        public string AssemblyDescription { get; set; }
        public string CurrencyId { get; set; }
        public double? CurrencyExchangeRate { get; set; }
        public DateTime? ProductionMixDate { get; set; }
        public double? ProductionUnit { get; set; }
        public string MixDescription { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? SystemDate { get; set; }
        public bool? Cleared { get; set; }
        public bool? Approved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
        public string AssemblyName { get; set; }
        public double? AssemblyWeight { get; set; }
    }
}
