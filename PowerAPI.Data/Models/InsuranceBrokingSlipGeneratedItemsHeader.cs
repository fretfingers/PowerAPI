using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceBrokingSlipGeneratedItemsHeader
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string BrokingSlipRefNo { get; set; }
        public string SectionTypeId { get; set; }
        public string Description { get; set; }
        public double? SumInsured { get; set; }
        public double? PremiumDue { get; set; }
        public DateTime? EnteredDate { get; set; }
        public string EnteredBy { get; set; }
        public string BranchCode { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public bool? Approved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string HeaderMemo1 { get; set; }
        public string HeaderMemo2 { get; set; }
        public string HeaderMemo3 { get; set; }
        public string HeaderMemo4 { get; set; }
        public string HeaderMemo5 { get; set; }
        public string HeaderMemo6 { get; set; }
        public string HeaderMemo7 { get; set; }
        public string HeaderMemo8 { get; set; }
        public string HeaderMemo9 { get; set; }
        public string HeaderMemo10 { get; set; }
        public string HeaderMemo11 { get; set; }
        public string HeaderMemo12 { get; set; }
        public double? SumInsuredDiscounted { get; set; }
        public string CustomerName { get; set; }
        public string BusinessClassId { get; set; }
        public string RiskTypeId { get; set; }
    }
}
