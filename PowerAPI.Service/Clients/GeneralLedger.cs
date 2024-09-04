using DevExpress.Data.Linq.Helpers;
using DevExpress.DataAccess.Wizard.Presenters;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
    public class GeneralLedger : IGeneralLedger
    {
        EnterpriseContext _DBContext;

        public GeneralLedger(EnterpriseContext DBContext)
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

        public async Task<Paging> GetCOA(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.LedgerChartOfAccounts> ledgerChartOfAccounts = new List<Data.ViewModels.LedgerChartOfAccounts>();
            try
            {
                var totalCount = await _DBContext.LedgerChartOfAccounts
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId
                                                        ).CountAsync();

                var result = await _DBContext.LedgerChartOfAccounts.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.GlaccountNumber)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                ledgerChartOfAccounts = await ledgerChartOfAccount(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    LedgerChartOfAccountList = ledgerChartOfAccounts
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetCOAByAccountType(PaginationParams Param, string accountType, ApiToken token)
        {
            List<Data.ViewModels.LedgerChartOfAccounts> ledgerChartOfAccounts = new List<Data.ViewModels.LedgerChartOfAccounts>();
            try
            {
                var totalCount = await _DBContext.LedgerChartOfAccounts
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.GlaccountType == accountType
                                                        ).CountAsync();

                var result = await _DBContext.LedgerChartOfAccounts.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.GlaccountType == accountType)//.ToListAsync();
                                                        .OrderBy(x => x.GlaccountNumber)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                ledgerChartOfAccounts = await ledgerChartOfAccount(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    LedgerChartOfAccountList = ledgerChartOfAccounts
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetCOAById(PaginationParams Param, string Id, ApiToken token)
        {
            List<Data.ViewModels.LedgerChartOfAccounts> ledgerChartOfAccounts = new List<Data.ViewModels.LedgerChartOfAccounts>();
            try
            {
                var totalCount = await _DBContext.LedgerChartOfAccounts
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.GlaccountNumber == Id
                                                        ).CountAsync();

                var result = await _DBContext.LedgerChartOfAccounts.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.GlaccountNumber == Id)//.ToListAsync();
                                                        .OrderBy(x => x.GlaccountNumber)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                ledgerChartOfAccounts = await ledgerChartOfAccount(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    LedgerChartOfAccountList = ledgerChartOfAccounts
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetCOAByName(PaginationParams Param, string name, ApiToken token)
        {
            List<Data.ViewModels.LedgerChartOfAccounts> ledgerChartOfAccounts = new List<Data.ViewModels.LedgerChartOfAccounts>();
            try
            {
                var totalCount = await _DBContext.LedgerChartOfAccounts
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.GlaccountName == name
                                                        ).CountAsync();

                var result = await _DBContext.LedgerChartOfAccounts.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.GlaccountName == name)//.ToListAsync();
                                                        .OrderBy(x => x.GlaccountNumber)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                ledgerChartOfAccounts = await ledgerChartOfAccount(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    LedgerChartOfAccountList = ledgerChartOfAccounts
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetCOAForeign(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.LedgerChartOfAccounts> ledgerChartOfAccounts = new List<Data.ViewModels.LedgerChartOfAccounts>();
            try
            {
                var totalCount = await _DBContext.LedgerChartOfAccountsForeign
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId
                                                        ).CountAsync();

                var result = await _DBContext.LedgerChartOfAccountsForeign.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.GlaccountNumber)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                ledgerChartOfAccounts = await ledgerChartOfAccountForeign(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    LedgerChartOfAccountList = ledgerChartOfAccounts
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        #region View Model Data List

        private async Task<List<Data.ViewModels.LedgerChartOfAccounts>> ledgerChartOfAccount(List<Data.Models.LedgerChartOfAccounts> obj, ApiToken token)
        {
            List<Data.ViewModels.LedgerChartOfAccounts> ledgerChartOfAccounts = new List<Data.ViewModels.LedgerChartOfAccounts>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.LedgerChartOfAccounts ledgerChartOfAccount in obj)
                    {
                        Data.ViewModels.LedgerChartOfAccounts ledgerChartOfAccountObj = new Data.ViewModels.LedgerChartOfAccounts();

                        var workflows = new List<TransactionWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<TransactionWorkflow> { };

                        ledgerChartOfAccountObj.CompanyId = ledgerChartOfAccount.CompanyId;
                        ledgerChartOfAccountObj.DivisionId = ledgerChartOfAccount.DivisionId;
                        ledgerChartOfAccountObj.DepartmentId = ledgerChartOfAccount.DepartmentId;
                        ledgerChartOfAccountObj.GlaccountNumber = ledgerChartOfAccount.GlaccountNumber;
                        ledgerChartOfAccountObj.GlaccountName = ledgerChartOfAccount.GlaccountName;
                        ledgerChartOfAccountObj.GlaccountDescription = ledgerChartOfAccount.GlaccountDescription;
                        ledgerChartOfAccountObj.GlaccountType = ledgerChartOfAccount.GlaccountType;
                        ledgerChartOfAccountObj.GlbalanceType = ledgerChartOfAccount.GlbalanceType;
                        ledgerChartOfAccountObj.GlreportingAccount = ledgerChartOfAccount.GlreportingAccount;
                        ledgerChartOfAccountObj.GlreportLevel = ledgerChartOfAccount.GlreportLevel;
                        ledgerChartOfAccountObj.CurrencyId = ledgerChartOfAccount.CurrencyId;
                        ledgerChartOfAccountObj.CurrencyExchangeRate = ledgerChartOfAccount.CurrencyExchangeRate;
                        ledgerChartOfAccountObj.GlaccountBalance = ledgerChartOfAccount.GlaccountBalance;
                        ledgerChartOfAccountObj.GlaccountBeginningBalance = ledgerChartOfAccount.GlaccountBeginningBalance;
                        ledgerChartOfAccountObj.GlotherNotes = ledgerChartOfAccount.GlotherNotes;
                        ledgerChartOfAccountObj.GlbudgetId = ledgerChartOfAccount.GlbudgetId;
                        ledgerChartOfAccountObj.GlcurrentYearBeginningBalance = ledgerChartOfAccount.GlcurrentYearBeginningBalance;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod1 = ledgerChartOfAccount.GlcurrentYearPeriod1;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod2 = ledgerChartOfAccount.GlcurrentYearPeriod2;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod3 = ledgerChartOfAccount.GlcurrentYearPeriod3;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod4 = ledgerChartOfAccount.GlcurrentYearPeriod4;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod5 = ledgerChartOfAccount.GlcurrentYearPeriod5;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod6 = ledgerChartOfAccount.GlcurrentYearPeriod6;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod7 = ledgerChartOfAccount.GlcurrentYearPeriod7;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod8 = ledgerChartOfAccount.GlcurrentYearPeriod8;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod9 = ledgerChartOfAccount.GlcurrentYearPeriod9;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod10 = ledgerChartOfAccount.GlcurrentYearPeriod10;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod11 = ledgerChartOfAccount.GlcurrentYearPeriod11;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod12 = ledgerChartOfAccount.GlcurrentYearPeriod12;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod13 = ledgerChartOfAccount.GlcurrentYearPeriod13;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod14 = ledgerChartOfAccount.GlcurrentYearPeriod14;
                        ledgerChartOfAccountObj.GlbudgetBeginningBalance = ledgerChartOfAccount.GlbudgetBeginningBalance;
                        ledgerChartOfAccountObj.GlbudgetPeriod1 = ledgerChartOfAccount.GlbudgetPeriod1;
                        ledgerChartOfAccountObj.GlbudgetPeriod2 = ledgerChartOfAccount.GlbudgetPeriod2;
                        ledgerChartOfAccountObj.GlbudgetPeriod3 = ledgerChartOfAccount.GlbudgetPeriod3;
                        ledgerChartOfAccountObj.GlbudgetPeriod4 = ledgerChartOfAccount.GlbudgetPeriod4;
                        ledgerChartOfAccountObj.GlbudgetPeriod5 = ledgerChartOfAccount.GlbudgetPeriod5;
                        ledgerChartOfAccountObj.GlbudgetPeriod6 = ledgerChartOfAccount.GlbudgetPeriod6;
                        ledgerChartOfAccountObj.GlbudgetPeriod7 = ledgerChartOfAccount.GlbudgetPeriod7;
                        ledgerChartOfAccountObj.GlbudgetPeriod8 = ledgerChartOfAccount.GlbudgetPeriod8;
                        ledgerChartOfAccountObj.GlbudgetPeriod9 = ledgerChartOfAccount.GlbudgetPeriod9;
                        ledgerChartOfAccountObj.GlbudgetPeriod10 = ledgerChartOfAccount.GlbudgetPeriod10;
                        ledgerChartOfAccountObj.GlbudgetPeriod11 = ledgerChartOfAccount.GlbudgetPeriod11;
                        ledgerChartOfAccountObj.GlbudgetPeriod12 = ledgerChartOfAccount.GlbudgetPeriod12;
                        ledgerChartOfAccountObj.GlbudgetPeriod13 = ledgerChartOfAccount.GlbudgetPeriod13;
                        ledgerChartOfAccountObj.GlbudgetPeriod14 = ledgerChartOfAccount.GlbudgetPeriod14;
                        ledgerChartOfAccountObj.GlpriorFiscalYear = ledgerChartOfAccount.GlpriorFiscalYear;
                        ledgerChartOfAccountObj.GlpriorYearBeginningBalance = ledgerChartOfAccount.GlpriorYearBeginningBalance;
                        ledgerChartOfAccountObj.GlpriorYearPeriod1 = ledgerChartOfAccount.GlpriorYearPeriod1;
                        ledgerChartOfAccountObj.GlpriorYearPeriod2 = ledgerChartOfAccount.GlpriorYearPeriod2;
                        ledgerChartOfAccountObj.GlpriorYearPeriod3 = ledgerChartOfAccount.GlpriorYearPeriod3;
                        ledgerChartOfAccountObj.GlpriorYearPeriod4 = ledgerChartOfAccount.GlpriorYearPeriod4;
                        ledgerChartOfAccountObj.GlpriorYearPeriod5 = ledgerChartOfAccount.GlpriorYearPeriod5;
                        ledgerChartOfAccountObj.GlpriorYearPeriod6 = ledgerChartOfAccount.GlpriorYearPeriod6;
                        ledgerChartOfAccountObj.GlpriorYearPeriod7 = ledgerChartOfAccount.GlpriorYearPeriod7;
                        ledgerChartOfAccountObj.GlpriorYearPeriod8 = ledgerChartOfAccount.GlpriorYearPeriod8;
                        ledgerChartOfAccountObj.GlpriorYearPeriod9 = ledgerChartOfAccount.GlpriorYearPeriod9;
                        ledgerChartOfAccountObj.GlpriorYearPeriod10 = ledgerChartOfAccount.GlpriorYearPeriod10;
                        ledgerChartOfAccountObj.GlpriorYearPeriod11 = ledgerChartOfAccount.GlpriorYearPeriod11;
                        ledgerChartOfAccountObj.GlpriorYearPeriod12 = ledgerChartOfAccount.GlpriorYearPeriod12;
                        ledgerChartOfAccountObj.GlpriorYearPeriod13 = ledgerChartOfAccount.GlpriorYearPeriod13;
                        ledgerChartOfAccountObj.GlpriorYearPeriod14 = ledgerChartOfAccount.GlpriorYearPeriod14;
                        ledgerChartOfAccountObj.LockedBy = ledgerChartOfAccount.LockedBy;
                        ledgerChartOfAccountObj.LockTs = ledgerChartOfAccount.LockTs;
                        ledgerChartOfAccountObj.GlsubAccount = ledgerChartOfAccount.GlsubAccount;
                        ledgerChartOfAccountObj.ControlAccount = ledgerChartOfAccount.ControlAccount;
                        ledgerChartOfAccountObj.CashFlowType = ledgerChartOfAccount.CashFlowType;
                        ledgerChartOfAccountObj.CashFlowItem = ledgerChartOfAccount.CashFlowItem;
                        ledgerChartOfAccountObj.IsHeaderAccount = ledgerChartOfAccount.IsHeaderAccount;
                        ledgerChartOfAccountObj.SubHeaderAccount = ledgerChartOfAccount.SubHeaderAccount;
                        ledgerChartOfAccountObj.BranchCode = ledgerChartOfAccount.BranchCode;
                        ledgerChartOfAccountObj.HeaderAccountName = ledgerChartOfAccount.HeaderAccountName;
                       

                        ledgerChartOfAccountObj.WorkFlowTrail = workflows;
                        ledgerChartOfAccountObj.ledgerChartOfAccountsBudgets = await _DBContext.LedgerChartOfAccountsBudgets.Where(x => x.CompanyId == token.CompanyId &&
                                                    x.DivisionId == token.DivisionId &&
                                                    x.DepartmentId == token.DepartmentId &&
                                                    x.GlaccountNumber == ledgerChartOfAccount.GlaccountNumber).ToListAsync();

                        ledgerChartOfAccountObj.ledgerChartOfAccountsPriorYears = await _DBContext.LedgerChartOfAccountsPriorYears.Where(x => x.CompanyId == token.CompanyId &&
                                                    x.DivisionId == token.DivisionId &&
                                                    x.DepartmentId == token.DepartmentId &&
                                                    x.GlaccountNumber == ledgerChartOfAccount.GlaccountNumber).ToListAsync();

                        ledgerChartOfAccounts.Add(ledgerChartOfAccountObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return ledgerChartOfAccounts;

        }

        private async Task<List<Data.ViewModels.LedgerChartOfAccounts>> ledgerChartOfAccountForeign(List<Data.Models.LedgerChartOfAccountsForeign> obj, ApiToken token)
        {
            List<Data.ViewModels.LedgerChartOfAccounts> ledgerChartOfAccounts = new List<Data.ViewModels.LedgerChartOfAccounts>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.LedgerChartOfAccountsForeign ledgerChartOfAccount in obj)
                    {
                        Data.ViewModels.LedgerChartOfAccounts ledgerChartOfAccountObj = new Data.ViewModels.LedgerChartOfAccounts();

                        var workflows = new List<TransactionWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<TransactionWorkflow> { };

                        ledgerChartOfAccountObj.CompanyId = ledgerChartOfAccount.CompanyId;
                        ledgerChartOfAccountObj.DivisionId = ledgerChartOfAccount.DivisionId;
                        ledgerChartOfAccountObj.DepartmentId = ledgerChartOfAccount.DepartmentId;
                        ledgerChartOfAccountObj.GlaccountNumber = ledgerChartOfAccount.GlaccountNumber;
                        ledgerChartOfAccountObj.GlaccountName = ledgerChartOfAccount.GlaccountName;
                        ledgerChartOfAccountObj.GlaccountDescription = ledgerChartOfAccount.GlaccountDescription;
                        ledgerChartOfAccountObj.GlaccountType = ledgerChartOfAccount.GlaccountType;
                        ledgerChartOfAccountObj.GlbalanceType = ledgerChartOfAccount.GlbalanceType;
                        ledgerChartOfAccountObj.GlreportingAccount = ledgerChartOfAccount.GlreportingAccount;
                        ledgerChartOfAccountObj.GlreportLevel = ledgerChartOfAccount.GlreportLevel;
                        ledgerChartOfAccountObj.CurrencyId = ledgerChartOfAccount.CurrencyId;
                        ledgerChartOfAccountObj.CurrencyExchangeRate = ledgerChartOfAccount.CurrencyExchangeRate;
                        ledgerChartOfAccountObj.GlaccountBalance = ledgerChartOfAccount.GlaccountBalance;
                        ledgerChartOfAccountObj.GlaccountBeginningBalance = ledgerChartOfAccount.GlaccountBeginningBalance;
                        ledgerChartOfAccountObj.GlotherNotes = ledgerChartOfAccount.GlotherNotes;
                        ledgerChartOfAccountObj.GlbudgetId = ledgerChartOfAccount.GlbudgetId;
                        ledgerChartOfAccountObj.GlcurrentYearBeginningBalance = ledgerChartOfAccount.GlcurrentYearBeginningBalance;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod1 = ledgerChartOfAccount.GlcurrentYearPeriod1;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod2 = ledgerChartOfAccount.GlcurrentYearPeriod2;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod3 = ledgerChartOfAccount.GlcurrentYearPeriod3;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod4 = ledgerChartOfAccount.GlcurrentYearPeriod4;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod5 = ledgerChartOfAccount.GlcurrentYearPeriod5;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod6 = ledgerChartOfAccount.GlcurrentYearPeriod6;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod7 = ledgerChartOfAccount.GlcurrentYearPeriod7;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod8 = ledgerChartOfAccount.GlcurrentYearPeriod8;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod9 = ledgerChartOfAccount.GlcurrentYearPeriod9;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod10 = ledgerChartOfAccount.GlcurrentYearPeriod10;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod11 = ledgerChartOfAccount.GlcurrentYearPeriod11;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod12 = ledgerChartOfAccount.GlcurrentYearPeriod12;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod13 = ledgerChartOfAccount.GlcurrentYearPeriod13;
                        ledgerChartOfAccountObj.GlcurrentYearPeriod14 = ledgerChartOfAccount.GlcurrentYearPeriod14;
                        ledgerChartOfAccountObj.GlbudgetBeginningBalance = ledgerChartOfAccount.GlbudgetBeginningBalance;
                        ledgerChartOfAccountObj.GlbudgetPeriod1 = ledgerChartOfAccount.GlbudgetPeriod1;
                        ledgerChartOfAccountObj.GlbudgetPeriod2 = ledgerChartOfAccount.GlbudgetPeriod2;
                        ledgerChartOfAccountObj.GlbudgetPeriod3 = ledgerChartOfAccount.GlbudgetPeriod3;
                        ledgerChartOfAccountObj.GlbudgetPeriod4 = ledgerChartOfAccount.GlbudgetPeriod4;
                        ledgerChartOfAccountObj.GlbudgetPeriod5 = ledgerChartOfAccount.GlbudgetPeriod5;
                        ledgerChartOfAccountObj.GlbudgetPeriod6 = ledgerChartOfAccount.GlbudgetPeriod6;
                        ledgerChartOfAccountObj.GlbudgetPeriod7 = ledgerChartOfAccount.GlbudgetPeriod7;
                        ledgerChartOfAccountObj.GlbudgetPeriod8 = ledgerChartOfAccount.GlbudgetPeriod8;
                        ledgerChartOfAccountObj.GlbudgetPeriod9 = ledgerChartOfAccount.GlbudgetPeriod9;
                        ledgerChartOfAccountObj.GlbudgetPeriod10 = ledgerChartOfAccount.GlbudgetPeriod10;
                        ledgerChartOfAccountObj.GlbudgetPeriod11 = ledgerChartOfAccount.GlbudgetPeriod11;
                        ledgerChartOfAccountObj.GlbudgetPeriod12 = ledgerChartOfAccount.GlbudgetPeriod12;
                        ledgerChartOfAccountObj.GlbudgetPeriod13 = ledgerChartOfAccount.GlbudgetPeriod13;
                        ledgerChartOfAccountObj.GlbudgetPeriod14 = ledgerChartOfAccount.GlbudgetPeriod14;
                        ledgerChartOfAccountObj.GlpriorFiscalYear = ledgerChartOfAccount.GlpriorFiscalYear;
                        ledgerChartOfAccountObj.GlpriorYearBeginningBalance = ledgerChartOfAccount.GlpriorYearBeginningBalance;
                        ledgerChartOfAccountObj.GlpriorYearPeriod1 = ledgerChartOfAccount.GlpriorYearPeriod1;
                        ledgerChartOfAccountObj.GlpriorYearPeriod2 = ledgerChartOfAccount.GlpriorYearPeriod2;
                        ledgerChartOfAccountObj.GlpriorYearPeriod3 = ledgerChartOfAccount.GlpriorYearPeriod3;
                        ledgerChartOfAccountObj.GlpriorYearPeriod4 = ledgerChartOfAccount.GlpriorYearPeriod4;
                        ledgerChartOfAccountObj.GlpriorYearPeriod5 = ledgerChartOfAccount.GlpriorYearPeriod5;
                        ledgerChartOfAccountObj.GlpriorYearPeriod6 = ledgerChartOfAccount.GlpriorYearPeriod6;
                        ledgerChartOfAccountObj.GlpriorYearPeriod7 = ledgerChartOfAccount.GlpriorYearPeriod7;
                        ledgerChartOfAccountObj.GlpriorYearPeriod8 = ledgerChartOfAccount.GlpriorYearPeriod8;
                        ledgerChartOfAccountObj.GlpriorYearPeriod9 = ledgerChartOfAccount.GlpriorYearPeriod9;
                        ledgerChartOfAccountObj.GlpriorYearPeriod10 = ledgerChartOfAccount.GlpriorYearPeriod10;
                        ledgerChartOfAccountObj.GlpriorYearPeriod11 = ledgerChartOfAccount.GlpriorYearPeriod11;
                        ledgerChartOfAccountObj.GlpriorYearPeriod12 = ledgerChartOfAccount.GlpriorYearPeriod12;
                        ledgerChartOfAccountObj.GlpriorYearPeriod13 = ledgerChartOfAccount.GlpriorYearPeriod13;
                        ledgerChartOfAccountObj.GlpriorYearPeriod14 = ledgerChartOfAccount.GlpriorYearPeriod14;
                        ledgerChartOfAccountObj.LockedBy = ledgerChartOfAccount.LockedBy;
                        ledgerChartOfAccountObj.LockTs = ledgerChartOfAccount.LockTs;
                        ledgerChartOfAccountObj.GlsubAccount = ledgerChartOfAccount.GlsubAccount;
                        ledgerChartOfAccountObj.ControlAccount = ledgerChartOfAccount.ControlAccount;
                        //ledgerChartOfAccountObj.CashFlowType = ledgerChartOfAccount.CashFlowType;
                        //ledgerChartOfAccountObj.CashFlowItem = ledgerChartOfAccount.CashFlowItem;
                        ledgerChartOfAccountObj.IsHeaderAccount = ledgerChartOfAccount.IsHeaderAccount;
                        ledgerChartOfAccountObj.SubHeaderAccount = ledgerChartOfAccount.SubHeaderAccount;
                        ledgerChartOfAccountObj.BranchCode = ledgerChartOfAccount.BranchCode;
                        //ledgerChartOfAccountObj.HeaderAccountName = ledgerChartOfAccount.HeaderAccountName;


                        ledgerChartOfAccountObj.WorkFlowTrail = workflows;
                        ledgerChartOfAccountObj.ledgerChartOfAccountsBudgets = await _DBContext.LedgerChartOfAccountsBudgets.Where(x => x.CompanyId == token.CompanyId &&
                                                    x.DivisionId == token.DivisionId &&
                                                    x.DepartmentId == token.DepartmentId &&
                                                    x.GlaccountNumber == ledgerChartOfAccount.GlaccountNumber).ToListAsync();

                        ledgerChartOfAccountObj.ledgerChartOfAccountsPriorYears = await _DBContext.LedgerChartOfAccountsPriorYears.Where(x => x.CompanyId == token.CompanyId &&
                                                    x.DivisionId == token.DivisionId &&
                                                    x.DepartmentId == token.DepartmentId &&
                                                    x.GlaccountNumber == ledgerChartOfAccount.GlaccountNumber).ToListAsync();

                        ledgerChartOfAccounts.Add(ledgerChartOfAccountObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return ledgerChartOfAccounts;

        }

        #endregion View Model Data List
    }
}
