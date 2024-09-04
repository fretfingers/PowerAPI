using PowerAPI.Data.Models;
using PowerAPI.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.IRepository
{
    public interface IInventory: IAccount
    {
        Task<Paging> GetInventory(PaginationParams Param, ApiToken token);
        Task<Paging> GetInventoryByType(PaginationParams Param, string ItemType, ApiToken token);
        Task<Paging> GetInventoryById(PaginationParams Param, string Id, ApiToken token);
        Task<Paging> GetInventoryUnitOfMeasure(PaginationParams Param, ApiToken token);
        Task<Paging> GetInventoryPricingCode(PaginationParams Param, ApiToken token);
        Task<Paging> GetInventoryAttributes(PaginationParams Param, ApiToken token);
        Task<Paging> GetInventorySurCharge(PaginationParams Param, ApiToken token);
        Task<Paging> GetInventoryAdvertType(PaginationParams Param, ApiToken token);
        Task<Paging> GetInventoryProductType(PaginationParams Param, ApiToken token);
    }
}
