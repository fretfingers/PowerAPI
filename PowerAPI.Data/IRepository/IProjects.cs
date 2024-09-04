using PowerAPI.Data.Models;
using PowerAPI.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.IRepository
{
    public interface IProjects : IAccount
    {
        Task<Paging> GetProjects(PaginationParams Param, ApiToken token);
        Task<Paging> GetProjectsByType(PaginationParams Param, string projectType, ApiToken token);
        Task<Paging> GetProjectsById(PaginationParams Param, string Id, ApiToken token);
        Task<Paging> GetProjectsByName(PaginationParams Param, string name, ApiToken token);
        //Task<Paging> GetProjectsByStatus(PaginationParams Param, string status, ApiToken token);
    }
}
