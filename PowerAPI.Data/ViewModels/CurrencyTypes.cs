using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.ViewModels
{
    public class CurrencyTypes
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string CurrencyId { get; set; }
        public string CurrencyType { get; set; }
        public string CurrenycySymbol { get; set; }
        public double? CurrencyExchangeRate { get; set; }
        public DateTime? CurrencyRateLastUpdate { get; set; }
        public int? CurrencyPrecision { get; set; }
        public string MajorUnits { get; set; }
        public string MinorUnits { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string BranchCode { get; set; }
        public List<CurrencyTypesHistory> currencyTypesHistories { get; set; }
        public ICollection<SetupWorkflow> WorkFlowTrail { get; set; }
        
    }
}
