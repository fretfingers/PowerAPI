using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.POCO
{
    public class OrderSummDashboard
    {
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CurrencyId { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
        public ICollection<OrderWorkflow> WorkFlowTrail { get; set; }
        
    }
}
