using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using PowerAPI.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.IRepository
{
    public interface IAppraisal : IAccount
    {
        Task<IEnumerable<Appraisal>> GetAll(ApiToken token);
        Task<IEnumerable<Appraisal>> GetByEmployee(string Id, string Mode, ApiToken token);
        Task<Appraisal> GetById(string Id, string Period, ApiToken token);
        Task<Appraisal> LoadAppraisalScoreCard(string Id, ApiToken token);

        Task<StatusMessage> Add(Appraisal appraisal, string Mode, ApiToken token);
        Task<StatusMessage> AddDetail(List<PayrollHrpayrollAppraisalDetail> appraisalDetails, ApiToken token);
        Task<StatusMessage> AddEmployeeDetail(List<PayrollHrpayrollEmployeeAppraisalDetail> appraisalEmployeeDetails, ApiToken token);
        Task<StatusMessage> AddOthers(List<PayrollHrpayrollAppraisalOthers> appraisalOthers, ApiToken token);
        Task<StatusMessage> AddComments(List<PayrollHrpayrollAppraisalComments> appraisalComments, ApiToken token);
        Task<StatusMessage> AddQuestionnaire(List<PayrollHrpayrollAppraisalQuestionnaire> appraisalQuestionnaires, ApiToken token);
        Task<StatusMessage> AddTraining(List<PayrollHrpayrollTrainingDetail> appraisalTraining, ApiToken token);
        Task<StatusMessage> Delete(Appraisal appraisal, ApiToken token);
        Task<StatusMessage> Update(Appraisal appraisal, ApiToken token);
        Task<StatusMessage> Approve(AppraisalAppModel appraisalAppModel, ApiToken token);
        Task<StatusMessage> Process(AppraisalAppModel appraisalAppModel, ApiToken token);
    }
}
