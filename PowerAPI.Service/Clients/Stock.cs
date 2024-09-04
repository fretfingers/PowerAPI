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
    public class Stock : IStock
    {
        EnterpriseContext _DBContext;

        public Stock(EnterpriseContext DBContext)
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

        public async Task<Paging> GetStock(PaginationParams Param, ApiToken token)
        {
            List<Data.ViewModels.InventoryByWarehouse> inventoriesByWarehouse = new List<Data.ViewModels.InventoryByWarehouse>();
            try
            {
                var totalCount = await _DBContext.InventoryByWarehouse
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId
                                                        ).CountAsync();

                var result = await _DBContext.InventoryByWarehouse.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId)//.ToListAsync();
                                                        .OrderBy(x => x.WarehouseId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                inventoriesByWarehouse = await inventoryByWarehouse(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    InventoryByWarehouseList = inventoriesByWarehouse
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetStockById(PaginationParams Param, string Id, ApiToken token)
        {
            List<Data.ViewModels.InventoryByWarehouse> inventoriesByWarehouse = new List<Data.ViewModels.InventoryByWarehouse>();
            try
            {
                var totalCount = await _DBContext.InventoryByWarehouse
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ItemId == Id).CountAsync();

                var result = await _DBContext.InventoryByWarehouse.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.ItemId == Id)//.ToListAsync();
                                                        .OrderBy(x => x.WarehouseId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                inventoriesByWarehouse = await inventoryByWarehouse(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    InventoryByWarehouseList = inventoriesByWarehouse
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetStockByItemFamily(PaginationParams Param, string ItemFamilyId, ApiToken token)
        {
            List<Data.ViewModels.InventoryByWarehouse> inventoriesByWarehouse = new List<Data.ViewModels.InventoryByWarehouse>();
            try
            {
                var totalCount = await (from warehouse in _DBContext.InventoryByWarehouse
                                 join inventoryItems in _DBContext.InventoryItems
                                 on new
                                 {
                                     warehouse.CompanyId,
                                     warehouse.DivisionId,
                                     warehouse.DepartmentId,
                                     warehouse.ItemId
                                 }
                                 equals new
                                 {
                                     inventoryItems.CompanyId,
                                     inventoryItems.DivisionId,
                                     inventoryItems.DepartmentId,
                                     inventoryItems.ItemId
                                 }
                                 where inventoryItems.ItemFamilyId == ItemFamilyId
                                           && warehouse.CompanyId == token.CompanyId
                                           && warehouse.DivisionId == token.DivisionId
                                           && warehouse.DepartmentId == token.DepartmentId
                                 select warehouse).CountAsync();



                var result = await (from warehouse in _DBContext.InventoryByWarehouse
                                    join inventoryItems in _DBContext.InventoryItems
                                    on new
                                    {
                                        warehouse.CompanyId,
                                        warehouse.DivisionId,
                                        warehouse.DepartmentId,
                                        warehouse.ItemId
                                    }
                                    equals new
                                    {
                                        inventoryItems.CompanyId,
                                        inventoryItems.DivisionId,
                                        inventoryItems.DepartmentId,
                                        inventoryItems.ItemId
                                    }
                                    where inventoryItems.ItemFamilyId == ItemFamilyId
                                              && warehouse.CompanyId == token.CompanyId
                                              && warehouse.DivisionId == token.DivisionId
                                              && warehouse.DepartmentId == token.DepartmentId
                                    select warehouse).OrderBy(x => x.WarehouseId)
                                                     .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                     .Take(Param.ItemsPerPage).ToListAsync();


                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                inventoriesByWarehouse = await inventoryByWarehouse(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    InventoryByWarehouseList = inventoriesByWarehouse
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetStockByName(PaginationParams Param, string name, ApiToken token)
        {
            List<Data.ViewModels.InventoryByWarehouse> inventoriesByWarehouse = new List<Data.ViewModels.InventoryByWarehouse>();
            try
            {
                var totalCount = await (from warehouse in _DBContext.InventoryByWarehouse
                                        join inventoryItems in _DBContext.InventoryItems
                                        on new
                                        {
                                            warehouse.CompanyId,
                                            warehouse.DivisionId,
                                            warehouse.DepartmentId,
                                            warehouse.ItemId
                                        }
                                        equals new
                                        {
                                            inventoryItems.CompanyId,
                                            inventoryItems.DivisionId,
                                            inventoryItems.DepartmentId,
                                            inventoryItems.ItemId
                                        }
                                        where inventoryItems.ItemName == name
                                                  && warehouse.CompanyId == token.CompanyId
                                                  && warehouse.DivisionId == token.DivisionId
                                                  && warehouse.DepartmentId == token.DepartmentId
                                        select warehouse).CountAsync();



                var result = await (from warehouse in _DBContext.InventoryByWarehouse
                                    join inventoryItems in _DBContext.InventoryItems
                                    on new
                                    {
                                        warehouse.CompanyId,
                                        warehouse.DivisionId,
                                        warehouse.DepartmentId,
                                        warehouse.ItemId
                                    }
                                    equals new
                                    {
                                        inventoryItems.CompanyId,
                                        inventoryItems.DivisionId,
                                        inventoryItems.DepartmentId,
                                        inventoryItems.ItemId
                                    }
                                    where inventoryItems.ItemName == name
                                              && warehouse.CompanyId == token.CompanyId
                                              && warehouse.DivisionId == token.DivisionId
                                              && warehouse.DepartmentId == token.DepartmentId
                                    select warehouse).OrderBy(x => x.WarehouseId)
                                                     .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                     .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                inventoriesByWarehouse = await inventoryByWarehouse(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    InventoryByWarehouseList = inventoriesByWarehouse
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetStockByWarehouse(PaginationParams Param, string warehouseId, ApiToken token)
        {
            List<Data.ViewModels.InventoryByWarehouse> inventoriesByWarehouse = new List<Data.ViewModels.InventoryByWarehouse>();
            try
            {
                var totalCount = await _DBContext.InventoryByWarehouse
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.WarehouseId == warehouseId).CountAsync();

                var result = await _DBContext.InventoryByWarehouse.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.WarehouseId == warehouseId)//.ToListAsync();
                                                        .OrderBy(x => x.WarehouseId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                inventoriesByWarehouse = await inventoryByWarehouse(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    InventoryByWarehouseList = inventoriesByWarehouse
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetStockByWarehouseByBin(PaginationParams Param, string warehouseId, string warehouseBinId, ApiToken token)
        {
            List<Data.ViewModels.InventoryByWarehouse> inventoriesByWarehouse = new List<Data.ViewModels.InventoryByWarehouse>();
            try
            {
                var totalCount = await _DBContext.InventoryByWarehouse
                                                 .Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.WarehouseId == warehouseId &&
                                                        x.WarehouseBinId == warehouseBinId).CountAsync();

                var result = await _DBContext.InventoryByWarehouse.Where(x =>
                                                        x.CompanyId == token.CompanyId &&
                                                        x.DivisionId == token.DivisionId &&
                                                        x.DepartmentId == token.DepartmentId &&
                                                        x.WarehouseId == warehouseId &&
                                                        x.WarehouseBinId == warehouseBinId)//.ToListAsync();
                                                        .OrderBy(x => x.WarehouseId)
                                                        .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                        .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                inventoriesByWarehouse = await inventoryByWarehouse(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    InventoryByWarehouseList = inventoriesByWarehouse
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Paging> GetStockByWarehouseUser(PaginationParams Param, string userId, ApiToken token)
        {
            List<Data.ViewModels.InventoryByWarehouse> inventoriesByWarehouse = new List<Data.ViewModels.InventoryByWarehouse>();
            try
            {
                var totalCount = await (from warehouse in _DBContext.InventoryByWarehouse
                                        join WarehouseByEmployees in _DBContext.WarehouseByEmployees
                                        on new
                                        {
                                            warehouse.CompanyId,
                                            warehouse.DivisionId,
                                            warehouse.DepartmentId,
                                            warehouse.WarehouseId
                                        }
                                        equals new
                                        {
                                            WarehouseByEmployees.CompanyId,
                                            WarehouseByEmployees.DivisionId,
                                            WarehouseByEmployees.DepartmentId,
                                            WarehouseByEmployees.WarehouseId
                                        }
                                        where WarehouseByEmployees.EmployeeId == userId
                                                  && warehouse.CompanyId == token.CompanyId
                                                  && warehouse.DivisionId == token.DivisionId
                                                  && warehouse.DepartmentId == token.DepartmentId
                                        select warehouse).CountAsync();



                var result = await (from warehouse in _DBContext.InventoryByWarehouse
                                    join warehouseByEmployees in _DBContext.WarehouseByEmployees
                                    on new
                                    {
                                        warehouse.CompanyId,
                                        warehouse.DivisionId,
                                        warehouse.DepartmentId,
                                        warehouse.WarehouseId
                                    }
                                    equals new
                                    {
                                        warehouseByEmployees.CompanyId,
                                        warehouseByEmployees.DivisionId,
                                        warehouseByEmployees.DepartmentId,
                                        warehouseByEmployees.WarehouseId
                                    }
                                    where warehouseByEmployees.EmployeeId == userId
                                              && warehouse.CompanyId == token.CompanyId
                                              && warehouse.DivisionId == token.DivisionId
                                              && warehouse.DepartmentId == token.DepartmentId
                                    select warehouse).OrderBy(x => x.WarehouseId)
                                                     .Skip((Param.Page - 1) * Param.ItemsPerPage)
                                                     .Take(Param.ItemsPerPage).ToListAsync();

                var paginationMetadata = new PaginationMetadata(Param.Page, totalCount, Param.ItemsPerPage);

                inventoriesByWarehouse = await inventoryByWarehouse(result, token);

                Paging pagination = new Paging
                {
                    PaginationMetadata = paginationMetadata,
                    InventoryByWarehouseList = inventoriesByWarehouse
                };

                return pagination;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        #region View Model Data List

        private async Task<List<Data.ViewModels.InventoryByWarehouse>> inventoryByWarehouse(List<Data.Models.InventoryByWarehouse> obj, ApiToken token)
        {
            List<Data.ViewModels.InventoryByWarehouse> inventoryByWarehouses = new List<Data.ViewModels.InventoryByWarehouse>();
            try
            {
                if (obj != null)
                {
                    foreach (Data.Models.InventoryByWarehouse inventoryByWarehouse in obj)
                    {
                        Data.ViewModels.InventoryByWarehouse inventoryByWarehouseObj = new Data.ViewModels.InventoryByWarehouse();

                        var workflows = new List<SetupWorkflow>();
                        var currentDate = DateTime.Now;

                        // Business Logic for adding Work Flow Trail In memoery
                        workflows = new List<SetupWorkflow> { };

                        inventoryByWarehouseObj.CompanyId = inventoryByWarehouse.CompanyId;
                        inventoryByWarehouseObj.DivisionId = inventoryByWarehouse.DivisionId;
                        inventoryByWarehouseObj.DepartmentId = inventoryByWarehouse.DepartmentId;
                        inventoryByWarehouseObj.ItemId = inventoryByWarehouse.ItemId;
                        inventoryByWarehouseObj.WarehouseId = inventoryByWarehouse.WarehouseId;
                        inventoryByWarehouseObj.WarehouseBinId = inventoryByWarehouse.WarehouseBinId;
                        inventoryByWarehouseObj.QtyOnHand = inventoryByWarehouse.QtyOnHand;
                        inventoryByWarehouseObj.QtyCommitted = inventoryByWarehouse.QtyCommitted;
                        inventoryByWarehouseObj.QtyOnOrder = inventoryByWarehouse.QtyOnOrder;
                        inventoryByWarehouseObj.QtyOnBackorder = inventoryByWarehouse.QtyOnBackorder;
                        inventoryByWarehouseObj.CycleCode = inventoryByWarehouse.CycleCode;
                        inventoryByWarehouseObj.LastCountDate = inventoryByWarehouse.LastCountDate;
                        inventoryByWarehouseObj.LockedBy = inventoryByWarehouse.LockedBy;
                        inventoryByWarehouseObj.LockTs = inventoryByWarehouse.LockTs;
                        inventoryByWarehouseObj.LastEditDate = inventoryByWarehouse.LastEditDate;
                        inventoryByWarehouseObj.CreationDate = inventoryByWarehouse.CreationDate;
                        inventoryByWarehouseObj.BranchCode = inventoryByWarehouse.BranchCode;
                        
                        inventoryByWarehouseObj.WorkFlowTrail = workflows;
                        inventoryByWarehouses.Add(inventoryByWarehouseObj);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return inventoryByWarehouses;

        }

        #endregion View Model Data List
    }
}
