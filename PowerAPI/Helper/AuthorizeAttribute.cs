using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerAPI.Helper
{
    /// <summary>
    /// 
    /// </summary>
    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    //public class AuthorizeAttribute 
    //public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    //{
        //public string user;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        //public void OnAuthorization(AuthorizationFilterContext context)
        //{
        //    //var user = (StatusMessage)context.HttpContext.Items["User"];

        //    if(context.HttpContext.Items["User"] != null)
        //    {
        //        user = context.HttpContext.Items["User"].ToString();
        //        if (user == null)
        //        {
        //            // not logged in
        //            context.Result = new JsonResult(new StatusMessage { Status = "Unauthorized", Message = "Authorization Token Invalid or Expired", data = null }) { StatusCode = StatusCodes.Status401Unauthorized };
        //        }
        //    }
        //    else
        //    {
        //        context.Result = new JsonResult(new StatusMessage { Status = "UnauthorizedModified", Message = "Authorization Token Invalid or Expired", data = null }) { StatusCode = StatusCodes.Status401Unauthorized };
        //    }


        //}
    //}
}
