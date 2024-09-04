using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.IRepository
{
    public interface IReports : IAccount
    {
        Task<IEnumerable<PayrollHrpayrollYearPayFile>> GetPayAnalysis(DateTime Period, ApiToken token);
        Task<Payslip> GetPayslip(string Id, DateTime Period, ApiToken token);
        Task<IEnumerable<PayrollHrpayrollYearPayFile>> GetPayslipRange(string Id, DateTime PeriodFrom, DateTime PeriodTo, ApiToken token);
        Task<IEnumerable<PayrollHrpayrollYearPayFile>> GetPensionRpt(string Id, DateTime Period, ApiToken token);
        Task<IEnumerable<PayrollHrpayrollYearPayFile>> GetPensionRangeRpt(string Id, DateTime PeriodFrom, DateTime PeriodTo, ApiToken token);
        Task<IEnumerable<PayrollHrpayrollYearPayFile>> GetTaxRpt(string Id, DateTime Period, ApiToken token);
        Task<IEnumerable<PayrollHrpayrollYearPayFile>> GetTaxRangeRpt(string Id, DateTime PeriodFrom, DateTime PeriodTo, ApiToken token);
        Task<byte[]> GetOrderReportById (string Id, ApiToken token);
        Task<byte[]> GetRmaReportById(string id, ApiToken tokenObj);
        Task<byte[]> GetSalesReceiptRollById(string id, ApiToken tokenObj);
    }
}
