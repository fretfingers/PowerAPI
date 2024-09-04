using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using PowerAPI.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.IRepository
{
    public interface IDashboardService
    {
        //Task<ICollection<OrderSummDashboard>> GetDashboardOrderSummary(string companyId, string divisionId, string departmentId, EnterpriseContext context);
        Task<ICollection<OrderSummDashboard>> GetDashboardOrderSummary(string companyId, string divisionId, string departmentId);
    }
}
