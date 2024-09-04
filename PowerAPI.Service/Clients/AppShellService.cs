using DevExpress.XtraRichEdit.Import.Html;
using Microsoft.EntityFrameworkCore;
using PowerAPI.Data.IRepository;
using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Service.Clients
{
    public class AppShellService : IAppShellService
    {
        private readonly EnterpriseContext _dbContext;
        private readonly IIdGenerator _idGen;
        public AppShellService(EnterpriseContext dbContext, IIdGenerator idGen)
        {
            _dbContext = dbContext;
            _idGen = idGen;
        }


        public async Task<List<NavigationDto>> GetNavigationInfo(ApiUser apiUser)
        {
            var menuItemsQuery = from p in _dbContext.RoleAccessPermission
                            join m in _dbContext.MainMenu on p.MenuId equals m.MenuId
                            where p.CompanyId == apiUser.CompanyId
                                  && p.DivisionId == apiUser.DivisionId
                                  && p.DepartmentId == apiUser.DepartmentId
                                  && p.RoleId == apiUser.Roles[0]
                            select new 
                            {
                                m.MenuId,
                                m.Name,
                                m.Action,
                                m.Icon,
                                m.ParentId
                            };

            var menuItems = await menuItemsQuery.ToListAsync();

            var favouriteMenuIds = await _dbContext.FavouritedMenuByUser
            .Where(f => f.CompanyId == apiUser.CompanyId &&
                        f.DivisionId == apiUser.DivisionId &&
                        f.DepartmentId == apiUser.DepartmentId &&
                        f.Username == apiUser.UserName)
            .Select(f => f.MenuId)
            .ToListAsync();

            var menuList = menuItems.Select(m => new NavigationDto
            {
                MenuId = m.MenuId,
                Name = m.Name,
                Action = string.IsNullOrEmpty(m.Action)? m.Action : RemoveExtensions(m.Action),
                MetaData = new MetaData { Icon = m.Icon },
                IsSelected = false,
                IsFavourite = favouriteMenuIds.Contains(m.MenuId),
                UserOrder = 0, // default value
                ParentId = m.ParentId ?? 0
            }).ToList();


            // Build the hierarchical structure
            var menuDictionary = menuList.ToDictionary(m => m.MenuId);
            var rootMenuItems = new List<NavigationDto>();

            foreach (var menuItem in menuList)
            {
                if (menuItem.ParentId != 0)
                {
                    if (menuDictionary.TryGetValue(menuItem.ParentId, out var parentMenuItem))
                    {
                        parentMenuItem.SubNav.Add(menuItem);
                    }
                }
                else
                {
                    rootMenuItems.Add(menuItem);
                }
            }

            return rootMenuItems;
        }

        public async Task<List<string>> GetCompany(string identityName)
        {
            // Split concatenated id representation
            string[] paramKeys = _idGen.SplitId(identityName);

            // derive username
            string username = paramKeys[3];

            return await _dbContext.Users
                .Where(user => user.Username == username)
                .Select(user => user.CompanyId)
                .Distinct()
                .ToListAsync();

        }

        public async Task<List<string>> GetDivision(string identityName)
        {
            // Split concatenated id representation
            string[] paramKeys = _idGen.SplitId(identityName);

            // derive username
            string username = paramKeys[3];

            return await _dbContext.Users
                .Where(user => user.Username == username)
                .Select(user => user.DivisionId)
                .Distinct()
                .ToListAsync();

        }

        public async Task<List<string>> GetDepartment(string identityName)
        {
            // Split concatenated id representation
            string[] paramKeys = _idGen.SplitId(identityName);

            // derive username
            string username = paramKeys[3];

            return await _dbContext.Users
                .Where(user => user.Username == username)
                .Select(user => user.DepartmentId)
                .Distinct()
                .ToListAsync();

        }

        public async Task<List<string>> GetBranch(string identityName)
        {
            // Split concatenated id representation
            string[] paramKeys = _idGen.SplitId(identityName);

            // derive username
            string companyId = paramKeys[0];
            string divisionId = paramKeys[1];
            string departmentId = paramKeys[2];
            string username = paramKeys[3];

            return await _dbContext.CompanyBranch
                .Where(x => x.CompanyId == companyId &&
                        x.DivisionId == divisionId &&
                        x.DepartmentId == departmentId
                      )
                .Select(user => user.DepartmentId)
                .Distinct()
                .ToListAsync();

        }
        public async Task<bool> FavouritedMenuExists(FavouriteNavMenuDto dto)
        {
            return await _dbContext.FavouritedMenuByUser.AnyAsync(f =>
                f.CompanyId == dto.CompanyId &&
                f.DivisionId == dto.DivisionId &&
                f.DepartmentId == dto.DepartmentId &&
                f.Username == dto.Username &&
                f.MenuId == dto.MenuId);
        }
        public async Task AddFavouriteMenu(FavouriteNavMenuDto dto)
        {
            try
            {
                // Split concatenated id representation
                //string[] paramKeys = _idGen.SplitId(identityName);

                // derive username
                string companyId = dto.CompanyId;
                string divisionId = dto.DivisionId;
                string departmentId = dto.DepartmentId;
                string username = dto.Username;
                int menuId = dto.MenuId;

                var currentDate = DateTime.Now;

                var newFavouriteNavRecord = new FavouritedMenuByUser()
                {
                    CompanyId = companyId,
                    DivisionId = divisionId,
                    DepartmentId = departmentId,
                    Username = username,
                    MenuId = menuId,
                    EntryDate = currentDate
                };

                _dbContext.FavouritedMenuByUser.Add(newFavouriteNavRecord);


                await _dbContext.SaveChangesAsync();

            }
            catch { throw; }
        }


        public async Task<FavouritedMenuByUser> GetFavouritedMenuAsync(string companyId, string divisionId, string departmentId, string username, int menuId)
        {
            return await _dbContext.FavouritedMenuByUser
                .FindAsync(companyId, divisionId, departmentId, username, menuId);
        }

        public async Task<bool> DeleteFavouritedMenuAsync(string companyId, string divisionId, string departmentId, string username, int menuId)
        {
            var favouritedMenu = await GetFavouritedMenuAsync(companyId, divisionId, departmentId, username, menuId);
            if (favouritedMenu == null)
            {
                return false;
            }

            _dbContext.FavouritedMenuByUser.Remove(favouritedMenu);
            await _dbContext.SaveChangesAsync();
            return true;
        }


        private static string RemoveExtensions(string input)
        {
            if (input.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
            {
                input = input.Substring(0, input.Length - 5);
            }

            if (input.StartsWith("..", StringComparison.OrdinalIgnoreCase))
            {
                input = input.Substring(2);
            }

            return input;
        }




    }


}
