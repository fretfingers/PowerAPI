using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using PowerAPI.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.IRepository
{
    public interface IEmployee : IAccount
    {
        Task<IEnumerable<PayEmployees>> GetAll(string Mode, ApiToken token);
        Task<IEnumerable<PayrollEmployees>> GetByDept(string Id, string Mode, ApiToken token);
        Task<IEnumerable<PayrollEmployees>> GetById(string Id, ApiToken token);
        Task<Approvals> GetApprovalsByEmployee(string Id, ApiToken token);
        Task<IEnumerable<PayrollHrpayrollJobClassHeader>> GetJobClass(ApiToken token);
        Task<IEnumerable<PayrollHrpayrollQualificationType>> GetQualificationType(ApiToken token);


        Task<StatusMessage> Add(PayrollEmployees employee, string Mode, ApiToken token);
        Task<StatusMessage> Delete(PayrollEmployees employee, ApiToken token);
        Task<StatusMessage> Update(PayrollEmployees employee, ApiToken token);
        Task<StatusMessage> Process(string Id, string type, ApiToken token);

        Task<bool> Login(string Username, string Password, ApiToken token);
        Task<StatusMessage> ChangePwd(PasswordModel changePasswordModel, ApiToken token);
        Task<StatusMessage> ResetPwd(PasswordModel resetPasswordModel, ApiToken token);
        Task <StatusMessage> AddEmployees(PayEmployees employeePolicy, ApiToken tokenObj);
        Task<StatusMessage> DeletePayEmployees(string employeeId, ApiToken tokenObj);
    }
}
