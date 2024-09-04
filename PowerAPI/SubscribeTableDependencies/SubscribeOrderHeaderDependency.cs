using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.ApplicationServices;
using PowerAPI.Data.IRepository;
using PowerAPI.Data.Models;
using PowerAPI.Hubs;
using PowerAPI.Middleware;
using PowerAPI.Service.Clients;
using System;
using System.Threading;
using System.Threading.Tasks;
using TableDependency.SqlClient;

namespace PowerAPI.SubscribeTableDependencies
{
    public class SubscribeOrderHeaderDependency: ISubscribeTableDependency
    {
        SqlTableDependency<OrderHeader> tableDependency;
        private readonly DashboardHub _dashboardHub;
        private readonly ILogger<SubscribeOrderHeaderDependency> _logger;

        public SubscribeOrderHeaderDependency(DashboardHub dashboardHub, ILogger<SubscribeOrderHeaderDependency> logger)
        {
            _dashboardHub = dashboardHub;
            _logger = logger;
        }

        public void SubscribeTableDependency(string connectionString)
        { 
            try
            {
                tableDependency = new SqlTableDependency<OrderHeader>(connectionString);
                tableDependency.OnChanged += TableDependency_OnChanged;
                tableDependency.OnError += TableDependency_OnError;
                tableDependency.Start();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }


        }

        private async void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<OrderHeader> e)
        {
            if(e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
            {
                await _dashboardHub.SendOrderSummaryDep(e.Entity.CompanyId, e.Entity.DivisionId, e.Entity.DepartmentId);
            }
        }

        private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            Console.WriteLine($"{nameof(OrderHeader)} SqlTableDependency error: {e.Error.Message}");
            _logger.LogError($"SqlTableDependency error: {e.Error.Message}");
        }







    }
}
