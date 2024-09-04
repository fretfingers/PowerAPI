using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using System.Threading.Tasks;

namespace PowerAPI.Data.IRepository
{
    public interface IApiAuthService
    {
        Task CreateUser(ApiUser record);
        Task<ApiUser> GetUser(string companyId, string divisionId, string departmentId, string username );
        Task UpdateUser(ApiUser record);
        Task<string> GetUserPasswordHash(string companyId, string divisionId, string departmentId, string userName);
        Task SetUserPasswordHash(string companyId, string divisionId, string departmentId, string userName, string passwordHash);
        Task<ApiUser> GetUserConcise(string usernameAppended);
        Task<ApiToken> GetAccess(string token);
    }
}
