using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using PowerAPI.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.IRepository
{
    public interface ICustomer
    {
        Task<ApiToken> GetAccess(string token);
        Task<bool> CustomerLogin(string Id, string password, ApiToken tokenObj);
        Task<CustomerInform> GetCustomerById(string username, ApiToken tokenObj);
        Task<CustomerInform> GetCustomerByEmail(string username, ApiToken tokenObj);
        //Task<IEnumerable<OrderHeader>> GetOrderByCustomer(string Id, ApiToken tokenObj);
        Task<CustomerInform> CustomerLoginEmail(string Username, string password, ApiToken tokenObj);
        Task<StatusMessage> ChangePwd(PasswordModel changePassword, ApiToken tokenObj);
        Task<StatusMessage> ResetPasswordOTP(string username, ApiToken tokenObj);
        Task<StatusMessage> ValidateOTP(string username, string oTP, ApiToken tokenObj);
        Task<StatusMessage> ResetPassword(string username, string oTP, string password, ApiToken tokenObj);
        Task<Paging> GetCustomer(PaginationParams Param, ApiToken token);
        Task<Paging> GetSalesRepresentatives(PaginationParams Param, ApiToken token);
    }
}
