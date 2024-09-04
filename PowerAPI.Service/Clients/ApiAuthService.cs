using DevExpress.Security.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PowerAPI.Data.IRepository;
using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using PowerAPI.Service.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace PowerAPI.Service.Clients
{
    public class ApiAuthService : IApiAuthService
    {
        EnterpriseContext _DBContext;
        IIdGenerator _idGen;
        IMemoryCache _cache;
        public ApiAuthService(EnterpriseContext DBContext, IIdGenerator idGen, IMemoryCache cache)
        {
            _DBContext = DBContext;
            _idGen = idGen;
            _cache = cache;
        }

        public async Task CreateUser(ApiUser record)
        {
            try
            {
                var apiUserExists = await _DBContext.Users.AnyAsync(x =>
                        x.CompanyId == record.CompanyId &&
                        x.DivisionId == record.DivisionId &&
                        x.DepartmentId == record.DepartmentId &&
                        x.Username == record.UserName);

                if (apiUserExists)
                {
                    throw new Exception($"An api user with username {record.UserName} already exists");
                }

                var currentDate = DateTime.Now;

                var newUser = new Users()
                {
                    CompanyId = record.CompanyId,
                    DivisionId = record.DivisionId,
                    DepartmentId = record.DepartmentId,
                    Username = record.UserName,
                    Email = record.Email,
                    Phone = record.Phone,
                    Active = record.Active,
                    Password = record.PasswordHash,
                    FirstName = record.FirstName,
                    LastName = record.LastName,
                    MiddleName = record.MiddleName,
                    RoleId = record.Roles.Count() > 0 ? record.Roles[0] : null,
                    RegistrationDate = currentDate
                };

                _DBContext.Users.Add(newUser);


                var newAccessPermission = new AccessPermissions()
                {
                    CompanyId = record.CompanyId,
                    DivisionId = record.DivisionId,
                    DepartmentId = record.DepartmentId,
                    EmployeeId = record.UserName,
                    DefaultPageToDisplay = record.DefaultSite,
                    RestrictMultipleLogins = record.RestrictMultipleLogins
                };

                _DBContext.AccessPermissions.Add(newAccessPermission);

                if (record.Warehouses.Count > 0)
                {

                    var warehouseList = new List<WarehouseByEmployees>();

                    foreach (var warehouse in record.Warehouses)
                    {

                        warehouseList.Add(
                             new WarehouseByEmployees()
                             {
                                 CompanyId = record.CompanyId,
                                 DivisionId = record.DivisionId,
                                 DepartmentId = record.DepartmentId,
                                 EmployeeId = record.UserName,
                                 WarehouseId = warehouse,
                                 SystemDate = currentDate
                             }
                            );
                    }

                    _DBContext.WarehouseByEmployees.AddRange(warehouseList);
                }


                await _DBContext.SaveChangesAsync();

            }
            catch { throw; }
        }

        public async Task<ApiUser> GetUser(string companyId, string divisionId, string departmentId, string username)
        {
            return await (from u in _DBContext.Users
                          join a in _DBContext.AccessPermissions
                                      on new { u.CompanyId, u.DivisionId, u.DepartmentId, EmployeeId = u.Username } equals new { a.CompanyId, a.DivisionId, a.DepartmentId, a.EmployeeId }
                          join w in _DBContext.WarehouseByEmployees
                          on new { u.CompanyId, u.DivisionId, u.DepartmentId, EmployeeId = u.Username } equals new { w.CompanyId, w.DivisionId, w.DepartmentId, w.EmployeeId }
                          where u.CompanyId == companyId
                              && u.DivisionId == divisionId
                              && u.DepartmentId == departmentId
                              && u.Username == username
                              && u.Active == true
                          group new { u, a, w } by new
                          {
                              u.CompanyId,
                              u.DivisionId,
                              u.DepartmentId,
                              u.Username,
                              u.Email,
                              u.Phone,
                              u.Active,
                              u.FirstName,
                              u.LastName,
                              u.MiddleName,
                              a.RestrictMultipleLogins,
                              a.DefaultPageToDisplay,
                              u.Password,
                              u.RefreshToken,
                              u.RefreshTokenExpiry,
                              a.EnforcePasswordReset,
                              u.RoleId,
                              u.BranchCode,
                              u.CurrentCompany,
                              u.CurrentDivision,
                              u.CurrentDepartment,
                              u.CurrentBranch,
                          } into grp
                          select new ApiUser
                          {
                              UserName = grp.Key.Username,
                              CompanyId = grp.Key.CompanyId,
                              DivisionId = grp.Key.DivisionId,
                              DepartmentId = grp.Key.DepartmentId,
                              Email = grp.Key.Email ?? "",
                              Phone = grp.Key.Phone ?? "",
                              Active = grp.Key.Active ?? false,
                              FirstName = grp.Key.FirstName ?? "",
                              LastName = grp.Key.LastName ?? "",
                              MiddleName = grp.Key.MiddleName ?? "",
                              Warehouses = grp.Select(item => item.w.WarehouseId).ToList(),
                              RestrictMultipleLogins = grp.Key.RestrictMultipleLogins ?? false,
                              DefaultHomeScreen = grp.Key.DefaultPageToDisplay,
                              PasswordHash = grp.Key.Password,
                              RefreshToken = grp.Key.RefreshToken,
                              EnforcePasswordReset = grp.Key.EnforcePasswordReset ?? false,
                              Roles = new List<int>() { grp.Key.RoleId ?? 1 },
                              BranchCode = grp.Key.BranchCode,
                              CurrentCompany = grp.Key.CurrentCompany,
                              CurrentDivision = grp.Key.CurrentDivision,
                              CurrentDepartment = grp.Key.CurrentDepartment,
                              CurrentBranch = grp.Key.CurrentBranch,
                          }).FirstOrDefaultAsync();



            //var user = await _DBContext.Users
            //        .Where(u => u.CompanyId == companyId
            //                    && u.DivisionId == divisionId
            //                    && u.DepartmentId == departmentId
            //                    && u.Username == username
            //                    && u.Active == true)
            //            .Select(u => new
            //            {
            //                u.CompanyId,
            //                u.DivisionId,
            //                u.DepartmentId,
            //                u.Username,
            //                u.Email,
            //                u.Phone,
            //                u.Active,
            //                u.FirstName,
            //                u.LastName,
            //                u.MiddleName,
            //                u.Password,
            //                u.RefreshToken,
            //                u.RoleId,
            //                u.BranchCode,
            //                u.CurrentCompany,
            //                u.CurrentDivision,
            //                u.CurrentDepartment,
            //                u.CurrentBranch
            //            })
            //        .FirstOrDefaultAsync();

            //if (user == null)
            //{
            //    return null;
            //}

            //var accessPermissions = await _DBContext.AccessPermissions
            //    .Where(a => a.CompanyId == user.CompanyId
            //                && a.DivisionId == user.DivisionId
            //                && a.DepartmentId == user.DepartmentId
            //                && a.EmployeeId == user.Username)
            //       .Select(a => new
            //       {
            //           a.RestrictMultipleLogins,
            //           a.DefaultPageToDisplay,
            //           a.EnforcePasswordReset
            //       })
            //    .ToListAsync();

            //var warehouses = await _DBContext.WarehouseByEmployees
            //    .Where(w => w.CompanyId == user.CompanyId
            //                && w.DivisionId == user.DivisionId
            //                && w.DepartmentId == user.DepartmentId
            //                && w.EmployeeId == user.Username)
            //        .Select(w => w.WarehouseId)
            //    .ToListAsync();

            //var apiUser = new ApiUser
            //{
            //    UserName = user.Username,
            //    CompanyId = user.CompanyId,
            //    DivisionId = user.DivisionId,
            //    DepartmentId = user.DepartmentId,
            //    Email = user.Email ?? "",
            //    Phone = user.Phone ?? "",
            //    Active = user.Active ?? false,
            //    FirstName = user.FirstName ?? "",
            //    LastName = user.LastName ?? "",
            //    MiddleName = user.MiddleName ?? "",
            //    Warehouses = warehouses,
            //    RestrictMultipleLogins = accessPermissions.Select(a => a.RestrictMultipleLogins).FirstOrDefault() ?? false,
            //    DefaultHomeScreen = accessPermissions.Select(a => a.DefaultPageToDisplay).FirstOrDefault(),
            //    PasswordHash = user.Password,
            //    RefreshToken = user.RefreshToken,
            //    EnforcePasswordReset = accessPermissions.Select(a => a.EnforcePasswordReset).FirstOrDefault() ?? false,
            //    Roles = new List<int> { user.RoleId ?? 1 },
            //    BranchCode = user.BranchCode,
            //    CurrentCompany = user.CurrentCompany,
            //    CurrentDivision = user.CurrentDivision,
            //    CurrentDepartment = user.CurrentDepartment,
            //    CurrentBranch = user.CurrentBranch
            //};

            //return apiUser;


        }

        public async Task<ApiUser> GetUserConcise(string usernameAppended)
        {

            string[] paramKeys = _idGen.SplitId(usernameAppended);

            string companyId = paramKeys[0];
            string divisionId = paramKeys[1];
            string departmentId = paramKeys[2];
            string usernameId = paramKeys[3];

            return await _DBContext.Users
                    .Where(u => u.CompanyId == companyId
                                && u.DivisionId == divisionId
                                && u.DepartmentId == departmentId
                                && u.Username == usernameId
                                && u.Active == true)
                        .Select(u => new ApiUser
                        {
                            CompanyId = u.CompanyId,
                            DivisionId = u.DivisionId,
                            DepartmentId = u.DepartmentId,
                            UserName = u.Username,
                            Email = u.Email,
                            Phone = u.Phone,
                            Active = u.Active ?? false,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            MiddleName = u.MiddleName,
                            RefreshToken = u.RefreshToken,
                            Roles = new List<int>() { u.RoleId ?? 1 },
                            BranchCode = u.BranchCode,
                            CurrentCompany = u.CurrentCompany,
                            CurrentDivision = u.CurrentDivision,
                            CurrentDepartment = u.CurrentDepartment,
                            CurrentBranch = u.CurrentBranch
                        })
                    .FirstOrDefaultAsync();
        }

        public async Task UpdateUser(ApiUser record)
        {
            try
            {

                var apiUser = await _DBContext.Users.Where(x =>
                        x.CompanyId == record.CompanyId &&
                        x.DivisionId == record.DivisionId &&
                        x.DepartmentId == record.DepartmentId &&
                        x.Username == record.UserName &&
                        x.Active == true
                        ).FirstOrDefaultAsync() ?? throw new Exception($"An api user with username {record.UserName} does not exist");

                var currentDate = DateTime.Now;

                apiUser.Email = record.Email;
                apiUser.Phone = record.Phone;
                apiUser.Active = record.Active;
                apiUser.Password = record.PasswordHash;
                apiUser.FirstName = record.FirstName;
                apiUser.LastName = record.LastName;
                apiUser.MiddleName = record.MiddleName;
                apiUser.RoleId = record.Roles.Count() > 0 ? record.Roles[0] : null;
                apiUser.RefreshToken = record.RefreshToken;
                apiUser.RefreshTokenExpiry = record.RefreshTokenExpiry;
                apiUser.CurrentCompany = record.CurrentCompany;
                apiUser.CurrentDivision = record.CurrentDivision;
                apiUser.CurrentDepartment = record.CurrentDepartment;
                apiUser.CurrentBranch = record.CurrentBranch;


                //var userAccessPermissions = await _DBContext.AccessPermissions.Where(x =>
                //        x.CompanyId == record.CompanyId &&
                //        x.DivisionId == record.DivisionId &&
                //        x.DepartmentId == record.DepartmentId &&
                //        x.EmployeeId == record.UserName)

                //    .FirstOrDefaultAsync();


                //if (userAccessPermissions != null)
                //{
                //    userAccessPermissions.DefaultPageToDisplay = record.DefaultSite;
                //    userAccessPermissions.RestrictMultipleLogins = record.RestrictMultipleLogins;
                //    userAccessPermissions.EnforcePasswordReset = record.EnforcePasswordReset;
                //}


                // TODO: Include other access permission properties.
                var userAccessPermissions = await _DBContext.AccessPermissions
                    .Where(x => x.CompanyId == record.CompanyId &&
                                x.DivisionId == record.DivisionId &&
                                x.DepartmentId == record.DepartmentId &&
                                x.EmployeeId == record.UserName)
                    .Select(x => new
                    {
                        x.CompanyId,
                        x.DivisionId,
                        x.DepartmentId,
                        x.EmployeeId,
                        x.DefaultPageToDisplay,
                        x.RestrictMultipleLogins,
                        x.EnforcePasswordReset
                    })
                    .FirstOrDefaultAsync();

                if (userAccessPermissions != null)
                {
                    var accessPermission = new AccessPermissions
                    {
                        CompanyId = userAccessPermissions.CompanyId,
                        DivisionId = userAccessPermissions.DivisionId,
                        DepartmentId = userAccessPermissions.DepartmentId,
                        EmployeeId = userAccessPermissions.EmployeeId,
                    };

                    if (userAccessPermissions.DefaultPageToDisplay != record.DefaultSite)
                    {
                        accessPermission.DefaultPageToDisplay = record.DefaultSite;
                        _DBContext.Entry(accessPermission).Property(x => x.DefaultPageToDisplay).IsModified = true;
                    }

                    if (userAccessPermissions.RestrictMultipleLogins != record.RestrictMultipleLogins)
                    {
                        accessPermission.RestrictMultipleLogins = record.RestrictMultipleLogins;
                        _DBContext.Entry(accessPermission).Property(x => x.RestrictMultipleLogins).IsModified = true;
                    }

                    if (userAccessPermissions.EnforcePasswordReset != record.EnforcePasswordReset)
                    {
                        accessPermission.EnforcePasswordReset = record.EnforcePasswordReset;
                        _DBContext.Entry(accessPermission).Property(x => x.EnforcePasswordReset).IsModified = true;
                    }


                }

                    var currentWarehouses = await _DBContext.WarehouseByEmployees.Where(x =>
                        x.CompanyId == record.CompanyId &&
                        x.DivisionId == record.DivisionId &&
                        x.DepartmentId == record.DepartmentId &&
                        x.EmployeeId == record.UserName
                        ).ToListAsync();


                var newWarehouses = new List<WarehouseByEmployees>();

                foreach (var warehouse in record.Warehouses)
                {
                    newWarehouses.Add(
                         new WarehouseByEmployees()
                         {
                             CompanyId = record.CompanyId,
                             DivisionId = record.DivisionId,
                             DepartmentId = record.DepartmentId,
                             EmployeeId = record.UserName,
                             WarehouseId = warehouse,
                             SystemDate = currentDate,
                             BranchCode = record.BranchCode,
                             EnteredBy = record.UserName
                         }
                        );
                }

                if (newWarehouses.Count > 0)
                {
                    if (currentWarehouses.Count > 0)
                    {
                        // Find entities in currentWarehouses but not in newWarehouses (for deletion)
                        var toDelete = currentWarehouses.Except(newWarehouses, new CompareWarehouses()).ToList();

                        // Find entities existing in both currentWarehouses and newWarehouses (for update)
                        //var toUpdate = currentWarehouses.Intersect(newWarehouses, new CompareWarehouses()).ToList();

                        if (toDelete.Count > 0)
                        {
                            currentWarehouses = currentWarehouses.Except(newWarehouses, new CompareWarehouses()).ToList();
                            _DBContext.WarehouseByEmployees.RemoveRange(currentWarehouses);
                        }

                        //if (toUpdate.Count > 0) {

                        //    currentWarehouses = currentWarehouses.Intersect(newWarehouses, new CompareWarehouses()).ToList();
                        //    _DBContext.WarehouseByEmployees.UpdateRange(currentWarehouses);
                        //}
                    }

                    // Find entities in newWarehouses but not in currentWarehouses (for insertion)
                    var toInsert = newWarehouses.Except(currentWarehouses, new CompareWarehouses()).ToList();
                    if (toInsert.Count > 0) _DBContext.WarehouseByEmployees.AddRange(toInsert);
                }


                await _DBContext.SaveChangesAsync();

            }
            catch { throw; }
        }


        public async Task<ApiToken> GetAccess(string token)
        {
            int days = 0;
            ApiToken apiToken = new ApiToken();
            var regInfo = new TblVersion();
            string cacheKey = "api_token";
            string regCache = "reg_cache";

            try
            {
                if (!_cache.TryGetValue(cacheKey, out apiToken) || apiToken is null)
                {
                    //get the comp on token
                    apiToken = await _DBContext.ApiToken.Where(x => x.Token == token).FirstOrDefaultAsync();

                    // Save the data in the cache
                    _cache.Set(cacheKey, apiToken, TimeSpan.FromMinutes(10));
                }

                if (apiToken != null)
                {
                    if (!_cache.TryGetValue(regCache, out regInfo) || regInfo is null)
                    {
                        //get reg info
                        regInfo = await _DBContext.TblVersion.Where(x => x.CompanyId == apiToken.CompanyId &&
                                                            x.DivisionId == apiToken.DivisionId &&
                                                            x.DepartmentId == apiToken.DepartmentId).FirstOrDefaultAsync();

                        // Save the data in the cache
                        _cache.Set(regCache, regInfo, TimeSpan.FromMinutes(10));
                    }

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

        public async Task<string> GetUserPasswordHash(string companyId, string divisionId, string departmentId, string userName)
        {
            var apiUser = await _DBContext.Users.Where(x =>
                x.CompanyId == companyId &&
                x.DivisionId == divisionId &&
                x.DepartmentId == departmentId &&
                x.Username == userName).FirstOrDefaultAsync() ?? throw new Exception($"An api user with username {userName} does not exist");

            return apiUser.Password ?? "";
        }

        public async Task SetUserPasswordHash(string companyId, string divisionId, string departmentId, string userName, string passwordHash)
        {
            var apiUser = await _DBContext.Users.Where(x =>
            x.CompanyId == companyId &&
            x.DivisionId == divisionId &&
            x.DepartmentId == departmentId &&
            x.Username == userName).FirstOrDefaultAsync() ?? throw new Exception($"An api user with username {userName} does not exist");

            apiUser.Password = passwordHash;

            await _DBContext.SaveChangesAsync();
        }

        private class CompareWarehouses : IEqualityComparer<WarehouseByEmployees>
        {
            public bool Equals(WarehouseByEmployees x, WarehouseByEmployees y)
            {
                return x.CompanyId == y.CompanyId && x.DivisionId == y.DivisionId
                    && x.DepartmentId == y.DepartmentId && x.EmployeeId == y.EmployeeId
                    && x.WarehouseId == y.WarehouseId;
            }

            public int GetHashCode([DisallowNull] WarehouseByEmployees obj)
            {
                unchecked // Avoid potential overflow warnings for simplicity
                {
                    int hash = obj.CompanyId.GetHashCode();
                    hash ^= obj.DivisionId.GetHashCode();
                    hash ^= obj.DepartmentId.GetHashCode();
                    hash ^= obj.EmployeeId.GetHashCode();
                    hash ^= obj.WarehouseId.GetHashCode();
                    return hash;
                }
            }
        }
    }
}




