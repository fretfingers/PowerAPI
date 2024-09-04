using PowerAPI.Data.Models;
using PowerAPI.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.IRepository
{
    public interface IAccount
    {
        Task<ApiToken> GetAccess(string token);
    }
}
