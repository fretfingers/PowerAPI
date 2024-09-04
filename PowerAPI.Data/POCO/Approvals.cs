using PowerAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerAPI.Data.POCO
{
    public class Approvals
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string EmployeeId { get; set; }
        public Appraisals Appraisals { get; set; }
        public Leaves Leaves { get; set; }
        public Loans Loans { get; set; }
        public Requisitions Requisitions { get; set; }
    }

    public class Appraisals
    {
        public string Icon { get; set; }
        public string Description { get; set; }
        public int totalCount { get; set; }
        public List<PayrollHrpayrollAppraisalHeader> data { get; set; }
    }

    public class Leaves
    {
        public string Icon { get; set; }
        public string Description { get; set; }
        public int totalCount { get; set; }
        public List<PayrollHrpayrollLeaveDetail> data { get; set; }
    }

    public class Loans
    {
        public string Icon { get; set; }
        public string Description { get; set; }
        public int totalCount { get; set; }
        public List<PayrollHrpayrollLoanDetail> data { get; set; }
    }

    public class Requisitions
    {
        public string Icon { get; set; }
        public string Description { get; set; }
        public int totalCount { get; set; }
        public List<RequisitionsHeader> data { get; set; }
    }

    public class BaseImageUrl
    {
        public string imgURL { get; set; }
    }


    //POCO for approval processing

    public class AppraisalAppModel
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string AppraisalID { get; set; }
        public string PeriodID { get; set; }
        public string ProcessBy { get; set; }
    }

    public class LoanAppModel
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string LoanType { get; set; }
        public string EmployeeId { get; set; }
        public string ProcessBy { get; set; }
    }

    public class LeaveAppModel
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string LeaveType { get; set; }
        public string EmployeeId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string ProcessBy { get; set; }
    }

    public class RequisitionAppModel
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string RequisitionID { get; set; }
        public string ProcessBy { get; set; }
    }


    public class PasswordModel
    {
        public string Username { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ResetPasswordModel
    {
        public string Username { get; set; }
        public string OTP { get; set; }
        public string Password { get; set; }
    }



}
