using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceBrokingSlipGeneratedItems
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string BrokingSlipRefNo { get; set; }
        public string SectionTypeId { get; set; }
        public string ManualNumbering { get; set; }
        public int BrokingSlipItemCount { get; set; }
        public string ItemsDescription { get; set; }
        public double? SumInsured { get; set; }
        public double? ExcessAmount { get; set; }
        public string ItemLocation { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
        public double? Discount { get; set; }
        public double? ItemRate { get; set; }
        public string DetailMemo1 { get; set; }
        public string DetailMemo2 { get; set; }
        public string DetailMemo3 { get; set; }
        public string DetailMemo4 { get; set; }
        public string DetailMemo5 { get; set; }
        public string DetailMemo6 { get; set; }
        public string DetailMemo7 { get; set; }
        public string DetailMemo8 { get; set; }
        public string DetailMemo9 { get; set; }
        public string DetailMemo10 { get; set; }
        public string DetailMemo11 { get; set; }
        public string DetailMemo12 { get; set; }
        public double? InsuranceRate { get; set; }
        public double? LoadingRate { get; set; }
        public double? ItemPremium { get; set; }
        public double? SumInsuredDiscounted { get; set; }
        public double? UndiscountedSumInsured { get; set; }
        public string BusinessClassId { get; set; }
        public string RiskTypeId { get; set; }
    }
}
