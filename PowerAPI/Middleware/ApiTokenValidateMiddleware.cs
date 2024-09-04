using DevExpress.XtraReports.Templates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
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
    public class ApiTokenValidateMiddleware
    {
        private readonly RequestDelegate _next;


        public ApiTokenValidateMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IApiAuthService apiAuthService)
        {

            //var routeData = context.GetRouteData();
            //var tokenValue = (string)routeData.Values["token"];

            var tokenValue = (string)context.Request.RouteValues["token"];


            if (!StringValues.IsNullOrEmpty(tokenValue))
            {
                var tokenObj = await apiAuthService.GetAccess(tokenValue);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays < 0)
                    {
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                        var json = JsonSerializer.Serialize(new { StatusCode = 401, Message = "Access Denied. License Expired. Contact System Administrator" }, options);

                        await context.Response.WriteAsync(json);
                        return;
                    }
                }
                else
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                    var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                    var json = JsonSerializer.Serialize(new { StatusCode = 401, Message = "Invalid Token" }, options);

                    await context.Response.WriteAsync(json);
                    return;
                }
            }
            else if (context.Request.Path.StartsWithSegments("/hubs"))
            {

                await _next(context);
            }
            else
            {



                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(new { StatusCode = 400, Message = "Api token is absent in request" }, options);

                await context.Response.WriteAsync(json);
                return;
            }


            await _next(context);
        }




    }
}
