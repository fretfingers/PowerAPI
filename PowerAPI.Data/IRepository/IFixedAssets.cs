using PowerAPI.Data.Models;
using PowerAPI.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.IRepository
{
    public interface IFixedAssets : IAccount
    {
        Task<Paging> GetFixedAssets(PaginationParams Param, ApiToken token);
        Task<Paging> GetFixedAssetsByType(PaginationParams Param, string assetType, ApiToken token);
        Task<Paging> GetFixedAssetsById(PaginationParams Param, string Id, ApiToken token);
        Task<Paging> GetFixedAssetsByName(PaginationParams Param, string name, ApiToken token);
        Task<Paging> GetFixedAssetsByStatus(PaginationParams Param, string status, ApiToken token);
    }
}
