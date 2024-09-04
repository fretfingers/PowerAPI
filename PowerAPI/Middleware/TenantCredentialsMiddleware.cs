using DevExpress.XtraReports.Templates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using PowerAPI.Data.IRepository;
using PowerAPI.Data.POCO;
using PowerAPI.Service.Clients;
using PowerAPI.Service.IdentityLibrary;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace PowerAPI.Middleware
{
    public class TenantCredentialsMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly IServiceProvider _serviceProvider;
        //private readonly UserManager<CustomIdentityUser> _userManager;

        public TenantCredentialsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync (HttpContext context, IApiAuthService apiAuthService)
        {
            if (context.User.Identity.IsAuthenticated  && !context.Response.HasStarted)
            {
                var username = context.User.Identity?.Name;

                // User may switch between Companies, Divisions, Department, Branches during active session
                // hence the need to validate the current location details the user has set.
                var user = await apiAuthService.GetUserConcise(username);
                if (user != null)
                {
                    context.Items["company"] = user.CurrentCompany??user.CompanyId;
                    context.Items["division"] = user.CurrentDivision?? user.DivisionId;
                    context.Items["department"] = user.CurrentDepartment?? user.DepartmentId;
                    context.Items["branch"] = user.CurrentBranch?? user.BranchCode;
                    context.Items["username"] = user.UserName;
                    context.Items["role"] = user.Roles[0];
                }
            }
     

            await _next(context);
        }




    }
}
