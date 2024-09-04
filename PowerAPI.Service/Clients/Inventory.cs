using DevExpress.DataAccess.Wizard.Presenters;
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
    public class Inventory : IInventory
    {
        EnterpriseContext _DBContext;

        public Inventory(EnterpriseContext DBContext)
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

        public async Task<Paging> GetInventory(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.InventoryItems> inventoryItems = new List<Data.ViewModels.InventoryItems>();
            try
            {
                var totalCount = await _DBContext.InventoryItems
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.InventoryItems.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.ItemId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                inventoryItems = await inventoryItem(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    InventoryItemsList = inventoryItems
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetInventoryById(PaginationParams Param, string Id, ApiToken token)
        {
            List<Data.ViewModels.InventoryItems> inventoryItems = new List<Data.ViewModels.InventoryItems>();
            try
            {
                var totalCount = await _DBContext.InventoryItems
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ItemId == Id).CountAsync();

                var result = await _DBContext.InventoryItems.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ItemId == Id)//.ToListAsync();
                                                        .OrderBy(x => x.ItemId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                inventoryItems = await inventoryItem(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    InventoryItemsList = inventoryItems
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetInventoryByType(PaginationParams Param, string ItemType, ApiToken token)
        {
            List<Data.ViewModels.InventoryItems> inventoryItems = new List<Data.ViewModels.InventoryItems>();
            try
            {
                var totalCount = await _DBContext.InventoryItems
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ItemTypeId == ItemType).CountAsync();

                var result = await _DBContext.InventoryItems.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ItemTypeId == ItemType)//.ToListAsync();
                                                        .OrderBy(x => x.ItemId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                inventoryItems = await inventoryItem(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    InventoryItemsList = inventoryItems
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetInventoryUnitOfMeasure(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.InventoryUnitOfMeasure> inventoryUnitOfMeasures = new List<Data.ViewModels.InventoryUnitOfMeasure>();
            try
            {
                var totalCount = await _DBContext.InventoryUnitOfMeasure
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.InventoryUnitOfMeasure.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.UnitOfMeasureId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                inventoryUnitOfMeasures = await inventoryUOM(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    InventoryUOMList = inventoryUnitOfMeasures
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetInventoryPricingCode(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.InventoryPricingCode> inventoryPricingCodes = new List<Data.ViewModels.InventoryPricingCode>();
            try
            {
                var totalCount = await _DBContext.InventoryPricingCode
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.InventoryPricingCode.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.ItemPricingCode)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                inventoryPricingCodes = await inventoryPricingCode(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    InventoryPricingCodeList = inventoryPricingCodes
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetInventoryAttributes(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.InventoryAttributes> inventoryAttributes = new List<Data.ViewModels.InventoryAttributes>();
            try
            {
                var totalCount = await _DBContext.InventoryAttributes
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.InventoryAttributes.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.AttributeId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                inventoryAttributes = await inventoryAttribute(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    InventoryAttributesList = inventoryAttributes
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetInventorySurCharge(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.SurCharge> surCharges = new List<Data.ViewModels.SurCharge>();
            try
            {
                var totalCount = await _DBContext.SurCharge
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.SurCharge.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.SurChargeId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                surCharges = await surCharge(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    InventorySurChargeList = surCharges
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetInventoryAdvertType(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.AdvertType> advertTypes = new List<Data.ViewModels.AdvertType>();
            try
            {
                var totalCount = await _DBContext.AdvertType
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.AdvertType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.AdvertTypeId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                advertTypes = await advertType(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    InventoryAdvertTypeList = advertTypes
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetInventoryProductType(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.ProductType> productTypes = new List<Data.ViewModels.ProductType>();
            try
            {
                var totalCount = await _DBContext.ProductType
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId).CountAsync();

                var result = await _DBContext.ProductType.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.ProductTypeId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                productTypes = await productType(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    InventoryProductTypeList = productTypes
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



















        #region View Model Data List

        private async Task<List<Data.ViewModels.InventoryItems>> inventoryItem(List<Data.Models.InventoryItems> obj, ApiToken token)
        {
            List<Data.ViewModels.InventoryItems> inventoryItems = new List<Data.ViewModels.InventoryItems>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.InventoryItems inventoryItem in obj)
                    {
                        Data.ViewModels.InventoryItems inventoryItemsObj = new Data.ViewModels.InventoryItems();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        inventoryItemsObj.CompanyId = inventoryItem.CompanyId;
                        inventoryItemsObj.DivisionId = inventoryItem.DivisionId;
                        inventoryItemsObj.DepartmentId = inventoryItem.DepartmentId;
                        inventoryItemsObj.ItemId = inventoryItem.ItemId;
                        inventoryItemsObj.IsActive = inventoryItem.IsActive;
                        inventoryItemsObj.ItemTypeId = inventoryItem.ItemTypeId;
                        inventoryItemsObj.ItemName = inventoryItem.ItemName;
                        inventoryItemsObj.ItemDescription = inventoryItem.ItemDescription;
                        inventoryItemsObj.ItemLongDescription = inventoryItem.ItemLongDescription;
                        inventoryItemsObj.ItemCategoryId = inventoryItem.ItemCategoryId;
                        inventoryItemsObj.ItemFamilyId = inventoryItem.ItemFamilyId;
                        inventoryItemsObj.SalesDescription = inventoryItem.SalesDescription;
                        inventoryItemsObj.PurchaseDescription = inventoryItem.PurchaseDescription;
                        inventoryItemsObj.PictureUrl = inventoryItem.PictureUrl;
                        inventoryItemsObj.LargePictureUrl = inventoryItem.LargePictureUrl;
                        inventoryItemsObj.ItemWeight = inventoryItem.ItemWeight;
                        inventoryItemsObj.ItemWeightMetric = inventoryItem.ItemWeightMetric;
                        inventoryItemsObj.ItemShipWeight = inventoryItem.ItemShipWeight;
                        inventoryItemsObj.ItemUpccode = inventoryItem.ItemUpccode;
                        inventoryItemsObj.ItemEpccode = inventoryItem.ItemEpccode;
                        inventoryItemsObj.ItemRfid = inventoryItem.ItemRfid;
                        inventoryItemsObj.ItemSize = inventoryItem.ItemSize;
                        inventoryItemsObj.ItemSizeCmm = inventoryItem.ItemSizeCmm;
                        inventoryItemsObj.ItemDimentions = inventoryItem.ItemDimentions;
                        inventoryItemsObj.ItemDimentionsCmm = inventoryItem.ItemDimentionsCmm;
                        inventoryItemsObj.ItemColor = inventoryItem.ItemColor;
                        inventoryItemsObj.ItemNrfcolor = inventoryItem.ItemNrfcolor;
                        inventoryItemsObj.ItemStyle = inventoryItem.ItemStyle;
                        inventoryItemsObj.ItemNrfstyle = inventoryItem.ItemNrfstyle;
                        inventoryItemsObj.ItemCareInstructions = inventoryItem.ItemCareInstructions;
                        inventoryItemsObj.ItemDefaultWarehouse = inventoryItem.ItemDefaultWarehouse;
                        inventoryItemsObj.ItemDefaultWarehouseBin = inventoryItem.ItemDefaultWarehouseBin;
                        inventoryItemsObj.ItemLocationX = inventoryItem.ItemLocationX;
                        inventoryItemsObj.ItemLocationY = inventoryItem.ItemLocationY;
                        inventoryItemsObj.ItemLocationZ = inventoryItem.ItemLocationZ;
                        inventoryItemsObj.DownloadLocation = inventoryItem.DownloadLocation;
                        inventoryItemsObj.DownloadPassword = inventoryItem.DownloadPassword;
                        inventoryItemsObj.ItemUom = inventoryItem.ItemUom;
                        inventoryItemsObj.GlitemSalesAccount = inventoryItem.GlitemSalesAccount;
                        inventoryItemsObj.GlitemCogsaccount = inventoryItem.GlitemCogsaccount;
                        inventoryItemsObj.GlitemInventoryAccount = inventoryItem.GlitemInventoryAccount;
                        inventoryItemsObj.PackId = inventoryItem.PackId;
                        inventoryItemsObj.CurrencyId = inventoryItem.CurrencyId;
                        inventoryItemsObj.CurrencyExchangeRate = inventoryItem.CurrencyExchangeRate;
                        inventoryItemsObj.Price = inventoryItem.Price;
                        inventoryItemsObj.ItemPricingCode = inventoryItem.ItemPricingCode;
                        inventoryItemsObj.PricingMethods = inventoryItem.PricingMethods;
                        inventoryItemsObj.Taxable = inventoryItem.Taxable;
                        inventoryItemsObj.VendorId = inventoryItem.VendorId;
                        inventoryItemsObj.LeadTime = inventoryItem.LeadTime;
                        inventoryItemsObj.LeadTimeUnit = inventoryItem.LeadTimeUnit;
                        inventoryItemsObj.ReOrderLevel = inventoryItem.ReOrderLevel;
                        inventoryItemsObj.ReOrderQty = inventoryItem.ReOrderQty;
                        inventoryItemsObj.BuildTime = inventoryItem.BuildTime;
                        inventoryItemsObj.BuildTimeUnit = inventoryItem.BuildTimeUnit;
                        inventoryItemsObj.UseageRate = inventoryItem.UseageRate;
                        inventoryItemsObj.UseageRateUnit = inventoryItem.UseageRateUnit;
                        inventoryItemsObj.SalesForecast = inventoryItem.SalesForecast;
                        inventoryItemsObj.SalesForecastUnit = inventoryItem.SalesForecastUnit;
                        inventoryItemsObj.CalculatedCover = inventoryItem.CalculatedCover;
                        inventoryItemsObj.CalculatedCoverUnits = inventoryItem.CalculatedCoverUnits;
                        inventoryItemsObj.IsAssembly = inventoryItem.IsAssembly;
                        inventoryItemsObj.ItemAssembly = inventoryItem.ItemAssembly;
                        inventoryItemsObj.Lifo = inventoryItem.Lifo;
                        inventoryItemsObj.Lifovalue = inventoryItem.Lifovalue;
                        inventoryItemsObj.Lifocost = inventoryItem.Lifocost;
                        inventoryItemsObj.Average = inventoryItem.Average;
                        inventoryItemsObj.AverageValue = inventoryItem.AverageValue;
                        inventoryItemsObj.AverageCost = inventoryItem.AverageCost;
                        inventoryItemsObj.Fifo = inventoryItem.Fifo;
                        inventoryItemsObj.Fifovalue = inventoryItem.Fifovalue;
                        inventoryItemsObj.Fifocost = inventoryItem.Fifocost;
                        inventoryItemsObj.Expected = inventoryItem.Expected;
                        inventoryItemsObj.ExpectedValue = inventoryItem.ExpectedValue;
                        inventoryItemsObj.ExpectedCost = inventoryItem.ExpectedCost;
                        inventoryItemsObj.Landed = inventoryItem.Landed;
                        inventoryItemsObj.LandedValue = inventoryItem.LandedValue;
                        inventoryItemsObj.LandedCost = inventoryItem.LandedCost;
                        inventoryItemsObj.Other = inventoryItem.Other;
                        inventoryItemsObj.OtherValue = inventoryItem.OtherValue;
                        inventoryItemsObj.OtherCost = inventoryItem.OtherCost;
                        inventoryItemsObj.Commissionable = inventoryItem.Commissionable;
                        inventoryItemsObj.CommissionType = inventoryItem.CommissionType;
                        inventoryItemsObj.CommissionPerc = inventoryItem.CommissionPerc;
                        inventoryItemsObj.Approved = inventoryItem.Approved;
                        inventoryItemsObj.ApprovedBy = inventoryItem.ApprovedBy;
                        inventoryItemsObj.ApprovedDate = inventoryItem.ApprovedDate;
                        inventoryItemsObj.EnteredBy = inventoryItem.EnteredBy;
                        inventoryItemsObj.TaxGroupId = inventoryItem.TaxGroupId;
                        inventoryItemsObj.TaxPercent = inventoryItem.TaxPercent;
                        inventoryItemsObj.LockedBy = inventoryItem.LockedBy;
                        inventoryItemsObj.LockTs = inventoryItem.LockTs;
                        inventoryItemsObj.IsSerialLotItem = inventoryItem.IsSerialLotItem;
                        inventoryItemsObj.IsWarrantyItem = inventoryItem.IsWarrantyItem;
                        inventoryItemsObj.WarrantyPeriod = inventoryItem.WarrantyPeriod;
                        inventoryItemsObj.MinimumQty = inventoryItem.MinimumQty;
                        inventoryItemsObj.LastEditDate = inventoryItem.LastEditDate;
                        inventoryItemsObj.CreationDate = inventoryItem.CreationDate;
                        inventoryItemsObj.GlitemFreightAccount = inventoryItem.GlitemFreightAccount;
                        inventoryItemsObj.GlitemHandlingAccount = inventoryItem.GlitemHandlingAccount;
                        inventoryItemsObj.AllowPurchaseTrans = inventoryItem.AllowPurchaseTrans;
                        inventoryItemsObj.AllowSalesTrans = inventoryItem.AllowSalesTrans;
                        inventoryItemsObj.AllowInventoryTrans = inventoryItem.AllowInventoryTrans;
                        inventoryItemsObj.ToleranceLevel = inventoryItem.ToleranceLevel;
                        inventoryItemsObj.BranchCode = inventoryItem.BranchCode;
                        inventoryItemsObj.EnforceQualityAssuranceOnPo = inventoryItem.EnforceQualityAssuranceOnPo;
                        inventoryItemsObj.IsPack = inventoryItem.IsPack;
                        inventoryItemsObj.AllowPack = inventoryItem.AllowPack;
                        inventoryItemsObj.MinimumQtyForPacking = inventoryItem.MinimumQtyForPacking;
                        inventoryItemsObj.ProjectId = inventoryItem.ProjectId;

                        inventoryItemsObj.WorkFlowTrail = workflows;
                        inventoryItems.Add(inventoryItemsObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return inventoryItems;

        }

        private async Task<List<Data.ViewModels.InventoryUnitOfMeasure>> inventoryUOM(List<Data.Models.InventoryUnitOfMeasure> obj, ApiToken token)
        {
            List<Data.ViewModels.InventoryUnitOfMeasure> inventoryUnitOfMeasures = new List<Data.ViewModels.InventoryUnitOfMeasure>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.InventoryUnitOfMeasure inventoryUnitOfMeasure in obj)
                    {
                        Data.ViewModels.InventoryUnitOfMeasure inventoryUnitOfMeasureObj = new Data.ViewModels.InventoryUnitOfMeasure();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        inventoryUnitOfMeasureObj.CompanyId = inventoryUnitOfMeasure.CompanyId;
                        inventoryUnitOfMeasureObj.DivisionId = inventoryUnitOfMeasure.DivisionId;
                        inventoryUnitOfMeasureObj.DepartmentId = inventoryUnitOfMeasure.DepartmentId;
                        inventoryUnitOfMeasureObj.UnitOfMeasureId = inventoryUnitOfMeasure.UnitOfMeasureId;
                        inventoryUnitOfMeasureObj.UnitOfMeasureDescription = inventoryUnitOfMeasure.UnitOfMeasureDescription;
                        inventoryUnitOfMeasureObj.LockedBy = inventoryUnitOfMeasure.LockedBy;
                        inventoryUnitOfMeasureObj.LockTs = inventoryUnitOfMeasure.LockTs;
                        inventoryUnitOfMeasureObj.BranchCode = inventoryUnitOfMeasure.BranchCode;

                        inventoryUnitOfMeasureObj.WorkFlowTrail = workflows;
                        inventoryUnitOfMeasures.Add(inventoryUnitOfMeasureObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return inventoryUnitOfMeasures;

        }

        
        private async Task<List<Data.ViewModels.InventoryPricingCode>> inventoryPricingCode(List<Data.Models.InventoryPricingCode> obj, ApiToken token)
        {
            List<Data.ViewModels.InventoryPricingCode> inventoryPricingCodes = new List<Data.ViewModels.InventoryPricingCode>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.InventoryPricingCode inventoryPricingCode in obj)
                    {
                        Data.ViewModels.InventoryPricingCode inventoryPricingCodeObj = new Data.ViewModels.InventoryPricingCode();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        inventoryPricingCodeObj.CompanyId = inventoryPricingCode.CompanyId;
                        inventoryPricingCodeObj.DivisionId = inventoryPricingCode.DivisionId;
                        inventoryPricingCodeObj.DepartmentId = inventoryPricingCode.DepartmentId;
                        inventoryPricingCodeObj.ItemId = inventoryPricingCode.ItemId;
                        inventoryPricingCodeObj.ItemPricingCode = inventoryPricingCode.ItemPricingCode;
                        inventoryPricingCodeObj.CurrencyId = inventoryPricingCode.CurrencyId;
                        inventoryPricingCodeObj.CurrencyExchangeRate = inventoryPricingCode.CurrencyExchangeRate;
                        inventoryPricingCodeObj.Price = inventoryPricingCode.Price;
                        inventoryPricingCodeObj.Msrp = inventoryPricingCode.Msrp;
                        inventoryPricingCodeObj.HotItem = inventoryPricingCode.HotItem;
                        inventoryPricingCodeObj.FeaturedItem = inventoryPricingCode.FeaturedItem;
                        inventoryPricingCodeObj.SaleItem = inventoryPricingCode.SaleItem;
                        inventoryPricingCodeObj.SalesPrice = inventoryPricingCode.SalesPrice;
                        inventoryPricingCodeObj.SaleStartDate = inventoryPricingCode.SaleStartDate;
                        inventoryPricingCodeObj.SaleEndDate = inventoryPricingCode.SaleEndDate;
                        inventoryPricingCodeObj.Approved = inventoryPricingCode.Approved;
                        inventoryPricingCodeObj.ApprovedBy = inventoryPricingCode.ApprovedBy;
                        inventoryPricingCodeObj.ApprovedDate = inventoryPricingCode.ApprovedDate;
                        inventoryPricingCodeObj.EnteredBy = inventoryPricingCode.EnteredBy;
                        inventoryPricingCodeObj.LockedBy = inventoryPricingCode.LockedBy;
                        inventoryPricingCodeObj.LockTs = inventoryPricingCode.LockTs;
                        inventoryPricingCodeObj.BranchCode = inventoryPricingCode.BranchCode;

                        inventoryPricingCodeObj.WorkFlowTrail = workflows;
                        inventoryPricingCodes.Add(inventoryPricingCodeObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return inventoryPricingCodes;

        }

        private async Task<List<Data.ViewModels.InventoryAttributes>> inventoryAttribute(List<Data.Models.InventoryAttributes> obj, ApiToken token)
        {
            List<Data.ViewModels.InventoryAttributes> inventoryAttributes = new List<Data.ViewModels.InventoryAttributes>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.InventoryAttributes inventoryAttribute in obj)
                    {
                        Data.ViewModels.InventoryAttributes inventoryAttributesObj = new Data.ViewModels.InventoryAttributes();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        inventoryAttributesObj.CompanyId = inventoryAttribute.CompanyId;
                        inventoryAttributesObj.DivisionId = inventoryAttribute.DivisionId;
                        inventoryAttributesObj.DepartmentId = inventoryAttribute.DepartmentId;
                        inventoryAttributesObj.AttributeId = inventoryAttribute.AttributeId;
                        inventoryAttributesObj.AttributeDescription = inventoryAttribute.AttributeDescription;
                        inventoryAttributesObj.LockedBy = inventoryAttribute.LockedBy;
                        inventoryAttributesObj.LockTs = inventoryAttribute.LockTs;
                        inventoryAttributesObj.BranchCode = inventoryAttribute.BranchCode;

                        inventoryAttributesObj.WorkFlowTrail = workflows;
                        inventoryAttributes.Add(inventoryAttributesObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return inventoryAttributes;

        }

        private async Task<List<Data.ViewModels.SurCharge>> surCharge(List<Data.Models.SurCharge> obj, ApiToken token)
        {
            List<Data.ViewModels.SurCharge> surCharges = new List<Data.ViewModels.SurCharge>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.SurCharge surCharge in obj)
                    {
                        Data.ViewModels.SurCharge surChargeObj = new Data.ViewModels.SurCharge();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        surChargeObj.CompanyId = surCharge.CompanyId;
                        surChargeObj.DivisionId = surCharge.DivisionId;
                        surChargeObj.DepartmentId = surCharge.DepartmentId;
                        surChargeObj.SurChargeId = surCharge.SurChargeId;
                        surChargeObj.SurChargeDescription = surCharge.SurChargeDescription;
                        surChargeObj.Percentage = surCharge.Percentage;
                        surChargeObj.LockedBy = surCharge.LockedBy;
                        surChargeObj.LockTs = surCharge.LockTs;
                        surChargeObj.BranchCode = surCharge.BranchCode;

                        surChargeObj.WorkFlowTrail = workflows;
                        surCharges.Add(surChargeObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return surCharges;

        }

        private async Task<List<Data.ViewModels.AdvertType>> advertType(List<Data.Models.AdvertType> obj, ApiToken token)
        {
            List<Data.ViewModels.AdvertType> advertTypes = new List<Data.ViewModels.AdvertType>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.AdvertType advertType in obj)
                    {
                        Data.ViewModels.AdvertType advertTypeObj = new Data.ViewModels.AdvertType();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        advertTypeObj.CompanyId = advertType.CompanyId;
                        advertTypeObj.DivisionId = advertType.DivisionId;
                        advertTypeObj.DepartmentId = advertType.DepartmentId;
                        advertTypeObj.AdvertTypeId = advertType.AdvertTypeId;
                        advertTypeObj.AdvertTypeDescription = advertType.AdvertTypeDescription;
                        advertTypeObj.LockedBy = advertType.LockedBy;
                        advertTypeObj.LockTs = advertType.LockTs;
                        advertTypeObj.BranchCode = advertType.BranchCode;

                        advertTypeObj.WorkFlowTrail = workflows;
                        advertTypes.Add(advertTypeObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return advertTypes;

        }

        private async Task<List<Data.ViewModels.ProductType>> productType(List<Data.Models.ProductType> obj, ApiToken token)
        {
            List<Data.ViewModels.ProductType> productTypes = new List<Data.ViewModels.ProductType>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.ProductType productType in obj)
                    {
                        Data.ViewModels.ProductType productTypeObj = new Data.ViewModels.ProductType();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        productTypeObj.CompanyId = productType.CompanyId;
                        productTypeObj.DivisionId = productType.DivisionId;
                        productTypeObj.DepartmentId = productType.DepartmentId;
                        productTypeObj.ProductTypeId = productType.ProductTypeId;
                        productTypeObj.ProductTypeDescription = productType.ProductTypeDescription;
                        productTypeObj.LockedBy = productType.LockedBy;
                        productTypeObj.LockTs = productType.LockTs;
                        productTypeObj.BranchCode = productType.BranchCode;

                        productTypeObj.WorkFlowTrail = workflows;
                        productTypes.Add(productTypeObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return productTypes;

        }

        #endregion View Model Data List
    }
}
