using PowerAPI.Data.Models;
using PowerAPI.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.IRepository
{
    public interface IBanks : IAccount
    {
        Task<Paging> GetCashbookBanks(PaginationParams Param, ApiToken token);
    }
}
