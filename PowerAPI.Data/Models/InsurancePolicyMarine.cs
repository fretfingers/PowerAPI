using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsurancePolicyMarine
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string CertificateNumber { get; set; }
        public string PolicyBrokerId { get; set; }
        public string ActualPolicyBrokerId { get; set; }
        public string ProjectTypeId { get; set; }
        public string ProjectId { get; set; }
        public string PolicyUnderwriterId { get; set; }
        public string CustomerId { get; set; }
        public string VendorId { get; set; }
        public string BusinessClassId { get; set; }
        public string RiskTypeId { get; set; }
        public string DescriptionGoods { get; set; }
        public string Conditions { get; set; }
        public string Conveyance { get; set; }
        public string Terms { get; set; }
        public string LocationFrom { get; set; }
        public string LocationMarTo { get; set; }
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
        public string ExaminedBy { get; set; }
        public DateTime? ExaminedDate { get; set; }
        public string MarksandNos { get; set; }
        public string CurrencyId { get; set; }
        public double? CurrencyExchangeRate { get; set; }
        public double? BottomLimit { get; set; }
        public string CertificateStatus { get; set; }
        public short? TwoRate { get; set; }
        public double? RateOne { get; set; }
        public double? ValueOne { get; set; }
        public double? PremiumAmount { get; set; }
        public double? ActualPremiumAmount { get; set; }
        public double? PremiumRate { get; set; }
        public DateTime? PolicyStartDate { get; set; }
        public DateTime? PolicyEndDate { get; set; }
        public DateTime? JoinDate { get; set; }
        public bool? NoteGenerated { get; set; }
        public bool? Active { get; set; }
        public double? ReturnPremium { get; set; }
        public string NoteNumber { get; set; }
        public string NoteType { get; set; }
        public string EndorsementNo { get; set; }
        public DateTime? RenewalDate { get; set; }
        public double? BrokerComRate { get; set; }
        public string InsurancePremiumMethodsId { get; set; }
        public string EmployeeId { get; set; }
        public string ContractTypeId { get; set; }
        public string IncomeTypeId { get; set; }
        public string MinimumPayable { get; set; }
        public bool? NoteGenerate { get; set; }
        public DateTime? EnteredDate { get; set; }
        public bool? SelectForInvoice { get; set; }
        public bool? SelectForDelete { get; set; }
        public DateTime? DeleteDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
    }
}
