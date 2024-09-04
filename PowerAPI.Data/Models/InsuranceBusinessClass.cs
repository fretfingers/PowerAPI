using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsuranceBusinessClass
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string BusinessClassId { get; set; }
        public string BusinessClassName { get; set; }
        public double? BrokersComm { get; set; }
        public string DetailDescription { get; set; }
        public string GlsalesAccount { get; set; }
        public string GlcommisionAccount { get; set; }
        public string Glvataccount { get; set; }
        public string Glcogaccount { get; set; }
        public double? ExistingBusiness { get; set; }
        public double? NewBusiness { get; set; }
        public bool? SubAgency { get; set; }
        public bool? Active { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public bool? ChargeVat { get; set; }
        public bool? DeclarativeBusiness { get; set; }
        public string InsuranceCategoryId { get; set; }
        public double? InsuranceRate1 { get; set; }
        public double? InsuranceRate2 { get; set; }
        public double? InsuranceRate3 { get; set; }
        public double? Discount1 { get; set; }
        public double? Discount2 { get; set; }
        public double? Discount3 { get; set; }
        public double? BuyBack1 { get; set; }
        public double? BuyBack2 { get; set; }
        public bool? MotorFleet { get; set; }
        public bool? MarineOpen { get; set; }
        public bool? Gpafleet { get; set; }
        public decimal? Basic { get; set; }
        public decimal? BurglaryExtension { get; set; }
        public decimal? CashInPersonalCustody { get; set; }
        public decimal? CashInSafePremises { get; set; }
        public decimal? CashInTransit { get; set; }
        public decimal? ElectricalMechanicalBdloading { get; set; }
        public decimal? ExcessBuyBack { get; set; }
        public decimal? Flood { get; set; }
        public decimal? FlueGas { get; set; }
        public decimal? Iarloading { get; set; }
        public decimal? LocalMedicalExpenses { get; set; }
        public decimal? MultiplierI { get; set; }
        public decimal? MultiplierIi { get; set; }
        public decimal? NonOccupationalRisksLoading { get; set; }
        public decimal? RothersI { get; set; }
        public decimal? RothersIi { get; set; }
        public decimal? RothersIii { get; set; }
        public decimal? OverseasMedicalExpenses { get; set; }
        public decimal? Perils { get; set; }
        public decimal? Srccommotion { get; set; }
        public decimal? Tarrif { get; set; }
        public decimal? ThirdPartyPd { get; set; }
        public decimal? Deductible { get; set; }
        public decimal? Fea { get; set; }
        public decimal? Fleet { get; set; }
        public decimal? GroupD { get; set; }
        public decimal? Lta { get; set; }
        public decimal? Ncd { get; set; }
        public decimal? DothersI { get; set; }
        public decimal? DothersIi { get; set; }
        public decimal? DothersIii { get; set; }
        public decimal? DothersIv { get; set; }
        public decimal? PackageD { get; set; }
        public decimal? SilentRisk { get; set; }
        public decimal? SpecialD { get; set; }
        public decimal? StockDeclarationD { get; set; }
        public string PolicyBrokerId { get; set; }
        public string NaicomdrAccount { get; set; }
        public string NaicomcrAccount { get; set; }
        public bool? IsRenewable { get; set; }
        public string GlunEarnedAccount { get; set; }
    }
}
