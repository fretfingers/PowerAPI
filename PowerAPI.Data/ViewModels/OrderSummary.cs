using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.ViewModels
{
    public class OrderSummary
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public double TotalOrderCount { get; set; }
        public double Subtotal { get; set; }
        public double DiscountAmount { get; set; }
        public double TaxAmount { get; set; }
        public double Total { get; set; }
        public double AvailableCredit { get; set; }
        public double AmountToPay { get; set; }
    }
}
