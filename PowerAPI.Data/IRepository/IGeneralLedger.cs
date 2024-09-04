using PowerAPI.Data.Models;
using PowerAPI.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.IRepository
{
    public interface IGeneralLedger : IAccount
    {
        Task<Paging> GetCOA(PaginationParams Param, ApiToken token);
        Task<Paging> GetCOAById(PaginationParams Param, string Id, ApiToken token);
        Task<Paging> GetCOAForeign(PaginationParams Param, ApiToken token);
        Task<Paging> GetCOAByAccountType(PaginationParams Param, string accountType, ApiToken token);      
        Task<Paging> GetCOAByName(PaginationParams Param, string name, ApiToken token);
    }
}
