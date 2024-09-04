using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsurancePolicyMotorfleet
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string RegistrationId { get; set; }
        public string PolicyBrokerId { get; set; }
        public string ActualPolicyBrokerId { get; set; }
        public string ProjectTypeId { get; set; }
        public string ProjectId { get; set; }
        public string PolicyUnderwriterId { get; set; }
        public string CustomerId { get; set; }
        public string VendorId { get; set; }
        public string BusinessClassId { get; set; }
        public string RiskTypeId { get; set; }
        public string ChasisId { get; set; }
        public string EngineeId { get; set; }
        public double? SumInsured { get; set; }
        public double? PremiumAmount { get; set; }
        public double? ActualPremiumAmount { get; set; }
        public double? PremiumRate { get; set; }
        public double? Discount1 { get; set; }
        public double? BuyBack1 { get; set; }
        public double? Srccrate { get; set; }
        public double? Tppdrate { get; set; }
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
        public string OwnersName { get; set; }
        public string OwnersAddress { get; set; }
        public string Make { get; set; }
        public string Uses { get; set; }
        public string MotorCertificateNo { get; set; }
        public double? BrokerComRate { get; set; }
        public string InsurancePremiumMethodsId { get; set; }
        public string EmployeeId { get; set; }
        public string ContractTypeId { get; set; }
        public string IncomeTypeId { get; set; }
        public string MinimumPayable { get; set; }
        public bool? NoteGenerate { get; set; }
        public DateTime? EnteredDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public bool? SelectForInvoice { get; set; }
        public bool? SelectForDelete { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}
