using Microsoft.EntityFrameworkCore;
using PowerAPI.Data.IRepository;
using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using PowerAPI.Data.ViewModels;
using PowerAPI.Service.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Service.Clients
{
    public class Banks : IBanks
    {
        EnterpriseContext _DBContext;

        public Banks(EnterpriseContext DBContext)
        {
            _DBContext = DBContext;
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

        public async Task<Paging> GetCashbookBanks(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.BankAccounts> bankAccounts = new List<Data.ViewModels.BankAccounts>();
            try
            {
                var totalCount = await _DBContext.BankAccounts
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.BankAccounts.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.BankAccountTypeId != "Salary")//.ToListAsync();
                                                        .OrderBy(x => x.BankId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                bankAccounts = await bankAccount(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    BankAccountsList = bankAccounts
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        #region View Model Data List

        private async Task<List<Data.ViewModels.BankAccounts>> bankAccount(List<Data.Models.BankAccounts> obj, ApiToken token)
        {
            List<Data.ViewModels.BankAccounts> bankAccounts = new List<Data.ViewModels.BankAccounts>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.BankAccounts bankAccount in obj)
                    {
                        Data.ViewModels.BankAccounts bankAccountsObj = new Data.ViewModels.BankAccounts();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        bankAccountsObj.CompanyId = bankAccount.CompanyId;
                        bankAccountsObj.DivisionId = bankAccount.DivisionId;
                        bankAccountsObj.DepartmentId = bankAccount.DepartmentId;
                        bankAccountsObj.BankId = bankAccount.BankId;
                        bankAccountsObj.BankAccountNumber = bankAccount.BankAccountNumber;
                        bankAccountsObj.BankName = bankAccount.BankName;
                        bankAccountsObj.BankAddress1 = bankAccount.BankAddress1;
                        bankAccountsObj.BankAddress2 = bankAccount.BankAddress2;
                        bankAccountsObj.BankCity = bankAccount.BankCity;
                        bankAccountsObj.BankState = bankAccount.BankState;
                        bankAccountsObj.BankZip = bankAccount.BankZip;
                        bankAccountsObj.BankCountry = bankAccount.BankCountry;
                        bankAccountsObj.BankPhone = bankAccount.BankPhone;
                        bankAccountsObj.BankFax = bankAccount.BankFax;
                        bankAccountsObj.BankContactName = bankAccount.BankContactName;
                        bankAccountsObj.BankEmail = bankAccount.BankEmail;
                        bankAccountsObj.BankWebsite = bankAccount.BankWebsite;
                        bankAccountsObj.SwiftCode = bankAccount.SwiftCode;
                        bankAccountsObj.RoutingCode = bankAccount.RoutingCode;
                        bankAccountsObj.CurrencyId = bankAccount.CurrencyId;
                        bankAccountsObj.CurrencyExchangeRate = bankAccount.CurrencyExchangeRate;
                        bankAccountsObj.NextCheckNumber = bankAccount.NextCheckNumber;
                        bankAccountsObj.NextDepositNumber = bankAccount.NextDepositNumber;
                        bankAccountsObj.Balance = bankAccount.Balance;
                        bankAccountsObj.UnpostedDeposits = bankAccount.UnpostedDeposits;
                        bankAccountsObj.GlbankAccount = bankAccount.GlbankAccount;
                        bankAccountsObj.Notes = bankAccount.Notes;
                        bankAccountsObj.CorrespondentBankId = bankAccount.CorrespondentBankId;
                        bankAccountsObj.Approved = bankAccount.Approved;
                        bankAccountsObj.ApprovedBy = bankAccount.ApprovedBy;
                        bankAccountsObj.ApprovedDate = bankAccount.ApprovedDate;
                        bankAccountsObj.EnteredBy = bankAccount.EnteredBy;
                        bankAccountsObj.LockedBy = bankAccount.LockedBy;
                        bankAccountsObj.LockTs = bankAccount.LockTs;
                        bankAccountsObj.BankAccountTypeId = bankAccount.BankAccountTypeId;
                        bankAccountsObj.ChequeDate = bankAccount.ChequeDate;
                        bankAccountsObj.ChequeNo = bankAccount.ChequeNo;
                        bankAccountsObj.FirstSign = bankAccount.FirstSign;
                        bankAccountsObj.SecondSign = bankAccount.SecondSign;
                        bankAccountsObj.FirstDesign = bankAccount.FirstDesign;
                        bankAccountsObj.SecondDesign = bankAccount.SecondDesign;
                        bankAccountsObj.BranchCode = bankAccount.BranchCode;

                        bankAccountsObj.WorkFlowTrail = workflows;
                        bankAccounts.Add(bankAccountsObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return bankAccounts;

        }

        #endregion View Model Data List
    }
}
