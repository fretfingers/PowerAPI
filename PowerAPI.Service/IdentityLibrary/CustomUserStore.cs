//using Microsoft.AspNet.Identity;
using PowerAPI.Data.Models;
using PowerAPI.Service.Clients;
using PowerAPI.Data.POCO;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Security.Claims;
using System.Linq;
using PowerAPI.Data.IRepository;
using DevExpress.ReportServer.ServiceModel.DataContracts;
using DevExpress.DirectX.Common.DirectWrite;



namespace PowerAPI.Service.IdentityLibrary
{
    public class CustomUserStore : IUserPasswordStore<CustomIdentityUser>
    {
        private readonly IApiAuthService _userService;
        private IIdGenerator _idGen;

        public CustomUserStore(IApiAuthService userService, IIdGenerator idGen)
        {
            _userService = userService;
            _idGen = idGen;
        }

        private CustomIdentityUser MapUser(ApiUser record)
        {

            string concatId = _idGen.GenerateId(record.CompanyId, record.DivisionId, record.DepartmentId, record.UserName);

            var result = new CustomIdentityUser()
            {
                Id = concatId,
                CompanyId = record.CompanyId,
                DivisionId = record.DivisionId,
                DepartmentId = record.DivisionId,
                UserName = concatId,
                unAppendedUsername = record.UserName,
                PhoneNumber = record.Phone,
                Email = record.Email,
                PasswordHash = record.PasswordHash,
                Phone = record.Phone,
                Active = record.Active,
                FirstName = record.FirstName,
                LastName = record.LastName,
                MiddleName = record.MiddleName,
                Roles = record.Roles,
                Warehouses = record.Warehouses,
                RefreshToken = record.RefreshToken,
                RefreshTokenExpiry = record.RefreshTokenExpiry,
                BranchCode = record.BranchCode,
                CurrentCompany = record.CurrentCompany??record.CompanyId,
                CurrentDivision = record.CurrentDivision??record.DivisionId,
                CurrentDepartment = record.CurrentDepartment??record.DepartmentId,
                CurrentBranch = record.CurrentBranch ??record.BranchCode,
            };

            return result;
        }


        #region IUserPasswordStore
        public async Task SetPasswordHashAsync(CustomIdentityUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);
            
            await _userService.SetUserPasswordHash(user.CompanyId, user.DivisionId, user.DepartmentId, user.UserName, passwordHash);
            return;
        }

        public async Task<string> GetPasswordHashAsync(CustomIdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);

            string[] paramKeys = _idGen.SplitId(user.UserName);

            string usernameId = paramKeys[3];

            var passwordHash = await _userService.GetUserPasswordHash(user.CompanyId, user.DivisionId, user.DepartmentId, usernameId);
            return passwordHash;
        }

        public async Task<bool> HasPasswordAsync(CustomIdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);

            string[] paramKeys = _idGen.SplitId(user.UserName);

            string usernameId = paramKeys[3];

            var passwordHash = await _userService.GetUserPasswordHash(user.CompanyId, user.DivisionId, user.DepartmentId, usernameId);
            return !string.IsNullOrWhiteSpace(passwordHash);
        }
        #endregion

        #region IUserStore
        public Task<string> GetUserIdAsync(CustomIdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);

            return Task.FromResult(user.Id);
        }

        public async Task<string> GetUserNameAsync(CustomIdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);

            string[] paramKeys = _idGen.SplitId(user.UserName);

            string companyId = paramKeys[0];
            string divisionId = paramKeys[1];
            string departmentId = paramKeys[2];
            string usernameId = paramKeys[3];


            var userEntity = new ApiUser()
            {
                Id = user.Id,
                UserName = usernameId,
                CompanyId = companyId,
                DivisionId = divisionId,
                DepartmentId = departmentId,
                Email = user.Email,
                Phone = user.Phone,
                Active = user.Active,
                DefaultSite = user.DefaultSite,
                PasswordHash = user.PasswordHash,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                EnforcePasswordReset = user.EnforcePasswordReset,
                RestrictMultipleLogins = user.RestrictMultipleLogins,
                AccType = user.AccType,
                Scope = user.Scope,
                Roles = user.Roles,
                Warehouses = user.Warehouses,
                DefaultHomeScreen = user.DefaultHomeScreen,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiry = user.RefreshTokenExpiry,
                BranchCode = user.BranchCode,
                CurrentCompany = user.CurrentCompany,
                CurrentDivision = user.CurrentDivision,
                CurrentDepartment = user.CurrentDepartment,
                CurrentBranch = user.CurrentBranch,
            };

            try
            {
                await _userService.UpdateUser(userEntity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not insert user {user.UserName}. Error:{ex.Message}");
            }



            return user.UserName;
        }

        public Task SetUserNameAsync(CustomIdentityUser user, string userName, CancellationToken cancellationToken)
        {
            throw new NotSupportedException("Changing the username is not supported.");
        }

        public Task<string> GetNormalizedUserNameAsync(CustomIdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);

            return Task.FromResult(user.UserName.ToUpper());
        }

        public Task SetNormalizedUserNameAsync(CustomIdentityUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(normalizedName);

            user.NormalizedUserName = normalizedName;

            return Task.FromResult(0);
        }

        public async Task<IdentityResult> CreateAsync(CustomIdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);

            // Split to retrieve composite keys from user Id string
            // composite keys will help filter users for various application tenants

            string[] paramKeys = _idGen.SplitId(user.UserName);

            string companyId = paramKeys[0];
            string divisionId = paramKeys[1];
            string departmentId = paramKeys[2];
            string usernameId = paramKeys[3];


            var userEntity = new ApiUser()
            {
                Id = user.Id,
                UserName = usernameId,
                CompanyId = companyId,
                DivisionId = divisionId,
                DepartmentId = departmentId,
                Email = user.Email,
                Phone = user.Phone,
                Active = user.Active,
                DefaultSite = user.DefaultSite,
                PasswordHash = user.PasswordHash,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                EnforcePasswordReset = user.EnforcePasswordReset,
                RestrictMultipleLogins = user.RestrictMultipleLogins,
                AccType = user.AccType,
                Scope = user.Scope,
                Roles = user.Roles,
                Warehouses = user.Warehouses,
                DefaultHomeScreen = user.DefaultHomeScreen
            };

            try
            {
                await _userService.CreateUser(userEntity);
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Could not insert user {user.UserName}. Error:{ex.Message}" });
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(CustomIdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);

            string[] paramKeys = _idGen.SplitId(user.UserName);

            string companyId = paramKeys[0];
            string divisionId = paramKeys[1];
            string departmentId = paramKeys[2];
            string usernameId = paramKeys[3];


            var userEntity = new ApiUser()
            {
                Id = user.Id,
                UserName = usernameId,
                CompanyId = companyId,
                DivisionId = divisionId,
                DepartmentId = departmentId,
                Email = user.Email,
                Phone = user.Phone,
                Active = user.Active,
                DefaultSite = user.DefaultSite,
                PasswordHash = user.PasswordHash,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                EnforcePasswordReset = user.EnforcePasswordReset,
                RestrictMultipleLogins = user.RestrictMultipleLogins,
                AccType = user.AccType,
                Scope = user.Scope,
                Roles = user.Roles,
                Warehouses = user.Warehouses,
                DefaultHomeScreen = user.DefaultHomeScreen,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiry = user.RefreshTokenExpiry,
                BranchCode = user.BranchCode,
                CurrentCompany = user.CurrentCompany,
                CurrentDivision = user.CurrentDivision,
                CurrentDepartment = user.CurrentDepartment,
                CurrentBranch = user.CurrentBranch,
            };

            try
            {
                await _userService.UpdateUser(userEntity);
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Could not insert user {user.UserName}. Error:{ex.Message}" });
            }

            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(CustomIdentityUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<CustomIdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(userId);

            string[] paramKeys = _idGen.SplitId(userId);

            string companyId = paramKeys[0];
            string divisionId = paramKeys[1];
            string departmentId = paramKeys[2];
            string usernameId = paramKeys[3];
            var userReq = await _userService.GetUser(companyId, divisionId, departmentId, usernameId);

            return userReq!= null ? MapUser(userReq): default(CustomIdentityUser);
        }

        public async Task<CustomIdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(normalizedUserName);

            string[] keys = _idGen.SplitId(normalizedUserName);

            string companyId = keys[0];
            string divisionId = keys[1];
            string departmentId = keys[2];
            string usernameId = keys[3];

            var userReq = await _userService.GetUser(companyId, divisionId, departmentId, usernameId);

            return userReq != null ? MapUser(userReq) : default(CustomIdentityUser);
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
