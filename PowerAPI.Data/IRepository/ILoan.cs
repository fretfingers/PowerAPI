using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.IRepository
{
    public interface ILoan : IAccount
    {
        Task<IEnumerable<PayrollHrpayrollLoanDetail>> GetAll(ApiToken token);
        Task<IEnumerable<PayrollHrpayrollLoanType>> GetLoanTypeByEmployee(string Id, ApiToken token);
        Task<IEnumerable<PayrollHrpayrollLoanDetail>> GetByEmployee(string Id, string Mode, ApiToken token);
        Task<PayrollHrpayrollLoanDetail> GetById(string Id, string loanType, ApiToken token);

        Task<StatusMessage> Add(PayrollHrpayrollLoanDetail loan, string Mode, ApiToken token);
        Task<StatusMessage> Recalc(PayrollHrpayrollLoanDetail loan, ApiToken token);
        Task<StatusMessage> Delete(PayrollHrpayrollLoanDetail loan, ApiToken token);
        Task<StatusMessage> Update(PayrollHrpayrollLoanDetail loan, ApiToken token);
        Task<StatusMessage> Approve(LoanAppModel loanAppModel, ApiToken token);
    }
}
