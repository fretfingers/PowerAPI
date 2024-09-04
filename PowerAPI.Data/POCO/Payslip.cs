using System;
using System.Collections.Generic;
using System.Text;

namespace PowerAPI.Data.POCO
{
    public class Payslip
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string EmployeeId { get; set; }
        public double GrossPay { get; set; }
        public double NETEarnings { get; set; }
        public double TotalDeductions { get; set; }
        public string CurrencyID { get; set; }
        public double CurrencyExchangeRate { get; set; }
        public List<Earnings> Earnings { get; set; }
        public List<Deductions> Deductions { get; set; }
        public DateTime Period { get; set; }
    }

    public class Earnings
    {
        public string PayTypeId { get; set; }
        public string AttrId { get; set; }
        public double? Amount { get; set; }
        public double? PayTypeBalance { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string PayTypeDescription { get; set; }
        public bool? Taxable { get; set; }
        public bool? ActiveYn { get; set; }
        public double? ExactAmount { get; set; }
        public bool? OnPayroll { get; set; }
        public double? Rate { get; set; }
        public double? Units { get; set; }
        public string Glaccount { get; set; }
        public string GlaccountEmployer { get; set; }
        public string GlaccountEmployerExp { get; set; }
        public string BranchCode { get; set; }
        public string CurrencyId { get; set; }
        public double? CurrencyExchangeRate { get; set; }
    }

    public class Deductions
    {
        public string PayTypeId { get; set; }
        public string AttrId { get; set; }
        public double? Amount { get; set; }
        public double? PayTypeBalance { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string PayTypeDescription { get; set; }
        public bool? Taxable { get; set; }
        public bool? ActiveYn { get; set; }
        public double? ExactAmount { get; set; }
        public bool? OnPayroll { get; set; }
        public double? Rate { get; set; }
        public double? Units { get; set; }
        public string Glaccount { get; set; }
        public string GlaccountEmployer { get; set; }
        public string GlaccountEmployerExp { get; set; }
        public string BranchCode { get; set; }
        public string CurrencyId { get; set; }
        public double? CurrencyExchangeRate { get; set; }
    }
}
