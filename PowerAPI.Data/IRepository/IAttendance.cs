using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.IRepository
{
    public interface IAttendance : IAccount
    {
        Task<IEnumerable<PayrollHrpayrollAttDetail>> GetAttendance(ApiToken token);
        Task<IEnumerable<PayrollHrpayrollAttDetail>> GetByEmployee(string Id, string Mode, ApiToken token);
        Task<PayrollHrpayrollAttDetail> GetById(string Id, DateTime AttendanceDate, ApiToken token);

        //Task<IEnumerable<LatenessReportDetails>> GetLatenessReportDetails(ApiToken token);
        Task<IEnumerable<LatenessReportSummary>> GetLatenessReportSummary(DateTime PeriodFrom, DateTime PeriodTo, ApiToken token);
        Task<IEnumerable<AbsenteeismReportDetail>> GetAbsenteeismReportDetail(ApiToken token);
        Task<IEnumerable<AbsenteeismReportSummary>> GetAbsenteeismReportSummary(DateTime PeriodFrom, DateTime PeriodTo, ApiToken tokenObj);
        Task<IEnumerable<DeductionSummary>> GetDeductionReport(DateTime PeriodFrom, DateTime PeriodTo, ApiToken tokenObj);
        Task<IEnumerable<DailyAttendance>> GetDailyAttendanceSummary(DateTime PeriodFrom, DateTime PeriodTo, ApiToken token);
        Task<IEnumerable<MonthlyAttendance>> GetMonthlyAttendanceSummary(DateTime PeriodFrom, DateTime PeriodTo, ApiToken token);

        Task<StatusMessage> Add(PayrollHrpayrollAttDetail attendance, string Mode, ApiToken token);
        Task<StatusMessage> Delete(PayrollHrpayrollAttDetail attendance, ApiToken token);
        Task<IEnumerable<LatenessReportDetails>> GetLatenessReportDetails(DateTime periodFrom, DateTime periodTo, ApiToken tokenObj);
        //Task<StatusMessage> Update(PayrollHrpayrollAttDetail attendance, ApiToken token);
        //Task<StatusMessage> Process(string Id, string type, ApiToken token);

    }
}
