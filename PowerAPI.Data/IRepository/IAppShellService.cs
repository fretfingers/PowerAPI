using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerAPI.Data.IRepository
{
    public interface IAppShellService
    {
        Task<List<NavigationDto>> GetNavigationInfo(ApiUser apiUser);
        Task<List<string>> GetCompany(string identityName);
        Task<List<string>> GetDivision(string identityName);
        Task<List<string>> GetDepartment(string identityName);
        Task<List<string>> GetBranch(string identityName);
        Task<bool> FavouritedMenuExists(FavouriteNavMenuDto dto);
        Task AddFavouriteMenu(FavouriteNavMenuDto dto);
        Task<FavouritedMenuByUser> GetFavouritedMenuAsync(string companyId, string divisionId, string departmentId, string username, int menuId);
        Task<bool> DeleteFavouritedMenuAsync(string companyId, string divisionId, string departmentId, string username, int menuId);
    }
}
