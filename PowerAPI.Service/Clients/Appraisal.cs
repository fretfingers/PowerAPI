using PowerAPI.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Text;
using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using System.Threading.Tasks;
using PowerAPI.Data.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PowerAPI.Service.Helper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace PowerAPI.Service.Clients
{
    public class Appraisal : IAppraisal
    {
        EnterpriseContext _DBContext;

        public Appraisal(EnterpriseContext DBContext)
        {
            _DBContext = DBContext;
        }

        public async Task<StatusMessage> Add(Data.ViewModels.Appraisal appraisal, string Mode, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();
            PayrollHrpayrollAppraisalHeader appraisalHeader = new PayrollHrpayrollAppraisalHeader();
            List<PayrollHrpayrollAppraisalDetail> appraisalDetail = new List<PayrollHrpayrollAppraisalDetail>();
            List<PayrollHrpayrollAppraisalQuestionnaire> appraisalQuestionnaire = new List<PayrollHrpayrollAppraisalQuestionnaire>();
            List<PayrollHrpayrollTrainingDetail> appraisalTraining = new List<PayrollHrpayrollTrainingDetail>();
            List<PayrollHrpayrollAppraisalOthers> appraisalOthers = new List<PayrollHrpayrollAppraisalOthers>();
            List<PayrollHrpayrollAppraisalComments> appraisalComments = new List<PayrollHrpayrollAppraisalComments>();

            try
            {
                if (appraisal != null)
                {
                    var emp = await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                                x.DivisionId == token.DivisionId &&
                                                                x.DepartmentId == token.DepartmentId &&
                                                                x.EmployeeId == appraisal.AppraisalId).FirstOrDefaultAsync();

                    var appraisalPeriod = await _DBContext.PayrollHrpayrollAppraisalPeriod.Where(x => x.CompanyId == token.CompanyId &&
                                                                x.DivisionId == token.DivisionId &&
                                                                x.DepartmentId == token.DepartmentId &&
                                                                x.PeriodId == appraisal.PeriodId &&
                                                                x.Active == true).FirstOrDefaultAsync();

                    if (emp != null && appraisalPeriod != null)
                    {
                        appraisalHeader.CompanyId = token.CompanyId;
                        appraisalHeader.DivisionId = token.DivisionId;
                        appraisalHeader.DepartmentId = token.DepartmentId;
                        appraisalHeader.PeriodId = appraisal.PeriodId;
                        appraisalHeader.AppraiseeId = appraisal.AppraiseeId;

                        appraisalHeader.AppraisalName = appraisal.AppraisalName;
                        appraisalHeader.Department = appraisal.Department;
                        appraisalHeader.GroupId = appraisal.GroupId;
                        appraisalHeader.AppraiseeId = appraisal.AppraiseeId;
                        appraisalHeader.TotalScore = appraisal.TotalScore;
                        appraisalHeader.MaxScore = appraisal.MaxScore;
                        appraisalHeader.Percentage = appraisal.Percentage;
                        appraisalHeader.Remark = appraisal.Remark;
                        appraisalHeader.EnteredBy = appraisal.EnteredBy;
                        appraisalHeader.EnteredDate = appraisal.EnteredDate;
                        appraisalHeader.Cleared = Mode == "Submit" ? true : false;
                        appraisalHeader.Approved = appraisal.Approved;
                        appraisalHeader.ApprovedBy = appraisal.ApprovedBy;
                        appraisalHeader.ApprovedDate = appraisal.ApprovedDate;
                        appraisalHeader.Posted = appraisal.Posted;
                        appraisalHeader.LockedBy = appraisal.LockedBy;
                        appraisalHeader.LockTs = appraisal.LockTs;
                        appraisalHeader.BranchCode = appraisal.BranchCode;
                        appraisalHeader.Reviewed = appraisal.Reviewed;
                        appraisalHeader.Attest = appraisal.Attest;
                        appraisalHeader.IsAttested = appraisal.IsAttested;
                        appraisalHeader.AttestationComment = appraisal.AttestationComment;
                        appraisalHeader.DateAttested = appraisal.DateAttested;
                        appraisalHeader.PostedBy = appraisal.PostedBy;
                        appraisalHeader.PostedDate = appraisal.PostedDate;
                        appraisalHeader.Confirmed = appraisal.Confirmed;
                        appraisalHeader.ConfirmedBy = appraisal.ConfirmedBy;
                        appraisalHeader.ConfirmedDate = appraisal.ConfirmedDate;
                        appraisalHeader.ApprovedByName = appraisal.ApprovedByName;
                        appraisalHeader.SecondLevelApprovedByName = appraisal.SecondLevelApprovedByName;

                        //_DBContext.Entry(appraisalHeader).State = EntityState.Modified;
                        //_DBContext.Entry(appraisal.qualitativeObjective).State = EntityState.Modified;
                        //_DBContext.Entry(appraisal.quantitativeObjective).State = EntityState.Modified;
                        //_DBContext.Entry(appraisal.appraisalComments).State = EntityState.Modified;
                        //_DBContext.Entry(appraisal.Questionnaire).State = EntityState.Modified;

                        _DBContext.SaveChanges();

                        await AddDetail(appraisal.qualitativeObjective, token);
                        await AddEmployeeDetail(appraisal.quantitativeObjective, token);
                        await AddComments(appraisal.appraisalComments, token);
                        await AddOthers(appraisal.appraisalOthers, token);
                        await AddQuestionnaire(appraisal.Questionnaire, token);
                        await AddTraining(appraisal.Training, token);

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";

                        //submit appraisal


                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Employee Does Not Exist";
                    }
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public Task<StatusMessage> Approve(AppraisalAppModel appraisalAppModel, ApiToken token)
        {
            throw new NotImplementedException();
        }

        public Task<StatusMessage> Delete(Data.ViewModels.Appraisal appraisal, ApiToken token)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiToken> GetAccess(string token)
        {
            int days = 0;
            ApiToken apiToken = new ApiToken();

            try
            {
                //get the comp on token
                apiToken = await _DBContext.ApiToken.Where(x => x.Token == token).FirstOrDefaultAsync();

                if (apiToken != null)
                {
                    //get reg info
                    var regInfo = await _DBContext.TblVersion.Where(x => x.CompanyId == apiToken.CompanyId &&
                                                            x.DivisionId == apiToken.DivisionId &&
                                                            x.DepartmentId == apiToken.DepartmentId).FirstOrDefaultAsync();
                    if (regInfo != null)
                    {
                        days = EnterpriseValidator.GetDaysLeft(regInfo.RegCode, regInfo.RegName);
                        apiToken.RegCode = regInfo.RegCode;
                        apiToken.RegCode = regInfo.RegName;
                        apiToken.TotalDays = days;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return apiToken;
        }

        public async Task<IEnumerable<Data.ViewModels.Appraisal>> GetAll(ApiToken token)
        {
            List<Data.ViewModels.Appraisal> appraisals = new List<Data.ViewModels.Appraisal>();
            try
            {
                var appraisal = await _DBContext.PayrollHrpayrollAppraisalHeader.Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId).ToListAsync();

                appraisals = await appr(appraisal, token);

            }
            catch (Exception)
            {

            }

            return appraisals;
        }

        private async Task<List<Data.ViewModels.Appraisal>> appr(List<PayrollHrpayrollAppraisalHeader> appraisal, ApiToken token)
        {
            List<Data.ViewModels.Appraisal> appraisals = new List<Data.ViewModels.Appraisal>();
            try
            {
                if (appraisal != null)
                {
                    foreach (PayrollHrpayrollAppraisalHeader appr in appraisal)
                    {
                        Data.ViewModels.Appraisal appraisalObj = new Data.ViewModels.Appraisal();

                        appraisalObj.CompanyId = appr.CompanyId;
                        appraisalObj.DivisionId = appr.DivisionId;
                        appraisalObj.DepartmentId = appr.DepartmentId;
                        appraisalObj.PeriodId = appr.PeriodId;
                        appraisalObj.AppraisalId = appr.AppraisalId;
                        appraisalObj.AppraisalName = appr.AppraisalName;
                        appraisalObj.Department = appr.Department;
                        appraisalObj.GroupId = appr.GroupId;
                        appraisalObj.AppraiseeId = appr.AppraiseeId;
                        appraisalObj.TotalScore = appr.TotalScore;
                        appraisalObj.MaxScore = appr.MaxScore;
                        appraisalObj.Percentage = appr.Percentage;
                        appraisalObj.Remark = appr.Remark;
                        appraisalObj.EnteredBy = appr.EnteredBy;
                        appraisalObj.EnteredDate = appr.EnteredDate;
                        appraisalObj.Cleared = appr.Cleared;
                        appraisalObj.Approved = appr.Approved;
                        appraisalObj.ApprovedBy = appr.ApprovedBy;
                        appraisalObj.ApprovedDate = appr.ApprovedDate;
                        appraisalObj.Posted = appr.Posted;
                        appraisalObj.LockedBy = appr.LockedBy;
                        appraisalObj.LockTs = appr.LockTs;
                        appraisalObj.BranchCode = appr.BranchCode;
                        appraisalObj.Reviewed = appr.Reviewed;
                        appraisalObj.Attest = appr.Attest;
                        appraisalObj.IsAttested = appr.IsAttested;
                        appraisalObj.AttestationComment = appr.AttestationComment;
                        appraisalObj.DateAttested = appr.DateAttested;
                        appraisalObj.PostedBy = appr.PostedBy;
                        appraisalObj.PostedDate = appr.PostedDate;
                        appraisalObj.Confirmed = appr.Confirmed;
                        appraisalObj.ConfirmedBy = appr.ConfirmedBy;
                        appraisalObj.ConfirmedDate = appr.ConfirmedDate;
                        appraisalObj.ApprovedByName = appr.ApprovedByName;
                        appraisalObj.SecondLevelApprovedByName = appr.SecondLevelApprovedByName;


                        appraisalObj.qualitativeObjective = await _DBContext.PayrollHrpayrollAppraisalDetail.Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.PeriodId == appr.PeriodId &&
                                                           x.AppraisalId == appr.AppraisalId).ToListAsync();

                        appraisalObj.quantitativeObjective = await _DBContext.PayrollHrpayrollEmployeeAppraisalDetail.Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.PeriodId == appr.PeriodId &&
                                                           x.AppraisalId == appr.AppraisalId).ToListAsync();


                        appraisalObj.Questionnaire = await _DBContext.PayrollHrpayrollAppraisalQuestionnaire.Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.PeriodId == appr.PeriodId &&
                                                           x.AppraisalId == appr.AppraisalId).ToListAsync();

                        appraisalObj.Training = await _DBContext.PayrollHrpayrollTrainingDetail.Where(x => x.CompanyId == token.CompanyId &&
                                                          x.DivisionId == token.DivisionId &&
                                                          x.DepartmentId == token.DepartmentId &&
                                                          x.PeriodId == appr.PeriodId &&
                                                          x.EmployeeId == appr.AppraisalId).ToListAsync();


                        appraisalObj.appraisalOthers = await _DBContext.PayrollHrpayrollAppraisalOthers.Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.PeriodId == appr.PeriodId &&
                                                           x.AppraisalId == appr.AppraisalId).ToListAsync();


                        appraisalObj.appraisalComments = await _DBContext.PayrollHrpayrollAppraisalComments.Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.PeriodId == appr.PeriodId &&
                                                           x.AppraisalId == appr.AppraisalId).ToListAsync();

                        appraisals.Add(appraisalObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return appraisals;
        }

        private async Task<StatusMessage> addAppraisalDetail(List<PayrollHrpayrollAppraisalDetail> appraisalDetail, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            //try
            //{
            //    if (appraisalDetail != null)
            //    {
            //        foreach (PayrollHrpayrollAppraisalHeader appr in appraisalDetail)
            //        {
            //            Data.ViewModels.Appraisal appraisalObj = new Data.ViewModels.Appraisal();

            //            appraisalObj.CompanyId = appr.CompanyId;
            //            appraisalObj.DivisionId = appr.DivisionId;
            //            appraisalObj.DepartmentId = appr.DepartmentId;
            //            appraisalObj.PeriodId = appr.PeriodId;
            //            appraisalObj.AppraisalId = appr.AppraisalId;
            //            appraisalObj.AppraisalName = appr.AppraisalName;
            //            appraisalObj.Department = appr.Department;
            //            appraisalObj.GroupId = appr.GroupId;
            //            appraisalObj.AppraiseeId = appr.AppraiseeId;
            //            appraisalObj.TotalScore = appr.TotalScore;
            //            appraisalObj.MaxScore = appr.MaxScore;
            //            appraisalObj.Percentage = appr.Percentage;
            //            appraisalObj.Remark = appr.Remark;
            //            appraisalObj.EnteredBy = appr.EnteredBy;
            //            appraisalObj.EnteredDate = appr.EnteredDate;
            //            appraisalObj.Cleared = appr.Cleared;
            //            appraisalObj.Approved = appr.Approved;
            //            appraisalObj.ApprovedBy = appr.ApprovedBy;
            //            appraisalObj.ApprovedDate = appr.ApprovedDate;
            //            appraisalObj.Posted = appr.Posted;
            //            appraisalObj.LockedBy = appr.LockedBy;
            //            appraisalObj.LockTs = appr.LockTs;
            //            appraisalObj.BranchCode = appr.BranchCode;
            //            appraisalObj.Reviewed = appr.Reviewed;
            //            appraisalObj.Attest = appr.Attest;
            //            appraisalObj.IsAttested = appr.IsAttested;
            //            appraisalObj.AttestationComment = appr.AttestationComment;
            //            appraisalObj.DateAttested = appr.DateAttested;
            //            appraisalObj.PostedBy = appr.PostedBy;
            //            appraisalObj.PostedDate = appr.PostedDate;
            //            appraisalObj.Confirmed = appr.Confirmed;
            //            appraisalObj.ConfirmedBy = appr.ConfirmedBy;
            //            appraisalObj.ConfirmedDate = appr.ConfirmedDate;
            //            appraisalObj.ApprovedByName = appr.ApprovedByName;
            //            appraisalObj.SecondLevelApprovedByName = appr.SecondLevelApprovedByName;


            //            appraisalObj.appraisalDetail = await _DBContext.PayrollHrpayrollAppraisalDetail.Where(x => x.CompanyId == token.CompanyId &&
            //                                               x.DivisionId == token.DivisionId &&
            //                                               x.DepartmentId == token.DepartmentId &&
            //                                               x.PeriodId == appr.PeriodId &&
            //                                               x.AppraisalId == appr.AppraisalId).ToListAsync();


            //            appraisalObj.appraisalQuestionnaire = await _DBContext.PayrollHrpayrollAppraisalQuestionnaire.Where(x => x.CompanyId == token.CompanyId &&
            //                                               x.DivisionId == token.DivisionId &&
            //                                               x.DepartmentId == token.DepartmentId &&
            //                                               x.PeriodId == appr.PeriodId &&
            //                                               x.AppraisalId == appr.AppraisalId).ToListAsync();


            //            appraisalObj.appraisalOthers = await _DBContext.PayrollHrpayrollAppraisalOthers.Where(x => x.CompanyId == token.CompanyId &&
            //                                               x.DivisionId == token.DivisionId &&
            //                                               x.DepartmentId == token.DepartmentId &&
            //                                               x.PeriodId == appr.PeriodId &&
            //                                               x.AppraisalId == appr.AppraisalId).ToListAsync();


            //            appraisalObj.appraisalComments = await _DBContext.PayrollHrpayrollAppraisalComments.Where(x => x.CompanyId == token.CompanyId &&
            //                                               x.DivisionId == token.DivisionId &&
            //                                               x.DepartmentId == token.DepartmentId &&
            //                                               x.PeriodId == appr.PeriodId &&
            //                                               x.AppraisalId == appr.AppraisalId).ToListAsync();

            //            appraisals.Add(appraisalObj);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{

            //}

            return statusMessage;


        }

        public async Task<IEnumerable<Data.ViewModels.Appraisal>> GetByEmployee(string Id, string Mode, ApiToken token)
        {
            List<Data.ViewModels.Appraisal> appraisals = new List<Data.ViewModels.Appraisal>();

            try
            {
                if (Mode == "Current")
                {
                    //get active appraisal period

                    var appraisalPeriod = await _DBContext.PayrollHrpayrollAppraisalPeriod.Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.Active == true).FirstOrDefaultAsync();

                    var appraisal = await _DBContext.PayrollHrpayrollAppraisalHeader.Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.AppraisalId == Id &&
                                                            x.PeriodId == appraisalPeriod.PeriodId &&
                                                            x.Approved == false).ToListAsync();

                    appraisals = await appr(appraisal, token);
                }
                else if (Mode == "History")
                {
                    var appraisal = await _DBContext.PayrollHrpayrollAppraisalHeader.Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.AppraisalId == Id &&
                                                            x.Approved == true).ToListAsync();

                    appraisals = await appr(appraisal, token);
                }
                else
                {
                    var appraisal = await _DBContext.PayrollHrpayrollAppraisalHeader.Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.AppraisalId == Id &&
                                                            x.Approved == false).ToListAsync();

                    appraisals = await appr(appraisal, token);
                }
            }
            catch (Exception ex)
            {

            }

            return appraisals;
        }

        public async Task<Data.ViewModels.Appraisal> GetById(string Id, string Period, ApiToken token)
        {
            List<Data.ViewModels.Appraisal> appraisals = new List<Data.ViewModels.Appraisal>();
            Data.ViewModels.Appraisal appraisal = new Data.ViewModels.Appraisal();

            try
            {
                var apprs = await _DBContext.PayrollHrpayrollAppraisalHeader.Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.AppraisalId == Id &&
                                                           x.PeriodId == Period).ToListAsync();

                appraisals = await appr(apprs, token);

                appraisal = appraisals.FirstOrDefault();
            }
            catch (Exception ex)
            {

            }

            return appraisal;
        }

        public Task<StatusMessage> Process(AppraisalAppModel appraisalAppModel, ApiToken token)
        {
            throw new NotImplementedException();
        }

        public Task<StatusMessage> Update(Data.ViewModels.Appraisal appraisal, ApiToken token)
        {
            throw new NotImplementedException();
        }

        public async Task<Data.ViewModels.Appraisal> LoadAppraisalScoreCard(string Id, ApiToken token)
        {
            List<Data.ViewModels.Appraisal> appraisals = new List<Data.ViewModels.Appraisal>();
            Data.ViewModels.Appraisal appraisal = new Data.ViewModels.Appraisal();

            try
            {

                 //get active period
                 var appraisalPeriod = await _DBContext.PayrollHrpayrollAppraisalPeriod.Where(x => x.CompanyId == token.CompanyId &&
                                                                x.DivisionId == token.DivisionId &&
                                                                x.DepartmentId == token.DepartmentId &&
                                                                x.Active == true).FirstOrDefaultAsync();

                if (appraisalPeriod == null)
                {
                    var empApp = await _DBContext.PayrollHrpayrollAppraisalHeader.Where(x => x.CompanyId == token.CompanyId &&
                                                                               x.DivisionId == token.DivisionId &&
                                                                               x.DepartmentId == token.DepartmentId &&
                                                                               x.AppraisalId == Id &&
                                                                               x.PeriodId == appraisalPeriod.PeriodId).ToListAsync();

                    if (empApp != null)
                    {
                        var employeeAppraisal = empApp.FirstOrDefault();

                        if(employeeAppraisal.Cleared == false && employeeAppraisal.TotalScore <= 0)
                        {
                            //load appraisal score card
                            await loadScoreCard(Id, appraisalPeriod.PeriodId, token);

                            var apprs = await _DBContext.PayrollHrpayrollAppraisalHeader.Where(x => x.CompanyId == token.CompanyId &&
                                                                       x.DivisionId == token.DivisionId &&
                                                                       x.DepartmentId == token.DepartmentId &&
                                                                       x.AppraisalId == Id &&
                                                                       x.PeriodId == appraisalPeriod.PeriodId &&
                                                                       x.Cleared == false
                                                                       ).ToListAsync();

                            appraisals = await appr(apprs, token);

                            appraisal = appraisals.FirstOrDefault();
                        }
                        else
                        {
                            appraisals = await appr(empApp, token);

                            appraisal = appraisals.FirstOrDefault();
                        }                   
                    }
                    else
                    {
                        //load appraisal score card
                        await loadScoreCard(Id, appraisalPeriod.PeriodId, token);

                        var apprs = await _DBContext.PayrollHrpayrollAppraisalHeader.Where(x => x.CompanyId == token.CompanyId &&
                                                                   x.DivisionId == token.DivisionId &&
                                                                   x.DepartmentId == token.DepartmentId &&
                                                                   x.AppraisalId == Id &&
                                                                   x.PeriodId == appraisalPeriod.PeriodId &&
                                                                   x.Cleared == false
                                                                   ).ToListAsync();

                        appraisals = await appr(apprs, token);

                        appraisal = appraisals.FirstOrDefault();
                    }
                }
                    
            }
            catch (Exception ex)
            {

            }

            return appraisal;
        }

        private async Task<StatusMessage> loadScoreCard(string AppraisalId, string PeriodId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var sCompanyID = new SqlParameter("@CompanyID", token.CompanyId);
                var sDivisionID = new SqlParameter("@DivisionID", token.DivisionId);
                var sDepartmentID = new SqlParameter("@DepartmentID", token.DepartmentId);
                var sAppraisalID = new SqlParameter("@AppraisalID", AppraisalId);
                var sPeriodID = new SqlParameter("@PeriodID", PeriodId);
                var sEnteredBy = new SqlParameter("@EnteredBy", AppraisalId);
                var PostingResult = new SqlParameter("@PostingResult", SqlDbType.NVarChar, 255);
                PostingResult.Direction = ParameterDirection.Output;

                await _DBContext.Database
                          .ExecuteSqlRawAsync("enterprise.PayrollAppraisal_LoadInfoESS @CompanyID, @DivisionID, @DepartmentID, @AppraisalID, @PeriodID, @EnteredBy, @PostingResult Out",
                                    parameters: new[] { sCompanyID, sDivisionID, sDepartmentID, sAppraisalID, sPeriodID, sEnteredBy, PostingResult });

                string result = PostingResult == null ? "" : PostingResult.Value.ToString();

                if (result == "")
                {
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Appraisal Score Card Loaded Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = result;
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> AddDetail(List<PayrollHrpayrollAppraisalDetail> appraisalDetails, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                if (appraisalDetails != null)
                {
                    foreach (PayrollHrpayrollAppraisalDetail appraisalDetail in appraisalDetails)
                    {
                        var emp = await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                                                        x.DivisionId == token.DivisionId &&
                                                                                        x.DepartmentId == token.DepartmentId &&
                                                                                        x.EmployeeId == appraisalDetail.AppraisalId).FirstOrDefaultAsync();

                        var appraisalPeriod = await _DBContext.PayrollHrpayrollAppraisalPeriod.Where(x => x.CompanyId == token.CompanyId &&
                                                                    x.DivisionId == token.DivisionId &&
                                                                    x.DepartmentId == token.DepartmentId &&
                                                                    x.PeriodId == appraisalDetail.PeriodId &&
                                                                    x.Active == true).FirstOrDefaultAsync();

                        var appraisal = await _DBContext.PayrollHrpayrollAppraisalDetail.Where(x => x.CompanyId == token.CompanyId &&
                                                                    x.DivisionId == token.DivisionId &&
                                                                    x.DepartmentId == token.DepartmentId &&
                                                                    x.DepartmentId == appraisalDetail.AppraisalId &&
                                                                    x.PeriodId == appraisalDetail.PeriodId &&
                                                                    x.AppraisalDetailInc == appraisalDetail.AppraisalDetailInc).FirstOrDefaultAsync();


                        if (emp != null)
                        {
                            if (appraisalPeriod != null)
                            {
                                if (appraisal != null)
                                {
                                    appraisal.CompanyId = token.CompanyId;
                                    appraisal.DivisionId = token.DivisionId;
                                    appraisal.DepartmentId = token.DepartmentId;
                                    appraisal.PeriodId = appraisalDetail.PeriodId;
                                    appraisal.AppraisalId = appraisalDetail.AppraisalId;
                                    appraisal.AppraisalDetailInc = appraisalDetail.AppraisalDetailInc;
                                    appraisal.AppraisalType = appraisalDetail.AppraisalType;
                                    appraisal.AppraisalGoal = appraisalDetail.AppraisalGoal;
                                    appraisal.AppraisalKeyResultId = appraisalDetail.AppraisalKeyResultId;
                                    appraisal.AppraisalKeyResultIndicator = appraisalDetail.AppraisalKeyResultIndicator;
                                    appraisal.MaxScore = appraisalDetail.MaxScore;
                                    appraisal.AppraiseeScore = appraisalDetail.AppraiseeScore;
                                    appraisal.AppraisalScore = appraisalDetail.AppraisalScore;
                                    appraisal.Remarks = appraisalDetail.Remarks;
                                    appraisal.BranchCode = appraisalDetail.BranchCode;
                                    appraisal.LockedBy = appraisalDetail.LockedBy;
                                    appraisal.LockTs = appraisalDetail.LockTs;
                                    appraisal.AppraiseePercent = appraisalDetail.AppraiseePercent;
                                    appraisal.ScoreCriteriaId = appraisalDetail.ScoreCriteriaId;
                                    appraisal.ReviewedAppraisalScore = appraisalDetail.ReviewedAppraisalScore;

                                    _DBContext.Entry(appraisal).State = EntityState.Modified;
                                    _DBContext.SaveChanges();

                                    statusMessage.Status = "Success";
                                    statusMessage.Message = "Success";
                                }
                                else
                                {
                                    appraisalDetail.CompanyId = token.CompanyId;
                                    appraisalDetail.DivisionId = token.DivisionId;
                                    appraisalDetail.DepartmentId = token.DepartmentId;

                                    _DBContext.Entry(appraisalDetail).State = EntityState.Added;
                                    _DBContext.SaveChanges();

                                    statusMessage.Status = "Success";
                                    statusMessage.Message = "Success";
                                }
                            }
                            else
                            {
                                statusMessage.Status = "Failed";
                                statusMessage.Message = "Invalid Appraisal Period";
                            }
                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "Employee Does Not Exist";
                        }
                    }             
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Appraisal Detail";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> AddOthers(List<PayrollHrpayrollAppraisalOthers> appraisalOthers, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                if (appraisalOthers != null)
                {
                    foreach (PayrollHrpayrollAppraisalOthers appraisalOther in appraisalOthers)
                    {
                        var emp = await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                                                        x.DivisionId == token.DivisionId &&
                                                                                        x.DepartmentId == token.DepartmentId &&
                                                                                        x.EmployeeId == appraisalOther.AppraisalId).FirstOrDefaultAsync();

                        var appraisalPeriod = await _DBContext.PayrollHrpayrollAppraisalPeriod.Where(x => x.CompanyId == token.CompanyId &&
                                                                    x.DivisionId == token.DivisionId &&
                                                                    x.DepartmentId == token.DepartmentId &&
                                                                    x.PeriodId == appraisalOther.PeriodId &&
                                                                    x.Active == true).FirstOrDefaultAsync();

                        var appraisal = await _DBContext.PayrollHrpayrollAppraisalOthers.Where(x => x.CompanyId == token.CompanyId &&
                                                                    x.DivisionId == token.DivisionId &&
                                                                    x.DepartmentId == token.DepartmentId &&
                                                                    x.AppraisalId == appraisalOther.AppraisalId &&
                                                                    x.PeriodId == appraisalOther.PeriodId &&
                                                                    x.CommentLineId == appraisalOther.CommentLineId).FirstOrDefaultAsync();


                        if (emp != null)
                        {
                            if (appraisalPeriod != null)
                            {
                                if (appraisal != null)
                                {
                                    appraisal.CompanyId = token.CompanyId;
                                    appraisal.DivisionId = token.DivisionId;
                                    appraisal.DepartmentId = token.DepartmentId;
                                    appraisal.PeriodId = appraisalOther.PeriodId;
                                    appraisal.AppraisalId = appraisalOther.AppraisalId;
                                    appraisal.CommentLineId = appraisalOther.CommentLineId;
                                    appraisal.Notes1 = appraisalOther.Notes1;
                                    appraisal.Notes2 = appraisalOther.Notes2;
                                    appraisal.Recommendations = appraisalOther.Recommendations;
                                    appraisal.Memo1 = appraisalOther.Memo1;
                                    appraisal.Memo2 = appraisalOther.Memo2;
                                    appraisal.Memo3 = appraisalOther.Memo3;
                                    appraisal.LockedBy = appraisalOther.LockedBy;
                                    appraisal.LockTs = appraisalOther.LockTs;
                                    appraisal.BranchCode = appraisalOther.BranchCode;
                                    appraisal.CommentDate = appraisalOther.CommentDate;
                                    appraisal.CommentBy = appraisalOther.CommentBy;

                                    _DBContext.Entry(appraisal).State = EntityState.Modified;
                                    _DBContext.SaveChanges();

                                    statusMessage.Status = "Success";
                                    statusMessage.Message = "Success";
                                }
                                else
                                {
                                    appraisalOther.CompanyId = token.CompanyId;
                                    appraisalOther.DivisionId = token.DivisionId;
                                    appraisalOther.DepartmentId = token.DepartmentId;

                                    _DBContext.Entry(appraisalOther).State = EntityState.Added;
                                    _DBContext.SaveChanges();

                                    statusMessage.Status = "Success";
                                    statusMessage.Message = "Success";
                                }
                            }
                            else
                            {
                                statusMessage.Status = "Failed";
                                statusMessage.Message = "Invalid Appraisal Period";
                            }
                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "Employee Does Not Exist";
                        }
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Appraisal Other Comments";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> AddComments(List<PayrollHrpayrollAppraisalComments> appraisalComments, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                if (appraisalComments != null)
                {
                    foreach (PayrollHrpayrollAppraisalComments appraisalComment in appraisalComments)
                    {
                        var emp = await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                                                        x.DivisionId == token.DivisionId &&
                                                                                        x.DepartmentId == token.DepartmentId &&
                                                                                        x.EmployeeId == appraisalComment.AppraisalId).FirstOrDefaultAsync();

                        var appraisalPeriod = await _DBContext.PayrollHrpayrollAppraisalPeriod.Where(x => x.CompanyId == token.CompanyId &&
                                                                    x.DivisionId == token.DivisionId &&
                                                                    x.DepartmentId == token.DepartmentId &&
                                                                    x.PeriodId == appraisalComment.PeriodId &&
                                                                    x.Active == true).FirstOrDefaultAsync();

                        var appraisal = await _DBContext.PayrollHrpayrollAppraisalComments.Where(x => x.CompanyId == token.CompanyId &&
                                                                    x.DivisionId == token.DivisionId &&
                                                                    x.DepartmentId == token.DepartmentId &&
                                                                    x.DepartmentId == appraisalComment.AppraisalId &&
                                                                    x.PeriodId == appraisalComment.PeriodId &&
                                                                    x.CommentLineId == appraisalComment.CommentLineId).FirstOrDefaultAsync();


                        if (emp != null)
                        {
                            if (appraisalPeriod != null)
                            {
                                if (appraisal != null)
                                {
                                    appraisal.CompanyId = token.CompanyId;
                                    appraisal.DivisionId = token.DivisionId;
                                    appraisal.DepartmentId = token.DepartmentId;
                                    appraisal.PeriodId = appraisalComment.PeriodId;
                                    appraisal.AppraisalId = appraisalComment.AppraisalId;
                                    appraisal.CommentLineId = appraisalComment.CommentLineId;
                                    appraisal.Notes1 = appraisalComment.Notes1;
                                    appraisal.Notes2 = appraisalComment.Notes2;
                                    appraisal.Recommendations = appraisalComment.Recommendations;
                                    appraisal.Memo1 = appraisalComment.Memo1;
                                    appraisal.Memo2 = appraisalComment.Memo2;
                                    appraisal.Memo3 = appraisalComment.Memo3;
                                    appraisal.LockedBy = appraisalComment.LockedBy;
                                    appraisal.LockTs = appraisalComment.LockTs;
                                    appraisal.BranchCode = appraisalComment.BranchCode;
                                    appraisal.CommentDate = appraisalComment.CommentDate;
                                    appraisal.CommentBy = appraisalComment.CommentBy;

                                    _DBContext.Entry(appraisal).State = EntityState.Modified;
                                    _DBContext.SaveChanges();

                                    statusMessage.Status = "Success";
                                    statusMessage.Message = "Success";
                                }
                                else
                                {
                                    appraisalComment.CompanyId = token.CompanyId;
                                    appraisalComment.DivisionId = token.DivisionId;
                                    appraisalComment.DepartmentId = token.DepartmentId;

                                    _DBContext.Entry(appraisalComment).State = EntityState.Added;
                                    _DBContext.SaveChanges();

                                    statusMessage.Status = "Success";
                                    statusMessage.Message = "Success";
                                }
                            }
                            else
                            {
                                statusMessage.Status = "Failed";
                                statusMessage.Message = "Invalid Appraisal Period";
                            }
                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "Employee Does Not Exist";
                        }
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Appraisal Comments";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> AddQuestionnaire(List<PayrollHrpayrollAppraisalQuestionnaire> appraisalQuestionnaires, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                if (appraisalQuestionnaires != null)
                {
                    foreach (PayrollHrpayrollAppraisalQuestionnaire appraisalQuestionnaire in appraisalQuestionnaires)
                    {
                        var emp = await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                                                        x.DivisionId == token.DivisionId &&
                                                                                        x.DepartmentId == token.DepartmentId &&
                                                                                        x.EmployeeId == appraisalQuestionnaire.AppraisalId).FirstOrDefaultAsync();

                        var appraisalPeriod = await _DBContext.PayrollHrpayrollAppraisalPeriod.Where(x => x.CompanyId == token.CompanyId &&
                                                                    x.DivisionId == token.DivisionId &&
                                                                    x.DepartmentId == token.DepartmentId &&
                                                                    x.PeriodId == appraisalQuestionnaire.PeriodId &&
                                                                    x.Active == true).FirstOrDefaultAsync();

                        var appraisal = await _DBContext.PayrollHrpayrollAppraisalQuestionnaire.Where(x => x.CompanyId == token.CompanyId &&
                                                                    x.DivisionId == token.DivisionId &&
                                                                    x.DepartmentId == token.DepartmentId &&
                                                                    x.AppraisalId == appraisalQuestionnaire.AppraisalId &&
                                                                    x.PeriodId == appraisalQuestionnaire.PeriodId &&
                                                                    x.QuestionId == appraisalQuestionnaire.QuestionId).FirstOrDefaultAsync();


                        if (emp != null)
                        {
                            if (appraisalPeriod != null)
                            {
                                if (appraisal != null)
                                {
                                    appraisal.CompanyId = token.CompanyId;
                                    appraisal.DivisionId = token.DivisionId;
                                    appraisal.DepartmentId = token.DepartmentId;
                                    appraisal.PeriodId = appraisalQuestionnaire.PeriodId;
                                    appraisal.AppraisalId = appraisalQuestionnaire.AppraisalId;
                                    appraisal.Department = appraisalQuestionnaire.Department;
                                    appraisal.QuestionId = appraisalQuestionnaire.QuestionId;
                                    appraisal.QuestionTypeId = appraisalQuestionnaire.QuestionTypeId;
                                    appraisal.QuestionDescription = appraisalQuestionnaire.QuestionDescription;
                                    appraisal.StaffRating = appraisalQuestionnaire.StaffRating;
                                    appraisal.ManagerRating = appraisalQuestionnaire.ManagerRating;
                                    appraisal.AnsweredBy = appraisalQuestionnaire.AnsweredBy;
                                    appraisal.Answer = appraisalQuestionnaire.Answer;
                                    appraisal.LockedBy = appraisalQuestionnaire.LockedBy;
                                    appraisal.LockTs = appraisalQuestionnaire.LockTs;
                                    appraisal.StaffComment = appraisalQuestionnaire.StaffComment;
                                    appraisal.ManagerComment = appraisalQuestionnaire.ManagerComment;
                                    appraisal.IsRecommendation = appraisalQuestionnaire.IsRecommendation;
                                    appraisal.DateAnswered = appraisalQuestionnaire.DateAnswered;

                                    _DBContext.Entry(appraisal).State = EntityState.Modified;
                                    _DBContext.SaveChanges();

                                    statusMessage.Status = "Success";
                                    statusMessage.Message = "Success";
                                }
                                else
                                {
                                    appraisalQuestionnaire.CompanyId = token.CompanyId;
                                    appraisalQuestionnaire.DivisionId = token.DivisionId;
                                    appraisalQuestionnaire.DepartmentId = token.DepartmentId;

                                    _DBContext.Entry(appraisalQuestionnaire).State = EntityState.Added;
                                    _DBContext.SaveChanges();

                                    statusMessage.Status = "Success";
                                    statusMessage.Message = "Success";
                                }
                            }
                            else
                            {
                                statusMessage.Status = "Failed";
                                statusMessage.Message = "Invalid Appraisal Period";
                            }
                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "Employee Does Not Exist";
                        }
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Appraisal Questionnaire";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> AddEmployeeDetail(List<PayrollHrpayrollEmployeeAppraisalDetail> appraisalEmployeeDetails, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                if (appraisalEmployeeDetails != null)
                {
                    foreach (PayrollHrpayrollEmployeeAppraisalDetail appraisalEmployeeDetail in appraisalEmployeeDetails)
                    {
                        var emp = await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                                                        x.DivisionId == token.DivisionId &&
                                                                                        x.DepartmentId == token.DepartmentId &&
                                                                                        x.EmployeeId == appraisalEmployeeDetail.AppraisalId).FirstOrDefaultAsync();

                        var appraisalPeriod = await _DBContext.PayrollHrpayrollAppraisalPeriod.Where(x => x.CompanyId == token.CompanyId &&
                                                                    x.DivisionId == token.DivisionId &&
                                                                    x.DepartmentId == token.DepartmentId &&
                                                                    x.PeriodId == appraisalEmployeeDetail.PeriodId &&
                                                                    x.Active == true).FirstOrDefaultAsync();

                        var appraisal = await _DBContext.PayrollHrpayrollEmployeeAppraisalDetail.Where(x => x.CompanyId == token.CompanyId &&
                                                                    x.DivisionId == token.DivisionId &&
                                                                    x.DepartmentId == token.DepartmentId &&
                                                                    x.DepartmentId == appraisalEmployeeDetail.AppraisalId &&
                                                                    x.PeriodId == appraisalEmployeeDetail.PeriodId &&
                                                                    x.AppraisalDetailInc == appraisalEmployeeDetail.AppraisalDetailInc).FirstOrDefaultAsync();


                        if (emp != null)
                        {
                            if (appraisalPeriod != null)
                            {
                                if (appraisal != null)
                                {
                                    appraisal.CompanyId = token.CompanyId;
                                    appraisal.DivisionId = token.DivisionId;
                                    appraisal.DepartmentId = token.DepartmentId;
                                    appraisal.PeriodId = appraisalEmployeeDetail.PeriodId;
                                    appraisal.AppraisalId = appraisalEmployeeDetail.AppraisalId;
                                    appraisal.AppraisalDetailInc = appraisalEmployeeDetail.AppraisalDetailInc;
                                    appraisal.AppraisalType = appraisalEmployeeDetail.AppraisalType;
                                    appraisal.AppraisalGoal = appraisalEmployeeDetail.AppraisalGoal;
                                    appraisal.AppraisalKeyResultId = appraisalEmployeeDetail.AppraisalKeyResultId;
                                    appraisal.AppraisalKeyResultIndicator = appraisalEmployeeDetail.AppraisalKeyResultIndicator;
                                    appraisal.MaxScore = appraisalEmployeeDetail.MaxScore;
                                    appraisal.AppraiseeScore = appraisalEmployeeDetail.AppraiseeScore;
                                    appraisal.AppraisalScore = appraisalEmployeeDetail.AppraisalScore;
                                    appraisal.Remarks = appraisalEmployeeDetail.Remarks;
                                    appraisal.LockedBy = appraisalEmployeeDetail.LockedBy;
                                    appraisal.LockTs = appraisalEmployeeDetail.LockTs;
                                    appraisal.AppraiseePercent = appraisalEmployeeDetail.AppraiseePercent;
                                    appraisal.ScoreCriteriaId = appraisalEmployeeDetail.ScoreCriteriaId;
                                    appraisal.ReviewedAppraisalScore = appraisalEmployeeDetail.ReviewedAppraisalScore;
                                    appraisal.ManagerRemark = appraisalEmployeeDetail.ManagerRemark;
                                    appraisal.Hrremark = appraisalEmployeeDetail.Hrremark;
                                    appraisal.ManagerCaptureDate = appraisalEmployeeDetail.ManagerCaptureDate;
                                    appraisal.EmployeeCaptureDate = appraisalEmployeeDetail.EmployeeCaptureDate;
                                    appraisal.HrcaptureDate = appraisalEmployeeDetail.HrcaptureDate;
                
                                    _DBContext.Entry(appraisal).State = EntityState.Modified;
                                    _DBContext.SaveChanges();

                                    statusMessage.Status = "Success";
                                    statusMessage.Message = "Success";
                                }
                                else
                                {
                                    appraisalEmployeeDetail.CompanyId = token.CompanyId;
                                    appraisalEmployeeDetail.DivisionId = token.DivisionId;
                                    appraisalEmployeeDetail.DepartmentId = token.DepartmentId;

                                    _DBContext.Entry(appraisalEmployeeDetail).State = EntityState.Added;
                                    _DBContext.SaveChanges();

                                    statusMessage.Status = "Success";
                                    statusMessage.Message = "Success";
                                }
                            }
                            else
                            {
                                statusMessage.Status = "Failed";
                                statusMessage.Message = "Invalid Appraisal Period";
                            }
                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "Employee Does Not Exist";
                        }
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Appraisal Detail";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> AddTraining(List<PayrollHrpayrollTrainingDetail> appraisalTrainings, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                if (appraisalTrainings != null)
                {
                    foreach (PayrollHrpayrollTrainingDetail appraisalTraining in appraisalTrainings)
                    {
                        var emp = await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                                                        x.DivisionId == token.DivisionId &&
                                                                                        x.DepartmentId == token.DepartmentId &&
                                                                                        x.EmployeeId == appraisalTraining.EmployeeId).FirstOrDefaultAsync();

                        var appraisalPeriod = await _DBContext.PayrollHrpayrollAppraisalPeriod.Where(x => x.CompanyId == token.CompanyId &&
                                                                    x.DivisionId == token.DivisionId &&
                                                                    x.DepartmentId == token.DepartmentId &&
                                                                    x.PeriodId == appraisalTraining.PeriodId &&
                                                                    x.Active == true).FirstOrDefaultAsync();

                        var appraisal = await _DBContext.PayrollHrpayrollTrainingDetail.Where(x => x.CompanyId == token.CompanyId &&
                                                                    x.DivisionId == token.DivisionId &&
                                                                    x.DepartmentId == token.DepartmentId &&
                                                                    x.EmployeeId == appraisalTraining.EmployeeId &&
                                                                    x.PeriodId == appraisalTraining.PeriodId).FirstOrDefaultAsync();


                        if (emp != null)
                        {
                            if (appraisalPeriod != null)
                            {
                                if (appraisal != null)
                                {
                                    appraisal.CompanyId = token.CompanyId;
                                    appraisal.DivisionId = token.DivisionId;
                                    appraisal.DepartmentId = token.DepartmentId;

                                    appraisal.TrainingId = appraisalTraining.TrainingId;
                                    appraisal.EmployeeId = appraisalTraining.EmployeeId;
                                    appraisal.Amount = appraisalTraining.Amount;
                                    appraisal.LockedBy = appraisalTraining.LockedBy;
                                    appraisal.LockTs = appraisalTraining.LockTs;
                                    appraisal.DateFrom = appraisalTraining.DateFrom;
                                    appraisal.DateTo = appraisalTraining.DateTo;
                                    appraisal.LineId = appraisalTraining.LineId;
                                    appraisal.CourseDescription = appraisalTraining.CourseDescription;
                                    appraisal.FacilitatorDescription = appraisalTraining.FacilitatorDescription;
                                    appraisal.BranchCode = appraisalTraining.BranchCode;
                                    appraisal.LockedBy = appraisalTraining.LockedBy;
                                    appraisal.LockTs = appraisalTraining.LockTs;
                                    appraisal.PeriodFrom = appraisalTraining.PeriodFrom;
                                    appraisal.PeriodTo = appraisalTraining.PeriodTo;
                                    appraisal.Feedback = appraisalTraining.Feedback;
                                    appraisal.FeedbackBy = appraisalTraining.FeedbackBy;
                                    appraisal.FeedbackDate = appraisalTraining.FeedbackDate;
                                    appraisal.FeedbackDone = appraisalTraining.FeedbackDone;
                                    appraisal.PeriodId = appraisalTraining.PeriodId;
                                    appraisal.TrainingFocus = appraisalTraining.TrainingFocus;


                                    _DBContext.Entry(appraisal).State = EntityState.Modified;
                                    _DBContext.SaveChanges();

                                    statusMessage.Status = "Success";
                                    statusMessage.Message = "Success";
                                }
                                else
                                {
                                    appraisalTraining.CompanyId = token.CompanyId;
                                    appraisalTraining.DivisionId = token.DivisionId;
                                    appraisalTraining.DepartmentId = token.DepartmentId;

                                    _DBContext.Entry(appraisalTraining).State = EntityState.Added;
                                    _DBContext.SaveChanges();

                                    statusMessage.Status = "Success";
                                    statusMessage.Message = "Success";
                                }
                            }
                            else
                            {
                                statusMessage.Status = "Failed";
                                statusMessage.Message = "Invalid Appraisal Period";
                            }
                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "Employee Does Not Exist";
                        }
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Appraisal Detail";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }
    }
}
