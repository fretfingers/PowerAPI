using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceMarineCertificate
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string PolicyBrokerId { get; set; }
        public string Bodereureno { get; set; }
        public string CertificateNumber { get; set; }
        public string BusinessclassId { get; set; }
        public string RisktypeId { get; set; }
        public string PolicyNumber { get; set; }
        public string ClientId { get; set; }
        public string LeadUnderwriterId { get; set; }
        public string EndorsementNo { get; set; }
        public string Officer { get; set; }
        public double? CommRate { get; set; }
        public int? NoteNumber { get; set; }
        public DateTime? TransDate { get; set; }
        public DateTime? Effdate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RenewalDate { get; set; }
        public double? ValueGood { get; set; }
        public double? InvoiceValue { get; set; }
        public string InvoiceNo { get; set; }
        public double? FreightValue { get; set; }
        public double? LoadingRate { get; set; }
        public double? BasicRate { get; set; }
        public double? WarRate { get; set; }
        public double? TotalRate { get; set; }
        public double? ActualRate { get; set; }
        public double? SumInsured { get; set; }
        public double? GrossPremium { get; set; }
        public double? CommissionDue { get; set; }
        public double? LeadUnderPer { get; set; }
        public string SingleCover { get; set; }
        public string Conveyance { get; set; }
        public string MarFrom { get; set; }
        public string MarTo { get; set; }
        public string DescriptionGoods { get; set; }
        public string Conditions { get; set; }
        public string Terms { get; set; }
        public string ExaminedBy { get; set; }
        public DateTime? ExaminedDate { get; set; }
        public string CoInsured { get; set; }
        public string MarksandNos { get; set; }
        public string CurrencyCode { get; set; }
        public double? ExchangeRate { get; set; }
        public double? BottomLimit { get; set; }
        public string CertificateStatus { get; set; }
        public short? TwoRate { get; set; }
        public double? RateOne { get; set; }
        public double? ValueOne { get; set; }
        public string Posttatus { get; set; }
        public DateTime? Postdate { get; set; }
        public double? RetrunPremium { get; set; }
        public string NoteGenerated { get; set; }
        public string ClientName { get; set; }
        public string ClientAddress1 { get; set; }
        public string ClientAddress2 { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress1 { get; set; }
        public string VendorAddress2 { get; set; }
        public bool? RoundNotePre { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
