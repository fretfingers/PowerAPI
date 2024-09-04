using PowerAPI.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Text;
using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PowerAPI.Service.Helper;
using System.Collections;
using PowerAPI.Data.ViewModels;

namespace PowerAPI.Service.Clients
{
    public class Setup : ISetup
    {
        EnterpriseContext _DBContext;

        public Setup(EnterpriseContext DBContext)
        {
            _DBContext = DBContext;
        }

        public async Task<StatusMessage> AddPayrollCompanyPolicy(PayrollHrpayrollCompanyStandard payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollCompanyPolicy = await _DBContext.PayrollHrpayrollCompanyStandard.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.CompanyStdId == payrollPolicy.CompanyStdId).FirstOrDefaultAsync();

                    if (payrollCompanyPolicy != null) // for update payroll policy
                    {
                        payrollCompanyPolicy.CompanyStdId = payrollPolicy.CompanyStdId;
                        payrollCompanyPolicy.TaxFiscalYear = payrollPolicy.TaxFiscalYear;
                        payrollCompanyPolicy.TaxPeriod = payrollPolicy.TaxPeriod;
                        payrollCompanyPolicy.ReliefOnGross = payrollPolicy.ReliefOnGross;
                        payrollCompanyPolicy.CurrentPeriod = payrollPolicy.CurrentPeriod;
                        payrollCompanyPolicy.AverageWrkAge = payrollPolicy.AverageWrkAge;
                        payrollCompanyPolicy.WorkingHrs = payrollPolicy.WorkingHrs;
                        payrollCompanyPolicy.PostingCompanyId = payrollPolicy.PostingCompanyId;
                        payrollCompanyPolicy.PostingDivisionId = payrollPolicy.PostingDivisionId;
                        payrollCompanyPolicy.PostingDepartmentId = payrollPolicy.PostingDepartmentId;
                        payrollCompanyPolicy.LockedBy = payrollPolicy.LockedBy;
                        payrollCompanyPolicy.LockTs = payrollPolicy.LockTs;
                        payrollCompanyPolicy.WorkingDays = payrollPolicy.WorkingDays;
                        payrollCompanyPolicy.TaxType = payrollPolicy.TaxType;
                        payrollCompanyPolicy.ReliefType = payrollPolicy.ReliefType;
                        payrollCompanyPolicy.Prorate = payrollPolicy.Prorate;
                        payrollCompanyPolicy.ResumptionTime = payrollPolicy.ResumptionTime;
                        payrollCompanyPolicy.GraceTime = payrollPolicy.GraceTime;
                        payrollCompanyPolicy.MidMonthDate = payrollPolicy.MidMonthDate;
                        payrollCompanyPolicy.MidMonthPeriod = payrollPolicy.MidMonthPeriod;
                        payrollCompanyPolicy.PorationBasis = payrollPolicy.PorationBasis;
                        payrollCompanyPolicy.Email = payrollPolicy.Email;
                        payrollCompanyPolicy.EmailClosed = payrollPolicy.EmailClosed;
                        payrollCompanyPolicy.SalaryAccount = payrollPolicy.SalaryAccount;
                        payrollCompanyPolicy.GrossUpNonStatutoryforTax = payrollPolicy.GrossUpNonStatutoryforTax;
                        payrollCompanyPolicy.BioSaturday = payrollPolicy.BioSaturday;
                        payrollCompanyPolicy.BioSunday = payrollPolicy.BioSunday;
                        payrollCompanyPolicy.NoofLatenessAsAbsent = payrollPolicy.NoofLatenessAsAbsent;
                        payrollCompanyPolicy.JournalBasis = payrollPolicy.JournalBasis;
                        payrollCompanyPolicy.PostJvasSummary = payrollPolicy.PostJvasSummary;
                        payrollCompanyPolicy.ConsolidateBillPerCostCentre = payrollPolicy.ConsolidateBillPerCostCentre;
                        payrollCompanyPolicy.LeaveDaysWithWeekends = payrollPolicy.LeaveDaysWithWeekends;
                        payrollCompanyPolicy.BranchCode = payrollPolicy.BranchCode;
                        payrollCompanyPolicy.PayrollProcessed = payrollPolicy.PayrollProcessed;
                        payrollCompanyPolicy.AutoPromotion = payrollPolicy.AutoPromotion;
                        payrollCompanyPolicy.EmployeeProbationPeriod = payrollPolicy.EmployeeProbationPeriod;
                        payrollCompanyPolicy.PayeaccountName = payrollPolicy.PayeaccountName;
                        payrollCompanyPolicy.NhfaccountName = payrollPolicy.NhfaccountName;
                        payrollCompanyPolicy.PromotionInAyear = payrollPolicy.PromotionInAyear;
                        payrollCompanyPolicy.DurationBeforeFirstPromotion = payrollPolicy.DurationBeforeFirstPromotion;
                        payrollCompanyPolicy.TaxRulesSubjectValue = payrollPolicy.TaxRulesSubjectValue;
                        payrollCompanyPolicy.HrreportPassword = payrollPolicy.HrreportPassword;

                        _DBContext.Entry(payrollCompanyPolicy).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                    else // insert or create payroll policy
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Unable to add new payroll company policy. Contact your system administrator";
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Company Standard Information";

                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<IEnumerable<Companies>> Companies(ApiToken token)
        {
            try
            {
                return await _DBContext.Companies.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
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

        public async Task<IEnumerable<Companies>> GetCompanies(ApiToken token)
        {
            try
            {
                return await _DBContext.Companies.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<IEnumerable<Departments>> GetDepartments(ApiToken token)
        {

            try
            {
                return await _DBContext.Departments.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }


        public async Task<IEnumerable<Divisions>> GetDivisions(ApiToken token)
        {

            try
            {
                return await _DBContext.Divisions.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }


        public async Task<IEnumerable<Data.Models.LedgerChartOfAccounts>> GetLedgerCOA(ApiToken token)
        {
            try
            {
                return await _DBContext.LedgerChartOfAccounts.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<IEnumerable<PayrollHrpayrollCompanyStandard>> GetPayrollCompanyPolicy(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollCompanyStandard.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<IEnumerable<PayrollHrpayrollReliefType>> PayrollReliefType(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollReliefType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<IEnumerable<PayrollHrpayrollTaxType>> GetPayrollTaxType(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollTaxType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;

        }

        public async Task<IEnumerable<PayrollJournalBasis>> GetPayrollJournalBasis(ApiToken token)
        {
            List<PayrollJournalBasis> payrollJournalBasis = new List<PayrollJournalBasis>();

            string[] journalBasis = { "COMBINED", "COST CENTER", "DEPARTMENT", "LOCATION", "PAYMENT TYPES" };
            int index = 0;

            try
            {
                for (index = 0; index < journalBasis.Length; index++)
                {
                    PayrollJournalBasis payrollJournalBasisObj = new PayrollJournalBasis();

                    payrollJournalBasisObj.CompanyId = token.CompanyId;
                    payrollJournalBasisObj.DivisionId = token.DivisionId;
                    payrollJournalBasisObj.DepartmentId = token.DepartmentId;
                    payrollJournalBasisObj.JournalBasis = journalBasis[index];

                    payrollJournalBasis.Add(payrollJournalBasisObj);
                }
            }
            catch (Exception ex)
            {

            }

            return payrollJournalBasis;
        }

        public async Task<StatusMessage> AddPayrollPFAType(PayrollHrpayrollPfa payrollPFA, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPFA != null)
                {
                    var payrollPFAType = await _DBContext.PayrollHrpayrollPfa.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.Pfaid == payrollPFA.Pfaid).FirstOrDefaultAsync();

                    if (payrollPFAType != null) // for update payroll policy
                    {

                        payrollPFAType.Pfaid = payrollPFA.Pfaid;
                        payrollPFAType.Pfadescription = payrollPFA.Pfadescription;
                        payrollPFAType.Pfaphone = payrollPFA.Pfaphone;
                        payrollPFAType.Pfaaddress = payrollPFA.Pfaaddress;
                        payrollPFAType.Pfaemail = payrollPFA.Pfaemail;
                        payrollPFAType.Pfacontact = payrollPFA.Pfacontact;
                        payrollPFAType.ReceivingBank = payrollPFA.ReceivingBank;
                        payrollPFAType.ReceivingAcctNo = payrollPFA.ReceivingAcctNo;
                        payrollPFAType.Pfalogo = payrollPFA.Pfalogo;
                        payrollPFAType.PfafreeCode = payrollPFA.PfafreeCode;
                        payrollPFAType.InstrumentNumber = payrollPFA.InstrumentNumber;
                        payrollPFAType.EmployerCode = payrollPFA.EmployerCode;
                        payrollPFAType.LockedBy = payrollPFA.LockedBy;
                        payrollPFAType.LockTs = payrollPFA.LockTs;
                        payrollPFAType.BranchCode = payrollPFA.BranchCode;
                        payrollPFAType.ReceivingBank = payrollPFA.ReceivingBank;

                        _DBContext.Entry(payrollPFAType).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                    else // insert or create payroll policy
                    {

                        payrollPFA.CompanyId = token.CompanyId;
                        payrollPFA.DivisionId = token.DivisionId;
                        payrollPFA.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPFA).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";


                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll PFA Type Information";

                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<IEnumerable<PayrollHrpayrollPfa>> GetPayrollPFAType(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollPfa.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<StatusMessage> AddPayrollLoanType(PayrollHrpayrollLoanType payrollloanType, ApiToken token)
        {

            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollloanType != null)
                {
                    var payrollLoanTypeId = await _DBContext.PayrollHrpayrollLoanType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.LoanTypeId == payrollloanType.LoanTypeId).FirstOrDefaultAsync();


                    if (payrollLoanTypeId != null) // for update payroll policy
                    {

                        payrollLoanTypeId.Description = payrollloanType.Description;
                        payrollLoanTypeId.InterestRate = payrollloanType.InterestRate;
                        payrollLoanTypeId.LockedBy = payrollloanType.LockedBy;
                        payrollLoanTypeId.LockTs = payrollloanType.LockTs;
                        payrollLoanTypeId.GlaccountNumber = payrollloanType.GlaccountNumber;
                        payrollLoanTypeId.InterestTypeId = payrollloanType.InterestTypeId;
                        payrollLoanTypeId.BranchCode = payrollloanType.BranchCode;



                        _DBContext.Entry(payrollLoanTypeId).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                    else // insert or create payroll loan policy
                    {

                        payrollloanType.CompanyId = token.CompanyId;
                        payrollloanType.DivisionId = token.DivisionId;
                        payrollloanType.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollloanType).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";


                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Loan Type Information";

                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<IEnumerable<PayrollHrpayrollLoanType>> GetPayrollLoanType(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollLoanType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;

        }

        public async Task<IEnumerable<PayrollHrpayrollAllowanceRelief>> GetPayrollAllowanceRelief(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollAllowanceRelief.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<StatusMessage> AddPayrollAllowanceRelief(PayrollHrpayrollAllowanceRelief payrollrelief, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollrelief != null)
                {
                    var payrollReliefType = await _DBContext.PayrollHrpayrollAllowanceRelief.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.FiscalYear == payrollrelief.FiscalYear).FirstOrDefaultAsync();


                    if (payrollReliefType != null) // for update payroll policy
                    {

                        payrollReliefType.ReliefRate = payrollrelief.ReliefRate;
                        payrollReliefType.ReliefTypeId = payrollrelief.ReliefTypeId;
                        payrollReliefType.ReliefAmount = payrollrelief.ReliefAmount;
                        payrollReliefType.LockedBy = payrollrelief.LockedBy;
                        payrollReliefType.LockTs = payrollrelief.LockTs;
                        payrollReliefType.BranchCode = payrollrelief.BranchCode;


                        _DBContext.Entry(payrollReliefType).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                    else // insert or create payroll allowance relief policy
                    {
                        payrollrelief.CompanyId = token.CompanyId;
                        payrollrelief.DivisionId = token.DivisionId;
                        payrollrelief.DepartmentId = token.DepartmentId;
                        payrollrelief.AutoId = 0;

                        _DBContext.Entry(payrollrelief).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";


                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Allowance Relief Information";

                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<IEnumerable<PayrollHrpayrollReliefType>> GetPayrollReliefType(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollReliefType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<IEnumerable<Data.ViewModels.PaymentTypes>> GetPayrollPayType(ApiToken token)
        {
            List<Data.ViewModels.PaymentTypes> paymentTypes = new List<Data.ViewModels.PaymentTypes>();

            try
            {
                var payTypes = await _DBContext.PayrollHrpayrollPayType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();

                paymentTypes = await payType(payTypes, token);
            }
            catch (Exception ex)
            {

            }

            return paymentTypes;

        }

        public async Task<StatusMessage> AddPayrollPayType(Data.ViewModels.PaymentTypes payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            List<PayrollHrpayrollPayTypeDetail> paymentTypesDetail = new List<PayrollHrpayrollPayTypeDetail>();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollPayType = await _DBContext.PayrollHrpayrollPayType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.PayTypeId == payrollPolicy.PayTypeId).FirstOrDefaultAsync();

                    if (payrollPayType != null) // for update payroll policy
                    {
                        payrollPayType.PayTypeDescription = payrollPolicy.PayTypeDescription;
                        payrollPayType.GlaccountNumber = payrollPolicy.GlaccountNumber;
                        payrollPayType.AttrDescription = payrollPolicy.AttrDescription;
                        payrollPayType.OperatorId = payrollPolicy.OperatorId;
                        payrollPayType.ConversionFactor = payrollPolicy.ConversionFactor;
                        payrollPayType.TotalAmount = payrollPolicy.TotalAmount;
                        payrollPayType.LockedBy = payrollPolicy.LockedBy;
                        payrollPayType.LockTs = payrollPolicy.LockTs;
                        payrollPayType.StatusId = payrollPolicy.StatusId;
                        payrollPayType.PayTypeDefault = payrollPolicy.PayTypeDefault;
                        payrollPayType.EmployeePercent = payrollPolicy.EmployeePercent;
                        payrollPayType.Zerorise = payrollPolicy.Zerorise;
                        payrollPayType.SortOrder = payrollPolicy.SortOrder;
                        payrollPayType.Accrued = payrollPolicy.Accrued;
                        payrollPayType.EmployerPercent = payrollPolicy.EmployerPercent;
                        payrollPayType.PayTypeGlaccountNumber = payrollPolicy.PayTypeGlaccountNumber;
                        payrollPayType.Taxable = payrollPolicy.Taxable;
                        payrollPayType.GlaccountNumber1 = payrollPolicy.GlaccountNumber1;
                        payrollPayType.Prorate = payrollPolicy.Prorate;
                        payrollPayType.IgnoreHourlyRate = payrollPolicy.IgnoreHourlyRate;
                        payrollPayType.BillingItem = payrollPolicy.BillingItem;
                        payrollPayType.ShowBalanceOnSlip = payrollPolicy.ShowBalanceOnSlip;
                        payrollPayType.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollPayType).State = EntityState.Modified;

                        if (payrollPolicy.paymentTypesDetail != null && payrollPolicy.paymentTypesDetail.Count > 0)
                        {
                            foreach (PayrollHrpayrollPayTypeDetail payDetail in payrollPolicy.paymentTypesDetail)
                            {
                                var payrollHrpayrollPayTypeDetailObj = await _DBContext.PayrollHrpayrollPayTypeDetail.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.PayTypeId == payDetail.PayTypeId &&
                                                        x.PayTypeDetailId == payDetail.PayTypeDetailId).FirstOrDefaultAsync();

                                if (payrollHrpayrollPayTypeDetailObj != null)
                                {
                                    //payrollHrpayrollPayTypeDetailObj.CompanyId = token.CompanyId;
                                    //payrollHrpayrollPayTypeDetailObj.DivisionId = token.DivisionId;
                                    //payrollHrpayrollPayTypeDetailObj.DepartmentId = token.DepartmentId;
                                    //payrollHrpayrollPayTypeDetailObj.PayTypeId = payDetail.PayTypeId;
                                    //payrollHrpayrollPayTypeDetailObj.PayTypeDetailId = payDetail.PayTypeDetailId;
                                    payrollHrpayrollPayTypeDetailObj.EmployeePercent = payDetail.EmployeePercent;
                                    payrollHrpayrollPayTypeDetailObj.EmployerPercent = payDetail.EmployerPercent;
                                    payrollHrpayrollPayTypeDetailObj.OperatorId = payDetail.OperatorId;
                                    payrollHrpayrollPayTypeDetailObj.Active = payDetail.Active;
                                    payrollHrpayrollPayTypeDetailObj.LockedBy = payDetail.LockedBy;
                                    payrollHrpayrollPayTypeDetailObj.LockTs = payDetail.LockTs;
                                    payrollHrpayrollPayTypeDetailObj.BranchCode = payDetail.BranchCode;

                                    _DBContext.Entry(payrollHrpayrollPayTypeDetailObj).State = EntityState.Modified;
                                }
                                else
                                {
                                    payDetail.CompanyId = token.CompanyId;
                                    payDetail.DivisionId = token.DivisionId;
                                    payDetail.DepartmentId = token.DepartmentId;

                                    _DBContext.Entry(payDetail).State = EntityState.Added;
                                }

                            }
                        }

                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                    else // insert or create payroll policy
                    {
                        PayrollHrpayrollPayType payrollHrpayrollPayType = new PayrollHrpayrollPayType();

                        payrollHrpayrollPayType.CompanyId = token.CompanyId;
                        payrollHrpayrollPayType.DivisionId = token.DivisionId;
                        payrollHrpayrollPayType.DepartmentId = token.DepartmentId;
                        payrollHrpayrollPayType.PayTypeId = payrollPolicy.PayTypeId;
                        payrollHrpayrollPayType.PayTypeDescription = payrollPolicy.PayTypeDescription;
                        payrollHrpayrollPayType.GlaccountNumber = payrollPolicy.GlaccountNumber;
                        payrollHrpayrollPayType.AttrDescription = payrollPolicy.AttrDescription;
                        payrollHrpayrollPayType.OperatorId = payrollPolicy.OperatorId;
                        payrollHrpayrollPayType.ConversionFactor = payrollPolicy.ConversionFactor;
                        payrollHrpayrollPayType.TotalAmount = payrollPolicy.TotalAmount;
                        payrollHrpayrollPayType.LockedBy = payrollPolicy.LockedBy;
                        payrollHrpayrollPayType.LockTs = payrollPolicy.LockTs;
                        payrollHrpayrollPayType.StatusId = payrollPolicy.StatusId;
                        payrollHrpayrollPayType.PayTypeDefault = payrollPolicy.PayTypeDefault;
                        payrollHrpayrollPayType.EmployeePercent = payrollPolicy.EmployeePercent;
                        payrollHrpayrollPayType.Zerorise = payrollPolicy.Zerorise;
                        payrollHrpayrollPayType.SortOrder = payrollPolicy.SortOrder;
                        payrollHrpayrollPayType.Accrued = payrollPolicy.Accrued;
                        payrollHrpayrollPayType.EmployerPercent = payrollPolicy.EmployerPercent;
                        payrollHrpayrollPayType.PayTypeGlaccountNumber = payrollPolicy.PayTypeGlaccountNumber;
                        payrollHrpayrollPayType.Taxable = payrollPolicy.Taxable;
                        payrollHrpayrollPayType.GlaccountNumber1 = payrollPolicy.GlaccountNumber1;
                        payrollHrpayrollPayType.Prorate = payrollPolicy.Prorate;
                        payrollHrpayrollPayType.IgnoreHourlyRate = payrollPolicy.IgnoreHourlyRate;
                        payrollHrpayrollPayType.BillingItem = payrollPolicy.BillingItem;
                        payrollHrpayrollPayType.ShowBalanceOnSlip = payrollPolicy.ShowBalanceOnSlip;
                        payrollHrpayrollPayType.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollHrpayrollPayType).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        if (payrollPolicy.paymentTypesDetail != null && payrollPolicy.paymentTypesDetail.Count > 0)
                        {
                            foreach (PayrollHrpayrollPayTypeDetail payDetail in payrollPolicy.paymentTypesDetail)
                            {
                                var payrollHrpayrollPayTypeDetailObj = await _DBContext.PayrollHrpayrollPayTypeDetail.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.PayTypeId == payDetail.PayTypeId &&
                                                        x.PayTypeDetailId == payDetail.PayTypeDetailId).FirstOrDefaultAsync();

                                if (payrollHrpayrollPayTypeDetailObj != null)
                                {
                                    //payrollHrpayrollPayTypeDetailObj.CompanyId = token.CompanyId;
                                    //payrollHrpayrollPayTypeDetailObj.DivisionId = token.DivisionId;
                                    //payrollHrpayrollPayTypeDetailObj.DepartmentId = token.DepartmentId;
                                    //payrollHrpayrollPayTypeDetailObj.PayTypeId = payDetail.PayTypeId;
                                    //payrollHrpayrollPayTypeDetailObj.PayTypeDetailId = payDetail.PayTypeDetailId;
                                    payrollHrpayrollPayTypeDetailObj.EmployeePercent = payDetail.EmployeePercent;
                                    payrollHrpayrollPayTypeDetailObj.EmployerPercent = payDetail.EmployerPercent;
                                    payrollHrpayrollPayTypeDetailObj.OperatorId = payDetail.OperatorId;
                                    payrollHrpayrollPayTypeDetailObj.Active = payDetail.Active;
                                    payrollHrpayrollPayTypeDetailObj.LockedBy = payDetail.LockedBy;
                                    payrollHrpayrollPayTypeDetailObj.LockTs = payDetail.LockTs;
                                    payrollHrpayrollPayTypeDetailObj.BranchCode = payDetail.BranchCode;

                                    _DBContext.Entry(payrollHrpayrollPayTypeDetailObj).State = EntityState.Modified;
                                    _DBContext.SaveChanges();
                                }
                                else
                                {
                                    payDetail.CompanyId = token.CompanyId;
                                    payDetail.DivisionId = token.DivisionId;
                                    payDetail.DepartmentId = token.DepartmentId;

                                    _DBContext.Entry(payDetail).State = EntityState.Added;
                                    _DBContext.SaveChanges();
                                }

                            }
                        }




                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Pay Type Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        private async Task<List<Data.ViewModels.PaymentTypes>> payType(List<PayrollHrpayrollPayType> paymentTypes, ApiToken token)
        {
            List<Data.ViewModels.PaymentTypes> payTypes = new List<Data.ViewModels.PaymentTypes>();
            try
            {
                if (paymentTypes != null)
                {
                    foreach (PayrollHrpayrollPayType paymentType in paymentTypes)
                    {
                        Data.ViewModels.PaymentTypes paymentTypeObj = new Data.ViewModels.PaymentTypes();

                        paymentTypeObj.CompanyId = paymentType.CompanyId;
                        paymentTypeObj.DivisionId = paymentType.DivisionId;
                        paymentTypeObj.DepartmentId = paymentType.DepartmentId;
                        paymentTypeObj.PayTypeId = paymentType.PayTypeId;
                        paymentTypeObj.PayTypeDescription = paymentType.PayTypeDescription;
                        paymentTypeObj.GlaccountNumber = paymentType.GlaccountNumber;
                        paymentTypeObj.AttrDescription = paymentType.AttrDescription;
                        paymentTypeObj.OperatorId = paymentType.OperatorId;
                        paymentTypeObj.ConversionFactor = paymentType.ConversionFactor;
                        paymentTypeObj.LockedBy = paymentType.LockedBy;
                        paymentTypeObj.LockTs = paymentType.LockTs;
                        paymentTypeObj.StatusId = paymentType.StatusId;
                        paymentTypeObj.PayTypeDefault = paymentType.PayTypeDefault;
                        paymentTypeObj.EmployeePercent = paymentType.EmployeePercent;
                        paymentTypeObj.Zerorise = paymentType.Zerorise;
                        paymentTypeObj.SortOrder = paymentType.SortOrder;
                        paymentTypeObj.Accrued = paymentType.Accrued;
                        paymentTypeObj.EmployerPercent = paymentType.EmployerPercent;
                        paymentTypeObj.PayTypeGlaccountNumber = paymentType.PayTypeGlaccountNumber;
                        paymentTypeObj.Taxable = paymentType.Taxable;
                        paymentTypeObj.GlaccountNumber1 = paymentType.GlaccountNumber;
                        paymentTypeObj.Prorate = paymentType.Prorate;
                        paymentTypeObj.IgnoreHourlyRate = paymentType.IgnoreHourlyRate;
                        paymentTypeObj.BillingItem = paymentType.BillingItem;
                        paymentTypeObj.ShowBalanceOnSlip = paymentType.ShowBalanceOnSlip;
                        paymentTypeObj.BranchCode = paymentType.BranchCode;

                        paymentTypeObj.paymentTypesDetail = await _DBContext.PayrollHrpayrollPayTypeDetail.Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.PayTypeId == paymentType.PayTypeId).ToListAsync();

                        payTypes.Add(paymentTypeObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return payTypes;

        }

        public async Task<IEnumerable<PayrollHrpayrollAttr>> GetPayrollAttribute(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollAttr.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<IEnumerable<PayrollHrpayrollOperator>> GetPayrollOperator(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollOperator.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<StatusMessage> AddpayrollStandardRelief(PayrollHrpayrollStandardRelief payrollPolicy, ApiToken token)
        {

            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollStandardRelief = await _DBContext.PayrollHrpayrollStandardRelief.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.FiscalYear == payrollPolicy.FiscalYear).FirstOrDefaultAsync();

                    if (payrollStandardRelief != null) // for update payroll policy
                    {
                        payrollStandardRelief.ReliefRate = payrollPolicy.ReliefRate;
                        payrollStandardRelief.PersonalAllowance = payrollPolicy.PersonalAllowance;
                        payrollStandardRelief.DisabilityAllowance = payrollPolicy.DisabilityAllowance;
                        payrollStandardRelief.ChildAllowance = payrollPolicy.ChildAllowance;
                        payrollStandardRelief.DependentAllowance = payrollPolicy.DependentAllowance;
                        payrollStandardRelief.LockedBy = payrollPolicy.LockedBy;
                        payrollStandardRelief.LockTs = payrollPolicy.LockTs;
                        payrollStandardRelief.MinimumTaxAmount = payrollPolicy.MinimumTaxAmount;
                        payrollStandardRelief.TaxGrossPercent = payrollPolicy.TaxGrossPercent;
                        payrollStandardRelief.AllStaffRelief = payrollPolicy.AllStaffRelief;
                        payrollStandardRelief.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollStandardRelief).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Standard Relief Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;


        }

        public async Task<IEnumerable<PayrollHrpayrollStandardRelief>> GetPayrollStandardRelief(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollStandardRelief.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;

        }

        public async Task<IEnumerable<PayrollHrpayrollLeaveType>> GetPayrollLeaveType(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollLeaveType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;

        }

        public async Task<StatusMessage> AddpayrollLeaveType(PayrollHrpayrollLeaveType payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty  
                if (payrollPolicy != null)
                {
                    var payrollLeaveType = await _DBContext.PayrollHrpayrollLeaveType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.LeaveTypeId == payrollPolicy.LeaveTypeId).FirstOrDefaultAsync();

                    if (payrollLeaveType != null) // for update payroll policy
                    {
                        payrollLeaveType.Description = payrollPolicy.Description;
                        payrollLeaveType.Payable = payrollPolicy.Payable;
                        payrollLeaveType.OnPayroll = payrollPolicy.OnPayroll;
                        payrollLeaveType.LockedBy = payrollPolicy.LockedBy;
                        payrollLeaveType.LockTs = payrollPolicy.LockTs;
                        payrollLeaveType.CalcOnLeaveDays = payrollPolicy.CalcOnLeaveDays;
                        payrollLeaveType.MaximumDays = payrollPolicy.MaximumDays;
                        payrollLeaveType.MaxWorkingdays = payrollPolicy.MaxWorkingdays;
                        payrollLeaveType.UsagePeriod = payrollPolicy.UsagePeriod;
                        payrollLeaveType.BranchCode = payrollPolicy.BranchCode;
                        payrollLeaveType.LeaveGender = payrollPolicy.LeaveGender;

                        _DBContext.Entry(payrollLeaveType).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Leave Type Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> AddPayrollTaxTable(PayrollHrpayrollTaxTable payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollTaxTable = await _DBContext.PayrollHrpayrollTaxTable.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.FiscalYear == payrollPolicy.FiscalYear).FirstOrDefaultAsync();

                    if (payrollTaxTable != null) // for update payroll policy
                    {
                        payrollTaxTable.StateId = payrollPolicy.StateId;
                        payrollTaxTable.BandWidth = payrollPolicy.BandWidth;
                        payrollTaxTable.LowerLimit = payrollPolicy.LowerLimit;
                        payrollTaxTable.UpperLimit = payrollPolicy.UpperLimit;
                        payrollTaxTable.TaxRate = payrollPolicy.TaxRate;
                        payrollTaxTable.FixedValue = payrollPolicy.FixedValue;
                        payrollTaxTable.LockedBy = payrollPolicy.LockedBy;
                        payrollTaxTable.LockTs = payrollPolicy.LockTs;
                        payrollTaxTable.BranchCode = payrollPolicy.BranchCode;
                        payrollTaxTable.CurrencyId = payrollPolicy.CurrencyId;

                        _DBContext.Entry(payrollTaxTable).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        //payrollTaxTable.StateId = payrollPolicy.StateId;
                        //payrollTaxTable.BandWidth = payrollPolicy.BandWidth;
                        //payrollTaxTable.LowerLimit = payrollPolicy.LowerLimit;
                        //payrollTaxTable.UpperLimit = payrollPolicy.UpperLimit;
                        //payrollTaxTable.TaxRate = payrollPolicy.TaxRate;
                        //payrollTaxTable.FixedValue = payrollPolicy.FixedValue;
                        //payrollTaxTable.LockedBy = payrollPolicy.LockedBy;
                        //payrollTaxTable.LockTs = payrollPolicy.LockTs;
                        //payrollTaxTable.BranchCode = payrollPolicy.BranchCode;
                        //payrollTaxTable.CurrencyId = payrollPolicy.CurrencyId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Tax Table Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<IEnumerable<PayrollHrpayrollTaxTable>> GetPayrollTaxTable(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollTaxTable.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;

        }

        public async Task<StatusMessage> DeletePayrollTaxTable(int FiscalYear, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var taxTable = await _DBContext.PayrollHrpayrollTaxTable.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.FiscalYear == FiscalYear).FirstOrDefaultAsync();
                if (taxTable != null)
                {
                    _DBContext.Entry(taxTable).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<StatusMessage> DeletePayrollComanyPolicy(string CompanyStdId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();
            try
            {
                var companyPolicy = await _DBContext.PayrollHrpayrollCompanyStandard.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.CompanyStdId == CompanyStdId).FirstOrDefaultAsync();

                if (companyPolicy != null)
                {
                    _DBContext.Entry(companyPolicy).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<StatusMessage> DeletePayrollStandardRelief(int FiscalYear, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var standardRelief = await _DBContext.PayrollHrpayrollStandardRelief.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.FiscalYear == FiscalYear).FirstOrDefaultAsync();
                if (standardRelief != null)
                {
                    _DBContext.Entry(standardRelief).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<StatusMessage> AddPayrollReliefType(PayrollHrpayrollReliefType payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollReliefType = await _DBContext.PayrollHrpayrollReliefType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ReliefTypeId == payrollPolicy.ReliefTypeId).FirstOrDefaultAsync();


                    if (payrollReliefType != null) // for update payroll policy
                    {

                        payrollReliefType.ReliefTypeDescription = payrollPolicy.ReliefTypeDescription;
                        payrollReliefType.LockedBy = payrollPolicy.LockedBy;
                        payrollReliefType.LockTs = payrollPolicy.LockTs;
                        payrollReliefType.BranchCode = payrollPolicy.BranchCode;


                        _DBContext.Entry(payrollReliefType).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                    else // insert or create payroll allowance relief policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollReliefType).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";


                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Allowance Relief Information";

                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> DeletePayrollHrPayrollReliefType(string ReliefTypeId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var ReliefType = await _DBContext.PayrollHrpayrollReliefType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ReliefTypeId == ReliefTypeId).FirstOrDefaultAsync();
                if (ReliefType != null)
                {
                    _DBContext.Entry(ReliefType).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<StatusMessage> DeletePayrollHrPayrollLoanType(string loanTypeId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var LoanType = await _DBContext.PayrollHrpayrollLoanType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.LoanTypeId == loanTypeId).FirstOrDefaultAsync();
                if (LoanType != null)
                {
                    _DBContext.Entry(LoanType).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> DeletePayrollHrPayrollLeaveType(string leaveTypeId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var LeaveType = await _DBContext.PayrollHrpayrollLeaveType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.LeaveTypeId == leaveTypeId).FirstOrDefaultAsync();
                if (LeaveType != null)
                {
                    _DBContext.Entry(LeaveType).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> DeletePayrollHrPayrollPayType(string payTypeId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var payType = await _DBContext.PayrollHrpayrollPayType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.PayTypeId == payTypeId).FirstOrDefaultAsync();
                if (payType != null)
                {
                    _DBContext.Entry(payType).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> DeletePayrollHrPayrollPFAType(string Pfaid, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var pfaid = await _DBContext.PayrollHrpayrollPfa.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.Pfaid == Pfaid).FirstOrDefaultAsync();
                if (pfaid != null)
                {
                    _DBContext.Entry(pfaid).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> DeletePayrollHrPayrollAllowanceRelief(int FiscalYear, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var fiscalYear = await _DBContext.PayrollHrpayrollAllowanceRelief.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.FiscalYear == FiscalYear).FirstOrDefaultAsync();
                if (fiscalYear != null)
                {
                    _DBContext.Entry(fiscalYear).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<IEnumerable<PayrollHrpayrollGroupHeader>> GetPayrollGroupHeader(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollGroupHeader.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;

        }

        public async Task<StatusMessage> AddpayrollGroupHeader(PayrollHrpayrollGroupHeader payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollGroupHeader = await _DBContext.PayrollHrpayrollGroupHeader.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.GroupId == payrollPolicy.GroupId).FirstOrDefaultAsync();

                    if (payrollGroupHeader != null) // for update payroll policy
                    {
                        payrollGroupHeader.GroupDescription = payrollPolicy.GroupDescription;
                        payrollGroupHeader.Frequency = payrollPolicy.Frequency;
                        payrollGroupHeader.LeaveDays = payrollPolicy.LeaveDays;
                        payrollGroupHeader.Amount = payrollPolicy.Amount;
                        payrollGroupHeader.GlaccountNumber = payrollPolicy.GlaccountNumber;
                        payrollGroupHeader.LockedBy = payrollPolicy.LockedBy;
                        payrollGroupHeader.LockTs = payrollPolicy.LockTs;
                        payrollGroupHeader.NextGroupId = payrollPolicy.NextGroupId;
                        payrollGroupHeader.LeaveTypeId = payrollPolicy.LeaveTypeId;
                        payrollGroupHeader.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollGroupHeader).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }

                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }

                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Group Header Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> DeletePayrollHrPayrollGroupHeader(string GroupId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var GroupHeader = await _DBContext.PayrollHrpayrollGroupHeader.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.GroupId == GroupId).FirstOrDefaultAsync();
                if (GroupHeader != null)
                {
                    _DBContext.Entry(GroupHeader).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<IEnumerable<PayrollHrpayrollTitle>> GetPayrollTitle(ApiToken token)
        {

            try
            {
                return await _DBContext.PayrollHrpayrollTitle.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;

        }

        public async Task<StatusMessage> AddPayrollTitle(PayrollHrpayrollTitle payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollTitleType = await _DBContext.PayrollHrpayrollTitle.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.TitleId == payrollPolicy.TitleId).FirstOrDefaultAsync();

                    if (payrollTitleType != null) // for update payroll policy
                    {
                        payrollTitleType.TitleDescription = payrollPolicy.TitleDescription;
                        payrollTitleType.LockedBy = payrollPolicy.LockedBy;
                        payrollTitleType.LockTs = payrollPolicy.LockTs;
                        payrollTitleType.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollTitleType).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }

                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        //payrollDesignationType.DesignationId = payrollPolicy.DesignationId;
                        //payrollDesignationType.DesignDescription = payrollPolicy.DesignDescription;
                        //payrollDesignationType.LockedBy = payrollPolicy.LockedBy;
                        //payrollDesignationType.LockTs = payrollPolicy.LockTs;
                        //payrollDesignationType.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }

                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Title Type Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<StatusMessage> DeletePayrollTitle(string TitleId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var Title = await _DBContext.PayrollHrpayrollTitle.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.TitleId == TitleId).FirstOrDefaultAsync();
                if (Title != null)
                {
                    _DBContext.Entry(Title).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<IEnumerable<PayrollHrpayrollDesignation>> GetPayrollDesignationType(ApiToken token)
        {

            try
            {
                return await _DBContext.PayrollHrpayrollDesignation.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;

        }

        public async Task<StatusMessage> AddPayrollDesignationType(PayrollHrpayrollDesignation payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollDesignationType = await _DBContext.PayrollHrpayrollDesignation.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.DesignationId == payrollPolicy.DesignationId).FirstOrDefaultAsync();

                    if (payrollDesignationType != null) // for update payroll policy
                    {
                        payrollDesignationType.DesignDescription = payrollPolicy.DesignDescription;
                        payrollDesignationType.LockedBy = payrollPolicy.LockedBy;
                        payrollDesignationType.LockTs = payrollPolicy.LockTs;
                        payrollDesignationType.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollDesignationType).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }

                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        //payrollDesignationType.DesignationId = payrollPolicy.DesignationId;
                        //payrollDesignationType.DesignDescription = payrollPolicy.DesignDescription;
                        //payrollDesignationType.LockedBy = payrollPolicy.LockedBy;
                        //payrollDesignationType.LockTs = payrollPolicy.LockTs;
                        //payrollDesignationType.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }

                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Designation Type Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<StatusMessage> DeletePayrollHrPayrollDesignation(string DesignationId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var Designation = await _DBContext.PayrollHrpayrollDesignation.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.DesignationId == DesignationId).FirstOrDefaultAsync();
                if (Designation != null)
                {
                    _DBContext.Entry(Designation).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<IEnumerable<PayrollHrpayrollCostCenter>> GetPayrollCostCenter(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollCostCenter.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;

        }

        public async Task<StatusMessage> AddPayrollCostCenter(PayrollHrpayrollCostCenter payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollCostCenter = await _DBContext.PayrollHrpayrollCostCenter.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.CostCenterId == payrollPolicy.CostCenterId).FirstOrDefaultAsync();

                    if (payrollCostCenter != null) // for update payroll policy
                    {
                        payrollCostCenter.Description = payrollPolicy.Description;
                        payrollCostCenter.LockedBy = payrollPolicy.LockedBy;
                        payrollCostCenter.LockTs = payrollPolicy.LockTs;
                        payrollCostCenter.GlaccountNumber = payrollPolicy.GlaccountNumber;
                        payrollCostCenter.ChargeOutRate = payrollPolicy.ChargeOutRate;
                        payrollCostCenter.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollCostCenter).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }

                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }

                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Cost Center Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<StatusMessage> DeletePayrollHrPayrollCostCenter(string CostCenterId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var CostCenter = await _DBContext.PayrollHrpayrollCostCenter.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.CostCenterId == CostCenterId).FirstOrDefaultAsync();
                if (CostCenter != null)
                {
                    _DBContext.Entry(CostCenter).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }
        public async Task<IEnumerable<PayrollHrpayrollOffenceType>> GetPayrollOffenceType(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollOffenceType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;

        }

        public async Task<StatusMessage> AddPayrollOffenceType(PayrollHrpayrollOffenceType payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollOffence = await _DBContext.PayrollHrpayrollOffenceType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.OffenceTypeId == payrollPolicy.OffenceTypeId).FirstOrDefaultAsync();

                    if (payrollOffence != null) // for update payroll policy
                    {
                        payrollOffence.Description = payrollPolicy.Description;
                        payrollOffence.LockedBy = payrollPolicy.LockedBy;
                        payrollOffence.LockTs = payrollPolicy.LockTs;
                        payrollOffence.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollOffence).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }

                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }

                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Cost Center Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<StatusMessage> DeletePayrollOffenceType(string OffenceTypeId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var offenceType = await _DBContext.PayrollHrpayrollOffenceType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.OffenceTypeId == OffenceTypeId).FirstOrDefaultAsync();
                if (offenceType != null)
                {
                    _DBContext.Entry(offenceType).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<IEnumerable<PayrollHrpayrollActiveReason>> GetPayrollActiveReason(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollActiveReason.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;

        }

        public async Task<StatusMessage> AddPayrollActiveReason(PayrollHrpayrollActiveReason payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollActiveReason = await _DBContext.PayrollHrpayrollActiveReason.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.ReasonId == payrollPolicy.ReasonId).FirstOrDefaultAsync();

                    if (payrollActiveReason != null) // for update payroll policy
                    {
                        payrollActiveReason.ReasonDescription = payrollPolicy.ReasonDescription;
                        payrollActiveReason.LockedBy = payrollPolicy.LockedBy;
                        payrollActiveReason.LockTs = payrollPolicy.LockTs;
                        payrollActiveReason.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollActiveReason).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }

                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }

                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Cost Center Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<StatusMessage> DeletePayrollActiveReason(string ReasonId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var activeReason = await _DBContext.PayrollHrpayrollActiveReason.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ReasonId == ReasonId).FirstOrDefaultAsync();
                if (activeReason != null)
                {
                    _DBContext.Entry(activeReason).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<IEnumerable<PayrollHrpayrollEmployeeActivity>> GetPayrollActivityType(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollEmployeeActivity.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;

        }

        public async Task<StatusMessage> AddPayrollActivityType(PayrollHrpayrollEmployeeActivity payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollActivityType = await _DBContext.PayrollHrpayrollEmployeeActivity.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.EmployeeActivityTypeId == payrollPolicy.EmployeeActivityTypeId).FirstOrDefaultAsync();

                    if (payrollActivityType != null) // for update payroll policy
                    {
                        payrollActivityType.EmployeeActivityTypeDescription = payrollPolicy.EmployeeActivityTypeDescription;
                        payrollActivityType.LockedBy = payrollPolicy.LockedBy;
                        payrollActivityType.LockTs = payrollPolicy.LockTs;
                        payrollActivityType.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollActivityType).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }

                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }

                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Cost Center Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<StatusMessage> DeletePayrollActivityType(string EmployeeActivityTypeId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var activityType = await _DBContext.PayrollHrpayrollEmployeeActivity.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.EmployeeActivityTypeId == EmployeeActivityTypeId).FirstOrDefaultAsync();
                if (activityType != null)
                {
                    _DBContext.Entry(activityType).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<IEnumerable<PayrollHrpayrollRelationship>> GetPayrollRelationshipType(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollRelationship.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;

        }

        public async Task<StatusMessage> AddPayrollRelationshipType(PayrollHrpayrollRelationship payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollRelationship = await _DBContext.PayrollHrpayrollRelationship.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.RelationshipId == payrollPolicy.RelationshipId).FirstOrDefaultAsync();

                    if (payrollRelationship != null) // for update payroll policy
                    {
                        payrollRelationship.RelationshipDescription = payrollPolicy.RelationshipDescription;
                        payrollRelationship.LockedBy = payrollPolicy.LockedBy;
                        payrollRelationship.LockTs = payrollPolicy.LockTs;
                        payrollRelationship.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollRelationship).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }

                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }

                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Cost Center Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<StatusMessage> DeletePayrollRelationshipType(string RelationshipId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var RelationshipType = await _DBContext.PayrollHrpayrollRelationship.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.RelationshipId == RelationshipId).FirstOrDefaultAsync();
                if (RelationshipType != null)
                {
                    _DBContext.Entry(RelationshipType).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }


        public async Task<IEnumerable<Data.ViewModels.JobClass>> GetPayrollJobClassHeader(ApiToken token)
        {
            List<Data.ViewModels.JobClass> jobClass = new List<Data.ViewModels.JobClass>();

            try
            {
                var jobs = await _DBContext.PayrollHrpayrollJobClassHeader.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
                jobClass = await jobClasses(jobs, token);
            }
            catch (Exception ex)
            {

            }

            return jobClass;

        }

        private async Task<List<Data.ViewModels.JobClass>> jobClasses(List<PayrollHrpayrollJobClassHeader> jobClass, ApiToken token)
        {
            List<Data.ViewModels.JobClass> jobs = new List<Data.ViewModels.JobClass>();
            try
            {
                if (jobClass != null)
                {
                    foreach (PayrollHrpayrollJobClassHeader jobsClass in jobClass)
                    {
                        Data.ViewModels.JobClass jobsClassObj = new Data.ViewModels.JobClass();

                        jobsClassObj.CompanyId = jobsClass.CompanyId;
                        jobsClassObj.DivisionId = jobsClass.DivisionId;
                        jobsClassObj.DepartmentId = jobsClass.DepartmentId;
                        jobsClassObj.JobClassId = jobsClass.JobClassId;
                        jobsClassObj.JobClassDescription = jobsClass.JobClassDescription;
                        jobsClassObj.LockedBy = jobsClass.LockedBy;
                        jobsClassObj.LockTs = jobsClass.LockTs;
                        jobsClassObj.BranchCode = jobsClass.BranchCode;

                        jobsClassObj.payrollJobClassDetails = await _DBContext.PayrollHrpayrollJobClassDetail.Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.JobClassId == jobsClass.JobClassId).ToListAsync();

                        jobs.Add(jobsClassObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return jobs;

        }

        public async Task<StatusMessage> AddPayrollJobClassHeader(Data.ViewModels.JobClass payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            List<PayrollHrpayrollJobClassDetail> payrollJobClassDetails = new List<PayrollHrpayrollJobClassDetail>();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollJobClass = await _DBContext.PayrollHrpayrollJobClassHeader.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.JobClassId == payrollPolicy.JobClassId).FirstOrDefaultAsync();

                    if (payrollJobClass != null) // for update payroll policy
                    {
                        payrollJobClass.JobClassDescription = payrollPolicy.JobClassDescription;
                        payrollJobClass.LockedBy = payrollPolicy.LockedBy;
                        payrollJobClass.LockTs = payrollPolicy.LockTs;
                        payrollJobClass.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollJobClass).State = EntityState.Modified;

                        if (payrollPolicy.payrollJobClassDetails != null && payrollPolicy.payrollJobClassDetails.Count > 0)
                        {
                            foreach (PayrollHrpayrollJobClassDetail jobDetail in payrollPolicy.payrollJobClassDetails)
                            {
                                var payrollHrpayrollJobClassDetailObj = await _DBContext.PayrollHrpayrollJobClassDetail.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.JobClassId == jobDetail.JobClassId &&
                                                        x.JobClassDetailId == jobDetail.JobClassDetailId).FirstOrDefaultAsync();

                                if (payrollHrpayrollJobClassDetailObj != null)
                                {
                                    payrollHrpayrollJobClassDetailObj.LockedBy = jobDetail.LockedBy;
                                    payrollHrpayrollJobClassDetailObj.LockTs = jobDetail.LockTs;
                                    payrollHrpayrollJobClassDetailObj.BranchCode = jobDetail.BranchCode;

                                    _DBContext.Entry(payrollHrpayrollJobClassDetailObj).State = EntityState.Modified;
                                }
                                else
                                {
                                    jobDetail.CompanyId = token.CompanyId;
                                    jobDetail.DivisionId = token.DivisionId;
                                    jobDetail.DepartmentId = token.DepartmentId;

                                    _DBContext.Entry(jobDetail).State = EntityState.Added;
                                }

                            }
                        }

                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                    else // insert or create payroll policy
                    {
                        PayrollHrpayrollJobClassHeader payrollHrpayrollJobClassHeader = new PayrollHrpayrollJobClassHeader();

                        payrollHrpayrollJobClassHeader.CompanyId = token.CompanyId;
                        payrollHrpayrollJobClassHeader.DivisionId = token.DivisionId;
                        payrollHrpayrollJobClassHeader.DepartmentId = token.DepartmentId;
                        payrollHrpayrollJobClassHeader.JobClassId = payrollPolicy.JobClassId;
                        payrollHrpayrollJobClassHeader.JobClassDescription = payrollPolicy.JobClassDescription;
                        payrollHrpayrollJobClassHeader.LockedBy = payrollPolicy.LockedBy;
                        payrollHrpayrollJobClassHeader.LockTs = payrollPolicy.LockTs;
                        payrollHrpayrollJobClassHeader.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollHrpayrollJobClassHeader).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        if (payrollPolicy.payrollJobClassDetails != null && payrollPolicy.payrollJobClassDetails.Count > 0)
                        {
                            foreach (PayrollHrpayrollJobClassDetail jobDetail in payrollPolicy.payrollJobClassDetails)
                            {
                                var payrollHrpayrollJobClassDetailObj = await _DBContext.PayrollHrpayrollJobClassDetail.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.JobClassId == jobDetail.JobClassId &&
                                                        x.JobClassDetailId == jobDetail.JobClassDetailId).FirstOrDefaultAsync();

                                if (payrollHrpayrollJobClassDetailObj != null)
                                {
                                    payrollHrpayrollJobClassDetailObj.LockedBy = jobDetail.LockedBy;
                                    payrollHrpayrollJobClassDetailObj.LockTs = jobDetail.LockTs;
                                    payrollHrpayrollJobClassDetailObj.BranchCode = jobDetail.BranchCode;

                                    _DBContext.Entry(payrollHrpayrollJobClassDetailObj).State = EntityState.Modified;
                                    _DBContext.SaveChanges();
                                }
                                else
                                {
                                    jobDetail.CompanyId = token.CompanyId;
                                    jobDetail.DivisionId = token.DivisionId;
                                    jobDetail.DepartmentId = token.DepartmentId;

                                    _DBContext.Entry(jobDetail).State = EntityState.Added;
                                    _DBContext.SaveChanges();
                                }

                            }
                        }




                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Pay Type Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> DeletePayrollHrPayrollJobClassHeader(string JobClassId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var JobClass = await _DBContext.PayrollHrpayrollJobClassHeader.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.JobClassId == JobClassId).FirstOrDefaultAsync();
                if (JobClass != null)
                {
                    _DBContext.Entry(JobClass).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";

                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<StatusMessage> DeletePayrollHrPayrollJobClassDetail(PayrollHrpayrollJobClassDetail jobClassDetail,  ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var JobClassDetail = await _DBContext.PayrollHrpayrollJobClassDetail.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.JobClassId == jobClassDetail.JobClassId &&
                                                        x.JobClassDetailId == jobClassDetail.JobClassDetailId).FirstOrDefaultAsync();
                if (JobClassDetail != null)
                {
                    _DBContext.Entry(JobClassDetail).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";

                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<IEnumerable<Data.Models.BankAccounts>> GetBankAccounts(ApiToken token)
        {
            try
            {
                return await _DBContext.BankAccounts.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<StatusMessage> AddBankAccounts(Data.Models.BankAccounts payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollBankAccount = await _DBContext.BankAccounts.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.BankId == payrollPolicy.BankId).FirstOrDefaultAsync();


                    if (payrollBankAccount != null) // for update payroll policy
                    {

                        payrollBankAccount.BankAccountNumber = payrollPolicy.BankAccountNumber;
                        payrollBankAccount.BankName = payrollPolicy.BankName;
                        payrollBankAccount.BankAddress1 = payrollPolicy.BankAddress1;
                        payrollBankAccount.BankAddress2 = payrollPolicy.BankAddress2;
                        payrollBankAccount.BankCity = payrollPolicy.BankCity;
                        payrollBankAccount.BankState = payrollPolicy.BankState;
                        payrollBankAccount.BankZip = payrollPolicy.BankZip;
                        payrollBankAccount.BankCountry = payrollPolicy.BankCountry;
                        payrollBankAccount.BankPhone = payrollPolicy.BankPhone;
                        payrollBankAccount.BankFax = payrollPolicy.BankFax;
                        payrollBankAccount.BankContactName = payrollPolicy.BankContactName;
                        payrollBankAccount.BankEmail = payrollPolicy.BankEmail;
                        payrollBankAccount.BankWebsite = payrollPolicy.BankWebsite;
                        payrollBankAccount.SwiftCode = payrollPolicy.SwiftCode;
                        payrollBankAccount.RoutingCode = payrollPolicy.RoutingCode;
                        payrollBankAccount.CurrencyId = payrollPolicy.CurrencyId;
                        payrollBankAccount.CurrencyExchangeRate = payrollPolicy.CurrencyExchangeRate;
                        payrollBankAccount.NextCheckNumber = payrollPolicy.NextCheckNumber;
                        payrollBankAccount.NextDepositNumber = payrollPolicy.NextDepositNumber;
                        payrollBankAccount.Balance = payrollPolicy.Balance;
                        payrollBankAccount.UnpostedDeposits = payrollPolicy.UnpostedDeposits;
                        payrollBankAccount.GlbankAccount = payrollPolicy.GlbankAccount;
                        payrollBankAccount.Notes = payrollPolicy.Notes;
                        payrollBankAccount.CorrespondentBankId = payrollPolicy.CorrespondentBankId;
                        payrollBankAccount.Approved = payrollPolicy.Approved;
                        payrollBankAccount.ApprovedBy = payrollPolicy.ApprovedBy;
                        payrollBankAccount.ApprovedDate = payrollPolicy.ApprovedDate;
                        payrollBankAccount.EnteredBy = payrollPolicy.EnteredBy;
                        payrollBankAccount.LockedBy = payrollPolicy.LockedBy;
                        payrollBankAccount.LockTs = payrollPolicy.LockTs;
                        payrollBankAccount.BankAccountTypeId = payrollPolicy.BankAccountTypeId;
                        payrollBankAccount.ChequeDate = payrollPolicy.ChequeDate;
                        payrollBankAccount.ChequeNo = payrollPolicy.ChequeNo;
                        payrollBankAccount.FirstSign = payrollPolicy.FirstSign;
                        payrollBankAccount.SecondSign = payrollPolicy.SecondSign;
                        payrollBankAccount.FirstDesign = payrollPolicy.FirstDesign;
                        payrollBankAccount.SecondDesign = payrollPolicy.SecondDesign;
                        payrollBankAccount.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollBankAccount).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                    else // insert or create payroll bank accounts policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";


                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Bank Accounts Information";

                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> DeleteBankAccounts(string bankId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var bank = await _DBContext.BankAccounts.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.BankId == bankId).FirstOrDefaultAsync();
                if (bank != null)
                {
                    _DBContext.Entry(bank).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";

                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<IEnumerable<PayrollHrpayrollNationality>> GetPayrollNationalitySetup(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollNationality.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;

        }

        public async Task<StatusMessage> AddPayrollNationality(PayrollHrpayrollNationality payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollNationality = await _DBContext.PayrollHrpayrollNationality.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.NationalityId == payrollPolicy.NationalityId).FirstOrDefaultAsync();

                    if (payrollNationality != null) // for update payroll policy
                    {
                        payrollNationality.NationalityId = payrollPolicy.NationalityId;
                        payrollNationality.NationalityDescription = payrollPolicy.NationalityDescription;
                        payrollNationality.LockedBy = payrollPolicy.LockedBy;
                        payrollNationality.LockTs = payrollPolicy.LockTs;
                        payrollNationality.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollNationality).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }

                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }

                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Nationality Setup Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> DeletePayrollHrPayrollNationality(string nationalityId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var nationality = await _DBContext.PayrollHrpayrollNationality.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.NationalityId == nationalityId).FirstOrDefaultAsync();
                if (nationality != null)
                {
                    _DBContext.Entry(nationality).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";

                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<IEnumerable<PayrollHrpayrollLga>> GetPayrollLGASetup(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollLga.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;

        }

        public async Task<StatusMessage> AddPayrollLGA(PayrollHrpayrollLga payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollLga = await _DBContext.PayrollHrpayrollLga.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.StateId == payrollPolicy.StateId).FirstOrDefaultAsync();

                    if (payrollLga != null) // for update payroll policy
                    {
                        payrollLga.LgaId = payrollPolicy.LgaId;
                        payrollLga.LgaDescription = payrollPolicy.LgaDescription;
                        payrollLga.LockedBy = payrollPolicy.LockedBy;
                        payrollLga.LockTs = payrollPolicy.LockTs;
                        payrollLga.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollLga).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }

                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }

                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll LGA Setup Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> DeletePayrollHrPayrollLGA(string stateId, ApiToken token)
        {

            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var lga = await _DBContext.PayrollHrpayrollLga.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.StateId == stateId).FirstOrDefaultAsync();
                if (lga != null)
                {
                    _DBContext.Entry(lga).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";

                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }



        public async Task<IEnumerable<PayrollHrpayrollLocation>> GetPayrollLocation(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollLocation.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;

        }

        public async Task<StatusMessage> AddPayrollLocation(PayrollHrpayrollLocation payrollPolicy, ApiToken token)
        {

            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollLocation = await _DBContext.PayrollHrpayrollLocation.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.LocationId == payrollPolicy.LocationId).FirstOrDefaultAsync();

                    if (payrollLocation != null) // for update payroll policy
                    {
                        payrollLocation.Description = payrollPolicy.Description;
                        payrollLocation.LockedBy = payrollPolicy.LockedBy;
                        payrollLocation.LockTs = payrollPolicy.LockTs;
                        payrollLocation.GlaccountNumber = payrollPolicy.GlaccountNumber;
                        payrollLocation.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollLocation).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }

                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }

                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Location Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<StatusMessage> DeletePayrollHrPayrollLocation(string LocationId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var payrollLocation = await _DBContext.PayrollHrpayrollLocation.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.LocationId == LocationId).FirstOrDefaultAsync();
                if (payrollLocation != null)
                {
                    _DBContext.Entry(payrollLocation).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";

                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<IEnumerable<PayrollHrpayrollState>> GetPayrollState(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollState.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;

        }

        public async Task<StatusMessage> AddPayrollState(PayrollHrpayrollState payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollState = await _DBContext.PayrollHrpayrollState.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.StateId == payrollPolicy.StateId).FirstOrDefaultAsync();

                    if (payrollState != null) // for update payroll policy
                    {
                        payrollState.NationalityId = payrollPolicy.NationalityId;
                        payrollState.StateDescription = payrollPolicy.StateDescription;
                        payrollState.LockedBy = payrollPolicy.LockedBy;
                        payrollState.LockTs = payrollPolicy.LockTs;
                        payrollState.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollState).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }

                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }

                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll State Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<StatusMessage> DeletePayrollHrPayrollState(string StateId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var payrollState = await _DBContext.PayrollHrpayrollState.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.StateId == StateId).FirstOrDefaultAsync();
                if (payrollState != null)
                {
                    _DBContext.Entry(payrollState).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";

                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<IEnumerable<PayrollHrpayrollCategory>> GetPayrollCategory(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollCategory.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;


        }

        public async Task<StatusMessage> AddPayrollCategory(PayrollHrpayrollCategory payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollCategory = await _DBContext.PayrollHrpayrollCategory.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.CategoryId == payrollPolicy.CategoryId).FirstOrDefaultAsync();

                    if (payrollCategory != null) // for update payroll policy
                    {
                        payrollCategory.CategoryDescription = payrollPolicy.CategoryDescription;
                        payrollCategory.LockedBy = payrollPolicy.LockedBy;
                        payrollCategory.LockTs = payrollPolicy.LockTs;
                        payrollCategory.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollCategory).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }

                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }

                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll State Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<StatusMessage> DeletePayrollHrPayrollCategory(string categoryId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var payrollCategory = await _DBContext.PayrollHrpayrollCategory.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.CategoryId == categoryId).FirstOrDefaultAsync();
                if (payrollCategory != null)
                {
                    _DBContext.Entry(payrollCategory).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";

                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<IEnumerable<PayrollHrpayrollGender>> GetPayrollGender(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollGender.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;


        }

        public async Task<IEnumerable<PayrollHrpayrollLoanInterestType>> GetPayrollInterestType(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollLoanInterestType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;


        }

        public async Task<IEnumerable<Data.ViewModels.EmployeeDept>> GetEmployeeDepartment(ApiToken token)
        {
            List<Data.ViewModels.EmployeeDept> employeeDepts = new List<Data.ViewModels.EmployeeDept>();

            try
            {
                var empDepts = await _DBContext.PayrollEmployeeDepartment.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();

                employeeDepts = await empDept(empDepts, token);
            }
            catch (Exception ex)
            {

            }

            return employeeDepts;

        }


       

        private async Task<List<Data.ViewModels.EmployeeDept>> empDept(List<PayrollEmployeeDepartment> employeeDepts, ApiToken token)
        {
            List<Data.ViewModels.EmployeeDept> empDepts = new List<Data.ViewModels.EmployeeDept>();
            try
            {
                if (employeeDepts != null)
                {
                    foreach (PayrollEmployeeDepartment employeeDepart in employeeDepts)
                    {
                        Data.ViewModels.EmployeeDept employeeDepartObj = new Data.ViewModels.EmployeeDept();

                        employeeDepartObj.CompanyId = employeeDepart.CompanyId;
                        employeeDepartObj.DivisionId = employeeDepart.DivisionId;
                        employeeDepartObj.DepartmentId = employeeDepart.DepartmentId;
                        employeeDepartObj.EmployeeDepartmentId = employeeDepart.EmployeeDepartmentId;
                        employeeDepartObj.EmployeeDepartmentDescription = employeeDepart.EmployeeDepartmentDescription;
                        employeeDepartObj.LockedBy = employeeDepart.LockedBy;
                        employeeDepartObj.LockTs = employeeDepart.LockTs;
                        employeeDepartObj.GlaccountNumber = employeeDepart.GlaccountNumber;
                        employeeDepartObj.BranchCode = employeeDepart.BranchCode;

                        employeeDepartObj.employeeDepartmentDetails = await _DBContext.PayrollEmployeeDepartmentDetails.Where(x => x.CompanyId == token.CompanyId &&
                                                           x.DivisionId == token.DivisionId &&
                                                           x.DepartmentId == token.DepartmentId &&
                                                           x.EmployeeDepartmentId == employeeDepart.EmployeeDepartmentId).ToListAsync();

                        empDepts.Add(employeeDepartObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return empDepts;

        }

        public async Task<StatusMessage> AddEmployeeDepartment(Data.ViewModels.EmployeeDept payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            List<PayrollEmployeeDepartmentDetails> employeeDepartmentDetails = new List<PayrollEmployeeDepartmentDetails>();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollEmployeeDept = await _DBContext.PayrollEmployeeDepartment.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.EmployeeDepartmentId == payrollPolicy.EmployeeDepartmentId).FirstOrDefaultAsync();

                    if (payrollEmployeeDept != null) // for update payroll policy
                    {
                        payrollEmployeeDept.EmployeeDepartmentDescription = payrollPolicy.EmployeeDepartmentDescription;
                        payrollEmployeeDept.LockedBy = payrollPolicy.LockedBy;
                        payrollEmployeeDept.LockTs = payrollPolicy.LockTs;
                        payrollEmployeeDept.GlaccountNumber = payrollPolicy.GlaccountNumber;
                        payrollEmployeeDept.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollEmployeeDept).State = EntityState.Modified;

                        if (payrollPolicy.employeeDepartmentDetails != null && payrollPolicy.employeeDepartmentDetails.Count > 0)
                        {
                            foreach (PayrollEmployeeDepartmentDetails empDepartment in payrollPolicy.employeeDepartmentDetails)
                            {
                                var PayrollEmployeeDepartmentDetailsObj = await _DBContext.PayrollEmployeeDepartmentDetails.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.PayTypeId == empDepartment.PayTypeId &&
                                                        x.EmployeeDepartmentId == empDepartment.EmployeeDepartmentId).FirstOrDefaultAsync();

                                if (PayrollEmployeeDepartmentDetailsObj != null)
                                {
                                    PayrollEmployeeDepartmentDetailsObj.GlaccountNumber = empDepartment.GlaccountNumber;
                                    PayrollEmployeeDepartmentDetailsObj.LockedBy = empDepartment.LockedBy;
                                    PayrollEmployeeDepartmentDetailsObj.LockTs = empDepartment.LockTs;
                                    PayrollEmployeeDepartmentDetailsObj.BranchCode = empDepartment.BranchCode;

                                    _DBContext.Entry(PayrollEmployeeDepartmentDetailsObj).State = EntityState.Modified;
                                }
                                else
                                {
                                    empDepartment.CompanyId = token.CompanyId;
                                    empDepartment.DivisionId = token.DivisionId;
                                    empDepartment.DepartmentId = token.DepartmentId;

                                    _DBContext.Entry(empDepartment).State = EntityState.Added;
                                }

                            }
                        }

                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                    else // insert or create payroll policy
                    {
                        PayrollEmployeeDepartment payrollEmployeeDepartment = new PayrollEmployeeDepartment();

                        payrollEmployeeDepartment.CompanyId = token.CompanyId;
                        payrollEmployeeDepartment.DivisionId = token.DivisionId;
                        payrollEmployeeDepartment.DepartmentId = token.DepartmentId;
                        payrollEmployeeDepartment.EmployeeDepartmentId = payrollPolicy.EmployeeDepartmentId;
                        payrollEmployeeDepartment.EmployeeDepartmentDescription = payrollPolicy.EmployeeDepartmentDescription;
                        payrollEmployeeDepartment.LockedBy = payrollPolicy.LockedBy;
                        payrollEmployeeDepartment.LockTs = payrollPolicy.LockTs; ;
                        payrollEmployeeDepartment.GlaccountNumber = payrollPolicy.GlaccountNumber;
                        payrollEmployeeDepartment.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollEmployeeDepartment).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        if (payrollPolicy.employeeDepartmentDetails != null && payrollPolicy.employeeDepartmentDetails.Count > 0)
                        {
                            foreach (PayrollEmployeeDepartmentDetails empDepartment in payrollPolicy.employeeDepartmentDetails)
                            {
                                var payrollEmployeeDepartmentDetailsObj = await _DBContext.PayrollEmployeeDepartmentDetails.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.PayTypeId == empDepartment.PayTypeId &&
                                                        x.EmployeeDepartmentId == empDepartment.EmployeeDepartmentId).FirstOrDefaultAsync();

                                if (payrollEmployeeDepartmentDetailsObj != null)
                                {
                                    payrollEmployeeDepartmentDetailsObj.GlaccountNumber = empDepartment.GlaccountNumber;
                                    payrollEmployeeDepartmentDetailsObj.LockedBy = empDepartment.LockedBy;
                                    payrollEmployeeDepartmentDetailsObj.LockTs = empDepartment.LockTs;
                                    payrollEmployeeDepartmentDetailsObj.BranchCode = empDepartment.BranchCode;

                                    _DBContext.Entry(payrollEmployeeDepartmentDetailsObj).State = EntityState.Modified;
                                    _DBContext.SaveChanges();
                                }
                                else
                                {
                                    empDepartment.CompanyId = token.CompanyId;
                                    empDepartment.DivisionId = token.DivisionId;
                                    empDepartment.DepartmentId = token.DepartmentId;

                                    _DBContext.Entry(empDepartment).State = EntityState.Added;
                                    _DBContext.SaveChanges();
                                }

                            }
                        }




                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Pay Type Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<StatusMessage> DeletePayrollEmployeeDepartment(string employeeDepartmentId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var employeeDepartment = await _DBContext.PayrollEmployeeDepartment.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.EmployeeDepartmentId == employeeDepartmentId).FirstOrDefaultAsync();
                if (employeeDepartment != null)
                {
                    _DBContext.Entry(employeeDepartment).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";

                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;


        }

        public async Task<StatusMessage> DeletePayrollEmployeeDepartmentDetails(string payTypeId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var employeeDepartmentDetail = await _DBContext.PayrollEmployeeDepartmentDetails.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.PayTypeId == payTypeId).FirstOrDefaultAsync();
                if (employeeDepartmentDetail != null)
                {
                    _DBContext.Entry(employeeDepartmentDetail).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";

                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;


        }

        public async Task<IEnumerable<PayrollHrpayrollPayTypeStatus>> GetPayrollStatus(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollPayTypeStatus.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<IEnumerable<PayrollHrpayrollStatus>> GetPayrollMaritalStatus(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollStatus.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }


        public async Task<StatusMessage> DeletePayrollHrPayrollStatus(string statusId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var CostCenter = await _DBContext.PayrollHrpayrollStatus.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.StatusId == statusId).FirstOrDefaultAsync();
                if (CostCenter != null)
                {
                    _DBContext.Entry(CostCenter).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<StatusMessage> AddpayrollMaritalStatus(PayrollHrpayrollStatus payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollStatus = await _DBContext.PayrollHrpayrollStatus.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.StatusId == payrollPolicy.StatusId).FirstOrDefaultAsync();

                    if (payrollStatus != null) // for update payroll policy
                    {
                        payrollStatus.StatusDescription = payrollPolicy.StatusDescription;
                        payrollStatus.LockedBy = payrollPolicy.LockedBy;
                        payrollStatus.LockTs = payrollPolicy.LockTs;
                        payrollStatus.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollStatus).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }

                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }

                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Cost Center Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<IEnumerable<PayrollHrpayrollLanguage>> GetPayrollLanguage(ApiToken token)
        {

            try
            {
                return await _DBContext.PayrollHrpayrollLanguage.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;

        }

        public async Task<StatusMessage> AddPayrollLanguage(PayrollHrpayrollLanguage payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollLanguageType = await _DBContext.PayrollHrpayrollLanguage.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.LanguageId == payrollPolicy.LanguageId).FirstOrDefaultAsync();

                    if (payrollLanguageType != null) // for update payroll policy
                    {
                        payrollLanguageType.LanguageDescription = payrollPolicy.LanguageDescription;
                        payrollLanguageType.LockedBy = payrollPolicy.LockedBy;
                        payrollLanguageType.LockTs = payrollPolicy.LockTs;
                        payrollLanguageType.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollLanguageType).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }

                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }

                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Language Type Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<StatusMessage> DeletePayrollLanguage(string LanguageId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var Language = await _DBContext.PayrollHrpayrollLanguage.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.LanguageId == LanguageId).FirstOrDefaultAsync();
                if (Language != null)
                {
                    _DBContext.Entry(Language).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Language does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<IEnumerable<PayrollHrpayrollCourseType>> GetPayrollCourseType(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollCourseType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<StatusMessage> AddpayrollCourseType(PayrollHrpayrollCourseType payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollCourseType = await _DBContext.PayrollHrpayrollCourseType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.CourseTypeId == payrollPolicy.CourseTypeId).FirstOrDefaultAsync();

                    if (payrollCourseType != null) // for update payroll policy
                    {
                        payrollCourseType.CourseTypeDescription = payrollPolicy.CourseTypeDescription;
                        payrollCourseType.LockedBy = payrollPolicy.LockedBy;
                        payrollCourseType.LockTs = payrollPolicy.LockTs;
                        payrollCourseType.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollCourseType).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }

                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }

                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Course Type Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> DeletePayrollCourseType(string CourseTypeId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var courseType = await _DBContext.PayrollHrpayrollCourseType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.CourseTypeId == CourseTypeId).FirstOrDefaultAsync();
                if (courseType != null)
                {
                    _DBContext.Entry(courseType).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Course Type does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<IEnumerable<PayrollHrpayrollQualificationClass>> GetPayrollQualificationClass(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollQualificationClass.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<StatusMessage> AddPayrollQualificationClass(PayrollHrpayrollQualificationClass payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollQualificationClass = await _DBContext.PayrollHrpayrollQualificationClass.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.QualificationClassTypeId == payrollPolicy.QualificationClassTypeId).FirstOrDefaultAsync();

                    if (payrollQualificationClass != null) // for update payroll policy
                    {
                        payrollQualificationClass.QualificationClassTypeDesc = payrollPolicy.QualificationClassTypeDesc;
                        payrollQualificationClass.LockedBy = payrollPolicy.LockedBy;
                        payrollQualificationClass.LockTs = payrollPolicy.LockTs;
                        payrollQualificationClass.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollQualificationClass).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }

                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }

                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Qualification Class Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> DeletePayrollQualificationClass(string qualificationClassTypeId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var qualificationClass = await _DBContext.PayrollHrpayrollQualificationClass.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.QualificationClassTypeId == qualificationClassTypeId).FirstOrDefaultAsync();
                if (qualificationClass != null)
                {
                    _DBContext.Entry(qualificationClass).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Qualification Class does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<IEnumerable<PayrollHrpayrollQualificationType>> GetPayrollQualificationType(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollQualificationType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<StatusMessage> AddpayrollQualificationType(PayrollHrpayrollQualificationType payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollQualificationType = await _DBContext.PayrollHrpayrollQualificationType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.QualificationTypeId == payrollPolicy.QualificationTypeId).FirstOrDefaultAsync();

                    if (payrollQualificationType != null) // for update payroll policy
                    {
                        payrollQualificationType.QualificationType = payrollPolicy.QualificationType;
                        payrollQualificationType.QualificationClassTypeId = payrollPolicy.QualificationClassTypeId;
                        payrollQualificationType.LockedBy = payrollPolicy.LockedBy;
                        payrollQualificationType.LockTs = payrollPolicy.LockTs;
                        payrollQualificationType.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollQualificationType).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }

                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }

                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Qualification Type Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<StatusMessage> DeletePayrollQualificationType(string qualificationTypeId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var qualificationType = await _DBContext.PayrollHrpayrollQualificationType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.QualificationTypeId == qualificationTypeId).FirstOrDefaultAsync();
                if (qualificationType != null)
                {
                    _DBContext.Entry(qualificationType).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Qualification Type does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<IEnumerable<PayrollHrpayrollGradeType>> GetPayrollGradeType(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollGradeType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<StatusMessage> AddPayrollGradeType(PayrollHrpayrollGradeType payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollGradeType = await _DBContext.PayrollHrpayrollGradeType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.GradeTypeId == payrollPolicy.GradeTypeId).FirstOrDefaultAsync();

                    if (payrollGradeType != null) // for update payroll policy
                    {
                        payrollGradeType.GradeTypeDescription = payrollPolicy.GradeTypeDescription;
                        payrollGradeType.LockedBy = payrollPolicy.LockedBy;
                        payrollGradeType.LockTs = payrollPolicy.LockTs;
                        payrollGradeType.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollGradeType).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }

                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }

                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Grade Type Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<StatusMessage> DeletePayrollGradeType(string gradeTypeId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var gradeType = await _DBContext.PayrollHrpayrollGradeType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.GradeTypeId == gradeTypeId).FirstOrDefaultAsync();
                if (gradeType != null)
                {
                    _DBContext.Entry(gradeType).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Grade Type does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;
        }

        public async Task<IEnumerable<PayrollHrpayrollInstitution>> GetPayrollInstitution(ApiToken token)
        {
            try
            {
                return await _DBContext.PayrollHrpayrollInstitution.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<StatusMessage> AddpayrollInstitution(PayrollHrpayrollInstitution payrollPolicy, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                if (payrollPolicy != null)
                {
                    var payrollInstitution = await _DBContext.PayrollHrpayrollInstitution.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.InstitutionId == payrollPolicy.InstitutionId).FirstOrDefaultAsync();

                    if (payrollInstitution != null) // for update payroll policy
                    {
                        payrollInstitution.InstitutionDescription = payrollPolicy.InstitutionDescription;
                        payrollInstitution.LockedBy = payrollPolicy.LockedBy;
                        payrollInstitution.LockTs = payrollPolicy.LockTs;
                        payrollInstitution.BranchCode = payrollPolicy.BranchCode;

                        _DBContext.Entry(payrollInstitution).State = EntityState.Modified;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }

                    else // insert or create payroll policy
                    {
                        payrollPolicy.CompanyId = token.CompanyId;
                        payrollPolicy.DivisionId = token.DivisionId;
                        payrollPolicy.DepartmentId = token.DepartmentId;

                        _DBContext.Entry(payrollPolicy).State = EntityState.Added;
                        _DBContext.SaveChanges();

                        statusMessage.Status = "Success";
                        statusMessage.Message = "Success";
                    }
                }

                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Payroll Institution Information";
                }
            }


            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }

            return statusMessage;

        }

        public async Task<StatusMessage> DeletePayrollInstitution(string institutionId, ApiToken token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                var Institution = await _DBContext.PayrollHrpayrollInstitution.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.InstitutionId == institutionId).FirstOrDefaultAsync();
                if (Institution != null)
                {
                    _DBContext.Entry(Institution).State = EntityState.Deleted;
                    _DBContext.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Institution does not exist";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }
            return statusMessage;
        }

        public async Task<Paging> GetBranch(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.CompanyBranch> companyBranches = new List<Data.ViewModels.CompanyBranch>();
            try
            {
                var totalCount = await _DBContext.CompanyBranch
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.CompanyBranch.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.BranchCode)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                companyBranches = await companyBranch(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    BranchList = companyBranches
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetCurrencies(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.CurrencyTypes> currencyTypes = new List<Data.ViewModels.CurrencyTypes>();
            try
            {
                var totalCount = await _DBContext.CurrencyTypes
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.CurrencyTypes.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.CurrencyId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                currencyTypes = await currencies(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    CurrencyList = currencyTypes
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetTaxes(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.Taxes> taxes = new List<Data.ViewModels.Taxes>();
            try
            {
                var totalCount = await _DBContext.Taxes
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.Taxes.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.TaxId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                taxes = await taxess(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    TaxList = taxes
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetTaxGroupDetail(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.TaxGroupDetail> taxGroupDetail = new List<Data.ViewModels.TaxGroupDetail>();
            try
            {
                var totalCount = await _DBContext.TaxGroupDetail
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.TaxGroupDetail.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.TaxId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                taxGroupDetail = await taxGroupDetails(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    TaxGroupDetailList = taxGroupDetail
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetTaxGroups(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.TaxGroups> taxGroup = new List<Data.ViewModels.TaxGroups>();
            try
            {
                var totalCount = await _DBContext.TaxGroups
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.TaxGroups.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.TaxGroupId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                taxGroup = await taxGroups(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    TaxGroupsList = taxGroup
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetDiscounts(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.MultipleDiscounts> multipleDiscount = new List<Data.ViewModels.MultipleDiscounts>();
            try
            {
                var totalCount = await _DBContext.MultipleDiscounts
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.MultipleDiscounts.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.MultipleDiscountId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                multipleDiscount = await multipleDiscounts(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    MultipleDiscountList = multipleDiscount
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetDiscountGroups(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.MultipleDiscountGroups> multipleDiscountGroups = new List<Data.ViewModels.MultipleDiscountGroups>();
            try
            {
                var totalCount = await _DBContext.MultipleDiscountGroups
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.MultipleDiscountGroups.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.MultipleDiscountGroupId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                multipleDiscountGroups = await multipleDiscountGroup(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    MultipleDiscountGroupList = multipleDiscountGroups
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetDiscountGroupDetail(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.MultipleDiscountGroupDetail> multipleDiscountGroupDetail = new List<Data.ViewModels.MultipleDiscountGroupDetail>();
            try
            {
                var totalCount = await _DBContext.MultipleDiscountGroupDetail
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.MultipleDiscountGroupDetail.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.MultipleDiscountId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                multipleDiscountGroupDetail = await multipleDiscountGroupDetails(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    MultipleDiscountGroupDetailList = multipleDiscountGroupDetail
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetTerms(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.Terms> terms = new List<Data.ViewModels.Terms>();
            try
            {
                var totalCount = await _DBContext.Terms
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.Terms.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.TermsId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                terms = await term(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    TermsList = terms
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetARTransactionTypes(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.ArtransactionTypes> arTransactionTypes = new List<Data.ViewModels.ArtransactionTypes>();
            try
            {
                var totalCount = await _DBContext.ArtransactionTypes
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.ArtransactionTypes.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.TransactionTypeId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                arTransactionTypes = await arTransactionType(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    ARTransactionTypesList = arTransactionTypes
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetLedgerAnalysis1(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.LedgerAnalysis> ledgerAnalyses = new List<Data.ViewModels.LedgerAnalysis>();
            try
            {
                var totalCount = await _DBContext.LedgerAnalysis
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.LedgerAnalysis.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.GlanalysisType)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                ledgerAnalyses = await ledgerAnalysis(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    LedgerAnalysisList = ledgerAnalyses
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetLedgerAnalysis2(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.LedgerAnalysis2> ledgerAnalyses2 = new List<Data.ViewModels.LedgerAnalysis2>();
            try
            {
                var totalCount = await _DBContext.LedgerAnalysis2
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.LedgerAnalysis2.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.GlanalysisType)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                ledgerAnalyses2 = await ledgerAnalysis2(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    LedgerAnalysis2List = ledgerAnalyses2
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetCashbookPaymentTypes(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.CashbookPaymentTypes> paymentTypes = new List<Data.ViewModels.CashbookPaymentTypes>();
            try
            {
                var totalCount = await _DBContext.PaymentTypes
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.PaymentTypes.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.PaymentTypeId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                paymentTypes = await paymentType(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    CashbookPaymentTypesList = paymentTypes
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<Paging> GetCreditCardTypes(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.CreditCardTypes> creditCardTypes = new List<Data.ViewModels.CreditCardTypes>();
            try
            {
                var totalCount = await _DBContext.CreditCardTypes
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.CreditCardTypes.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.CreditCardTypeId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                creditCardTypes = await creditCardType(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    CreditCardTypesList = creditCardTypes
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }




        #region Buid ViewModel List Data
        private async Task<List<Data.ViewModels.CompanyBranch>> companyBranch(List<Data.Models.CompanyBranch> obj, ApiToken token)
        {
            List<Data.ViewModels.CompanyBranch> companyBranches = new List<Data.ViewModels.CompanyBranch>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.CompanyBranch companyBranch in obj)
                    {
                        Data.ViewModels.CompanyBranch companyBranchObj = new Data.ViewModels.CompanyBranch();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        companyBranchObj.CompanyId = companyBranch.CompanyId;
                        companyBranchObj.DivisionId = companyBranch.DivisionId;
                        companyBranchObj.DepartmentId = companyBranch.DepartmentId;
                        companyBranchObj.BranchCode = companyBranch.BranchCode;
                        companyBranchObj.BranchName = companyBranch.BranchName;
                        companyBranchObj.BranchManager = companyBranch.BranchManager;
                        companyBranchObj.BranchManager = companyBranch.BranchManager;
                        companyBranchObj.BranchAddress = companyBranch.BranchAddress;
                        companyBranchObj.ContactPerson = companyBranch.ContactPerson;
                        companyBranchObj.LockedBy = companyBranch.LockedBy;
                        companyBranchObj.LockTs = companyBranch.LockTs;
                        companyBranchObj.BranchAddress2 = companyBranch.BranchAddress2;
                        companyBranchObj.BranchAddress3 = companyBranch.BranchAddress3;
                        companyBranchObj.BranchCity = companyBranch.BranchCity;
                        companyBranchObj.BranchState = companyBranch.BranchState;
                        companyBranchObj.BranchCountry = companyBranch.BranchCountry;
                        companyBranchObj.BranchZip = companyBranch.BranchZip;
                        companyBranchObj.BranchPhone = companyBranch.BranchPhone;
                        companyBranchObj.BranchEmail = companyBranch.BranchEmail;
                        companyBranchObj.BranchFax = companyBranch.BranchFax;
                        companyBranchObj.BranchNotes = companyBranch.BranchNotes;

                        companyBranchObj.WorkFlowTrail  = workflows;
                        companyBranches.Add(companyBranchObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return companyBranches;

        }

        private async Task<List<Data.ViewModels.CurrencyTypes>> currencies(List<Data.Models.CurrencyTypes> obj, ApiToken token)
        {
            List<Data.ViewModels.CurrencyTypes> currencyTypes = new List<Data.ViewModels.CurrencyTypes>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.CurrencyTypes currency in obj)
                    {
                        Data.ViewModels.CurrencyTypes currencyTypesObj = new Data.ViewModels.CurrencyTypes();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        currencyTypesObj.CompanyId = currency.CompanyId;
                        currencyTypesObj.DivisionId = currency.DivisionId;
                        currencyTypesObj.DepartmentId = currency.DepartmentId;
                        currencyTypesObj.CurrencyId = currency.CurrencyId;
                        currencyTypesObj.CurrencyType = currency.CurrencyType;
                        currencyTypesObj.CurrenycySymbol = currency.CurrenycySymbol;
                        currencyTypesObj.CurrencyExchangeRate = currency.CurrencyExchangeRate;
                        currencyTypesObj.CurrencyRateLastUpdate = currency.CurrencyRateLastUpdate;
                        currencyTypesObj.CurrencyPrecision = currency.CurrencyPrecision;
                        currencyTypesObj.MajorUnits = currency.MajorUnits;
                        currencyTypesObj.MinorUnits = currency.MinorUnits;
                        currencyTypesObj.LockedBy = currency.LockedBy;
                        currencyTypesObj.LockTs = currency.LockTs;
                        currencyTypesObj.BranchCode = currency.BranchCode;
                        currencyTypesObj.WorkFlowTrail = workflows;

                        currencyTypesObj.currencyTypesHistories = await  _DBContext.CurrencyTypesHistory
                                                                    .Where(x => x.CompanyId == token.CompanyId &&
                                                                           x.DivisionId == token.DivisionId &&
                                                                           x.DepartmentId == token.DepartmentId &&
                                                                           x.CurrencyId == currency.CurrencyId).ToListAsync();

                        currencyTypes.Add(currencyTypesObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return currencyTypes;

        }

        private async Task<List<Data.ViewModels.Taxes>> taxess(List<Data.Models.Taxes> obj, ApiToken token)
        {
            List<Data.ViewModels.Taxes> taxes = new List<Data.ViewModels.Taxes>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.Taxes tax in obj)
                    {
                        Data.ViewModels.Taxes taxObj = new Data.ViewModels.Taxes();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        taxObj.CompanyId = tax.CompanyId;
                        taxObj.DivisionId = tax.DivisionId;
                        taxObj.DepartmentId = tax.DepartmentId;
                        taxObj.TaxId = tax.TaxId;
                        taxObj.TaxDescription = tax.TaxDescription;
                        taxObj.TaxPercent = tax.TaxPercent;
                        taxObj.GltaxAccount = tax.GltaxAccount;
                        taxObj.TaxOrder = tax.TaxOrder;
                        taxObj.Approved = tax.Approved;
                        taxObj.ApprovedBy = tax.ApprovedBy;
                        taxObj.ApprovedDate = tax.ApprovedDate;
                        taxObj.EnteredBy = tax.EnteredBy;
                        taxObj.LockedBy = tax.LockedBy;
                        taxObj.LockTs = tax.LockTs;
                        taxObj.BranchCode = tax.BranchCode;
                        taxObj.DivisionId = tax.DivisionId;

                        taxObj.WorkFlowTrail = workflows;

                        taxes.Add(taxObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return taxes;

        }

        
        private async Task<List<Data.ViewModels.TaxGroupDetail>> taxGroupDetails(List<Data.Models.TaxGroupDetail> obj, ApiToken token)
        {
            List<Data.ViewModels.TaxGroupDetail> taxGroupDetails = new List<Data.ViewModels.TaxGroupDetail>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.TaxGroupDetail taxGroupDetail in obj)
                    {
                        Data.ViewModels.TaxGroupDetail taxGroupDetailObj = new Data.ViewModels.TaxGroupDetail();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        taxGroupDetailObj.CompanyId = taxGroupDetail.CompanyId;
                        taxGroupDetailObj.DivisionId = taxGroupDetail.DivisionId;
                        taxGroupDetailObj.DepartmentId = taxGroupDetail.DepartmentId;
                        taxGroupDetailObj.TaxGroupDetailId = taxGroupDetail.TaxGroupDetailId;
                        taxGroupDetailObj.TaxId = taxGroupDetail.TaxId;
                        taxGroupDetailObj.Description = taxGroupDetail.Description;
                        taxGroupDetailObj.GltaxAccount = taxGroupDetail.GltaxAccount;
                        taxGroupDetailObj.TaxPercent = taxGroupDetail.TaxPercent;
                        taxGroupDetailObj.TaxOrder = taxGroupDetail.TaxOrder;
                        taxGroupDetailObj.TaxOnTax = taxGroupDetail.TaxOnTax;
                        taxGroupDetailObj.LockedBy = taxGroupDetail.LockedBy;
                        taxGroupDetailObj.LockTs = taxGroupDetail.LockTs;
                        taxGroupDetailObj.BranchCode = taxGroupDetail.BranchCode;

                        taxGroupDetailObj.WorkFlowTrail = workflows;

                        taxGroupDetails.Add(taxGroupDetailObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return taxGroupDetails;

        }

        
        private async Task<List<Data.ViewModels.TaxGroups>> taxGroups(List<Data.Models.TaxGroups> obj, ApiToken token)
        {
            List<Data.ViewModels.TaxGroups> taxGroups = new List<Data.ViewModels.TaxGroups>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.TaxGroups taxGroup in obj)
                    {
                        Data.ViewModels.TaxGroups taxGroupsObj = new Data.ViewModels.TaxGroups();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        taxGroupsObj.CompanyId = taxGroup.CompanyId;
                        taxGroupsObj.DivisionId = taxGroup.DivisionId;
                        taxGroupsObj.DepartmentId = taxGroup.DepartmentId;
                        taxGroupsObj.TaxGroupId = taxGroup.TaxGroupId;
                        taxGroupsObj.TaxGroupDetailId = taxGroup.TaxGroupDetailId;
                        taxGroupsObj.TotalPercent = taxGroup.TotalPercent;
                        taxGroupsObj.TaxOnTax = taxGroup.TaxOnTax;
                        taxGroupsObj.LockedBy = taxGroup.LockedBy;
                        taxGroupsObj.LockTs = taxGroup.LockTs;
                        taxGroupsObj.BranchCode = taxGroup.BranchCode;


                        taxGroupsObj.WorkFlowTrail = workflows;

                        taxGroups.Add(taxGroupsObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return taxGroups;

        }

        
        private async Task<List<Data.ViewModels.MultipleDiscounts>> multipleDiscounts(List<Data.Models.MultipleDiscounts> obj, ApiToken token)
        {
            List<Data.ViewModels.MultipleDiscounts> multipleDiscounts = new List<Data.ViewModels.MultipleDiscounts>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.MultipleDiscounts multipleDiscount in obj)
                    {
                        Data.ViewModels.MultipleDiscounts multipleDiscountsObj = new Data.ViewModels.MultipleDiscounts();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        multipleDiscountsObj.CompanyId = multipleDiscount.CompanyId;
                        multipleDiscountsObj.DivisionId = multipleDiscount.DivisionId;
                        multipleDiscountsObj.DepartmentId = multipleDiscount.DepartmentId;
                        multipleDiscountsObj.MultipleDiscountId = multipleDiscount.MultipleDiscountId;
                        multipleDiscountsObj.MultipleDiscountDescription = multipleDiscount.MultipleDiscountDescription;
                        multipleDiscountsObj.MultipleDiscountPercent = multipleDiscount.MultipleDiscountPercent;
                        multipleDiscountsObj.GlmultipleDiscountAccount = multipleDiscount.GlmultipleDiscountAccount;
                        multipleDiscountsObj.MultipleDiscountOrder = multipleDiscount.MultipleDiscountOrder;
                        multipleDiscountsObj.Approved = multipleDiscount.Approved;
                        multipleDiscountsObj.ApprovedBy = multipleDiscount.ApprovedBy;
                        multipleDiscountsObj.ApprovedDate = multipleDiscount.ApprovedDate;
                        multipleDiscountsObj.EnteredBy = multipleDiscount.EnteredBy;
                        multipleDiscountsObj.LockedBy = multipleDiscount.LockedBy;
                        multipleDiscountsObj.LockTs = multipleDiscount.LockTs;
                        multipleDiscountsObj.BranchCode = multipleDiscount.BranchCode;

                        multipleDiscountsObj.WorkFlowTrail = workflows;

                        multipleDiscounts.Add(multipleDiscountsObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return multipleDiscounts;

        }

        
        private async Task<List<Data.ViewModels.MultipleDiscountGroups>> multipleDiscountGroup(List<Data.Models.MultipleDiscountGroups> obj, ApiToken token)
        {
            List<Data.ViewModels.MultipleDiscountGroups> multipleDiscountGroups = new List<Data.ViewModels.MultipleDiscountGroups>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.MultipleDiscountGroups multipleDiscountGroup in obj)
                    {
                        Data.ViewModels.MultipleDiscountGroups multipleDiscountGroupsObj = new Data.ViewModels.MultipleDiscountGroups();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        multipleDiscountGroupsObj.CompanyId = multipleDiscountGroup.CompanyId;
                        multipleDiscountGroupsObj.DivisionId = multipleDiscountGroup.DivisionId;
                        multipleDiscountGroupsObj.DepartmentId = multipleDiscountGroup.DepartmentId;
                        multipleDiscountGroupsObj.MultipleDiscountGroupId = multipleDiscountGroup.MultipleDiscountGroupId;
                        multipleDiscountGroupsObj.MultipleDiscountGroupDetailId = multipleDiscountGroup.MultipleDiscountGroupDetailId;
                        multipleDiscountGroupsObj.TotalPercent = multipleDiscountGroup.TotalPercent;
                        multipleDiscountGroupsObj.MultipleDiscountOnMultipleDiscount = multipleDiscountGroup.MultipleDiscountOnMultipleDiscount;
                        multipleDiscountGroupsObj.LockedBy = multipleDiscountGroup.LockedBy;
                        multipleDiscountGroupsObj.LockTs = multipleDiscountGroup.LockTs;
                        multipleDiscountGroupsObj.BranchCode = multipleDiscountGroup.BranchCode;
                        multipleDiscountGroupsObj.ItemId = multipleDiscountGroup.ItemId;
                        
                        multipleDiscountGroupsObj.WorkFlowTrail = workflows;

                        multipleDiscountGroups.Add(multipleDiscountGroupsObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return multipleDiscountGroups;

        }

        
        private async Task<List<Data.ViewModels.MultipleDiscountGroupDetail>> multipleDiscountGroupDetails(List<Data.Models.MultipleDiscountGroupDetail> obj, ApiToken token)
        {
            List<Data.ViewModels.MultipleDiscountGroupDetail> multipleDiscountGroupsDetail = new List<Data.ViewModels.MultipleDiscountGroupDetail>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.MultipleDiscountGroupDetail multipleDiscountGroupDetail in obj)
                    {
                        Data.ViewModels.MultipleDiscountGroupDetail multipleDiscountGroupDetailObj = new Data.ViewModels.MultipleDiscountGroupDetail();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        //Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        multipleDiscountGroupDetailObj.CompanyId = multipleDiscountGroupDetail.CompanyId;
                        multipleDiscountGroupDetailObj.DivisionId = multipleDiscountGroupDetail.DivisionId;
                        multipleDiscountGroupDetailObj.DepartmentId = multipleDiscountGroupDetail.DepartmentId;
                        multipleDiscountGroupDetailObj.MultipleDiscountGroupDetailId = multipleDiscountGroupDetail.MultipleDiscountGroupDetailId;
                        multipleDiscountGroupDetailObj.MultipleDiscountId = multipleDiscountGroupDetail.MultipleDiscountId;
                        multipleDiscountGroupDetailObj.GlmultipleDiscountAccount = multipleDiscountGroupDetail.GlmultipleDiscountAccount;
                        multipleDiscountGroupDetailObj.MultipleDiscountPercent = multipleDiscountGroupDetail.MultipleDiscountPercent;
                        multipleDiscountGroupDetailObj.MultipleDiscountOrder = multipleDiscountGroupDetail.MultipleDiscountOrder;
                        multipleDiscountGroupDetailObj.MultipleDiscountOnMultipleDiscount = multipleDiscountGroupDetail.MultipleDiscountOnMultipleDiscount;
                        multipleDiscountGroupDetailObj.LockedBy = multipleDiscountGroupDetail.LockedBy;
                        multipleDiscountGroupDetailObj.LockTs = multipleDiscountGroupDetail.LockTs;
                        multipleDiscountGroupDetailObj.BranchCode = multipleDiscountGroupDetail.BranchCode;

                        multipleDiscountGroupDetailObj.WorkFlowTrail = workflows;
                        multipleDiscountGroupsDetail.Add(multipleDiscountGroupDetailObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return multipleDiscountGroupsDetail;

        }

        private async Task<List<Data.ViewModels.Terms>> term(List<Data.Models.Terms> obj, ApiToken token)
        {
            List<Data.ViewModels.Terms> terms = new List<Data.ViewModels.Terms>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.Terms term in obj)
                    {
                        Data.ViewModels.Terms termsObj = new Data.ViewModels.Terms();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        //Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        termsObj.CompanyId = term.CompanyId;
                        termsObj.DivisionId = term.DivisionId;
                        termsObj.DepartmentId = term.DepartmentId;
                        termsObj.TermsId = term.TermsId;
                        termsObj.TermsDescription = term.TermsDescription;
                        termsObj.NetDays = term.NetDays;
                        termsObj.DiscountPercent = term.DiscountPercent;
                        termsObj.DiscountDays = term.DiscountDays;
                        termsObj.LockedBy = term.LockedBy;
                        termsObj.LockTs = term.LockTs;
                        termsObj.BranchCode = term.BranchCode;

                        termsObj.WorkFlowTrail = workflows;
                        terms.Add(termsObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return terms;

        }

        private async Task<List<Data.ViewModels.OrderTypes>> orderType(List<Data.Models.OrderTypes> obj, ApiToken token)
        {
            List<Data.ViewModels.OrderTypes> orderTypes = new List<Data.ViewModels.OrderTypes>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.OrderTypes orderType in obj)
                    {
                        Data.ViewModels.OrderTypes orderTypesObj = new Data.ViewModels.OrderTypes();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        //Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        orderTypesObj.CompanyId = orderType.CompanyId;
                        orderTypesObj.DivisionId = orderType.DivisionId;
                        orderTypesObj.DepartmentId = orderType.DepartmentId;
                        orderTypesObj.OrderTypeId = orderType.OrderTypeId;
                        orderTypesObj.OrderTypeDescription = orderType.OrderTypeDescription;
                        orderTypesObj.LockedBy = orderType.LockedBy;
                        orderTypesObj.LockTs = orderType.LockTs;
                        orderTypesObj.BranchCode = orderType.BranchCode;

                        orderTypesObj.WorkFlowTrail = workflows;
                        orderTypes.Add(orderTypesObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return orderTypes;

        }

        
        private async Task<List<Data.ViewModels.LedgerAnalysis>> ledgerAnalysis(List<Data.Models.LedgerAnalysis> obj, ApiToken token)
        {
            List<Data.ViewModels.LedgerAnalysis> ledgerAnalyses = new List<Data.ViewModels.LedgerAnalysis>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.LedgerAnalysis ledgerAnalysis in obj)
                    {
                        Data.ViewModels.LedgerAnalysis ledgerAnalysisObj = new Data.ViewModels.LedgerAnalysis();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        //Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        ledgerAnalysisObj.CompanyId = ledgerAnalysis.CompanyId;
                        ledgerAnalysisObj.DivisionId = ledgerAnalysis.DivisionId;
                        ledgerAnalysisObj.DepartmentId = ledgerAnalysis.DepartmentId;
                        ledgerAnalysisObj.GlanalysisType = ledgerAnalysis.GlanalysisType;
                        ledgerAnalysisObj.GlanalysisTypeDescription = ledgerAnalysis.GlanalysisTypeDescription;
                        ledgerAnalysisObj.EstimatedAmount = ledgerAnalysis.EstimatedAmount;
                        ledgerAnalysisObj.LockedBy = ledgerAnalysis.LockedBy;
                        ledgerAnalysisObj.LockTs = ledgerAnalysis.LockTs;
                        ledgerAnalysisObj.BranchCode = ledgerAnalysis.BranchCode;

                        ledgerAnalysisObj.WorkFlowTrail = workflows;
                        ledgerAnalyses.Add(ledgerAnalysisObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return ledgerAnalyses;

        }

        
        private async Task<List<Data.ViewModels.LedgerAnalysis2>> ledgerAnalysis2(List<Data.Models.LedgerAnalysis2> obj, ApiToken token)
        {
            List<Data.ViewModels.LedgerAnalysis2> ledgerAnalyses2 = new List<Data.ViewModels.LedgerAnalysis2>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.LedgerAnalysis2 ledgerAnalysis in obj)
                    {
                        Data.ViewModels.LedgerAnalysis2 ledgerAnalysisObj = new Data.ViewModels.LedgerAnalysis2();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        //Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        ledgerAnalysisObj.CompanyId = ledgerAnalysis.CompanyId;
                        ledgerAnalysisObj.DivisionId = ledgerAnalysis.DivisionId;
                        ledgerAnalysisObj.DepartmentId = ledgerAnalysis.DepartmentId;
                        ledgerAnalysisObj.GlanalysisType = ledgerAnalysis.GlanalysisType;
                        ledgerAnalysisObj.GlanalysisTypeDescription = ledgerAnalysis.GlanalysisTypeDescription;
                        ledgerAnalysisObj.EstimatedAmount = ledgerAnalysis.EstimatedAmount;
                        ledgerAnalysisObj.LockedBy = ledgerAnalysis.LockedBy;
                        ledgerAnalysisObj.LockTs = ledgerAnalysis.LockTs;
                        ledgerAnalysisObj.BranchCode = ledgerAnalysis.BranchCode;

                        ledgerAnalysisObj.WorkFlowTrail = workflows;
                        ledgerAnalyses2.Add(ledgerAnalysisObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return ledgerAnalyses2;

        }

        private async Task<List<Data.ViewModels.CashbookPaymentTypes>> paymentType(List<Data.Models.PaymentTypes> obj, ApiToken token)
        {
            List<Data.ViewModels.CashbookPaymentTypes> paymentTypes = new List<Data.ViewModels.CashbookPaymentTypes>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.PaymentTypes paymentType in obj)
                    {
                        Data.ViewModels.CashbookPaymentTypes paymentTypesObj = new Data.ViewModels.CashbookPaymentTypes();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        //Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        paymentTypesObj.CompanyId = paymentType.CompanyId;
                        paymentTypesObj.DivisionId = paymentType.DivisionId;
                        paymentTypesObj.DepartmentId = paymentType.DepartmentId;
                        paymentTypesObj.PaymentTypeId = paymentType.PaymentTypeId;
                        paymentTypesObj.PaymentTypeDescription = paymentType.PaymentTypeDescription;
                        paymentTypesObj.LockedBy = paymentType.LockedBy;
                        paymentTypesObj.LockTs = paymentType.LockTs;
                        paymentTypesObj.LastEditDate = paymentType.LastEditDate;
                        paymentTypesObj.CreationDate = paymentType.CreationDate;
                        paymentTypesObj.BranchCode = paymentType.BranchCode;

                        paymentTypesObj.WorkFlowTrail = workflows;
                        paymentTypes.Add(paymentTypesObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return paymentTypes;

        }

        
        private async Task<List<Data.ViewModels.ArtransactionTypes>> arTransactionType(List<Data.Models.ArtransactionTypes> obj, ApiToken token)
        {
            List<Data.ViewModels.ArtransactionTypes> artransactionTypes = new List<Data.ViewModels.ArtransactionTypes>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.ArtransactionTypes artransactionType in obj)
                    {
                        Data.ViewModels.ArtransactionTypes artransactionTypeObj = new Data.ViewModels.ArtransactionTypes();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        //Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        artransactionTypeObj.CompanyId = artransactionType.CompanyId;
                        artransactionTypeObj.DivisionId = artransactionType.DivisionId;
                        artransactionTypeObj.DepartmentId = artransactionType.DepartmentId;
                        artransactionTypeObj.TransactionTypeId = artransactionType.TransactionTypeId;
                        artransactionTypeObj.TransactionDescription = artransactionType.TransactionDescription;
                        artransactionTypeObj.LockedBy = artransactionType.LockedBy;
                        artransactionTypeObj.LockTs = artransactionType.LockTs;
                        artransactionTypeObj.BranchCode = artransactionType.BranchCode;

                        artransactionTypeObj.WorkFlowTrail = workflows;
                        artransactionTypes.Add(artransactionTypeObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return artransactionTypes;

        }


        

        private async Task<List<Data.ViewModels.CreditCardTypes>> creditCardType(List<Data.Models.CreditCardTypes> obj, ApiToken token)
        {
            List<Data.ViewModels.CreditCardTypes> creditCardTypes = new List<Data.ViewModels.CreditCardTypes>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.CreditCardTypes creditCardType in obj)
                    {
                        Data.ViewModels.CreditCardTypes creditCardTypesObj = new Data.ViewModels.CreditCardTypes();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        //Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        creditCardTypesObj.CompanyId = creditCardType.CompanyId;
                        creditCardTypesObj.DivisionId = creditCardType.DivisionId;
                        creditCardTypesObj.DepartmentId = creditCardType.DepartmentId;
                        creditCardTypesObj.CreditCardTypeId = creditCardType.CreditCardTypeId;
                        creditCardTypesObj.CreditCardDescription = creditCardType.CreditCardDescription;
                        creditCardTypesObj.LockedBy = creditCardType.LockedBy;
                        creditCardTypesObj.LockTs = creditCardType.LockTs;
                        creditCardTypesObj.BranchCode = creditCardType.BranchCode;
   
                        creditCardTypesObj.WorkFlowTrail = workflows;
                        creditCardTypes.Add(creditCardTypesObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return creditCardTypes;

        }






        #endregion Build View Model List Data


    }

}



    

    
    
  
 






    




