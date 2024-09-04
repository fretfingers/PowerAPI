using PowerAPI.Data.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Data.ViewModels
{
    public class SalesRepresentatives
    {
        public string CompanyId { get; set; }
        public string DivisionId { get; set; }
        public string DepartmentId { get; set; }
        public string SalesRepId { get; set; }
        public string Name { get; set; }
        public short? Method { get; set; }
        public double? Target1 { get; set; }
        public double? Commission1 { get; set; }
        public double? Target2 { get; set; }
        public double? Commission2 { get; set; }
        public double? Target3 { get; set; }
        public double? Commission3 { get; set; }
        public double? Target4 { get; set; }
        public double? Commission4 { get; set; }
        public double? Target5 { get; set; }
        public double? Commission5 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public bool? Active { get; set; }
        public string BankAccount { get; set; }
        public string Comment1 { get; set; }
        public string Comment2 { get; set; }
        public string Comment3 { get; set; }
        public decimal? EstimatedValue { get; set; }
        public decimal? ActualValue { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockTs { get; set; }
        public string SalesGroupId { get; set; }
        public string BranchCode { get; set; }
        public double? CommissionPercent { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public ICollection<SetupWorkflow> WorkFlowTrail { get; set; }
    }
}
