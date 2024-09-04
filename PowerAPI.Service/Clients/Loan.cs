using PowerAPI.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Text;
using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using System.Threading.Tasks;
using System.Linq;
using PowerAPI.Service.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;

namespace PowerAPI.Service.Clients
{
    public class Loan : ILoan
    {
        EnterpriseContext _DBContext;

        public Loan(EnterpriseContext DBContext)
        {
            _DBContext = DBContext;
        }
        public async Task<StatusMessage> Add(PayrollHrpayrollLoanDetail loan, string Mode, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();
            StatusMessage loanRecalc = new StatusMessage();

            try
            {
                var emp = await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.EmployeeId == loan.EmployeeId).FirstOrDefaultAsync();

                var loanType = await _DBContext.PayrollHrpayrollLoanType.Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.LoanTypeId == loan.LoanType).FirstOrDefaultAsync();

                var approver = await _DBContext.PayrollEmployees.Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.EmployeeId == loan.ApprovedBy).FirstOrDefaultAsync();

                var empLoans = await _DBContext.PayrollHrpayrollLoanDetail.Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.EmployeeId == loan.EmployeeId &&
                                                            x.LoanType == loan.LoanType).FirstOrDefaultAsync();
                if(emp != null)
                {
                    if(loanType != null)
                    {
                        if (empLoans == null)
                        {
                            
                            loan.Cleared = false;
                            loan.Approved = false;
                            loan.Posted = false;
                            loan.ActiveYn = true;
                            loan.OnPayroll = true;
                            loan.OutPay = loan.TotalPrincipal;
                            loan.TotalPay = 0;
                            loan.MonthlyPayWoint = loan.MonthlyPay;
                            loan.OutPeriod = (double)loan.TotalPeriod;

                            loan.ApprovedDate = null;
                            loan.PostedDate = null;

                            loan.EmployeeName = loan.EmployeeName == "" || loan.EmployeeName == null ? emp.EmployeeName + " " + emp.EmployeeFirstname : loan.EmployeeName;

                            loan.CompanyId = token.CompanyId;
                            loan.DivisionId = token.DivisionId;
                            loan.DepartmentId = token.DepartmentId;

                            loan.ApprovedByName = approver == null ? "" : approver.EmployeeName + " " + approver.EmployeeFirstname;

                            //assign default
                            loan.LockedBy = null;
                            loan.LockTs = null;
                            loan.BranchCode = token.BranchCode;
                            loan.GlaccountNumber = null;

                            loanRecalc = await Recalc(loan, token);

                            if(loanRecalc != null)
                            {
                                loan = loanRecalc.data;
                            }

                            _DBContext.Entry(loan).State = EntityState.Added;
                            _DBContext.SaveChanges();

                            if (Mode == "Submit")
                            {
                                //call procedure to request approval;
                                statusMessage = submit(loan, token);

                                if (statusMessage.Status == "Failed")
                                {
                                    //delete entry
                                    _DBContext.Entry(loan).State = EntityState.Deleted;
                                    _DBContext.SaveChanges();
                                }

                            }
                            else
                            {
                                //call procedure to request approval;
                                statusMessage = submit(loan, token);

                                if (statusMessage.Status == "Failed")
                                {
                                    //delete entry
                                    _DBContext.Entry(loan).State = EntityState.Deleted;
                                    _DBContext.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "Employee Already Have As Similar Loan Previously Captured. Request Another Loan Type.";
                        }
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Invalid Loan Type";
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Employee Does Not Exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public Task<StatusMessage> Delete(PayrollHrpayrollLoanDetail loan, ApiToken token)
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

        public async Task<IEnumerable<PayrollHrpayrollLoanDetail>> GetAll(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollLoanDetail.OrderByDescending(x => x.EffectiveDate).Where(x => x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<PayrollHrpayrollLoanDetail>> GetByEmployee(string Id, string Mode, ApiToken token)
        {
            try
            {
                if (Mode == "Current" || Mode == "" || Mode == null)
                {
                    return await _DBContext.PayrollHrpayrollLoanDetail.OrderByDescending(x => x.EffectiveDate).Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.EmployeeId == Id).ToListAsync();
                }
                else //(Mode == "History")
                {
                    //come back to implement loan history properly
                    //however lets use this for now
                    return await _DBContext.PayrollHrpayrollLoanDetail.OrderByDescending(x => x.EffectiveDate).Where(x => x.CompanyId == token.CompanyId &&
                                                            x.DivisionId == token.DivisionId &&
                                                            x.DepartmentId == token.DepartmentId &&
                                                            x.EmployeeId == Id).ToListAsync();

                    
                    //return await _DBContext.PayrollHrpayrollLoanHistory.Where(x => x.CompanyId == token.CompanyId &&
                    //                                        x.DivisionId == token.DivisionId &&
                    //                                        x.DepartmentId == token.DepartmentId &&
                    //                                        x.EmployeeId == Id).ToListAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PayrollHrpayrollLoanDetail> GetById(string Id, string loanType, ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollLoanDetail.Where(x => x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.EmployeeId == Id &&
                                                        x.LoanType == loanType).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<PayrollHrpayrollLoanType>> GetLoanTypeByEmployee(string Id, ApiToken token)
        {
            try
            {
                //need to implement employee loan on group setup and adjust this code to get loan based on
                //employee group setup
                return await _DBContext.PayrollHrpayrollLoanType.Where(x => x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<StatusMessage> Approve(LoanAppModel loanAppModel, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                if (loanAppModel != null)
                {
                    var ssEmployee = await _DBContext.PayrollEmployees
                                           .Where(x => x.CompanyId == token.CompanyId &&
                                                       x.DivisionId == token.DivisionId &&
                                                       x.DepartmentId == token.DepartmentId &&
                                                       x.EmployeeId == loanAppModel.ProcessBy &&
                                                       x.EmployeeTypeId == "Salary" &&
                                                       x.ActiveYn == true).FirstOrDefaultAsync();

                    var payrollEmployee = await _DBContext.PayrollEmployees
                                               .Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.EmployeeId == loanAppModel.ProcessBy &&
                                                           x.EmployeeTypeId == "User" &&
                                                           x.ActiveYn == true).FirstOrDefaultAsync();

                    if (ssEmployee != null)
                    {
                        PayrollEmployees systemUser = new PayrollEmployees();

                        if (ssEmployee.EmployeeEmailAddress != null)
                        {

                            systemUser = await _DBContext.PayrollEmployees
                                                       .Where(x => x.CompanyId == token.CompanyId &&
                                                                   x.DivisionId == token.DivisionId &&
                                                                   x.DepartmentId == token.DepartmentId &&
                                                                   x.EmployeeEmailAddress == ssEmployee.EmployeeEmailAddress &&
                                                                   x.EmployeeTypeId == "User" &&
                                                                   x.ActiveYn == true).FirstOrDefaultAsync();
                            if (systemUser != null)
                            {
                                //get approval rights
                                var userPermissions = await _DBContext.AccessPermissions
                                                       .Where(x => x.CompanyId == token.CompanyId &&
                                                                   x.DivisionId == token.DivisionId &&
                                                                   x.DepartmentId == token.DepartmentId &&
                                                                   x.EmployeeId == systemUser.EmployeeId).FirstOrDefaultAsync();

                                if (userPermissions != null)
                                {
                                    userPermissions.SscanApproveLoan = userPermissions.SscanApproveLoan == null || userPermissions.SscanApproveLoan == false ? false : true;
                                    
                                    if (userPermissions.SscanApproveLoan == true)
                                    {
                                        loanAppModel.ProcessBy = systemUser.EmployeeId;

                                        //call procedure to approval;
                                        statusMessage = await approve(loanAppModel, token);

                                        if (statusMessage.Status == "Success")
                                        {
                                            //call procedure to post;
                                            statusMessage = await post(loanAppModel, token);
                                        }
                                    }
                                    else
                                    {
                                        statusMessage.Status = "Failed";
                                        statusMessage.Message = "You dont have access to perform loan approval. Contact system Administrator.";

                                    }
                                }
                                else
                                {
                                    statusMessage.Status = "Failed";
                                    statusMessage.Message = "You dont have access to perform this function. Contact system Administrator.";
                                }

                            }
                            else
                            {
                                statusMessage.Status = "Failed";
                                statusMessage.Message = "Could not find ERP User profile mapped to your Employee No. Contact system Administrator.";
                            }
                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "An email address needs to be setup for your User profile on ERP. Contact system Administrator.";
                        }
                    }
                    else if (payrollEmployee != null)
                    {
                        //get approval rights
                        var userPermissions = await _DBContext.AccessPermissions
                                               .Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.EmployeeId == payrollEmployee.EmployeeId).FirstOrDefaultAsync();

                        if (userPermissions != null)
                        {
                            userPermissions.SscanApproveLoan = userPermissions.SscanApproveLoan == null || userPermissions.SscanApproveLoan == false ? false : true;

                            if (userPermissions.SscanApproveLoan == true)
                            {
                                //call procedure to approval;
                                statusMessage = await approve(loanAppModel, token);

                                if (statusMessage.Status == "Success")
                                {
                                    //call procedure to post;
                                    statusMessage = await post(loanAppModel, token);
                                }
                            }
                            else
                            {
                                statusMessage.Status = "Failed";
                                statusMessage.Message = "You dont have access to perform loan approval. Contact system Administrator.";

                            }
                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "You dont have access to perform this function. Contact system Administrator.";
                        }
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Invalid Approved By";
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Loan does not exist.";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> Recalc(PayrollHrpayrollLoanDetail loan, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            DateTime loanDate;
            DateTime liquidationDate;
            float monthlyPay;
            float monthlyInterest;
            float loanInterestRate = 0;
            float loanInterest;

            try
            {
                //compute loan details
                loanDate = (DateTime)loan.EffectiveDate;
                liquidationDate = loanDate.AddMonths((int)loan.TotalPeriod);

                monthlyPay = (float)(loan.TotalPrincipal / loan.TotalPeriod);

                var loanType = await _DBContext.PayrollHrpayrollLoanType.Where(x => x.CompanyId == token.CompanyId &&
                                           x.DivisionId == token.DivisionId &&
                                           x.DepartmentId == token.DepartmentId &&
                                           x.LoanTypeId == loan.LoanType).FirstOrDefaultAsync();
            
                if(loanType != null)
                {
                    loanInterestRate = loanType.InterestRate != null ? (float)loanType.InterestRate : 0;
                }

                loanInterest = loanInterestRate != 0 ? (float)((float)loan.TotalPrincipal * loanInterestRate * 0.01) : 0;
                monthlyInterest = loanInterest != 0 ? (float)(loanInterest / loan.TotalPeriod) : 0;

                //populate loan object
                loan.MonthlyPay = (decimal)(monthlyPay + monthlyInterest);
                loan.MonthlyPayWoint = (decimal)monthlyPay;
                loan.IntMthPay = (decimal)monthlyInterest;
                loan.LiquidationDate = liquidationDate;
                loan.OutPay = loan.TotalPrincipal;
                loan.OutPeriod = loan.TotalPeriod;

                statusMessage.Status = "Success";
                statusMessage.Message = "Loan Computed Successfully";
                statusMessage.data = loan;
            }
            catch (Exception)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = "Loan re-computation failed.";
            }

            return statusMessage;
        }

        public Task<StatusMessage> Update(PayrollHrpayrollLoanDetail loan, ApiToken token)
        {
            throw new NotImplementedException();
        }

        private StatusMessage submit(PayrollHrpayrollLoanDetail loan, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var sCompanyID = new SqlParameter("@CompanyID", token.CompanyId);
                var sDivisionID = new SqlParameter("@DivisionID", token.DivisionId);
                var sDepartmentID = new SqlParameter("@DepartmentID", token.DepartmentId);
                var sLoanType = new SqlParameter("@LoanType", loan.LoanType);
                var sEmployeeID = new SqlParameter("@EmployeeID", loan.EmployeeId);
                var sEnteredBy = new SqlParameter("@EnteredBy", loan.EmployeeId);
                var PostingResult = new SqlParameter("@PostingResult", SqlDbType.NVarChar, 255);
                PostingResult.Direction = ParameterDirection.Output;

                _DBContext.Database
                          .ExecuteSqlRaw("enterprise.PayrollLoan_Cleared @CompanyID, @DivisionID, @DepartmentID, @LoanType, @EmployeeID, @EnteredBy, @PostingResult Out",
                                    parameters: new[] { sCompanyID, sDivisionID, sDepartmentID, sLoanType, sEmployeeID, sEnteredBy, PostingResult });

                 string result = PostingResult == null ? "" : PostingResult.Value.ToString();

                if (result == "")
                {
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
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

        private async Task<StatusMessage> approve(LoanAppModel loan, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var sCompanyID = new SqlParameter("@CompanyID", token.CompanyId);
                var sDivisionID = new SqlParameter("@DivisionID", token.DivisionId);
                var sDepartmentID = new SqlParameter("@DepartmentID", token.DepartmentId);
                var sLoanType = new SqlParameter("@LoanType", loan.LoanType);
                var sEmployeeID = new SqlParameter("@EmployeeID", loan.EmployeeId);
                var sEnteredBy = new SqlParameter("@EnteredBy", loan.ProcessBy);
                var PostingResult = new SqlParameter("@PostingResult", SqlDbType.NVarChar, 255);
                PostingResult.Direction = ParameterDirection.Output;

                await _DBContext.Database
                          .ExecuteSqlRawAsync("enterprise.PayrollLoan_Approve @CompanyID, @DivisionID, @DepartmentID, @LoanType, @EmployeeID, @EnteredBy, @PostingResult Out",
                                    parameters: new[] { sCompanyID, sDivisionID, sDepartmentID, sLoanType, sEmployeeID, sEnteredBy, PostingResult });

                string result = PostingResult == null ? "" : PostingResult.Value.ToString();

                if (result == "")
                {
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Loan Approved Successfully";
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


        private async Task<StatusMessage> post(LoanAppModel loan, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var sCompanyID = new SqlParameter("@CompanyID", token.CompanyId);
                var sDivisionID = new SqlParameter("@DivisionID", token.DivisionId);
                var sDepartmentID = new SqlParameter("@DepartmentID", token.DepartmentId);
                var sLoanType = new SqlParameter("@LoanType", loan.LoanType);
                var sEmployeeID = new SqlParameter("@EmployeeID", loan.EmployeeId);
                var sEnteredBy = new SqlParameter("@EnteredBy", loan.ProcessBy);
                var PostingResult = new SqlParameter("@PostingResult", SqlDbType.NVarChar, 255);
                PostingResult.Direction = ParameterDirection.Output;

                await _DBContext.Database
                          .ExecuteSqlRawAsync("enterprise.PayrollLoan_Post @CompanyID, @DivisionID, @DepartmentID, @LoanType, @EmployeeID, @EnteredBy, @PostingResult Out",
                                    parameters: new[] { sCompanyID, sDivisionID, sDepartmentID, sLoanType, sEmployeeID, sEnteredBy, PostingResult });

                string result = PostingResult == null ? "" : PostingResult.Value.ToString();

                if (result == "")
                {
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Loan Posted Successfully";
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
    }
}
