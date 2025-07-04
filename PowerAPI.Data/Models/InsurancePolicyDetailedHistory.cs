﻿using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsurancePolicyDetailedHistory
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string PolicyBrokerId { get; set; }
        public string PolicyLocationId { get; set; }
        public string PolicyDivisionId { get; set; }
        public string PolicyDepartmentId { get; set; }
        public string ProjectTypeId { get; set; }
        public string ProjectId { get; set; }
        public string UnitId { get; set; }
        public string ActualPolicyBrokerId { get; set; }
        public string PolicyUnderwriterId { get; set; }
        public string PackagePololicyId { get; set; }
        public string EndorsementId { get; set; }
        public string BusinessClassId { get; set; }
        public string RiskTypeId { get; set; }
        public string CustomerId { get; set; }
        public string VendorId { get; set; }
        public string BrokerId { get; set; }
        public string PolicyDescription { get; set; }
        public bool? PolicyCoInsured { get; set; }
        public bool? PolicyCoBroked { get; set; }
        public double? SumInsured { get; set; }
        public double? PremiumAmount { get; set; }
        public double? BasicPremium { get; set; }
        public bool? Commissionable { get; set; }
        public double? BrokerCommission { get; set; }
        public string MasterPolicyDocumentName { get; set; }
        public DateTime? PolicyStartDate { get; set; }
        public DateTime? PolicyEndDate { get; set; }
        public DateTime? RenewalDate { get; set; }
        public double? InsuranceRate1 { get; set; }
        public double? InsuranceRate2 { get; set; }
        public double? InsuranceRate3 { get; set; }
        public double? InsuranceRate4 { get; set; }
        public double? InsuranceRate5 { get; set; }
        public double? InsuranceRate6 { get; set; }
        public double? InsuranceRate7 { get; set; }
        public double? InsuranceRate8 { get; set; }
        public double? InsuranceRate9 { get; set; }
        public double? InsuranceRate10 { get; set; }
        public double? Discount1 { get; set; }
        public double? Discount2 { get; set; }
        public double? Discount3 { get; set; }
        public double? Discount4 { get; set; }
        public double? Discount5 { get; set; }
        public double? Discount6 { get; set; }
        public double? Discount7 { get; set; }
        public double? Discount8 { get; set; }
        public double? Discount9 { get; set; }
        public double? Discount10 { get; set; }
        public double? BuyBack1 { get; set; }
        public double? BuyBack2 { get; set; }
        public double? BuyBack3 { get; set; }
        public double? BuyBack4 { get; set; }
        public double? BuyBack5 { get; set; }
        public double? BuyBack6 { get; set; }
        public double? BuyBack7 { get; set; }
        public double? BuyBack8 { get; set; }
        public double? BuyBack9 { get; set; }
        public double? BuyBack10 { get; set; }
        public string EmployeeId { get; set; }
        public string InsurancePremiumMethodsId { get; set; }
        public string ContractTypeId { get; set; }
        public string IncomeTypeId { get; set; }
        public string PremiumTypeId { get; set; }
        public string PaymentModeId { get; set; }
        public double? MinimumPayable { get; set; }
        public double? TotalUnderApp { get; set; }
        public double? BalanceApp { get; set; }
        public bool? Cleared { get; set; }
        public bool? Approved { get; set; }
        public bool? Void { get; set; }
        public DateTime? EnterDate { get; set; }
        public bool? MotorFleet { get; set; }
        public bool? MarineOpen { get; set; }
        public bool? Gpafleet { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public decimal? NoOfEmployee { get; set; }
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
        public string BrokingSlipId { get; set; }
        public bool? BrokingSlipUsed { get; set; }
        public string CustomerName { get; set; }
        public string PolicyPageNumbering { get; set; }
        public bool? InActive { get; set; }
        public int TransactionId { get; set; }
        public DateTime? LastRenewedDate { get; set; }
        public string LastRenewedBy { get; set; }
    }
}
