using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceBrokingSlipGenerated
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string BrokingSlipRefNo { get; set; }
        public string BrokingSlipRefNoPackageId { get; set; }
        public int BrokingSlipAutono { get; set; }
        public string CustomerId { get; set; }
        public string DummyName { get; set; }
        public string Location { get; set; }
        public string BusinessClassId { get; set; }
        public string Business { get; set; }
        public string SummaryCover { get; set; }
        public string CoverageLocation { get; set; }
        public int? NoofItemsCovered { get; set; }
        public int? NoofExtensionRequired { get; set; }
        public string ProjectTypeId { get; set; }
        public string ProjectId { get; set; }
        public double? SumInsured { get; set; }
        public double? ExcessAmount { get; set; }
        public DateTime? DueDate { get; set; }
        public string EmployeeId { get; set; }
        public bool? Cleared { get; set; }
        public string ClearedBy { get; set; }
        public DateTime? ClearedDate { get; set; }
        public bool? Approved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public bool? Void { get; set; }
        public string VoidBy { get; set; }
        public DateTime? VoidDate { get; set; }
        public DateTime? EnterDate { get; set; }
        public string PeriodCover { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public bool? ExcessInPercent { get; set; }
        public double? ExcessAcctualAmount { get; set; }
        public string CustomerName { get; set; }
        public string DocumentName { get; set; }
    }
}
