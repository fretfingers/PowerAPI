using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PowerAPI.Data.Models;
using PowerAPI.Service.Clients;
using SignalRSwaggerGen.Attributes;
using System;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using PowerAPI.Data.IRepository;


namespace PowerAPI.Hubs
{

    [Authorize]
    [SignalRHub]
    public class DashboardHub : Hub
    {

        private IServiceProvider _serviceProvider;
        IConfiguration _configuration;

        public DashboardHub(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Send Order Summary Hub method that's used for retrieving order summary.
        /// <returns>Returns order summary details</returns>
        /// </summary>
        public async Task SendOrderSummary()
        {

            var services = new ServiceCollection();

            services.AddSingleton(_configuration);
            services.AddScoped<DashboardService>();
            services.AddDbContext<EnterpriseContext>((options)
                  =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("Enterprise"));
            });
            _serviceProvider = services.BuildServiceProvider();


            using (var scope = _serviceProvider.CreateAsyncScope())
            {
                var httpContext = Context.GetHttpContext();
                var _dbContext = scope.ServiceProvider.GetRequiredService<EnterpriseContext>();
                var _dashboardService = scope.ServiceProvider.GetRequiredService<DashboardService>();


                var currentCompany = httpContext.Items["company"].ToString();
                var currentDivision = httpContext.Items["division"].ToString();
                var currentDepartment = httpContext.Items["department"].ToString();

                var orders = await _dashboardService.GetDashboardOrderSummary(currentCompany, currentDivision, currentDepartment);
                
                await Clients.All.SendAsync("ReceivedOrders", orders);
                //}
            }

        }

        /// <summary>
        /// Internal Dependency Handler
        /// <returns>Returns order summary details</returns>
        /// </summary>
        public async Task SendOrderSummaryDep(string currentCompany, string currentDivision, string currentDepartment)
        {

            var services = new ServiceCollection();

            services.AddSingleton(_configuration);
            services.AddScoped<DashboardService>();
            services.AddDbContext<EnterpriseContext>((options)
                  =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("Enterprise"));
            });
            _serviceProvider = services.BuildServiceProvider();


            using (var scope = _serviceProvider.CreateAsyncScope())
            {
                var _dbContext = scope.ServiceProvider.GetRequiredService<EnterpriseContext>();
                var _dashboardService = scope.ServiceProvider.GetRequiredService<DashboardService>();

                var orders = await _dashboardService.GetDashboardOrderSummary(currentCompany, currentDivision, currentDepartment);
                
                await Clients.All.SendAsync("ReceivedOrders", orders);
            }

        }



    }
}
