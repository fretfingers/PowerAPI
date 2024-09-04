using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.IRepository
{
    public interface ILeave : IAccount
    {
        Task<IEnumerable<PayrollHrpayrollLeaveDetail>> GetAll(ApiToken token);
        Task<IEnumerable<PayrollHrpayrollLeaveType>> GetLeaveTypeByEmployee(string Id, ApiToken token);
        Task<IEnumerable<PayrollHrpayrollLeaveDetail>> GetByEmployee(string Id, string Mode, ApiToken token);
        Task<PayrollHrpayrollLeaveDetail> GetById(string Id, string leaveType, DateTime startDate, DateTime endDate, ApiToken token);

        Task<StatusMessage> AddLeave(PayrollHrpayrollLeaveDetail leave, string Mode, ApiToken token);
        Task<StatusMessage> Delete(PayrollHrpayrollLeaveDetail leave, ApiToken token);
        Task<StatusMessage> Update(PayrollHrpayrollLeaveDetail leave, ApiToken token);
        Task<StatusMessage> Approve(LeaveAppModel leaveAppModel, ApiToken token);

    }
}
