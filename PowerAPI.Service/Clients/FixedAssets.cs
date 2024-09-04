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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Service.Clients
{
    public class FixedAssets : IFixedAssets
    {
        EnterpriseContext _DBContext;

        public FixedAssets(EnterpriseContext DBContext)
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

        public async Task<Paging> GetFixedAssets(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.FixedAssets> fixedAssets = new List<Data.ViewModels.FixedAssets>();
            try
            {
                var totalCount = await _DBContext.FixedAssets
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId
                                                        ).CountAsync();

                var result = await _DBContext.FixedAssets.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.AssetId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                fixedAssets = await fixedAsset(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    FixedAssetsList = fixedAssets
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetFixedAssetsById(PaginationParams Param, string Id, ApiToken token)
        {
            List<Data.ViewModels.FixedAssets> fixedAssets = new List<Data.ViewModels.FixedAssets>();
            try
            {
                var totalCount = await _DBContext.FixedAssets
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.AssetId == Id
                                                        ).CountAsync();

                var result = await _DBContext.FixedAssets.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.AssetId == Id)//.ToListAsync();
                                                        .OrderBy(x => x.AssetId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                fixedAssets = await fixedAsset(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    FixedAssetsList = fixedAssets
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetFixedAssetsByName(PaginationParams Param, string name, ApiToken token)
        {
            List<Data.ViewModels.FixedAssets> fixedAssets = new List<Data.ViewModels.FixedAssets>();
            try
            {
                var totalCount = await _DBContext.FixedAssets
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.AssetName == name
                                                        ).CountAsync();

                var result = await _DBContext.FixedAssets.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.AssetName == name)//.ToListAsync();
                                                        .OrderBy(x => x.AssetId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                fixedAssets = await fixedAsset(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    FixedAssetsList = fixedAssets
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetFixedAssetsByStatus(PaginationParams Param, string status, ApiToken token)
        {
            List<Data.ViewModels.FixedAssets> fixedAssets = new List<Data.ViewModels.FixedAssets>();
            try
            {
                var totalCount = await _DBContext.FixedAssets
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.AssetStatusId == status
                                                        ).CountAsync();

                var result = await _DBContext.FixedAssets.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.AssetStatusId == status)//.ToListAsync();
                                                        .OrderBy(x => x.AssetId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                fixedAssets = await fixedAsset(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    FixedAssetsList = fixedAssets
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetFixedAssetsByType(PaginationParams Param, string assetType, ApiToken token)
        {
            List<Data.ViewModels.FixedAssets> fixedAssets = new List<Data.ViewModels.FixedAssets>();
            try
            {
                var totalCount = await _DBContext.FixedAssets
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.AssetTypeId == assetType
                                                        ).CountAsync();

                var result = await _DBContext.FixedAssets.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.AssetTypeId == assetType)//.ToListAsync();
                                                        .OrderBy(x => x.AssetId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                fixedAssets = await fixedAsset(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    FixedAssetsList = fixedAssets
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        #region View Model Data List

        private async Task<List<Data.ViewModels.FixedAssets>> fixedAsset(List<Data.Models.FixedAssets> obj, ApiToken token)
        {
            List<Data.ViewModels.FixedAssets> fixedAssets = new List<Data.ViewModels.FixedAssets>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.FixedAssets fixedAsset in obj)
                    {
                        Data.ViewModels.FixedAssets fixedAssetsObj = new Data.ViewModels.FixedAssets();

                        var workflows = new List<TransactionWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<TransactionWorkflow>
                                            {
                                                new TransactionWorkflow
                                                {
                                                    TransactionId = fixedAsset.AssetId,
                                                    TransactionDate = fixedAsset.AssetInServiceDate ?? currentDate,
                                                    StepSequence = 1,
                                                    Status = "Draft",
                                                    IsCompleted = false,
                                                    DateCompleted = null
                                                },
                                                new TransactionWorkflow
                                                {
                                                    TransactionId = fixedAsset.AssetId,
                                                    TransactionDate = fixedAsset.AssetInServiceDate ?? currentDate,
                                                    StepSequence = 2,
                                                    Status = "Active",
                                                    IsCompleted = false,
                                                    DateCompleted = fixedAsset.AssetInServiceDate?? null
                                                },
                                                new TransactionWorkflow
                                                {
                                                    TransactionId = fixedAsset.AssetId,
                                                    TransactionDate = fixedAsset.AssetInServiceDate ?? currentDate,
                                                    StepSequence = 3,
                                                    Status = "Disposed",
                                                    IsCompleted = true,
                                                    DateCompleted = fixedAsset.AssetAcutalDisposalDate?? null
                                                }
                                            };

                        var lastCompletedWorkflow = workflows
                                                        .Where(ow => ow.IsCompleted)
                                                        .OrderBy(workflow => workflow.StepSequence)
                                                        .LastOrDefault();

                        fixedAssetsObj.CompanyId = fixedAsset.CompanyId;
                        fixedAssetsObj.DivisionId = fixedAsset.DivisionId;
                        fixedAssetsObj.DepartmentId = fixedAsset.DepartmentId;
                        fixedAssetsObj.AssetId = fixedAsset.AssetId;
                        fixedAssetsObj.AssetName = fixedAsset.AssetName;
                        fixedAssetsObj.AssetSerialNumber = fixedAsset.AssetSerialNumber;
                        fixedAssetsObj.AssetTypeId = fixedAsset.AssetTypeId;
                        fixedAssetsObj.AssetStatusId = fixedAsset.AssetStatusId;
                        fixedAssetsObj.VendorId = fixedAsset.VendorId;
                        fixedAssetsObj.AssetDescription = fixedAsset.AssetDescription;
                        fixedAssetsObj.AssetLocation = fixedAsset.AssetLocation;
                        fixedAssetsObj.AssetUsedBy = fixedAsset.AssetUsedBy;
                        fixedAssetsObj.AssetDepreciationMethodId = fixedAsset.AssetDepreciationMethodId;
                        fixedAssetsObj.AssetInServiceDate = fixedAsset.AssetInServiceDate;
                        fixedAssetsObj.CurrencyId = fixedAsset.CurrencyId;
                        fixedAssetsObj.CurrencyExchangeRate = fixedAsset.CurrencyExchangeRate;
                        fixedAssetsObj.AssetOriginalCost = fixedAsset.AssetOriginalCost;
                        fixedAssetsObj.AssetUsefulLife = fixedAsset.AssetUsefulLife;
                        fixedAssetsObj.AssetSalvageValue = fixedAsset.AssetSalvageValue;
                        fixedAssetsObj.AssetSalesPrice = fixedAsset.AssetSalesPrice;
                        fixedAssetsObj.AssetPlanedDisposalDate = fixedAsset.AssetPlanedDisposalDate;
                        fixedAssetsObj.AssetAcutalDisposalDate = fixedAsset.AssetAcutalDisposalDate;
                        fixedAssetsObj.AssetActualDisposalAmount = fixedAsset.AssetActualDisposalAmount;
                        fixedAssetsObj.LastDepreciationDate = fixedAsset.LastDepreciationDate;
                        fixedAssetsObj.LastDepreciationAmount = fixedAsset.LastDepreciationAmount;
                        fixedAssetsObj.AccumulatedDepreciation = fixedAsset.AccumulatedDepreciation;
                        fixedAssetsObj.DepreciationPeriod = fixedAsset.DepreciationPeriod;
                        fixedAssetsObj.AssetBookValue = fixedAsset.AssetBookValue;
                        fixedAssetsObj.GlfixedAssetAccount = fixedAsset.GlfixedAssetAccount;
                        fixedAssetsObj.GlfixedDepreciationAccount = fixedAsset.GlfixedDepreciationAccount;
                        fixedAssetsObj.GlfixedAccumDepreciationAccount = fixedAsset.GlfixedAccumDepreciationAccount;
                        fixedAssetsObj.GlfixedDisposalAccount = fixedAsset.GlfixedDisposalAccount;
                        fixedAssetsObj.Approved = fixedAsset.Approved;
                        fixedAssetsObj.ApprovedBy = fixedAsset.ApprovedBy;
                        fixedAssetsObj.ApprovedDate = fixedAsset.ApprovedDate;
                        fixedAssetsObj.EnteredBy = fixedAsset.EnteredBy;
                        fixedAssetsObj.LockedBy = fixedAsset.LockedBy;
                        fixedAssetsObj.LockTs = fixedAsset.LockTs;
                        fixedAssetsObj.Posted = fixedAsset.Posted;
                        fixedAssetsObj.Disposed = fixedAsset.Disposed;
                        fixedAssetsObj.Narratives = fixedAsset.Narratives;
                        fixedAssetsObj.BankId = fixedAsset.BankId;
                        fixedAssetsObj.ProfitLoss = fixedAsset.ProfitLoss;
                        fixedAssetsObj.ProjectId = fixedAsset.ProjectId;
                        fixedAssetsObj.RevalueValue = fixedAsset.RevalueValue;
                        fixedAssetsObj.LastRevaluationDate = fixedAsset.LastRevaluationDate;
                        fixedAssetsObj.NoOfUnits = fixedAsset.NoOfUnits;
                        fixedAssetsObj.GlanalysisType1 = fixedAsset.GlanalysisType1;
                        fixedAssetsObj.GlanalysisType2 = fixedAsset.GlanalysisType2;
                        fixedAssetsObj.PercentageDisposed = fixedAsset.PercentageDisposed;
                        fixedAssetsObj.ActiveYn = fixedAsset.ActiveYn;
                        fixedAssetsObj.Depreciated = fixedAsset.Depreciated;
                        fixedAssetsObj.GlrevaluationSurplusAccount = fixedAsset.GlrevaluationSurplusAccount;
                        fixedAssetsObj.GlrevaluationDeficitAccount = fixedAsset.GlrevaluationDeficitAccount;
                        fixedAssetsObj.AcquisitionMode = fixedAsset.AcquisitionMode;
                        fixedAssetsObj.MasterAssetId = fixedAsset.MasterAssetId;
                        fixedAssetsObj.GlopeningBalanceContra = fixedAsset.GlopeningBalanceContra;
                        fixedAssetsObj.OpeningBalanceDate = fixedAsset.OpeningBalanceDate;
                        fixedAssetsObj.BookOpeningBalanceToGl = fixedAsset.BookOpeningBalanceToGl;
                        fixedAssetsObj.BranchCode = fixedAsset.BranchCode;
                        fixedAssetsObj.ProductionMachine = fixedAsset.ProductionMachine;

                        fixedAssetsObj.Status = lastCompletedWorkflow?.Status ?? "Draft";
                        fixedAssetsObj.WorkFlowTrail = workflows;
                        fixedAssets.Add(fixedAssetsObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return fixedAssets;

        }

        #endregion View Model Data List
    }
}
