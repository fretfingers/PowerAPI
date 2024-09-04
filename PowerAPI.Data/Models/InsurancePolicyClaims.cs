using System;
using System.Collections.Generic;

namespace PowerAPI.Data.Models
{
    public partial class InsurancePolicyClaims
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string BrokerClaimId { get; set; }
        public string PolicyBrokerId { get; set; }
        public string ActualBrokerClaimId { get; set; }
        public string ActualPolicyBrokerId { get; set; }
        public int TransactionId { get; set; }
        public string UnderwriterClaimId { get; set; }
        public string BusinessClassId { get; set; }
        public string RiskTypeId { get; set; }
        public string CustomerId { get; set; }
        public string VendorId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? AccidentDate { get; set; }
        public string AccidentDetails { get; set; }
        public bool? ThirdPartyInvolved { get; set; }
        public string ThirdPartyClaimNo { get; set; }
        public DateTime? CustomerReportDate { get; set; }
        public DateTime? BrokerUnderwriterDate { get; set; }
        public DateTime? AdjusterAppointedDate { get; set; }
        public DateTime? AdjusterReportDate { get; set; }
        public DateTime? AdjusterReportApproveDate { get; set; }
        public string Dvnumber { get; set; }
        public DateTime? DvreceiveBrokerDate { get; set; }
        public bool? DvReceivedBroker { get; set; }
        public DateTime? DvreceiveCustomerDate { get; set; }
        public bool? DvreceiveCustomer { get; set; }
        public DateTime? DvreturnCustomerDate { get; set; }
        public bool? DvreturnedCustomer { get; set; }
        public DateTime? DvbackVendorDate { get; set; }
        public bool? DvbackVendor { get; set; }
        public DateTime? DvpaymentReceiveCustomer { get; set; }
        public bool? DvcustomerPayments { get; set; }
        public double? CustomerEstimate { get; set; }
        public double? AdjusterEstimate { get; set; }
        public double? Dvamount { get; set; }
        public double? TotalReceived { get; set; }
        public string ClaimsStatus { get; set; }
        public string Settlementdelayby { get; set; }
        public string ReasonforDelay { get; set; }
        public bool? ClaimsRepudated { get; set; }
        public string RepudateReason { get; set; }
        public DateTime? RepudateDate { get; set; }
        public string EnteredBy { get; set; }
        public string EnteredBy2 { get; set; }
        public int? DocumentsRequired { get; set; }
        public int? DocumentsDelivered { get; set; }
        public int? DocumentsOutstanding { get; set; }
        public string DocumentStutus { get; set; }
        public string Dvto { get; set; }
        public string Dvattn { get; set; }
        public DateTime? DvLetterDate { get; set; }
        public string FirSignPost { get; set; }
        public string SecSignPost { get; set; }
        public bool? ReportSubmitted { get; set; }
        public string NotificationName { get; set; }
        public bool? Individual { get; set; }
        public string InspectionSite { get; set; }
        public string SecondSignatory { get; set; }
        public string ProjectTypeId { get; set; }
        public string ProjectId { get; set; }
        public DateTime? RegisteredDate { get; set; }
        public bool? Closed { get; set; }
        public string ClosedBy { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string BrokerId { get; set; }
        public string ClaimCloseTypeId { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool? Cleared { get; set; }
        public string ClearedBy { get; set; }
        public DateTime? ClearedDate { get; set; }
        public bool? Approved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public bool? Void { get; set; }
        public string VoidBy { get; set; }
        public DateTime? VoidDate { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string CustomerName { get; set; }
        public string UnderwriterPolicyId { get; set; }
        public string InsuranceCategoryId { get; set; }
        public string ClaimsDocUpload { get; set; }
    }
}
