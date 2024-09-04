using PowerAPI.Data.Models;
using PowerAPI.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.IRepository
{
    public interface IStock : IAccount
    {
        Task<Paging> GetStock(PaginationParams Param, ApiToken token);
        Task<Paging> GetStockByWarehouse(PaginationParams Param, string warehouseId, ApiToken token);
        Task<Paging> GetStockByWarehouseByBin(PaginationParams Param, string warehouseId, string warehouseBinId, ApiToken token);
        Task<Paging> GetStockByItemFamily(PaginationParams Param, string ItemFamilyId, ApiToken token);
        Task<Paging> GetStockById(PaginationParams Param, string Id, ApiToken token);
        Task<Paging> GetStockByName(PaginationParams Param, string name, ApiToken token);
        Task<Paging> GetStockByWarehouseUser(PaginationParams Param, string userId, ApiToken token);
    }
}
